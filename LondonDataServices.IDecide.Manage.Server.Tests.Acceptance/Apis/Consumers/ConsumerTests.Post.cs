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
        public async Task ShouldPostConsumerAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer expectedConsumer = inputConsumer;

            // when 
            await this.apiBroker.PostConsumerAsync(inputConsumer);

            Consumer actualConsumer =
                await this.apiBroker.GetConsumerByIdAsync(inputConsumer.Id);

            // then
            actualConsumer.Should().BeEquivalentTo(expectedConsumer, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
        }
    }
}