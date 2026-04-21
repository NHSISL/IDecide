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
using System.Web;
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
        public async Task<IActionResult> Session(CancellationToken cancellationToken)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            string accessToken = await this.apiPlatformClient
           .CareIdentityServiceClient
           .GetAccessTokenAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                sub = User.FindFirstValue(ClaimTypes.NameIdentifier),
                upn = User.FindFirstValue(ClaimTypes.Upn),
                name = User.FindFirstValue(ClaimTypes.Name),
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await this.apiPlatformClient
                .CareIdentityServiceClient
                .LogoutAsync(cancellationToken);

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("bff-cookie");

            return Redirect(@"\");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(code))
            {
                this.logger.LogWarning("Authorization code is missing from callback");
                return BadRequest("Authorization code is missing");
            }

            if (string.IsNullOrEmpty(state))
            {
                this.logger.LogWarning("State parameter is missing from callback");
                return BadRequest("State parameter is missing");
            }

            try
            {
                // ✅ Use SDK's built-in method - handles token storage correctly
                var userInfo = await this.apiPlatformClient
                    .CareIdentityServiceClient
                    .GetUserInfoAsync(code, state, cancellationToken);

                string userInfoJson = JsonSerializer.Serialize(userInfo);

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.NhsIdUserUid == userInfo.NhsIdUserUid, cancellationToken);

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
                    user.RawUserInfo = userInfoJson;
                }

                await context.SaveChangesAsync(cancellationToken);

                if (!user.IsAuthorised)
                {
                    await this.apiPlatformClient
                        .CareIdentityServiceClient
                        .LogoutAsync(cancellationToken);

                    HttpContext.Session.Clear();
                    await HttpContext.SignOutAsync("bff-cookie");

                    return Redirect("/unauthorised");
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
            new Claim(ClaimTypes.Name, userInfo.Name),
            new Claim(ClaimTypes.Upn, userInfo.NhsIdUserUid),
        };

                foreach (var role in userInfo.NhsIdNrbacRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                }

                var identity = new ClaimsIdentity(claims, "OAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("bff-cookie", principal);

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