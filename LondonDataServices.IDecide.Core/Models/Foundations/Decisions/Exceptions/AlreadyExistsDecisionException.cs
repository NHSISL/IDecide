// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions
{
    public class AlreadyExistsDecisionException : Xeption
    {
        public AlreadyExistsDecisionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}