// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class NhsLoginServiceServiceException : Xeption
    {
        public NhsLoginServiceServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
