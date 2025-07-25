// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions
{
    public class LockedDecisionTypeException : Xeption
    {
        public LockedDecisionTypeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
