// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class DecisionTypeDependencyException : Xeption
    {
        public DecisionTypeDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}