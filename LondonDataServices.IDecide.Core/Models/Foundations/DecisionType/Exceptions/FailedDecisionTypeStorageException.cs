// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions
{
    public class FailedDecisionTypeStorageException : Xeption
    {
        public FailedDecisionTypeStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}