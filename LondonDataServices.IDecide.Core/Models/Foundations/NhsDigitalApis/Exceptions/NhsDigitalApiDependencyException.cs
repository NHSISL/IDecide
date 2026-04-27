// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class NhsDigitalApiDependencyException : Xeption
    {
        public NhsDigitalApiDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
