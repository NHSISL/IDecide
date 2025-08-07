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
        public async Task ShouldAddDecisionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Decision randomDecision = CreateRandomDecision(randomDateTimeOffset);
            Decision inputDecision = randomDecision;
            Decision auditAppliedDecision = inputDecision.DeepClone();
            auditAppliedDecision.CreatedBy = randomUserId;
            auditAppliedDecision.CreatedDate = randomDateTimeOffset;
            auditAppliedDecision.UpdatedBy = randomUserId;
            auditAppliedDecision.UpdatedDate = randomDateTimeOffset;
            Decision storageDecision = auditAppliedDecision.DeepClone();
            Decision expectedDecision = storageDecision.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(inputDecision))
                    .ReturnsAsync(auditAppliedDecision);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDecisionAsync(auditAppliedDecision))
                    .ReturnsAsync(storageDecision);

            // when
            Decision actualDecision = await this.decisionService
                .AddDecisionAsync(inputDecision);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(inputDecision),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(auditAppliedDecision),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}