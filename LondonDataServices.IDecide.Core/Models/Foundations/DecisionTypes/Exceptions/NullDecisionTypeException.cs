// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class NullDecisionTypeException : Xeption
    {
        public NullDecisionTypeException(string message)
            : base(message)
        { }
    }
}