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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedBadRequestObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.GetConsumerAdoptionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedInternalServerErrorObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.GetConsumerAdoptionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundConsumerAdoptionException =
                new NotFoundConsumerAdoptionException(
                    message: someMessage);

            var consumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: someMessage,
                    innerException: notFoundConsumerAdoptionException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundConsumerAdoptionException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedNotFoundObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(consumerAdoptionValidationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.GetConsumerAdoptionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
