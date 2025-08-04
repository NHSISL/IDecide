// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Mappers;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public class PatientOrchestrationService : IPatientOrchestrationService
    {
        private readonly IPdsService pdsService;

        public PatientOrchestrationService(IPdsService pdsService)
        {
            this.pdsService = pdsService;
        }

        public async ValueTask<Patient> PatientLookupByDetailsAsync(PatientLookup patientLookup)
        {
            PatientLookup responsePatientLookup = await this.pdsService.PatientLookupByDetailsAsync(patientLookup);
            // add validation to throw error if more than one patient returned
            Hl7.Fhir.Model.Patient fhirPatient = responsePatientLookup.Patients.Patients.First();
            Patient patientToRedact = LocalPatientMapper.FromFhirPatient(fhirPatient);
            Patient redactedPatient = patientToRedact.GetRedactedPatient();

            return redactedPatient;
        }
    }
}
