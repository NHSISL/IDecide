// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService
    {
        private static void ValidatePatientLookupIsNotNull(PatientLookup patientLookup)
        {
            if (patientLookup is null)
            {
                throw new NullPatientLookupException("Patient lookup is null.");
            }
        }

        private static void ValidatePatientLookupPatientIsExactMatch(PatientLookup patientLookup)
        {
            if (patientLookup.Patients.Count != 1)
            {
                throw new NoExactPatientFoundException(
                    patientLookup.Patients.Count == 0 
                        ? "No matching patient found." : "Multiple matching patients found.");
            }
        }
    }
}
