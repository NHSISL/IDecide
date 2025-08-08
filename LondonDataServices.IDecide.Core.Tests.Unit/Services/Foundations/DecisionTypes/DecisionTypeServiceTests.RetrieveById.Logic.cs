// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveDecisionTypeByIdAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = randomDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionType.Id))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType =
                await this.decisionTypeService.RetrieveDecisionTypeByIdAsync(inputDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}