// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions
{
    public class AuditDependencyValidationException : Xeption
    {
        public AuditDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}