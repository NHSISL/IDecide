// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions
{
    public class AlreadyExistsUserException : Xeption
    {
        public AlreadyExistsUserException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public AlreadyExistsUserException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
