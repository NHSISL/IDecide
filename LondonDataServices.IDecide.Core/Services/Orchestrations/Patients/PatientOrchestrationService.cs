// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService : IPatientOrchestrationService
    {
        private readonly IPdsService pdsService;
        private readonly ILoggingBroker loggingBroker;

        public PatientOrchestrationService(IPdsService pdsService, ILoggingBroker loggingBroker)
        {
            this.pdsService = pdsService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Patient> PatientLookupByDetailsAsync(PatientLookup patientLookup) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupIsNotNull(patientLookup);
                PatientLookup responsePatientLookup = await this.pdsService.PatientLookupByDetailsAsync(patientLookup);
                ValidatePatientLookupPatientIsExactMatch(responsePatientLookup);
                Patient patientToRedact = responsePatientLookup.Patients.First();
                Patient redactedPatient = patientToRedact.Redact();

                return redactedPatient;
            });
    }
}
