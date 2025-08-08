// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions
{
    public class InvalidDecisionTypeException : Xeption
    {
        public InvalidDecisionTypeException(string message)
            : base(message)
        { }
    }
}