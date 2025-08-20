// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        public DbSet<ConsumerStatus> ConsumerStatuses { get; set; }

        public async ValueTask<ConsumerStatus> InsertConsumerStatusAsync(ConsumerStatus consumerStatus) =>
            await InsertAsync(consumerStatus);

        public async ValueTask<IQueryable<ConsumerStatus>> SelectAllConsumerStatusesAsync() =>
            await SelectAllAsync<ConsumerStatus>();

        public async ValueTask<ConsumerStatus> SelectConsumerStatusByIdAsync(Guid consumerStatusId) =>
            await SelectAsync<ConsumerStatus>(consumerStatusId);

        public async ValueTask<ConsumerStatus> UpdateConsumerStatusAsync(ConsumerStatus consumerStatus) =>
            await UpdateAsync(consumerStatus);

        public async ValueTask<ConsumerStatus> DeleteConsumerStatusAsync(ConsumerStatus consumerStatus) =>
            await DeleteAsync(consumerStatus);
    }
}