// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions
{
    public class NullDecisionTypeException : Xeption
    {
        public NullDecisionTypeException(string message)
            : base(message)
        { }
    }
}
