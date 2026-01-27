// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class ClientNhsLoginException : Xeption
    {
        public ClientNhsLoginException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
