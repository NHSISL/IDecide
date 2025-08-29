// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions
{
    public class FailedDecisionOrchestrationServiceException : Xeption
    {
        public FailedDecisionOrchestrationServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
