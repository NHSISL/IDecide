// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions
{
    public class ExceededMaxRetryCountException : Xeption
    {
        public ExceededMaxRetryCountException(string message)
            : base(message)
        { }
    }
}
