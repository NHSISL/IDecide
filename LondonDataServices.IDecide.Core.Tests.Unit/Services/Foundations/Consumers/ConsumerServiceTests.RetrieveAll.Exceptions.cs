// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedConsumerStorageException =
                new FailedConsumerStorageException(
                    message: "Failed consumer storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerDependencyException =
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, contact support.",
                    innerException: failedConsumerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumersAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Consumer>> retrieveAllConsumersTask = this.consumerService.RetrieveAllConsumersAsync();

            ConsumerDependencyException actualConsumerDependencyException =
                await Assert.ThrowsAsync<ConsumerDependencyException>(
                    retrieveAllConsumersTask.AsTask);

            // then
            actualConsumerDependencyException.Should()
                .BeEquivalentTo(expectedConsumerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerDependencyException))),
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

            var failedConsumerServiceException =
                new FailedConsumerServiceException(
                    message: "Failed consumer service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerServiceException =
                new ConsumerServiceException(
                    message: "Consumer service error occurred, contact support.",
                    innerException: failedConsumerServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumersAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<Consumer>> retrieveAllConsumersTask =
                this.consumerService.RetrieveAllConsumersAsync();

            ConsumerServiceException actualConsumerServiceException =
                await Assert.ThrowsAsync<ConsumerServiceException>(retrieveAllConsumersTask.AsTask);

            // then
            actualConsumerServiceException.Should()
                .BeEquivalentTo(expectedConsumerServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
