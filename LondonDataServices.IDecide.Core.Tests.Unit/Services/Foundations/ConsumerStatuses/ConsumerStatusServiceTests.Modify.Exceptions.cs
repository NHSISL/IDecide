// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            SqlException sqlException = GetSqlException();

            var failedConsumerStatusStorageException =
                new FailedConsumerStatusStorageException(
                    message: "Failed consumerStatus storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerStatusDependencyException =
                new ConsumerStatusDependencyException(
                    message: "ConsumerStatus dependency error occurred, contact support.",
                    innerException: failedConsumerStatusStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(randomConsumerStatus);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerStatus>(),
                    It.IsAny<ConsumerStatus>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            ConsumerStatus someConsumerStatus = CreateRandomConsumerStatus();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidConsumerStatusReferenceException =
                new InvalidConsumerStatusReferenceException(
                    message: "Invalid consumerStatus reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            ConsumerStatusDependencyValidationException expectedConsumerStatusDependencyValidationException =
                new ConsumerStatusDependencyValidationException(
                    message: "ConsumerStatus dependency validation occurred, please try again.",
                    innerException: invalidConsumerStatusReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(someConsumerStatus);

            ConsumerStatusDependencyValidationException actualConsumerStatusDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConsumerStatusDependencyValidationException))),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerStatus>(),
                    It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            var databaseUpdateException = new DbUpdateException();

            var failedConsumerStatusStorageException =
                new FailedConsumerStatusStorageException(
                    message: "Failed consumerStatus storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedConsumerStatusDependencyException =
                new ConsumerStatusDependencyException(
                    message: "ConsumerStatus dependency error occurred, contact support.",
                    innerException: failedConsumerStatusStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(randomConsumerStatus);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerStatus>(),
                    It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedConsumerStatusException =
                new LockedConsumerStatusException(
                    message: "Locked consumerStatus record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedConsumerStatusDependencyValidationException =
                new ConsumerStatusDependencyValidationException(
                    message: "ConsumerStatus dependency validation occurred, please try again.",
                    innerException: lockedConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(randomConsumerStatus);

            ConsumerStatusDependencyValidationException actualConsumerStatusDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerStatus>(),
                    It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            var serviceException = new Exception();

            var failedConsumerStatusServiceException =
                new FailedConsumerStatusServiceException(
                    message: "Failed consumerStatus service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerStatusServiceException =
                new ConsumerStatusServiceException(
                    message: "ConsumerStatus service error occurred, contact support.",
                    innerException: failedConsumerStatusServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .Throws(serviceException);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(randomConsumerStatus);

            ConsumerStatusServiceException actualConsumerStatusServiceException =
                await Assert.ThrowsAsync<ConsumerStatusServiceException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusServiceException.Should()
                .BeEquivalentTo(expectedConsumerStatusServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerStatus>(),
                    It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
