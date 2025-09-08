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
        public async Task ShouldDeleteConsumerByIdAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();
            Consumer inputConsumer = randomConsumer;
            Consumer expectedConsumer = inputConsumer;

            // when
            Consumer deletedConsumer =
                await this.apiBroker.DeleteConsumerByIdAsync(inputConsumer.Id);

            List<Consumer> actualResult =
                await this.apiBroker.GetSpecificConsumerByIdAsync(inputConsumer.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}