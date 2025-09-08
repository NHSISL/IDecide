// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedBadRequestObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PostConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedInternalServerErrorObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PostConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsConsumerErrorOccurredAsync()
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsConsumerException =
                new AlreadyExistsConsumerException(
                    message: someMessage,
                    innerException: someInnerException);

            var consumerDependencyValidationException =
                new ConsumerDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsConsumerException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsConsumerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedConflictObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(consumerDependencyValidationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PostConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.AddConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}