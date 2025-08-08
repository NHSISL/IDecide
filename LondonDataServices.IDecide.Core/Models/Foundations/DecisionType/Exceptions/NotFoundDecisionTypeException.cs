// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions
{
    public class NotFoundDecisionTypeException : Xeption
    {
        public NotFoundDecisionTypeException(string message)
            : base(message)
        { }
    }
}