// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions
{
    public class UnauthorizedConsumerOrchestrationServiceException : Xeption
    {
        public UnauthorizedConsumerOrchestrationServiceException(string message)
            : base(message)
        { }
    }
}
