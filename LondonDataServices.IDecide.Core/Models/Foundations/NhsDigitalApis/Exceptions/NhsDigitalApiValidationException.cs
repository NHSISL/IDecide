// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class NhsDigitalApiValidationException : Xeption
    {
        public NhsDigitalApiValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
