// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldModifyConsumerAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Consumer randomConsumer = CreateRandomModifyConsumer(randomDateTimeOffset);
            Consumer inputConsumer = randomConsumer;
            Consumer storageConsumer = inputConsumer.DeepClone();
            storageConsumer.UpdatedDate = randomConsumer.CreatedDate;
            Consumer auditAppliedConsumer = inputConsumer.DeepClone();
            auditAppliedConsumer.UpdatedBy = randomUserId;
            auditAppliedConsumer.UpdatedDate = randomDateTimeOffset;
            Consumer auditEnsuredConsumer = auditAppliedConsumer.DeepClone();
            Consumer updatedConsumer = inputConsumer;
            Consumer expectedConsumer = updatedConsumer.DeepClone();
            Guid consumerId = inputConsumer.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumer))
                    .ReturnsAsync(auditAppliedConsumer);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerByIdAsync(consumerId))
                    .ReturnsAsync(storageConsumer);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumer, storageConsumer))
                    .ReturnsAsync(auditEnsuredConsumer);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateConsumerAsync(auditEnsuredConsumer))
                    .ReturnsAsync(updatedConsumer);

            // when
            Consumer actualConsumer =
                await this.consumerService.ModifyConsumerAsync(inputConsumer);

            // then
            actualConsumer.Should().BeEquivalentTo(expectedConsumer);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumer),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(consumerId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumer, storageConsumer),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAsync(auditEnsuredConsumer),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

