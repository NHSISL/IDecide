// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldModifyConsumerAdoptionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            ConsumerAdoption randomConsumerAdoption = CreateRandomModifyConsumerAdoption(randomDateTimeOffset);
            ConsumerAdoption inputConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption storageConsumerAdoption = inputConsumerAdoption.DeepClone();
            storageConsumerAdoption.UpdatedDate = randomConsumerAdoption.CreatedDate;
            ConsumerAdoption auditAppliedConsumerAdoption = inputConsumerAdoption.DeepClone();
            auditAppliedConsumerAdoption.UpdatedBy = randomUserId;
            auditAppliedConsumerAdoption.UpdatedDate = randomDateTimeOffset;
            ConsumerAdoption auditEnsuredConsumerAdoption = auditAppliedConsumerAdoption.DeepClone();
            ConsumerAdoption updatedConsumerAdoption = inputConsumerAdoption;
            ConsumerAdoption expectedConsumerAdoption = updatedConsumerAdoption.DeepClone();
            Guid consumerAdoptionId = inputConsumerAdoption.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumerAdoption))
                    .ReturnsAsync(auditAppliedConsumerAdoption);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(consumerAdoptionId))
                    .ReturnsAsync(storageConsumerAdoption);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumerAdoption, storageConsumerAdoption))
                    .ReturnsAsync(auditEnsuredConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateConsumerAdoptionAsync(auditEnsuredConsumerAdoption))
                    .ReturnsAsync(updatedConsumerAdoption);

            // when
            ConsumerAdoption actualConsumerAdoption =
                await this.consumerAdoptionService.ModifyConsumerAdoptionAsync(inputConsumerAdoption);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(expectedConsumerAdoption);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputConsumerAdoption),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(consumerAdoptionId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedConsumerAdoption, storageConsumerAdoption),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(auditEnsuredConsumerAdoption),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
