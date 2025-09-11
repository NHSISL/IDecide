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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedBadRequestObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PutConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedInternalServerErrorObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PutConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();
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
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(consumerAdoptionValidationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PutConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsConsumerAdoptionErrorOccurredAsync()
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
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(consumerAdoptionDependencyValidationException);

            // when
            ActionResult<ConsumerAdoption> actualActionResult =
                await this.consumerAdoptionsController.PutConsumerAdoptionAsync(someConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.ModifyConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
