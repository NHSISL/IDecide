// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldModifyDecisionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            DecisionType randomDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset);
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = inputDecisionType.DeepClone();
            storageDecisionType.UpdatedDate = randomDecisionType.CreatedDate;
            DecisionType auditAppliedDecisionType = inputDecisionType.DeepClone();
            auditAppliedDecisionType.UpdatedBy = randomUserId;
            auditAppliedDecisionType.UpdatedDate = randomDateTimeOffset;
            DecisionType auditEnsuredDecisionType = auditAppliedDecisionType.DeepClone();
            DecisionType updatedDecisionType = inputDecisionType;
            DecisionType expectedDecisionType = updatedDecisionType.DeepClone();
            Guid decisionTypeId = inputDecisionType.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputDecisionType))
                    .ReturnsAsync(auditAppliedDecisionType);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(decisionTypeId))
                    .ReturnsAsync(storageDecisionType);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedDecisionType, storageDecisionType))
                    .ReturnsAsync(auditEnsuredDecisionType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateDecisionTypeAsync(auditEnsuredDecisionType))
                    .ReturnsAsync(updatedDecisionType);

            // when
            DecisionType actualDecisionType =
                await this.decisionTypeService.ModifyDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputDecisionType),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(decisionTypeId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedDecisionType, storageDecisionType),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(auditEnsuredDecisionType),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}