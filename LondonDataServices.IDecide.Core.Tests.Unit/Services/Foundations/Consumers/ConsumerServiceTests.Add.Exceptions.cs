// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();
            SqlException sqlException = GetSqlException();

            var failedConsumerStorageException =
                new FailedConsumerStorageException(
                    message: "Failed consumer storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerDependencyException =
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, contact support.",
                    innerException: failedConsumerStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(someConsumer);

            ConsumerDependencyException actualConsumerDependencyException =
                await Assert.ThrowsAsync<ConsumerDependencyException>(
                    addConsumerTask.AsTask);

            // then
            actualConsumerDependencyException.Should()
                .BeEquivalentTo(expectedConsumerDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerDependencyException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAsync(It.IsAny<Consumer>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfConsumerAlreadyExsitsAndLogItAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer alreadyExistsConsumer = randomConsumer;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsConsumerException =
                new AlreadyExistsConsumerException(
                    message: "Consumer with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedConsumerDependencyValidationException =
                new ConsumerDependencyValidationException(
                    message: "Consumer dependency validation occurred, please try again.",
                    innerException: alreadyExistsConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(alreadyExistsConsumer);

            // then
            ConsumerDependencyValidationException actualConsumerDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerDependencyValidationException>(
                    addConsumerTask.AsTask);

            actualConsumerDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerDependencyValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAsync(It.IsAny<Consumer>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
