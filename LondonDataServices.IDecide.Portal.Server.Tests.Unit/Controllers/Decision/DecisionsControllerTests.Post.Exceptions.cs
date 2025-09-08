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

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Decision someDecision = CreateRandomDecision();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PostDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Decision someDecision = CreateRandomDecision();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedInternalServerErrorObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PostDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsDecisionErrorOccurredAsync()
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
                service.AddDecisionAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(decisionDependencyValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.PostDecisionAsync(someDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}