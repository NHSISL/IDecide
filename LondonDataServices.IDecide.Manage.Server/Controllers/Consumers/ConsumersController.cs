// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumersController : RESTFulController
    {
        private readonly IConsumerService consumerService;

        public ConsumersController(IConsumerService consumerService) =>
            this.consumerService = consumerService;

        [HttpPost]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
        public async ValueTask<ActionResult<Consumer>> PostConsumerAsync([FromBody] Consumer consumer)
        {
            try
            {
                Consumer addedConsumer =
                    await this.consumerService.AddConsumerAsync(consumer);

                return Created(addedConsumer);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                return BadRequest(consumerValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
               when (consumerDependencyValidationException.InnerException is AlreadyExistsConsumerException)
            {
                return Conflict(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                return BadRequest(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                return InternalServerError(consumerDependencyException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                return InternalServerError(consumerServiceException);
            }
        }

        [HttpGet]
        [ConfigurableEnableQuery]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
        public async ValueTask<ActionResult<IQueryable<Consumer>>> Get()
        {
            try
            {
                IQueryable<Consumer> retrievedConsumers =
                    await this.consumerService.RetrieveAllConsumersAsync();

                return Ok(retrievedConsumers);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                return InternalServerError(consumerDependencyException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                return InternalServerError(consumerServiceException);
            }
        }

        [HttpGet("{consumerId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
        public async ValueTask<ActionResult<Consumer>> GetConsumerByIdAsync(Guid consumerId)
        {
            try
            {
                Consumer consumer = await this.consumerService.RetrieveConsumerByIdAsync(consumerId);

                return Ok(consumer);
            }
            catch (ConsumerValidationException consumerValidationException)
                when (consumerValidationException.InnerException is NotFoundConsumerException)
            {
                return NotFound(consumerValidationException.InnerException);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                return BadRequest(consumerValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                return BadRequest(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                return InternalServerError(consumerDependencyException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                return InternalServerError(consumerServiceException);
            }
        }

        [HttpPut]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
        public async ValueTask<ActionResult<Consumer>> PutConsumerAsync([FromBody] Consumer consumer)
        {
            try
            {
                Consumer modifiedConsumer =
                    await this.consumerService.ModifyConsumerAsync(consumer);

                return Ok(modifiedConsumer);
            }
            catch (ConsumerValidationException consumerValidationException)
                when (consumerValidationException.InnerException is NotFoundConsumerException)
            {
                return NotFound(consumerValidationException.InnerException);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                return BadRequest(consumerValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
               when (consumerDependencyValidationException.InnerException is AlreadyExistsConsumerException)
            {
                return Conflict(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                return BadRequest(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                return InternalServerError(consumerDependencyException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                return InternalServerError(consumerServiceException);
            }
        }

        [HttpDelete("{consumerId}")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
        public async ValueTask<ActionResult<Consumer>> DeleteConsumerByIdAsync(Guid consumerId)
        {
            try
            {
                Consumer deletedConsumer =
                    await this.consumerService.RemoveConsumerByIdAsync(consumerId);

                return Ok(deletedConsumer);
            }
            catch (ConsumerValidationException consumerValidationException)
                when (consumerValidationException.InnerException is NotFoundConsumerException)
            {
                return NotFound(consumerValidationException.InnerException);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                return BadRequest(consumerValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
                when (consumerDependencyValidationException.InnerException is LockedConsumerException)
            {
                return Locked(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                return BadRequest(consumerDependencyValidationException.InnerException);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                return InternalServerError(consumerDependencyException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                return InternalServerError(consumerServiceException);
            }
        }
    }
}
