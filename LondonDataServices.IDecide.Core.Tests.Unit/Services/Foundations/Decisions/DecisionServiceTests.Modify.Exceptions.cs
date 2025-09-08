// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
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

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(randomDecision);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<Decision>(), It.IsAny<Decision>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidDecisionReferenceException =
                new InvalidDecisionReferenceException(
                    message: "Invalid decision reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            DecisionDependencyValidationException expectedDecisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: invalidDecisionReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(someDecision);

            DecisionDependencyValidationException actualDecisionDependencyValidationException =
                await Assert.ThrowsAsync<DecisionDependencyValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDecisionDependencyValidationException))),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<Decision>(), It.IsAny<Decision>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            var databaseUpdateException = new DbUpdateException();

            var failedDecisionStorageException =
                new FailedDecisionStorageException(
                    message: "Failed decision storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedDecisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: failedDecisionStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(randomDecision);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<Decision>(), It.IsAny<Decision>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedDecisionException =
                new LockedDecisionException(
                    message: "Locked decision record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedDecisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: lockedDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(randomDecision);

            DecisionDependencyValidationException actualDecisionDependencyValidationException =
                await Assert.ThrowsAsync<DecisionDependencyValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<Decision>(), It.IsAny<Decision>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            var serviceException = new Exception();

            var failedDecisionServiceException =
                new FailedDecisionServiceException(
                    message: "Failed decision service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionServiceException =
                new DecisionServiceException(
                    message: "Decision service error occurred, contact support.",
                    innerException: failedDecisionServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()))
                    .Throws(serviceException);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(randomDecision);

            DecisionServiceException actualDecisionServiceException =
                await Assert.ThrowsAsync<DecisionServiceException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionServiceException.Should()
                .BeEquivalentTo(expectedDecisionServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<Decision>(), It.IsAny<Decision>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}