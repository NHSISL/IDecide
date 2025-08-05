// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class DecisionTypeValidationException : Xeption
    {
        public DecisionTypeValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}