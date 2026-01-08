// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionTypesController : RESTFulController
    {
        private readonly IDecisionTypeService decisionTypeService;

        public DecisionTypesController(IDecisionTypeService decisionTypeService) =>
            this.decisionTypeService = decisionTypeService;

        [HttpPost]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<DecisionType>> PostDecisionTypeAsync([FromBody] DecisionType decisionType)
        {
            try
            {
                DecisionType addedDecisionType =
                    await this.decisionTypeService.AddDecisionTypeAsync(decisionType);

                return Created(addedDecisionType);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
            {
                return BadRequest(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
               when (decisionTypeDependencyValidationException.InnerException is AlreadyExistsDecisionTypeException)
            {
                return Conflict(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
            {
                return BadRequest(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyException decisionTypeDependencyException)
            {
                return InternalServerError(decisionTypeDependencyException);
            }
            catch (DecisionTypeServiceException decisionTypeServiceException)
            {
                return InternalServerError(decisionTypeServiceException);
            }
        }

        [HttpGet]
        [ConfigurableEnableQuery]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<IQueryable<DecisionType>>> Get()
        {
            try
            {
                IQueryable<DecisionType> retrievedDecisionTypes =
                    await this.decisionTypeService.RetrieveAllDecisionTypesAsync();

                return Ok(retrievedDecisionTypes);
            }
            catch (DecisionTypeDependencyException decisionTypeDependencyException)
            {
                return InternalServerError(decisionTypeDependencyException);
            }
            catch (DecisionTypeServiceException decisionTypeServiceException)
            {
                return InternalServerError(decisionTypeServiceException);
            }
        }

        [HttpGet("{decisionTypeId}")]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<DecisionType>> GetDecisionTypeByIdAsync(Guid decisionTypeId)
        {
            try
            {
                DecisionType decisionType = await this.decisionTypeService.RetrieveDecisionTypeByIdAsync(decisionTypeId);

                return Ok(decisionType);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
                when (decisionTypeValidationException.InnerException is NotFoundDecisionTypeException)
            {
                return NotFound(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
            {
                return BadRequest(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
            {
                return BadRequest(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyException decisionTypeDependencyException)
            {
                return InternalServerError(decisionTypeDependencyException);
            }
            catch (DecisionTypeServiceException decisionTypeServiceException)
            {
                return InternalServerError(decisionTypeServiceException);
            }
        }

        [HttpPut]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<DecisionType>> PutDecisionTypeAsync([FromBody] DecisionType decisionType)
        {
            try
            {
                DecisionType modifiedDecisionType =
                    await this.decisionTypeService.ModifyDecisionTypeAsync(decisionType);

                return Ok(modifiedDecisionType);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
                when (decisionTypeValidationException.InnerException is NotFoundDecisionTypeException)
            {
                return NotFound(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
            {
                return BadRequest(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
               when (decisionTypeDependencyValidationException.InnerException is AlreadyExistsDecisionTypeException)
            {
                return Conflict(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
            {
                return BadRequest(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyException decisionTypeDependencyException)
            {
                return InternalServerError(decisionTypeDependencyException);
            }
            catch (DecisionTypeServiceException decisionTypeServiceException)
            {
                return InternalServerError(decisionTypeServiceException);
            }
        }

        [HttpDelete("{decisionTypeId}")]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<DecisionType>> DeleteDecisionTypeByIdAsync(Guid decisionTypeId)
        {
            try
            {
                DecisionType deletedDecisionType =
                    await this.decisionTypeService.RemoveDecisionTypeByIdAsync(decisionTypeId);

                return Ok(deletedDecisionType);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
                when (decisionTypeValidationException.InnerException is NotFoundDecisionTypeException)
            {
                return NotFound(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeValidationException decisionTypeValidationException)
            {
                return BadRequest(decisionTypeValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
                when (decisionTypeDependencyValidationException.InnerException is LockedDecisionTypeException)
            {
                return Locked(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyValidationException decisionTypeDependencyValidationException)
            {
                return BadRequest(decisionTypeDependencyValidationException.InnerException);
            }
            catch (DecisionTypeDependencyException decisionTypeDependencyException)
            {
                return InternalServerError(decisionTypeDependencyException);
            }
            catch (DecisionTypeServiceException decisionTypeServiceException)
            {
                return InternalServerError(decisionTypeServiceException);
            }
        }
    }
}
