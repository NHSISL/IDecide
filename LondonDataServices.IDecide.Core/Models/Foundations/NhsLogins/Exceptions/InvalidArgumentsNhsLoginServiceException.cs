// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class InvalidArgumentsNhsLoginServiceException : Xeption
    {
        public InvalidArgumentsNhsLoginServiceException(string message)
            : base(message)
        { }
    }
}