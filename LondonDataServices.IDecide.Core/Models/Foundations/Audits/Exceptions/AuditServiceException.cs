// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions
{
    public class AuditServiceException : Xeption
    {
        public AuditServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}