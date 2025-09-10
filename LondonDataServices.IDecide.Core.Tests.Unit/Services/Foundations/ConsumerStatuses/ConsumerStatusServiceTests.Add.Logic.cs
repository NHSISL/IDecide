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
        public async Task ShouldAddConsumerStatusAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus(randomDateTimeOffset);
            ConsumerStatus inputConsumerStatus = randomConsumerStatus;
            ConsumerStatus auditAppliedConsumerStatus = inputConsumerStatus.DeepClone();
            auditAppliedConsumerStatus.CreatedBy = randomUserId;
            auditAppliedConsumerStatus.CreatedDate = randomDateTimeOffset;
            auditAppliedConsumerStatus.UpdatedBy = randomUserId;
            auditAppliedConsumerStatus.UpdatedDate = randomDateTimeOffset;
            ConsumerStatus storageConsumerStatus = auditAppliedConsumerStatus.DeepClone();
            ConsumerStatus expectedConsumerStatus = storageConsumerStatus.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(inputConsumerStatus))
                    .ReturnsAsync(auditAppliedConsumerStatus);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertConsumerStatusAsync(auditAppliedConsumerStatus))
                    .ReturnsAsync(storageConsumerStatus);

            // when
            ConsumerStatus actualConsumerStatus =
                await this.consumerStatusService.AddConsumerStatusAsync(inputConsumerStatus);

            // then
            actualConsumerStatus.Should().BeEquivalentTo(expectedConsumerStatus);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(inputConsumerStatus),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(auditAppliedConsumerStatus),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
