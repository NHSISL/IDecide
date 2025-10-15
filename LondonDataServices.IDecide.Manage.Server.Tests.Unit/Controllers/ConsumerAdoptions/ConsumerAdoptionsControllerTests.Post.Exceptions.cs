// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedBadRequestObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PostConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedInternalServerErrorObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PostConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsConsumerAdoptionErrorOccurredAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsConsumerAdoptionException =
                new AlreadyExistsConsumerAdoptionException(
                    message: someMessage,
                    innerException: someInnerException);

            var consumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsConsumerAdoptionException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsConsumerAdoptionException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedConflictObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(consumerAdoptionDependencyValidationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PostConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.AddConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}