// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class InvalidNhsDigitalApiUserInfoException : Xeption
    {
        public InvalidNhsDigitalApiUserInfoException(string message)
            : base(message)
        { }
    }
}
