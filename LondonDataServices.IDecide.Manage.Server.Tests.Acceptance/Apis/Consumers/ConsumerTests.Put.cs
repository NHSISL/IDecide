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
        public async Task ShouldPutConsumerAsync()
        {
            // given
            Consumer randomConsumer = 
                await PostRandomConsumerAsync();

            Consumer modifiedConsumer = 
                UpdateConsumerWithRandomValues(randomConsumer);

            // when
            await this.apiBroker.PutConsumerAsync(modifiedConsumer);
            
            Consumer actualConsumer = await this.apiBroker
                .GetConsumerByIdAsync(randomConsumer.Id);

            // then
            actualConsumer.Should().BeEquivalentTo(modifiedConsumer, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteConsumerByIdAsync(actualConsumer.Id);
        }
    }
}