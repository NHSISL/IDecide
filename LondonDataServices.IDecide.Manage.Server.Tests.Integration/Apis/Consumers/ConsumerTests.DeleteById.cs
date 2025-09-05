// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Consumers
{
    public partial class ConsumerApiTests
    {
        [Fact]
        public async Task ShouldDeleteConsumerAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();

            // when
            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumer.Id);

            // then
            ValueTask<Consumer> getConsumerByIdTask = 
                this.apiBroker.GetConsumerByIdAsync(randomConsumer.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getConsumerByIdTask.AsTask);
        }
    }
}
