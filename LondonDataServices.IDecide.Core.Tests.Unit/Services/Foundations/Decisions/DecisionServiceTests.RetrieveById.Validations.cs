// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidDecisionId = Guid.Empty;

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.Id),
                values: "Id is required");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            // when
            ValueTask<Decision> retrieveDecisionByIdTask =
                this.decisionService.RetrieveDecisionByIdAsync(invalidDecisionId);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    retrieveDecisionByIdTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfDecisionIsNotFoundAndLogItAsync()
        {
            //given
            Guid someDecisionId = Guid.NewGuid();
            Decision noDecision = null;

            var notFoundDecisionException = new NotFoundDecisionException(
                $"Couldn't find decision type with decisionId: {someDecisionId}.");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: notFoundDecisionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noDecision);

            //when
            ValueTask<Decision> retrieveDecisionByIdTask =
                this.decisionService.RetrieveDecisionByIdAsync(someDecisionId);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    retrieveDecisionByIdTask.AsTask);

            //then
            actualDecisionValidationException.Should().BeEquivalentTo(expectedDecisionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}