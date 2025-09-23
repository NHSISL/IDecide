// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionsController : RESTFulController
    {
        private readonly IDecisionService decisionService;

        public DecisionsController(IDecisionService decisionService) =>
            this.decisionService = decisionService;

        [HttpPost]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,Decisions.Create,iDecide.Manage")]
        public async ValueTask<ActionResult<Decision>> PostDecisionAsync([FromBody] Decision decision)
        {
            try
            {
                Decision addedDecision =
                    await this.decisionService.AddDecisionAsync(decision);

                return Created(addedDecision);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                return BadRequest(decisionValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
               when (decisionDependencyValidationException.InnerException is AlreadyExistsDecisionException)
            {
                return Conflict(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                return BadRequest(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                return InternalServerError(decisionDependencyException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                return InternalServerError(decisionServiceException);
            }
        }

        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,Decisions.Read")]
        public async ValueTask<ActionResult<IQueryable<Decision>>> Get()
        {
            try
            {
                IQueryable<Decision> retrievedDecisions =
                    await this.decisionService.RetrieveAllDecisionsAsync();

                return Ok(retrievedDecisions);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                return InternalServerError(decisionDependencyException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                return InternalServerError(decisionServiceException);
            }
        }

        [HttpGet("{decisionId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,Decisions.Read")]
        public async ValueTask<ActionResult<Decision>> GetDecisionByIdAsync(Guid decisionId)
        {
            try
            {
                Decision decision = await this.decisionService.RetrieveDecisionByIdAsync(decisionId);

                return Ok(decision);
            }
            catch (DecisionValidationException decisionValidationException)
                when (decisionValidationException.InnerException is NotFoundDecisionException)
            {
                return NotFound(decisionValidationException.InnerException);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                return BadRequest(decisionValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                return BadRequest(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                return InternalServerError(decisionDependencyException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                return InternalServerError(decisionServiceException);
            }
        }

        [HttpPut]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,Decisions.Update")]
        public async ValueTask<ActionResult<Decision>> PutDecisionAsync([FromBody] Decision decision)
        {
            try
            {
                Decision modifiedDecision =
                    await this.decisionService.ModifyDecisionAsync(decision);

                return Ok(modifiedDecision);
            }
            catch (DecisionValidationException decisionValidationException)
                when (decisionValidationException.InnerException is NotFoundDecisionException)
            {
                return NotFound(decisionValidationException.InnerException);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                return BadRequest(decisionValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
               when (decisionDependencyValidationException.InnerException is AlreadyExistsDecisionException)
            {
                return Conflict(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                return BadRequest(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                return InternalServerError(decisionDependencyException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                return InternalServerError(decisionServiceException);
            }
        }

        [HttpDelete("{decisionId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,Decisions.Delete")]
        public async ValueTask<ActionResult<Decision>> DeleteDecisionByIdAsync(Guid decisionId)
        {
            try
            {
                Decision deletedDecision =
                    await this.decisionService.RemoveDecisionByIdAsync(decisionId);

                return Ok(deletedDecision);
            }
            catch (DecisionValidationException decisionValidationException)
                when (decisionValidationException.InnerException is NotFoundDecisionException)
            {
                return NotFound(decisionValidationException.InnerException);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                return BadRequest(decisionValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
                when (decisionDependencyValidationException.InnerException is LockedDecisionException)
            {
                return Locked(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                return BadRequest(decisionDependencyValidationException.InnerException);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                return InternalServerError(decisionDependencyException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                return InternalServerError(decisionServiceException);
            }
        }
    }
}
