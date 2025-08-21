// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
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
        private readonly ISecurityBroker securityBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IPdsService pdsService;
        private readonly IPatientService patientService;
        private readonly INotificationService notificationService;

        public PatientOrchestrationService(
            ILoggingBroker loggingBroker,
            ISecurityBroker securityBroker,
            IDateTimeBroker dateTimeBroker,
            IPdsService pdsService,
            IPatientService patientService,
            INotificationService notificationService)
        {
            this.loggingBroker = loggingBroker;
            this.securityBroker = securityBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.pdsService = pdsService;
            this.patientService = patientService;
            this.notificationService = notificationService;
        }

        public ValueTask<Patient> PatientLookupAsync(PatientLookup patientLookup) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupIsNotNull(patientLookup);

                if (string.IsNullOrWhiteSpace(patientLookup.SearchCriteria.NhsNumber))
                {
                    PatientLookup responsePatientLookup =
                    await this.pdsService.PatientLookupByDetailsAsync(patientLookup);

                    ValidatePatientLookupPatientIsExactMatch(responsePatientLookup);
                    Patient redactedPatient = responsePatientLookup.Patients.First().Redact();

                    return redactedPatient;
                }
                else
                {
                    var nhsNumber = patientLookup.SearchCriteria.NhsNumber;
                    ValidatePatientLookupByNhsNumberArguments(nhsNumber);
                    Patient maybePatient = await this.pdsService.PatientLookupByNhsNumberAsync(nhsNumber);
                    ValidatePatientIsNotNull(maybePatient);
                    Patient redactedPatient = maybePatient.Redact();

                    return redactedPatient;
                }
            });

        public ValueTask RecordPatientInformation(
            string nhsNumber,
            string captcha,
            string notificationPreference,
            bool generateNewCode = false)
        {
            throw new System.NotImplementedException();
        }

        virtual internal string GenerateValidationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(5);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[1];

                for (int i = 0; i < 5; i++)
                {
                    int index;
                    rng.GetBytes(buffer);
                    index = buffer[0] % chars.Length;
                    result.Append(chars[index]);
                }
            }

            return result.ToString();
        }
    }
}
