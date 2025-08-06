// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService : IPatientOrchestrationService
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IPdsService pdsService;
        private readonly IPatientService patientService;
        private readonly INotificationService notificationService;

        public PatientOrchestrationService(
            ILoggingBroker loggingBroker,
            IPdsService pdsService,
            IPatientService patientService,
            INotificationService notificationService)
        {
            this.loggingBroker = loggingBroker;
            this.pdsService = pdsService;
            this.patientService = patientService;
            this.notificationService = notificationService;
        }

        public ValueTask<Patient> PatientLookupByDetailsAsync(PatientLookup patientLookup) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupIsNotNull(patientLookup);

                PatientLookup responsePatientLookup =
                    await this.pdsService.PatientLookupByDetailsAsync(patientLookup);

                ValidatePatientLookupPatientIsExactMatch(responsePatientLookup);
                Patient redactedPatient = responsePatientLookup.Patients.First().Redact();

                return redactedPatient;
            });

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupByNhsNumberArguments(nhsNumber);
                Patient maybePatient = await this.pdsService.PatientLookupByNhsNumberAsync(nhsNumber);
                ValidatePatientIsNotNull(maybePatient);
                Patient redactedPatient = maybePatient.Redact();

                return redactedPatient;
            });
    }
}
