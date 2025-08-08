// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldModifyDecisionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Decision randomDecision = CreateRandomModifyDecision(randomDateTimeOffset);
            Decision inputDecision = randomDecision;
            Decision storageDecision = inputDecision.DeepClone();
            storageDecision.UpdatedDate = randomDecision.CreatedDate;
            Decision auditAppliedDecision = inputDecision.DeepClone();
            auditAppliedDecision.UpdatedBy = randomUserId;
            auditAppliedDecision.UpdatedDate = randomDateTimeOffset;
            Decision auditEnsuredDecision = auditAppliedDecision.DeepClone();
            Decision updatedDecision = inputDecision;
            Decision expectedDecision = updatedDecision.DeepClone();
            Guid decisionId = inputDecision.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputDecision))
                    .ReturnsAsync(auditAppliedDecision);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(decisionId))
                    .ReturnsAsync(storageDecision);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedDecision, storageDecision))
                    .ReturnsAsync(auditEnsuredDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateDecisionAsync(auditEnsuredDecision))
                    .ReturnsAsync(updatedDecision);

            // when
            Decision actualDecision =
                await this.decisionService.ModifyDecisionAsync(inputDecision);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputDecision),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(decisionId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedDecision, storageDecision),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(auditEnsuredDecision),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}