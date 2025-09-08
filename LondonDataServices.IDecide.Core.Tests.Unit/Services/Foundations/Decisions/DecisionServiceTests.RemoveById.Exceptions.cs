// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            SqlException sqlException = GetSqlException();

            var failedDecisionStorageException =
                new FailedDecisionStorageException(
                    message: "Failed decision storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: failedDecisionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(randomDecision.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.RemoveDecisionByIdAsync(randomDecision.Id);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    addDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(randomDecision.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someDecisionId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedDecisionException =
                new LockedDecisionException(
                    message: "Locked decision record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedDecisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: lockedDecisionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Decision> removeDecisionByIdTask =
                this.decisionService.RemoveDecisionByIdAsync(someDecisionId);

            DecisionDependencyValidationException actualDecisionDependencyValidationException =
                await Assert.ThrowsAsync<DecisionDependencyValidationException>(
                    removeDecisionByIdTask.AsTask);

            // then
            actualDecisionDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someDecisionId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedDecisionStorageException =
                new FailedDecisionStorageException(
                    message: "Failed decision storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: failedDecisionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Decision> deleteDecisionTask =
                this.decisionService.RemoveDecisionByIdAsync(someDecisionId);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    deleteDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someDecisionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDecisionServiceException =
                new FailedDecisionServiceException(
                    message: "Failed decision service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionServiceException =
                new DecisionServiceException(
                    message: "Decision service error occurred, contact support.",
                    innerException: failedDecisionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Decision> removeDecisionByIdTask =
                this.decisionService.RemoveDecisionByIdAsync(someDecisionId);

            DecisionServiceException actualDecisionServiceException =
                await Assert.ThrowsAsync<DecisionServiceException>(
                    removeDecisionByIdTask.AsTask);

            // then
            actualDecisionServiceException.Should()
                .BeEquivalentTo(expectedDecisionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}