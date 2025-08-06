// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions
{
    public class ClientPdsException : Xeption
    {
        public ClientPdsException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
