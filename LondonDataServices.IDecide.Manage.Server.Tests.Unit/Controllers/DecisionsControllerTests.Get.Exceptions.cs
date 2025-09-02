// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Manage.Server.Models.Foundations.Decisions.Exceptions;
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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.GetDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<Decision>(expectedBadRequestObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.GetDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
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
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionValidationException);

            // when
            ActionResult<Decision> actualActionResult =
                await this.decisionsController.GetDecisionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
