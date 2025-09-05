// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Consumers
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
            actualConsumers.Should().NotBeNull();

            foreach (Consumer expectedConsumer in expectedConsumers)
            {
                Consumer actualConsumer = actualConsumers
                    .Single(consumer => consumer.Id == expectedConsumer.Id);

                actualConsumer.Should().BeEquivalentTo(
                    expectedConsumer,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (Consumer createdConsumer in expectedConsumers)
            {
                await this.apiBroker.DeleteConsumerByIdAsync(createdConsumer.Id);
            }
        }
    }
}
