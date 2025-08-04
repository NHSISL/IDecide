// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions
{
    public class PatientOrchestrationServiceException : Xeption
    {
        public PatientOrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
