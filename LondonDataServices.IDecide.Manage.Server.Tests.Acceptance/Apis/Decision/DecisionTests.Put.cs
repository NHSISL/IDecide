// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldPutDecisionAsync()
        {
            // given
            Decision randomDecision = 
                await PostRandomDecisionAsync();

            Decision modifiedDecision = 
                UpdateDecisionWithRandomValues(randomDecision);

            // when
            await this.apiBroker.PutDecisionAsync(modifiedDecision);
            
            Decision actualDecision = await this.apiBroker
                .GetDecisionByIdAsync(randomDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(modifiedDecision, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
        }
    }
}