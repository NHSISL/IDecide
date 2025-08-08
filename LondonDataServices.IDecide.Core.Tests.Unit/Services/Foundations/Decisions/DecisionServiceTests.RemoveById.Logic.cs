// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldRemoveDecisionByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputDecisionId = randomId;
            Decision randomDecision = CreateRandomDecision();
            Decision storageDecision = randomDecision;
            Decision expectedInputDecision = storageDecision;
            Decision deletedDecision = expectedInputDecision;
            Decision expectedDecision = deletedDecision.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(inputDecisionId))
                    .ReturnsAsync(storageDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteDecisionAsync(expectedInputDecision))
                    .ReturnsAsync(deletedDecision);

            // when
            Decision actualDecision = await this.decisionService
                .RemoveDecisionByIdAsync(inputDecisionId);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(inputDecisionId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDecisionAsync(expectedInputDecision),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}