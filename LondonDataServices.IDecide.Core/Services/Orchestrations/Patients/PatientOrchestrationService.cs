// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
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

        public ValueTask RecordPatientInformationAsync(
            string nhsNumber,
            string notificationPreference,
            bool generateNewCode = false) =>
            TryCatch(async () =>
            {
                ValidateRecordPatientInformationArguments(
                    nhsNumber: nhsNumber,
                    notificationPreference: notificationPreference);

                bool isAuthenticatedUserWithRole = await CheckIfIsAuthenticatedUserWithRequiredRoleAsync();
                IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
                Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == nhsNumber);
                Patient patientToRecord = null;
                DateTimeOffset now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                bool codeIsExpired =
                    maybeMatchingPatient is null ? true : maybeMatchingPatient.ValidationCodeExpiresOn <= now;

                Enum.TryParse(
                    notificationPreference, out NotificationPreference notificationPreferenceType);

                if (maybeMatchingPatient is null)
                {
                    patientToRecord = await GenerateNewPatientWithCodeAsync(
                        nhsNumber, notificationPreferenceType, now);

                    await SendValidationCodeNotificationAsync(patientToRecord);

                    return;
                }

                if (codeIsExpired is false
                    && maybeMatchingPatient.ValidationCodeMatchedOn is null
                    && generateNewCode is false)
                {
                    throw new ValidPatientCodeExistsException(message:
                        "A valid code already exists for this patient, please go to the enter code screen.");
                }

                if (isAuthenticatedUserWithRole)
                {
                    patientToRecord = await UpdatePatientWithNewCodeAsync(
                        maybeMatchingPatient, notificationPreferenceType, now, true);

                    await SendValidationCodeNotificationAsync(patientToRecord);

                    return;
                }

                if (codeIsExpired)
                {
                    patientToRecord = await UpdatePatientWithNewCodeAsync(
                        maybeMatchingPatient, notificationPreferenceType, now, true);

                    await SendValidationCodeNotificationAsync(patientToRecord);

                    return;
                }

                if (maybeMatchingPatient.RetryCount >= this.decisionConfigurations.MaxRetryCount)
                {
                    throw new MaxRetryAttemptsExceededException(message:
                        "The maximum number of validation attempts has been exceeded, please contact support.");
                }

                patientToRecord = await UpdatePatientWithNewCodeAsync(
                    maybeMatchingPatient, notificationPreferenceType, now);

                await SendValidationCodeNotificationAsync(patientToRecord);
            });

        virtual internal async ValueTask<Patient> GenerateNewPatientWithCodeAsync(
            string nhsNumber,
            NotificationPreference notificationPreference,
            DateTimeOffset now)
        {
            Patient pdsPatient = await this.pdsService.PatientLookupByNhsNumberAsync(nhsNumber);
            string validationCode = await this.patientService.GenerateValidationCodeAsync();

            DateTimeOffset expirationDate =
                now.AddMinutes(decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

            pdsPatient.ValidationCode = validationCode;
            pdsPatient.ValidationCodeExpiresOn = expirationDate;
            pdsPatient.ValidationCodeMatchedOn = null;
            pdsPatient.NotificationPreference = notificationPreference;
            Patient patientToRecord = pdsPatient;
            Patient recordedPatient = await this.patientService.AddPatientAsync(patientToRecord);

            return recordedPatient;
        }

        virtual internal async ValueTask<Patient> UpdatePatientWithNewCodeAsync(
            Patient currentPatient,
            NotificationPreference notificationPreference,
            DateTimeOffset now,
            bool resetRetryCount = false)
        {
            Patient patientToUpdate = currentPatient;
            Patient pdsPatient = await this.pdsService.PatientLookupByNhsNumberAsync(patientToUpdate.NhsNumber);
            patientToUpdate.Address = pdsPatient.Address;
            patientToUpdate.DateOfBirth = pdsPatient.DateOfBirth;
            patientToUpdate.Email = pdsPatient.Email;
            patientToUpdate.Gender = pdsPatient.Gender;
            patientToUpdate.GivenName = pdsPatient.GivenName;
            patientToUpdate.NhsNumber = pdsPatient.NhsNumber;
            patientToUpdate.Phone = pdsPatient.Phone;
            patientToUpdate.PostCode = pdsPatient.PostCode;
            patientToUpdate.Surname = pdsPatient.Surname;
            patientToUpdate.Title = pdsPatient.Title;
            patientToUpdate.NotificationPreference = notificationPreference;
            string validationCode = await this.patientService.GenerateValidationCodeAsync();

            DateTimeOffset expirationDate =
                now.AddMinutes(decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

            patientToUpdate.ValidationCode = validationCode;
            patientToUpdate.ValidationCodeMatchedOn = null;
            patientToUpdate.ValidationCodeExpiresOn = expirationDate;

            if (resetRetryCount)
            {
                patientToUpdate.RetryCount = 0;
            }

            Patient modifiedPatient = await this.patientService.ModifyPatientAsync(patientToUpdate);

            return modifiedPatient;
        }

        virtual internal async ValueTask<bool> CheckIfIsAuthenticatedUserWithRequiredRoleAsync()
        {
            var currentUserIsAuthenticated = await this.securityBroker.IsCurrentUserAuthenticatedAsync();

            if (currentUserIsAuthenticated)
            {
                bool userIsInWorkflowRole = false;

                foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
                {
                    if (await this.securityBroker.IsInRoleAsync(role))
                    {
                        userIsInWorkflowRole = true;
                        break;
                    }
                }

                if (userIsInWorkflowRole is false)
                {
                    throw new UnauthorizedPatientOrchestrationServiceException(
                        message: "The current user is not authorized to perform this operation.");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                bool isCaptchaValid = await this.securityBroker.ValidateCaptchaAsync();

                if (isCaptchaValid is false)
                {
                    throw new InvalidCaptchaPatientOrchestrationServiceException(
                        message: "The provided captcha token is invalid.");
                }
                else
                {
                    return false;
                }
            }
        }

        virtual internal async ValueTask SendValidationCodeNotificationAsync(Patient patientToSend)
        {
            NotificationInfo notificationInfo = new NotificationInfo
            {
                Patient = patientToSend
            };

            await this.notificationService.SendCodeNotificationAsync(notificationInfo);
        }

        public ValueTask VerifyPatientCodeAsync(string nhsNumber, string verificationCode)
        {
            throw new NotImplementedException();
        }
    }
}
