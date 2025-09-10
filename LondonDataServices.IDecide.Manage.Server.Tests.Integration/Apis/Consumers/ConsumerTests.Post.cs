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
        public async Task ShouldPostConsumerAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer expectedConsumer = randomConsumer;

            // when 
            await this.apiBroker.PostConsumerAsync(randomConsumer);

            Consumer actualConsumer =
                await this.apiBroker.GetConsumerByIdAsync(randomConsumer.Id);

            // then
            actualConsumer.Should().BeEquivalentTo(
                expectedConsumer,
                options => options
                    .Excluding(consumer => consumer.CreatedBy)
                    .Excluding(consumer => consumer.CreatedDate)
                    .Excluding(consumer => consumer.UpdatedBy)
                    .Excluding(consumer => consumer.UpdatedDate));

            await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
        }
    }
}
