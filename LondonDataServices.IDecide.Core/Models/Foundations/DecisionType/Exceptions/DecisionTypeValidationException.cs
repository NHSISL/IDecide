// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions
{
    public class DecisionTypeValidationException : Xeption
    {
        public DecisionTypeValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}