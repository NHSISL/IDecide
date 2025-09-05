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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.DeleteDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedInternalServerErrorObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.DeleteDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.DeleteDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedDecisionException =
                new LockedDecisionException(
                    message: someMessage,
                    innerException: someInnerException);

            var decisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: someMessage,
                    innerException: lockedDecisionException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedDecisionException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedConflictObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionDependencyValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.DeleteDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RemoveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
