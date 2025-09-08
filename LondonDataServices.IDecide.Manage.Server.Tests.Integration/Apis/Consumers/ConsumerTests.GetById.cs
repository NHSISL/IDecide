// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Consumers
{
    public partial class ConsumerApiTests
    {
        [Fact]
        public async Task ShouldGetConsumerByIdAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();
            Consumer expectedConsumer = randomConsumer;

            // when
            Consumer actualConsumer = 
                await this.apiBroker.GetConsumerByIdAsync(randomConsumer.Id);

            // then
            actualConsumer.Should().BeEquivalentTo(expectedConsumer);
            await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
        }
    }
}
