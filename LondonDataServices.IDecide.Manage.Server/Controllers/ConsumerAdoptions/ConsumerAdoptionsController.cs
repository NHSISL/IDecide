// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerAdoptionsController : RESTFulController
    {
        private readonly IConsumerAdoptionService consumerAdoptionService;

        public ConsumerAdoptionsController(IConsumerAdoptionService consumerAdoptionService) =>
            this.consumerAdoptionService = consumerAdoptionService;

        [HttpPost]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Consumers")]
        public async ValueTask<ActionResult<ConsumerAdoption>> PostConsumerAdoptionAsync([FromBody] ConsumerAdoption consumerAdoption)
        {
            try
            {
                ConsumerAdoption addedConsumerAdoption =
                    await this.consumerAdoptionService.AddConsumerAdoptionAsync(consumerAdoption);

                return Created(addedConsumerAdoption);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
            {
                return BadRequest(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
               when (consumerAdoptionDependencyValidationException.InnerException is AlreadyExistsConsumerAdoptionException)
            {
                return Conflict(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
            {
                return BadRequest(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyException consumerAdoptionDependencyException)
            {
                return InternalServerError(consumerAdoptionDependencyException);
            }
            catch (ConsumerAdoptionServiceException consumerAdoptionServiceException)
            {
                return InternalServerError(consumerAdoptionServiceException);
            }
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Consumers")]
        public async ValueTask<ActionResult<IQueryable<ConsumerAdoption>>> Get()
        {
            try
            {
                IQueryable<ConsumerAdoption> retrievedConsumerAdoptions =
                    await this.consumerAdoptionService.RetrieveAllConsumerAdoptionsAsync();

                return Ok(retrievedConsumerAdoptions);
            }
            catch (ConsumerAdoptionDependencyException consumerAdoptionDependencyException)
            {
                return InternalServerError(consumerAdoptionDependencyException);
            }
            catch (ConsumerAdoptionServiceException consumerAdoptionServiceException)
            {
                return InternalServerError(consumerAdoptionServiceException);
            }
        }

        [HttpGet("{consumerAdoptionId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Consumers")]
        public async ValueTask<ActionResult<ConsumerAdoption>> GetConsumerAdoptionByIdAsync(Guid consumerAdoptionId)
        {
            try
            {
                ConsumerAdoption consumerAdoption = await this.consumerAdoptionService.RetrieveConsumerAdoptionByIdAsync(consumerAdoptionId);

                return Ok(consumerAdoption);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
                when (consumerAdoptionValidationException.InnerException is NotFoundConsumerAdoptionException)
            {
                return NotFound(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
            {
                return BadRequest(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
            {
                return BadRequest(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyException consumerAdoptionDependencyException)
            {
                return InternalServerError(consumerAdoptionDependencyException);
            }
            catch (ConsumerAdoptionServiceException consumerAdoptionServiceException)
            {
                return InternalServerError(consumerAdoptionServiceException);
            }
        }

        [HttpPut]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Consumers")]
        public async ValueTask<ActionResult<ConsumerAdoption>> PutConsumerAdoptionAsync([FromBody] ConsumerAdoption consumerAdoption)
        {
            try
            {
                ConsumerAdoption modifiedConsumerAdoption =
                    await this.consumerAdoptionService.ModifyConsumerAdoptionAsync(consumerAdoption);

                return Ok(modifiedConsumerAdoption);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
                when (consumerAdoptionValidationException.InnerException is NotFoundConsumerAdoptionException)
            {
                return NotFound(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
            {
                return BadRequest(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
               when (consumerAdoptionDependencyValidationException.InnerException is AlreadyExistsConsumerAdoptionException)
            {
                return Conflict(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
            {
                return BadRequest(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyException consumerAdoptionDependencyException)
            {
                return InternalServerError(consumerAdoptionDependencyException);
            }
            catch (ConsumerAdoptionServiceException consumerAdoptionServiceException)
            {
                return InternalServerError(consumerAdoptionServiceException);
            }
        }

        [HttpDelete("{consumerAdoptionId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Consumers")]
        public async ValueTask<ActionResult<ConsumerAdoption>> DeleteConsumerAdoptionByIdAsync(Guid consumerAdoptionId)
        {
            try
            {
                ConsumerAdoption deletedConsumerAdoption =
                    await this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(consumerAdoptionId);

                return Ok(deletedConsumerAdoption);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
                when (consumerAdoptionValidationException.InnerException is NotFoundConsumerAdoptionException)
            {
                return NotFound(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
            {
                return BadRequest(consumerAdoptionValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
                when (consumerAdoptionDependencyValidationException.InnerException is LockedConsumerAdoptionException)
            {
                return Locked(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
            {
                return BadRequest(consumerAdoptionDependencyValidationException.InnerException);
            }
            catch (ConsumerAdoptionDependencyException consumerAdoptionDependencyException)
            {
                return InternalServerError(consumerAdoptionDependencyException);
            }
            catch (ConsumerAdoptionServiceException consumerAdoptionServiceException)
            {
                return InternalServerError(consumerAdoptionServiceException);
            }
        }
    }
}
