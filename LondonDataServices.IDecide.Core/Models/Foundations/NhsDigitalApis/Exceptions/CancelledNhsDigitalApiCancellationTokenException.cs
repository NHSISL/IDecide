// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class CancelledNhsDigitalApiCancellationTokenException : Xeption
    {
        public CancelledNhsDigitalApiCancellationTokenException(string message)
            : base(message)
        { }
    }
}
