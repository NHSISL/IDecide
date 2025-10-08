// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions
{
    public class ConsumerOrchestrationDependencyException : Xeption
    {
        public ConsumerOrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
