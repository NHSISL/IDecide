// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Decision someDecision = CreateRandomDecision();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PutDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Decision someDecision = CreateRandomDecision();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PutDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();
            string someMessage = GetRandomString();

            var notFoundDecisionException =
                new NotFoundDecisionException(
                    message: someMessage);

            var decisionValidationException =
                new DecisionValidationException(
                    message: someMessage,
                    innerException: notFoundDecisionException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundDecisionException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedNotFoundObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(decisionValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PutDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsDecisionErrorOccurredAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsDecisionException =
                new AlreadyExistsDecisionException(
                    message: someMessage,
                    innerException: someInnerException);

            var decisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsDecisionException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsDecisionException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedConflictObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(decisionDependencyValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PutDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.ModifyDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
