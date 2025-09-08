// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Consumers
{
    public partial class ConsumerApiTests
    {
        [Fact]
        public async Task ShouldGetAllConsumersAsync()
        {
            // given
            List<Consumer> randomConsumers = await PostRandomConsumersAsync();
            List<Consumer> expectedConsumers = randomConsumers;

            // when
            List<Consumer> actualConsumers = await this.apiBroker.GetAllConsumersAsync();

            // then
            foreach (Consumer expectedConsumer in expectedConsumers)
            {
                Consumer actualConsumer = 
                    actualConsumers.Single(approval => approval.Id == expectedConsumer.Id);

                actualConsumer.Should().BeEquivalentTo(expectedConsumer, options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate));

                await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
            }
        }
    }
}