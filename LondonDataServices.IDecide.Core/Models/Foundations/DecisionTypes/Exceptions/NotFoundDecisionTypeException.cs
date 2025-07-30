// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class NotFoundDecisionTypeException : Xeption
    {
        public NotFoundDecisionTypeException(string message)
            : base(message)
        { }
    }
}