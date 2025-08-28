// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<ConsumerStatus> InsertConsumerStatusAsync(ConsumerStatus consumerStatus);
        ValueTask<IQueryable<ConsumerStatus>> SelectAllConsumerStatusesAsync();
        ValueTask<ConsumerStatus> SelectConsumerStatusByIdAsync(Guid consumerStatusId);
        ValueTask<ConsumerStatus> UpdateConsumerStatusAsync(ConsumerStatus consumerStatus);
        ValueTask<ConsumerStatus> DeleteConsumerStatusAsync(ConsumerStatus consumerStatus);
    }
}