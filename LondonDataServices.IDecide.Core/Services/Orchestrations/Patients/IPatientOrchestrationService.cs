// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public interface IPatientOrchestrationService
    {
        ValueTask<Patient> PatientLookupByDetailsAsync(PatientLookup patientLookup);
    }
}
