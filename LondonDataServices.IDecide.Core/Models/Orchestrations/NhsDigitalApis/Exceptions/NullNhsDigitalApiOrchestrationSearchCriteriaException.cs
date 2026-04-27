// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions
{
    public class NullNhsDigitalApiOrchestrationSearchCriteriaException : Xeption
    {
        public NullNhsDigitalApiOrchestrationSearchCriteriaException(string message)
            : base(message)
        { }
    }
}
