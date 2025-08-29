// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldPostDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType expectedDecisionType = randomDecisionType;

            // when 
            await this.apiBroker.PostDecisionTypeAsync(randomDecisionType);

            DecisionType actualDecisionType =
                await this.apiBroker.GetDecisionTypeByIdAsync(randomDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(
                expectedDecisionType,
                options => options
                    .Excluding(decisionType => decisionType.CreatedBy)
                    .Excluding(decisionType => decisionType.CreatedDate)
                    .Excluding(decisionType => decisionType.UpdatedBy)
                    .Excluding(decisionType => decisionType.UpdatedDate));

            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}
