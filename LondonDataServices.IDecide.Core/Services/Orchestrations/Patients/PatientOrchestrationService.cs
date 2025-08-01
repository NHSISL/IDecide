// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
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

        public ValueTask<Patient> PatientLookupByDetailsAsync(PatientLookup patientLookup)
        {
            throw new System.NotImplementedException();
        }

        virtual internal Patient RedactPatientDetails(Patient patient)
        {
            throw new System.NotImplementedException();
        }
    }
}
