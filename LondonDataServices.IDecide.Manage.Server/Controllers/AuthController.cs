// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Manage.Server.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : RESTFulController
    {
        private readonly INhsDigitalApiOrchestrationService nhsDigitalApiOrchestrationService;
        private readonly ILogger<AuthController> logger;

        public AuthController(
            INhsDigitalApiOrchestrationService nhsDigitalApiOrchestrationService,
            ILogger<AuthController> logger)
        {
            this.nhsDigitalApiOrchestrationService = nhsDigitalApiOrchestrationService;
            this.logger = logger;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(CancellationToken cancellationToken)
        {
            try
            {
                string url = await this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(cancellationToken);

                this.logger.LogInformation("Initiating CIS2 authentication.");

                return Redirect(url);
            }
            catch (NhsDigitalApiOrchestrationValidationException nhsDigitalApiOrchestrationValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyValidationException
                nhsDigitalApiOrchestrationDependencyValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationDependencyValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyException nhsDigitalApiOrchestrationDependencyException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationDependencyException);
            }
            catch (NhsDigitalApiOrchestrationServiceException nhsDigitalApiOrchestrationServiceException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationServiceException);
            }
        }

        [Authorize]
        [HttpGet("session")]
        public async Task<IActionResult> Session(CancellationToken cancellationToken)
        {
            try
            {
                string accessToken = await this.nhsDigitalApiOrchestrationService
                    .GetAccessTokenAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return Unauthorized();
                }

                return Ok(new SessionResponse
                {
                    Sub = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Upn = User.FindFirstValue(ClaimTypes.Upn),
                    Name = User.FindFirstValue(ClaimTypes.Name),
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
                });
            }
            catch (NhsDigitalApiOrchestrationValidationException nhsDigitalApiOrchestrationValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyValidationException
                nhsDigitalApiOrchestrationDependencyValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationDependencyValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyException nhsDigitalApiOrchestrationDependencyException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationDependencyException);
            }
            catch (NhsDigitalApiOrchestrationServiceException nhsDigitalApiOrchestrationServiceException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationServiceException);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            try
            {
                await this.nhsDigitalApiOrchestrationService.LogoutAsync(cancellationToken);
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync("bff-cookie");

                return Redirect(@"\");
            }
            catch (NhsDigitalApiOrchestrationValidationException nhsDigitalApiOrchestrationValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyValidationException
                nhsDigitalApiOrchestrationDependencyValidationException)
            {
                return BadRequest(nhsDigitalApiOrchestrationDependencyValidationException.InnerException);
            }
            catch (NhsDigitalApiOrchestrationDependencyException nhsDigitalApiOrchestrationDependencyException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationDependencyException);
            }
            catch (NhsDigitalApiOrchestrationServiceException nhsDigitalApiOrchestrationServiceException)
            {
                return InternalServerError(nhsDigitalApiOrchestrationServiceException);
            }
        }
    }
}
