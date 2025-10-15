// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
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

        [HttpPost("PatientDecision")]
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

        [HttpGet("PatientDecision")]
        public async ValueTask<ActionResult<List<Decision>>> GetPatientDecisionsAsync(
            [FromQuery] DateTimeOffset? from = null,
            [FromQuery] string decisionType = null)
        {
            try
            {
                List<Decision> decisions = await this.decisionOrchestrationService
                    .RetrieveAllPendingAdoptionDecisionsForConsumer(
                        changesSinceDate: from ?? default,
                        decisionType: decisionType);

                return Ok(decisions);
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
