// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions
{
    public class InvalidDecisionException : Xeption
    {
        public InvalidDecisionException(string message)
            : base(message)
        { }
    }
}