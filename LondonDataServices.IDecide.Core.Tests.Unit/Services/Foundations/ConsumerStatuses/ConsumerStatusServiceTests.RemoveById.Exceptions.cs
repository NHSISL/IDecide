// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
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

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(randomConsumerStatus.Id))
                    .Throws(sqlException);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(randomConsumerStatus.Id);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    addConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(randomConsumerStatus.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
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
            Guid someConsumerStatusId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedConsumerStatusException =
                new LockedConsumerStatusException(
                    message: "Locked consumerStatus record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedConsumerStatusDependencyValidationException =
                new ConsumerStatusDependencyValidationException(
                    message: "ConsumerStatus dependency validation occurred, please try again.",
                    innerException: lockedConsumerStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<ConsumerStatus> removeConsumerStatusByIdTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(someConsumerStatusId);

            ConsumerStatusDependencyValidationException actualConsumerStatusDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyValidationException>(
                    removeConsumerStatusByIdTask.AsTask);

            // then
            actualConsumerStatusDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
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
            Guid someConsumerStatusId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedConsumerStatusStorageException =
                new FailedConsumerStatusStorageException(
                    message: "Failed consumerStatus storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerStatusDependencyException =
                new ConsumerStatusDependencyException(
                    message: "ConsumerStatus dependency error occurred, contact support.",
                    innerException: failedConsumerStatusStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerStatus> deleteConsumerStatusTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(someConsumerStatusId);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    deleteConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyException))),
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
            Guid someConsumerStatusId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedConsumerStatusServiceException =
                new FailedConsumerStatusServiceException(
                    message: "Failed consumerStatus service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerStatusServiceException =
                new ConsumerStatusServiceException(
                    message: "ConsumerStatus service error occurred, contact support.",
                    innerException: failedConsumerStatusServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ConsumerStatus> removeConsumerStatusByIdTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(someConsumerStatusId);

            ConsumerStatusServiceException actualConsumerStatusServiceException =
                await Assert.ThrowsAsync<ConsumerStatusServiceException>(
                    removeConsumerStatusByIdTask.AsTask);

            // then
            actualConsumerStatusServiceException.Should()
                .BeEquivalentTo(expectedConsumerStatusServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusServiceException))),
                        Times.Once());

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
