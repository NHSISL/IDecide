// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class FailedNhsDigitalApiServiceException : Xeption
    {
        public FailedNhsDigitalApiServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
