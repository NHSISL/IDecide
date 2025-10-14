// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerStatusController : RESTFulController
    {
        private readonly IConsumerOrchestrationService consumerOrchestrationService;

        public ConsumerStatusController(IConsumerOrchestrationService consumerOrchestrationService) =>
            this.consumerOrchestrationService = consumerOrchestrationService;

        [HttpPost("AdoptPatientDecisions")]
        public async ValueTask<ActionResult> AdoptPatientDecisionsAsync([FromBody] List<Decision> decisions)
        {
            try
            {
                await this.consumerOrchestrationService.AdoptPatientDecisions(decisions);

                return Ok();
            }
            catch (ConsumerOrchestrationValidationException consumerOrchestrationValidationException)
            {
                return BadRequest(consumerOrchestrationValidationException.InnerException);
            }
            catch (ConsumerOrchestrationDependencyValidationException
                consumerOrchestrationDependencyValidationException)
            {
                return BadRequest(consumerOrchestrationDependencyValidationException.InnerException);
            }
            catch (ConsumerOrchestrationDependencyException consumerOrchestrationDependencyException)
            {
                return InternalServerError(consumerOrchestrationDependencyException);
            }
            catch (ConsumerOrchestrationServiceException consumerOrchestrationServiceException)
            {
                return InternalServerError(consumerOrchestrationServiceException);
            }
        }
    }
}
