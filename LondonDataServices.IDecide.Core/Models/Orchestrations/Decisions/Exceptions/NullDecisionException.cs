// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions
{
    public class NullDecisionException : Xeption
    {
        public NullDecisionException(string message)
            : base(message)
        { }
    }
}
