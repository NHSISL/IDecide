// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions
{
    public class InvalidNhsDigitalApiOrchestrationArgumentException : Xeption
    {
        public InvalidNhsDigitalApiOrchestrationArgumentException(string message)
            : base(message)
        { }
    }
}
