// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<ConsumerAdoption> InsertConsumerAdoptionAsync(ConsumerAdoption consumerAdoption);
        ValueTask BulkInsertConsumerAdoptionsAsync(List<ConsumerAdoption> consumerAdoptions);
        ValueTask<IQueryable<ConsumerAdoption>> SelectAllConsumerAdoptionsAsync();
        ValueTask<ConsumerAdoption> SelectConsumerAdoptionByIdAsync(Guid consumerAdoptionId);
        ValueTask<ConsumerAdoption> UpdateConsumerAdoptionAsync(ConsumerAdoption consumerAdoption);
        ValueTask BulkUpdateConsumerAdoptionsAsync(List<ConsumerAdoption> consumerAdoptions);
        ValueTask<ConsumerAdoption> DeleteConsumerAdoptionAsync(ConsumerAdoption consumerAdoption);
    }
}