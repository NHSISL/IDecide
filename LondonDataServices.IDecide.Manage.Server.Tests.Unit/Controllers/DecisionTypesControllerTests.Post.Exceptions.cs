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

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PostDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedBadRequestObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PostDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsDecisionTypeErrorOccurredAsync()
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
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(decisionTypeDependencyValidationException);

            // when
            ActionResult<DecisionType> actualActionResult =
                await this.decisionTypesController.PostDecisionTypeAsync(someDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.AddDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}