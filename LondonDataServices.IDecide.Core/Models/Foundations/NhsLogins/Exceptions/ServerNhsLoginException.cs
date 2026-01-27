// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions
{
    public class ServerNhsLoginException : Xeption
    {
        public ServerNhsLoginException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
