// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();
            SqlException sqlException = GetSqlException();

            var failedDecisionTypeStorageException =
                new FailedDecisionTypeStorageException(
                    message: "Failed decisionType storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: failedDecisionTypeStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDecisionTypeAlreadyExsitsAndLogItAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType alreadyExistsDecisionType = randomDecisionType;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsDecisionTypeException =
                new AlreadyExistsDecisionTypeException(
                    message: "DecisionType with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedDecisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please try again.",
                    innerException: alreadyExistsDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(alreadyExistsDecisionType);

            // then
            DecisionTypeDependencyValidationException actualDecisionTypeDependencyValidationException =
                await Assert.ThrowsAsync<DecisionTypeDependencyValidationException>(
                    addDecisionTypeTask.AsTask);

            actualDecisionTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
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
            DecisionType someDecisionType = CreateRandomDecisionType();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidDecisionTypeReferenceException =
                new InvalidDecisionTypeReferenceException(
                    message: "Invalid decisionType reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedDecisionTypeValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please try again.",
                    innerException: invalidDecisionTypeReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

            // then
            DecisionTypeDependencyValidationException actualDecisionTypeDependencyValidationException =
                await Assert.ThrowsAsync<DecisionTypeDependencyValidationException>(
                    addDecisionTypeTask.AsTask);

            actualDecisionTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
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
            DecisionType someDecisionType = CreateRandomDecisionType();

            var databaseUpdateException =
                new DbUpdateException();

            var failedDecisionTypeStorageException =
                new FailedDecisionTypeStorageException(
                    message: "Failed decisionType storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedDecisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: failedDecisionTypeStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
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
            DecisionType someDecisionType = CreateRandomDecisionType();
            var serviceException = new Exception();

            var failedDecisionTypeServiceException =
                new FailedDecisionTypeServiceException(
                    message: "Failed decisionType service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionTypeServiceException =
                new DecisionTypeServiceException(
                    message: "DecisionType service error occurred, contact support.",
                    innerException: failedDecisionTypeServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

            DecisionTypeServiceException actualDecisionTypeServiceException =
                await Assert.ThrowsAsync<DecisionTypeServiceException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<DecisionType>()),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}