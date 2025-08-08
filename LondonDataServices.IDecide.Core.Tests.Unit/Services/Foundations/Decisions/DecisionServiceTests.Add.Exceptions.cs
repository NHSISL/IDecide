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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();
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
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(someDecision);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    addDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDecisionAlreadyExsitsAndLogItAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision alreadyExistsDecision = randomDecision;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsDecisionException =
                new AlreadyExistsDecisionException(
                    message: "Decision with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedDecisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: alreadyExistsDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(alreadyExistsDecision);

            // then
            DecisionDependencyValidationException actualDecisionDependencyValidationException =
                await Assert.ThrowsAsync<DecisionDependencyValidationException>(
                    addDecisionTask.AsTask);

            actualDecisionDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
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

            var expectedDecisionValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: invalidDecisionReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(someDecision);

            // then
            DecisionDependencyValidationException actualDecisionDependencyValidationException =
                await Assert.ThrowsAsync<DecisionDependencyValidationException>(
                    addDecisionTask.AsTask);

            actualDecisionDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();

            var databaseUpdateException =
                new DbUpdateException();

            var failedDecisionStorageException =
                new FailedDecisionStorageException(
                    message: "Failed decision storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedDecisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: failedDecisionStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(someDecision);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    addDecisionTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Decision someDecision = CreateRandomDecision();
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
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(someDecision);

            DecisionServiceException actualDecisionServiceException =
                await Assert.ThrowsAsync<DecisionServiceException>(
                    addDecisionTask.AsTask);

            // then
            actualDecisionServiceException.Should()
                .BeEquivalentTo(expectedDecisionServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Decision>()),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}