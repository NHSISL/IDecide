// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class NhsLoginServiceException : Xeption
    {
        public NhsLoginServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
