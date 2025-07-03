// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions
{
    public class InvalidAuditException : Xeption
    {
        public InvalidAuditException(string message)
            : base(message)
        { }
    }
}