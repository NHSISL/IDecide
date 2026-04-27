// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions
{
    public class NhsDigitalApiOrchestrationValidationException : Xeption
    {
        public NhsDigitalApiOrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
