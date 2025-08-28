// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
            ValueTask<ConsumerStatus> retrieveConsumerStatusByIdTask =
                this.consumerStatusService.RetrieveConsumerStatusByIdAsync(someId);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    retrieveConsumerStatusByIdTask.AsTask);

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
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
            ValueTask<ConsumerStatus> retrieveConsumerStatusByIdTask =
                this.consumerStatusService.RetrieveConsumerStatusByIdAsync(someId);

            ConsumerStatusServiceException actualConsumerStatusServiceException =
                await Assert.ThrowsAsync<ConsumerStatusServiceException>(
                    retrieveConsumerStatusByIdTask.AsTask);

            // then
            actualConsumerStatusServiceException.Should()
                .BeEquivalentTo(expectedConsumerStatusServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedConsumerStatusServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
