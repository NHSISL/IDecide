// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientDecisionController : RESTFulController
    {
        private readonly IDecisionOrchestrationService decisionOrchestrationService;

        public PatientDecisionController(IDecisionOrchestrationService decisionOrchestrationService) =>
            this.decisionOrchestrationService = decisionOrchestrationService;

        [HttpPost("PostPatientDecision")]
        public async ValueTask<ActionResult> PostPatientDecisionAsync([FromBody] Decision decision)
        {
            try
            {
                await this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(decision);

                return Ok();
            }
            catch (DecisionOrchestrationValidationException decisionOrchestrationValidationException)
            {
                return BadRequest(decisionOrchestrationValidationException.InnerException);
            }
            catch (DecisionOrchestrationDependencyValidationException
                decisionOrchestrationDependencyValidationException)
            {
                return BadRequest(decisionOrchestrationDependencyValidationException.InnerException);
            }
            catch (DecisionOrchestrationDependencyException decisionOrchestrationDependencyException)
            {
                return InternalServerError(decisionOrchestrationDependencyException);
            }
            catch (DecisionOrchestrationServiceException decisionOrchestrationServiceException)
            {
                return InternalServerError(decisionOrchestrationServiceException);
            }
        }
    }
}
