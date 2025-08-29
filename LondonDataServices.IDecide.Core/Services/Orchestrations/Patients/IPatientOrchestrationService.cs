// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public interface IPatientOrchestrationService
    {
        ValueTask<Patient> PatientLookupAsync(PatientLookup patientLookup);

        ValueTask RecordPatientInformationAsync(
            string nhsNumber,
            string captcha,
            string notificationPreference,
            bool generateNewCode = false);
    }
}
