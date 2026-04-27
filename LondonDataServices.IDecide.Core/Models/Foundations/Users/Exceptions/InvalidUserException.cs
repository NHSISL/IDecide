// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions
{
    public class InvalidUserException : Xeption
    {
        public InvalidUserException(string message)
            : base(message)
        { }
    }
}
