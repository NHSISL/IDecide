// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions
{
    public class FailedDecisionTypeStorageException : Xeption
    {
        public FailedDecisionTypeStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}