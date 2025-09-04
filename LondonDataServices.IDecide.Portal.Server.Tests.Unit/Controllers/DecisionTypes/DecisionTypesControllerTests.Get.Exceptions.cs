// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
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
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.GetDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.GetDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundDecisionTypeException =
                new NotFoundDecisionTypeException(
                    message: someMessage);

            var decisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: someMessage,
                    innerException: notFoundDecisionTypeException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundDecisionTypeException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedNotFoundObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionTypeValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.GetDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
