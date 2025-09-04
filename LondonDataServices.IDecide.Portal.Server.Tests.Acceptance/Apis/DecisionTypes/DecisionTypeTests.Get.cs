// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldGetAllDecisionTypesAsync()
        {
            // given
            List<DecisionType> randomDecisionTypes = await PostRandomDecisionTypesAsync();
            List<DecisionType> expectedDecisionTypes = randomDecisionTypes;

            // when
            List<DecisionType> actualDecisionTypes = await this.apiBroker.GetAllDecisionTypesAsync();

            // then
            foreach (DecisionType expectedDecisionType in expectedDecisionTypes)
            {
                DecisionType actualDecisionType = 
                    actualDecisionTypes.Single(approval => approval.Id == expectedDecisionType.Id);

                actualDecisionType.Should().BeEquivalentTo(expectedDecisionType, options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate));

                await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
            }
        }
    }
}