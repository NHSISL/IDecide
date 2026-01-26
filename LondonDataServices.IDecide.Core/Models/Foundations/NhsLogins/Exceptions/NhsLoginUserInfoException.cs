// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class NhsLoginUserInfoException : Xeption
    {
        public NhsLoginUserInfoException(string message)
            : base(message)
        { }
    }
}