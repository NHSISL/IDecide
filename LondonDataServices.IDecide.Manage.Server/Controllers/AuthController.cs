// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly ApplicationDbContext context;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            ISecureTokenStorage secureTokenStorage,
            StorageBroker storageBroker,
           ApplicationDbContext context)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.secureTokenStorage = secureTokenStorage;
            this.context = context;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var clientId = configuration["CIS:ClientId"];
            var redirectUri = configuration["CIS:RedirectUri"];
            var authEndpoint = configuration["CIS:AuthEndpoint"];
            var acrValues = configuration["CIS:AALLevel"];
            var stateBytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(stateBytes);

            var csrfState = Convert.ToBase64String(stateBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            secureTokenStorage.StoreCSRFState(HttpContext, csrfState);
            await HttpContext.Session.CommitAsync();

            var authUrl = $"{authEndpoint}" +
                $"?client_id={clientId}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&response_type=code" +
                $"&state={csrfState}" +
                (string.IsNullOrEmpty(acrValues) ? "" : $"&acr_values={acrValues}");

            logger.LogInformation("Initiating CIS2 authentication with state parameter");

            return Redirect(authUrl);
        }

        [AllowAnonymous]
        [HttpGet("session")]
        public IActionResult Session()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var expiresAtClaim = User.FindFirstValue(ClaimTypes.Expiration);
            DateTime? expiresAt = string.IsNullOrEmpty(expiresAtClaim)
                ? null
                : DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAtClaim)).UtcDateTime;

            return Ok(new
            {
                sub = User.FindFirstValue(ClaimTypes.NameIdentifier),
                upn = User.FindFirstValue(ClaimTypes.Upn),
                name = User.FindFirstValue(ClaimTypes.Name),
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                expiresAt = expiresAt
            });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync("bff-cookie");
            }

            return Ok();
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

                var user = await context.Users.FirstOrDefaultAsync(u => u.NhsIdUserUid == userInfo.NhsIdUserUid);

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

                var expiresAt = DateTimeOffset.UtcNow.AddSeconds(int.Parse(token.ExpiresIn));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
                    new Claim(ClaimTypes.Name, userInfo.Name),
                    new Claim(ClaimTypes.Upn, userInfo.NhsIdUserUid),
                    new Claim(ClaimTypes.Expiration, expiresAt.ToUnixTimeSeconds().ToString()),
                };

                foreach (var role in userInfo.NhsIdNrbacRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
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