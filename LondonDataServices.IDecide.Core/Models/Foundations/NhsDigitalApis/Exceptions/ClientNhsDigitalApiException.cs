// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class ClientNhsDigitalApiException : Xeption
    {
        public ClientNhsDigitalApiException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
