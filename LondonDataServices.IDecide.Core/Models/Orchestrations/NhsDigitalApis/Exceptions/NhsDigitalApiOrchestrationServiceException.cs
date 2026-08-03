// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions
{
    public class NhsDigitalApiOrchestrationServiceException : Xeption
    {
        public NhsDigitalApiOrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
