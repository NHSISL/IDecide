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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.DeleteDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.DeleteDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
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
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionTypeValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.DeleteDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedDecisionTypeException =
                new LockedDecisionTypeException(
                    message: someMessage,
                    innerException: someInnerException);

            var decisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: someMessage,
                    innerException: lockedDecisionTypeException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedDecisionTypeException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedConflictObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(decisionTypeDependencyValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.DeleteDecisionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RemoveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
