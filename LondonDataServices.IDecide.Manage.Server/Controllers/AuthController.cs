// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Hl7.Fhir.Model.CdsHooks;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Manage.Server.Data;
using LondonDataServices.IDecide.Manage.Server.Models;
using LondonDataServices.IDecide.Manage.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthController> logger;
        private readonly ISecureTokenStorage secureTokenStorage;
        private readonly IApiPlatformClient apiPlatformClient;
        private readonly ApplicationDbContext context;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            ISecureTokenStorage secureTokenStorage,
            IApiPlatformClient apiPlatformClient,
            StorageBroker storageBroker,
            ApplicationDbContext context)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.secureTokenStorage = secureTokenStorage;
            this.apiPlatformClient = apiPlatformClient;
            this.context = context;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(CancellationToken cancellationToken)
        {
            string url = await this.apiPlatformClient
                .CareIdentityServiceClient
                .BuildLoginUrlAsync(cancellationToken);

            this.logger.LogInformation("Initiating CIS2 authentication.");

            return Redirect(url);
        }

        [Authorize]
        [HttpGet("session")]
        public IActionResult Session()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var expiresAtClaim = User.FindFirstValue(ClaimTypes.Expiration);
            string expiresAtIso = expiresAtClaim;

            if (!string.IsNullOrEmpty(expiresAtClaim))
            {
                if (DateTimeOffset.TryParse(expiresAtClaim, out var expiresAtDateTime))
                {
                    expiresAtIso = expiresAtDateTime.ToString("o");
                }
            }

            return Ok(new
            {
                sub = User.FindFirstValue(ClaimTypes.NameIdentifier),
                upn = User.FindFirstValue(ClaimTypes.Upn),
                name = User.FindFirstValue(ClaimTypes.Name),
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                expiresAt = expiresAtIso
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutEndpoint = configuration["CIS:LogoutEndpoint"];
            var postLogoutRedirectUri = configuration["CIS:PostLogoutRedirectUri"];
            var idToken = User.FindFirstValue("id_token");

            if (User.Identity?.IsAuthenticated == true)
            {
                HttpContext.Session.Clear();
                secureTokenStorage.ClearTokens(HttpContext);
                await HttpContext.SignOutAsync("bff-cookie");
            }

            var logoutUrl = $"{logoutEndpoint}" +
                $"?id_token_hint={idToken}" +
                $"&post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}";

            logger.LogInformation("User logged out, redirecting to CIS2 logout");

            return Ok(new { logoutUrl });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing");
            }

            var expectedCRSFState = secureTokenStorage.GetCSRFState(HttpContext);

            if (string.IsNullOrEmpty(expectedCRSFState) || state != expectedCRSFState)
            {
                return BadRequest("Invalid state parameter");
            }

            secureTokenStorage.ClearCSRFState(HttpContext);

            try
            {
                var client = httpClientFactory.CreateClient();
                var tokenEndpoint = configuration["CIS:TokenEndpoint"];
                var clientId = configuration["CIS:ClientId"];
                var clientSecret = configuration["CIS:ClientSecret"];
                var redirectUri = configuration["CIS:RedirectUri"];

                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                });

                var response = await client.PostAsync(tokenEndpoint, tokenRequest);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<TokenResult>(json);

                if (token == null)
                {
                    throw new Exception("Could not Process token");
                }

                logger.LogInformation(
                    $"Token response - ID Token present: {!string.IsNullOrEmpty(token.IdToken)}");

                if (string.IsNullOrEmpty(token.IdToken))
                {
                    logger.LogWarning("ID Token is missing from CIS2 response, logout may not work");
                }

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                var userInfoEndpoint = configuration["CIS:UserInfoEndpoint"];
                var userInfoResponse = await client.GetAsync(userInfoEndpoint);
                userInfoResponse.EnsureSuccessStatusCode();
                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<NhsUserInfo>(userInfoJson);

                if (userInfo == null)
                {
                    throw new Exception("Could not Process User Info");
                }

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.NhsIdUserUid == userInfo.NhsIdUserUid);

                if (user == null)
                {
                    user = new User
                    {
                        NhsIdUserUid = userInfo.NhsIdUserUid,
                        Name = userInfo.Name,
                        Sub = userInfo.Sub,
                        RawUserInfo = userInfoJson,
                        LastLoginAt = DateTime.UtcNow,
                        IsAuthorised = false,
                        CreatedBy = "HealthCareWorkerLogin",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "HealthCareWorkerLogin",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Users.Add(user);
                }
                else
                {
                    user.LastLoginAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();

                if (!user.IsAuthorised)
                {
                    HttpContext.Session.Clear();
                    await HttpContext.SignOutAsync("bff-cookie");

                    return Redirect("/unauthorised");
                }

                var expiresAt = DateTimeOffset.UtcNow
                    .AddSeconds(int.Parse(token.RefreshTokenExpiresIn));

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
                        new Claim(ClaimTypes.Name, userInfo.Name),
                        new Claim(ClaimTypes.Upn, userInfo.NhsIdUserUid),
                        new Claim(ClaimTypes.Expiration, expiresAt.ToString("o")),
                        new Claim("id_token", token.IdToken ?? string.Empty),
                    };

                foreach (var role in userInfo.NhsIdNrbacRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                }

                var identity = new ClaimsIdentity(claims, "OAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("bff-cookie", principal);

                secureTokenStorage.StoreAccessToken(
                    HttpContext,
                    token.AccessToken,
                    int.Parse(token.ExpiresIn));

                secureTokenStorage.StoreRefreshToken(
                    HttpContext,
                    token.RefreshToken,
                    int.Parse(token.RefreshTokenExpiresIn));

                return Redirect("/home");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during OAuth callback");

                return StatusCode(500, "Authentication failed");
            }
        }
    }
}