// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions
{
    public class DecisionOrchestrationValidationException : Xeption
    {
        public DecisionOrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
