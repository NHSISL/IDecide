// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public interface IConsumerStatusService
    {
        ValueTask<ConsumerStatus> AddConsumerStatusAsync(ConsumerStatus consumerStatus);
        ValueTask<ConsumerStatus> ModifyConsumerStatusAsync(ConsumerStatus consumerStatus);
    }
}
