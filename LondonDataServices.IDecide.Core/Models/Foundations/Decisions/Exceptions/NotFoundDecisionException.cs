// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions
{
    public class NotFoundDecisionException : Xeption
    {
        public NotFoundDecisionException(string message)
            : base(message)
        { }
    }
}