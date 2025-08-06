// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions
{
    public class FailedPatientOrchestrationServiceException : Xeption
    {
        public FailedPatientOrchestrationServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
