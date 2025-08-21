// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Consumers
{
    public interface IConsumerService
    {
        ValueTask<Consumer> AddConsumerAsync(Consumer consumer);
        ValueTask<IQueryable<Consumer>> RetrieveAllConsumersAsync();
        ValueTask<Consumer> ModifyConsumerAsync(Consumer consumer);
    }
}
