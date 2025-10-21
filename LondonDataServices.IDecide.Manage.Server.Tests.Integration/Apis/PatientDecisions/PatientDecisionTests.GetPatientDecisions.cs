// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.PatientDecisions
{
    public partial class PatientDecisionTests
    {
        [Fact]
        public async Task ShouldGetPatientDecisions()
        {
            // given
            string userId = TestAuthHandler.TestUserId;

            Consumer randomConsumerWithMatchingEntraId =
                await PostRandomConsumerWithMatchingEntraIdEntryAsync(userId);

            // when
            List<Decision> actualDecisions = await this.apiBroker.GetPatientDecisionsAsync();

            // then
            actualDecisions.Should().NotBeNull();

            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumerWithMatchingEntraId.Id);
        }
    }
}
