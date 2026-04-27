// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class CancelledNhsDigitalApiBuildLoginUrlException : Xeption
    {
        public CancelledNhsDigitalApiBuildLoginUrlException(string message)
            : base(message)
        { }
    }
}
