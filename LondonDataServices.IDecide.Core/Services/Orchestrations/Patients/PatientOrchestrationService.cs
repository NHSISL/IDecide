// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
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
        private readonly DecisionConfigurations decisionConfigurations;

        public PatientOrchestrationService(
            ILoggingBroker loggingBroker,
            ISecurityBroker securityBroker,
            IDateTimeBroker dateTimeBroker,
            IPdsService pdsService,
            IPatientService patientService,
            INotificationService notificationService,
            DecisionConfigurations decisionConfigurations)
        {
            this.loggingBroker = loggingBroker;
            this.securityBroker = securityBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.pdsService = pdsService;
            this.patientService = patientService;
            this.notificationService = notificationService;
            this.decisionConfigurations = decisionConfigurations;
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
            bool generateNewCode = false) =>
            TryCatch(async () =>
            {
                ValidateRecordPatientInformationArguments(
                    nhsNumber: nhsNumber,
                    captchaToken: captcha,
                    notificationPreference: notificationPreference);

                bool isCaptchaValid = await this.securityBroker.ValidateCaptchaAsync(captcha);

                if (isCaptchaValid is false)
                {
                    throw new InvalidCaptchaPatientOrchestrationServiceException(
                        message: "The provided captcha token is invalid.");
                }

                IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
                Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == nhsNumber);
                Patient patientToRecord = null;
                DateTimeOffset now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                Enum.TryParse(
                    notificationPreference, out NotificationPreference notificationPreferenceType);

                if (maybeMatchingPatient is null)
                {
                    Patient pdsPatient = await this.pdsService.PatientLookupByNhsNumberAsync(nhsNumber);
                    string validationCode = GenerateValidationCode();

                    DateTimeOffset expirationDate =
                        now.AddMinutes(decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

                    pdsPatient.ValidationCode = validationCode;
                    pdsPatient.ValidationCodeExpiresOn = expirationDate;
                    pdsPatient.NotificationPreference = notificationPreferenceType;
                    patientToRecord = pdsPatient;

                    await this.patientService.AddPatientAsync(patientToRecord);
                }
                else
                {
                    if (maybeMatchingPatient.ValidationCodeExpiresOn <= now
                        || (maybeMatchingPatient.ValidationCodeExpiresOn > now && generateNewCode == true))
                    {
                        Patient pdsPatient = await this.pdsService.PatientLookupByNhsNumberAsync(nhsNumber);
                        maybeMatchingPatient.Address = pdsPatient.Address;
                        maybeMatchingPatient.DateOfBirth = pdsPatient.DateOfBirth;
                        maybeMatchingPatient.Email = pdsPatient.Email;
                        maybeMatchingPatient.Gender = pdsPatient.Gender;
                        maybeMatchingPatient.GivenName = pdsPatient.GivenName;
                        maybeMatchingPatient.NhsNumber = pdsPatient.NhsNumber;
                        maybeMatchingPatient.Phone = pdsPatient.Phone;
                        maybeMatchingPatient.PostCode = pdsPatient.PostCode;
                        maybeMatchingPatient.Surname = pdsPatient.Surname;
                        maybeMatchingPatient.Title = pdsPatient.Title;
                        maybeMatchingPatient.NotificationPreference = notificationPreferenceType;
                        string validationCode = GenerateValidationCode();

                        DateTimeOffset expirationDate =
                            now.AddMinutes(decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

                        maybeMatchingPatient.ValidationCode = validationCode;
                        maybeMatchingPatient.ValidationCodeExpiresOn = expirationDate;
                        patientToRecord = maybeMatchingPatient;

                        await this.patientService.ModifyPatientAsync(patientToRecord);
                    }
                    else
                    {
                        throw new ValidPatientCodeExistsException(message:
                            "A valid code already exists for this patient, please go to the enter code screen.");
                    }
                }

                NotificationInfo notificationInfo = new NotificationInfo
                {
                    Patient = patientToRecord
                };

                await this.notificationService.SendCodeNotificationAsync(notificationInfo);
            });

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
