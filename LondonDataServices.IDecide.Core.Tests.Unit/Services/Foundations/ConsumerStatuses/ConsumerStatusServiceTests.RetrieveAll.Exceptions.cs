// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
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
                broker.SelectAllConsumerStatusesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ConsumerStatus>> retrieveAllConsumerStatusesTask =
                this.consumerStatusService.RetrieveAllConsumerStatusesAsync();

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    retrieveAllConsumerStatusesTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerStatusesAsync(),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedConsumerStatusServiceException =
                new FailedConsumerStatusServiceException(
                    message: "Failed consumerStatus service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerStatusServiceException =
                new ConsumerStatusServiceException(
                    message: "ConsumerStatus service error occurred, contact support.",
                    innerException: failedConsumerStatusServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerStatusesAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ConsumerStatus>> retrieveAllConsumerStatusesTask =
                this.consumerStatusService.RetrieveAllConsumerStatusesAsync();

            ConsumerStatusServiceException actualConsumerStatusServiceException =
                await Assert.ThrowsAsync<ConsumerStatusServiceException>(retrieveAllConsumerStatusesTask.AsTask);

            // then
            actualConsumerStatusServiceException.Should()
                .BeEquivalentTo(expectedConsumerStatusServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerStatusesAsync(),
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
