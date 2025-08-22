// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
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
                broker.SelectConsumerByIdAsync(randomConsumer.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.RemoveConsumerByIdAsync(randomConsumer.Id);

            ConsumerDependencyException actualConsumerDependencyException =
                await Assert.ThrowsAsync<ConsumerDependencyException>(
                    addConsumerTask.AsTask);

            // then
            actualConsumerDependencyException.Should()
                .BeEquivalentTo(expectedConsumerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(randomConsumer.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerAsync(It.IsAny<Consumer>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
