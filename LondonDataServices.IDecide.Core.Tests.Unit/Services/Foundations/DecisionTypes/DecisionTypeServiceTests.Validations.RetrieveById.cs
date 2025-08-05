// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidDecisionTypeId = Guid.Empty;

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Id),
                values: "Id is required");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> retrieveDecisionTypeByIdTask =
                this.decisionTypeService.RetrieveDecisionTypeByIdAsync(invalidDecisionTypeId);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    retrieveDecisionTypeByIdTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfDecisionTypeIsNotFoundAndLogItAsync()
        {
            //given
            Guid someDecisionTypeId = Guid.NewGuid();
            DecisionType noDecisionType = null;

            var notFoundDecisionTypeException = new NotFoundDecisionTypeException(
                $"Couldn't find decision type with decisionTypeId: {someDecisionTypeId}.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: notFoundDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noDecisionType);

            //when
            ValueTask<DecisionType> retrieveDecisionTypeByIdTask =
                this.decisionTypeService.RetrieveDecisionTypeByIdAsync(someDecisionTypeId);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    retrieveDecisionTypeByIdTask.AsTask);

            //then
            actualDecisionTypeValidationException.Should().BeEquivalentTo(expectedDecisionTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}