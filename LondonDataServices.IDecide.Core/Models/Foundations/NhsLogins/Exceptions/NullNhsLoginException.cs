// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class NullNhsLoginException : Xeption
    {
        public NullNhsLoginException(string message)
            : base(message: message)
        { }
    }
}