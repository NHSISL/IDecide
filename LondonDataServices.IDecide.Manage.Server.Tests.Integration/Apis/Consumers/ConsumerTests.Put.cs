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
        public async Task ShouldPutConsumerAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();
            Consumer modifiedConsumer = UpdateConsumerWithRandomValues(randomConsumer);

            // when
            await this.apiBroker.PutConsumerAsync(modifiedConsumer);
            Consumer actualConsumer = await this.apiBroker.GetConsumerByIdAsync(randomConsumer.Id);

            // then
            actualConsumer.Should().BeEquivalentTo(
                modifiedConsumer,
                options => options
                    .Excluding(consumer => consumer.CreatedBy)
                    .Excluding(consumer => consumer.CreatedDate)
                    .Excluding(consumer => consumer.UpdatedBy)
                    .Excluding(consumer => consumer.UpdatedDate));

            await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
        }
    }
}
