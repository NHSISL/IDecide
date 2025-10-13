// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions
{
    public class FailedConsumerOrchestrationServiceException : Xeption
    {
        public FailedConsumerOrchestrationServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
