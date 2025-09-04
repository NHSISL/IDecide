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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PutDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedInternalServerErrorObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PutDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();
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
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(decisionTypeValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PutDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsDecisionTypeErrorOccurredAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsDecisionTypeException =
                new AlreadyExistsDecisionTypeException(
                    message: someMessage,
                    innerException: someInnerException);

            var decisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsDecisionTypeException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsDecisionTypeException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedConflictObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(decisionTypeDependencyValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PutDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.ModifyDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
