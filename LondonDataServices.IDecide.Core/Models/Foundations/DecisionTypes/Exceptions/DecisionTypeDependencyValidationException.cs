// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class DecisionTypeDependencyValidationException : Xeption
    {
        public DecisionTypeDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}