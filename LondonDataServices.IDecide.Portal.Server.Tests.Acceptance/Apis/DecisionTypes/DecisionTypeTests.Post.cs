// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.DecisionTypes
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldPostDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType expectedDecisionType = inputDecisionType;

            // when 
            await this.apiBroker.PostDecisionTypeAsync(inputDecisionType);

            DecisionType actualDecisionType =
                await this.apiBroker.GetDecisionTypeByIdAsync(inputDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}