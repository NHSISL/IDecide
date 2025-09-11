// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            SqlException sqlException = GetSqlException();

            var failedConsumerAdoptionStorageException =
                new FailedConsumerAdoptionStorageException(
                    message: "Failed consumerAdoption storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: failedConsumerAdoptionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(randomConsumerAdoption.Id))
                    .Throws(sqlException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(randomConsumerAdoption.Id);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    addConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(randomConsumerAdoption.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
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
            Guid someConsumerAdoptionId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedConsumerAdoptionException =
                new LockedConsumerAdoptionException(
                    message: "Locked consumerAdoption record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedConsumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: lockedConsumerAdoptionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<ConsumerAdoption> removeConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(someConsumerAdoptionId);

            ConsumerAdoptionDependencyValidationException actualConsumerAdoptionDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyValidationException>(
                    removeConsumerAdoptionByIdTask.AsTask);

            // then
            actualConsumerAdoptionDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
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
            Guid someConsumerAdoptionId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedConsumerAdoptionStorageException =
                new FailedConsumerAdoptionStorageException(
                    message: "Failed consumerAdoption storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: failedConsumerAdoptionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerAdoption> deleteConsumerAdoptionTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(someConsumerAdoptionId);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    deleteConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
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
            Guid someConsumerAdoptionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedConsumerAdoptionServiceException =
                new FailedConsumerAdoptionServiceException(
                    message: "Failed consumerAdoption service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: failedConsumerAdoptionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ConsumerAdoption> removeConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(someConsumerAdoptionId);

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(
                    removeConsumerAdoptionByIdTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionServiceException))),
                        Times.Once());

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
