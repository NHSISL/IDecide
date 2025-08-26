// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ConsumerStatus someConsumerStatus = CreateRandomConsumerStatus();
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
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(someConsumerStatus);

            ConsumerStatusDependencyException actualConsumerStatusDependencyException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyException>(
                    addConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusDependencyException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfConsumerStatusAlreadyExistsAndLogItAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            ConsumerStatus alreadyExistsConsumerStatus = randomConsumerStatus;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsConsumerStatusException =
                new AlreadyExistsConsumerStatusException(
                    message: "ConsumerStatus with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedConsumerStatusDependencyValidationException =
                new ConsumerStatusDependencyValidationException(
                    message: "ConsumerStatus dependency validation occurred, please try again.",
                    innerException: alreadyExistsConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerStatus>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(alreadyExistsConsumerStatus);

            // then
            ConsumerStatusDependencyValidationException actualConsumerStatusDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerStatusDependencyValidationException>(
                    addConsumerStatusTask.AsTask);

            actualConsumerStatusDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerStatus>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusDependencyValidationException))),
                        Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
