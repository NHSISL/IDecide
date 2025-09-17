// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions
{
    public interface IConsumerAdoptionService
    {
        ValueTask<ConsumerAdoption> AddConsumerAdoptionAsync(ConsumerAdoption consumerAdoption);
        ValueTask<IQueryable<ConsumerAdoption>> RetrieveAllConsumerAdoptionsAsync();
        ValueTask<ConsumerAdoption> RetrieveConsumerAdoptionByIdAsync(Guid consumerAdoptionId);
        ValueTask<ConsumerAdoption> ModifyConsumerAdoptionAsync(ConsumerAdoption consumerAdoption);
        ValueTask<ConsumerAdoption> RemoveConsumerAdoptionByIdAsync(Guid consumerAdoptionId);
    }
}
