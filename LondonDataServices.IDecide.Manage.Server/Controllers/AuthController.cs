// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
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
    }
}
