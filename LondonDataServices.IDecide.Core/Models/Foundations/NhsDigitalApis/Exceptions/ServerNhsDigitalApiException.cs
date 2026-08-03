// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class ServerNhsDigitalApiException : Xeption
    {
        public ServerNhsDigitalApiException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
