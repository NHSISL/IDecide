// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions
{
    public class InvalidDecisionIdsException : Xeption
    {
        public InvalidDecisionIdsException(string message)
            : base(message)
        { }
    }
}
