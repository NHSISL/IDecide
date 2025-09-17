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
        public async Task ShouldAddConsumerAdoptionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(randomDateTimeOffset);
            ConsumerAdoption inputConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption auditAppliedConsumerAdoption = inputConsumerAdoption.DeepClone();
            auditAppliedConsumerAdoption.CreatedBy = randomUserId;
            auditAppliedConsumerAdoption.CreatedDate = randomDateTimeOffset;
            auditAppliedConsumerAdoption.UpdatedBy = randomUserId;
            auditAppliedConsumerAdoption.UpdatedDate = randomDateTimeOffset;
            ConsumerAdoption storageConsumerAdoption = auditAppliedConsumerAdoption.DeepClone();
            ConsumerAdoption expectedConsumerAdoption = storageConsumerAdoption.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(inputConsumerAdoption))
                    .ReturnsAsync(auditAppliedConsumerAdoption);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertConsumerAdoptionAsync(auditAppliedConsumerAdoption))
                    .ReturnsAsync(storageConsumerAdoption);

            // when
            ConsumerAdoption actualConsumerAdoption =
                await this.consumerAdoptionService.AddConsumerAdoptionAsync(inputConsumerAdoption);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(expectedConsumerAdoption);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(inputConsumerAdoption),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(auditAppliedConsumerAdoption),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
