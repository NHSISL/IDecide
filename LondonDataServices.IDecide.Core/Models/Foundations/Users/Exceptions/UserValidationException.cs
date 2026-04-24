// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions
{
    public class UserValidationException : Xeption
    {
        public UserValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
