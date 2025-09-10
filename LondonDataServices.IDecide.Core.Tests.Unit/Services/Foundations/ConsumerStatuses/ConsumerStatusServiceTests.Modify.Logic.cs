// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldModifyConsumerStatusAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            ConsumerStatus randomConsumerStatus = CreateRandomModifyConsumerStatus(randomDateTimeOffset);
            ConsumerStatus inputConsumerStatus = randomConsumerStatus;
            ConsumerStatus storageConsumerStatus = inputConsumerStatus.DeepClone();
            storageConsumerStatus.UpdatedDate = randomConsumerStatus.CreatedDate;
            ConsumerStatus auditAppliedConsumerStatus = inputConsumerStatus.DeepClone();
            auditAppliedConsumerStatus.UpdatedBy = randomUserId;
            auditAppliedConsumerStatus.UpdatedDate = randomDateTimeOffset;
            ConsumerStatus auditEnsuredConsumerStatus = auditAppliedConsumerStatus.DeepClone();
            ConsumerStatus updatedConsumerStatus = inputConsumerStatus;
            ConsumerStatus expectedConsumerStatus = updatedConsumerStatus.DeepClone();
            Guid consumerStatusId = inputConsumerStatus.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumerStatus))
                    .ReturnsAsync(auditAppliedConsumerStatus);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(consumerStatusId))
                    .ReturnsAsync(storageConsumerStatus);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumerStatus, storageConsumerStatus))
                    .ReturnsAsync(auditEnsuredConsumerStatus);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateConsumerStatusAsync(auditEnsuredConsumerStatus))
                    .ReturnsAsync(updatedConsumerStatus);

            // when
            ConsumerStatus actualConsumerStatus =
                await this.consumerStatusService.ModifyConsumerStatusAsync(inputConsumerStatus);

            // then
            actualConsumerStatus.Should().BeEquivalentTo(expectedConsumerStatus);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumerStatus),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(consumerStatusId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumerStatus, storageConsumerStatus),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(auditEnsuredConsumerStatus),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
