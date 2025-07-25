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
        public async Task ShouldAddDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = inputDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.storageBroker.Setup(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType = await this.decisionTypeService
                .AddDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBroker.Verify(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType),
                    Times.Once);

            this.storageBroker.VerifyNoOtherCalls();
        }
    }
}
