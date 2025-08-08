// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions
{
    public class AlreadyExistsDecisionTypeException : Xeption
    {
        public AlreadyExistsDecisionTypeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}