// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class InvalidDecisionTypeException : Xeption
    {
        public InvalidDecisionTypeException(string message)
            : base(message)
        { }
    }
}