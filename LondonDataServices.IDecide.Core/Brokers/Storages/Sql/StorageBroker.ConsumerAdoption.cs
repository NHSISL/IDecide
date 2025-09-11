// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        public DbSet<ConsumerAdoption> ConsumerAdoptions { get; set; }

        public async ValueTask<ConsumerAdoption> InsertConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            await InsertAsync(consumerAdoption);

        public async ValueTask<IQueryable<ConsumerAdoption>> SelectAllConsumerAdoptionsAsync() =>
            await SelectAllAsync<ConsumerAdoption>();

        public async ValueTask<ConsumerAdoption> SelectConsumerAdoptionByIdAsync(Guid consumerAdoptionId) =>
            await SelectAsync<ConsumerAdoption>(consumerAdoptionId);

        public async ValueTask<ConsumerAdoption> UpdateConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            await UpdateAsync(consumerAdoption);

        public async ValueTask<ConsumerAdoption> DeleteConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            await DeleteAsync(consumerAdoption);
    }
}