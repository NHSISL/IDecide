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

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(It.IsAny<ConsumerStatus>(), It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
