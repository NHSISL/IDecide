// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions
{
    public class DecisionValidationException : Xeption
    {
        public DecisionValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}