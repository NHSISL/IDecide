// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public partial class PdsService
    {
        private static void ValidatePatientLookupIsNotNull(PatientLookup patientLookup)
        {
            if (patientLookup is null)
            {
                throw new NullPatientLookupException("Patient lookup is null.");
            }
        }
    }
}