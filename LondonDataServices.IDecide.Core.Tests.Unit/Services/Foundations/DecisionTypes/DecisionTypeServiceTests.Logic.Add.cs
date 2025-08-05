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

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldAddDecisionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset);
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType auditAppliedDecisionType = inputDecisionType.DeepClone();
            auditAppliedDecisionType.CreatedBy = randomUserId;
            auditAppliedDecisionType.CreatedDate = randomDateTimeOffset;
            auditAppliedDecisionType.UpdatedBy = randomUserId;
            auditAppliedDecisionType.UpdatedDate = randomDateTimeOffset;
            DecisionType storageDecisionType = auditAppliedDecisionType.DeepClone();
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditAsync(inputDecisionType))
                    .ReturnsAsync(auditAppliedDecisionType);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDecisionTypeAsync(auditAppliedDecisionType))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType = await this.decisionTypeService
                .AddDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditAsync(inputDecisionType))
                    .ReturnsAsync(auditAppliedDecisionType);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(auditAppliedDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}