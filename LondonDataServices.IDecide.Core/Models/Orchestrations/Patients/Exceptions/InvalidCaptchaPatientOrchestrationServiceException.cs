// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions
{
    public class InvalidCaptchaPatientOrchestrationServiceException : Xeption
    {
        public InvalidCaptchaPatientOrchestrationServiceException(string message)
            : base(message)
        { }
    }
}
