// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public interface IConsumerStatusService
    {
        ValueTask<ConsumerStatus> AddConsumerStatusAsync(ConsumerStatus consumerStatus);
        ValueTask<IQueryable<ConsumerStatus>> RetrieveAllConsumerStatusesAsync();
        ValueTask<ConsumerStatus> RetrieveConsumerStatusByIdAsync(Guid consumerStatusId);
        ValueTask<ConsumerStatus> ModifyConsumerStatusAsync(ConsumerStatus consumerStatus);
        ValueTask<ConsumerStatus> RemoveConsumerStatusByIdAsync(Guid consumerStatusId);
    }
}
