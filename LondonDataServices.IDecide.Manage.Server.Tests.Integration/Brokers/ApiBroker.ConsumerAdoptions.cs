// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerAdoptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string consumerAdoptionsRelativeUrl = "api/consumeradoptions";

        public async ValueTask<ConsumerAdoption> PostConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            await this.apiFactoryClient.PostContentAsync(consumerAdoptionsRelativeUrl, consumerAdoption);

        public async ValueTask<List<ConsumerAdoption>> GetAllConsumerAdoptionsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<ConsumerAdoption>>(consumerAdoptionsRelativeUrl);

        public async ValueTask<ConsumerAdoption> GetConsumerAdoptionByIdAsync(Guid consumerAdoptionId) =>
            await this.apiFactoryClient.GetContentAsync<ConsumerAdoption>($"{consumerAdoptionsRelativeUrl}/{consumerAdoptionId}");

        public async ValueTask<ConsumerAdoption> PutConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            await this.apiFactoryClient.PutContentAsync(consumerAdoptionsRelativeUrl, consumerAdoption);

        public async ValueTask<ConsumerAdoption> DeleteConsumerAdoptionByIdAsync(Guid consumerAdoptionId) =>
            await this.apiFactoryClient.DeleteContentAsync<ConsumerAdoption>($"{consumerAdoptionsRelativeUrl}/{consumerAdoptionId}");
    }
}