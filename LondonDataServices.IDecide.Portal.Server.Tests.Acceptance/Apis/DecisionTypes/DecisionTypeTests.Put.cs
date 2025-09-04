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
        public async Task ShouldPutDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = 
                await PostRandomDecisionTypeAsync();

            DecisionType modifiedDecisionType = 
                UpdateDecisionTypeWithRandomValues(randomDecisionType);

            // when
            await this.apiBroker.PutDecisionTypeAsync(modifiedDecisionType);
            
            DecisionType actualDecisionType = await this.apiBroker
                .GetDecisionTypeByIdAsync(randomDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(modifiedDecisionType, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}