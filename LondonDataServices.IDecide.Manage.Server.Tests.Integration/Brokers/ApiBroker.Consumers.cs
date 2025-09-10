// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string consumersRelativeUrl = "api/consumers";

        public async ValueTask<Consumer> PostConsumerAsync(Consumer consumer) =>
            await this.apiFactoryClient.PostContentAsync(consumersRelativeUrl, consumer);

        public async ValueTask<List<Consumer>> GetAllConsumersAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Consumer>>(consumersRelativeUrl);

        public async ValueTask<Consumer> GetConsumerByIdAsync(Guid consumerId) =>
            await this.apiFactoryClient.GetContentAsync<Consumer>($"{consumersRelativeUrl}/{consumerId}");

        public async ValueTask<Consumer> PutConsumerAsync(Consumer consumer) =>
            await this.apiFactoryClient.PutContentAsync(consumersRelativeUrl, consumer);

        public async ValueTask<Consumer> DeleteConsumerByIdAsync(Guid consumerId) =>
            await this.apiFactoryClient.DeleteContentAsync<Consumer>($"{consumersRelativeUrl}/{consumerId}");
    }
}