// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldRetrieveDecisionByIdAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision inputDecision = randomDecision;
            Decision storageDecision = randomDecision;
            Decision expectedDecision = storageDecision.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(inputDecision.Id))
                    .ReturnsAsync(storageDecision);

            // when
            Decision actualDecision =
                await this.decisionService.RetrieveDecisionByIdAsync(inputDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(inputDecision.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}