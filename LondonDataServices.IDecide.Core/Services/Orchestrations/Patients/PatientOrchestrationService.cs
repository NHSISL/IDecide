// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Audits;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
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
        private readonly IAuditBroker auditBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IPdsService pdsService;
        private readonly IPatientService patientService;
        private readonly INotificationService notificationService;
        private readonly DecisionConfigurations decisionConfigurations;

        public PatientOrchestrationService(
            ILoggingBroker loggingBroker,
            ISecurityBroker securityBroker,
            IDateTimeBroker dateTimeBroker,
            IAuditBroker auditBroker,
            IIdentifierBroker identifierBroker,
            IPdsService pdsService,
            IPatientService patientService,
            INotificationService notificationService,
            DecisionConfigurations decisionConfigurations)
        {
            this.loggingBroker = loggingBroker;
            this.securityBroker = securityBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.auditBroker = auditBroker;
            this.identifierBroker = identifierBroker;
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

        public async ValueTask VerifyPatientCodeAsync(string nhsNumber, string verificationCode)
        {
            ValidateVerifyPatientCodeArguments(nhsNumber: nhsNumber, verificationCode: verificationCode);
            bool isAuthenticatedUserWithRole = await CheckIfIsAuthenticatedUserWithRequiredRoleAsync();
            IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
            Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == nhsNumber);
            Patient patientToUpdate = maybeMatchingPatient;
            ValidatePatientExists(maybeMatchingPatient);
            Guid correlationId = await this.identifierBroker.GetIdentifierAsync();

            if (isAuthenticatedUserWithRole)
            {
                var currentUser = await this.securityBroker.GetCurrentUserAsync();

                await this.auditBroker.LogInformationAsync(
                    auditType: "Patient Code",
                    title: "Validating Patient Code",
                    message: $"User {currentUser.UserId} is validating a code for patient {nhsNumber}.",
                    fileName: null,
                    correlationId: correlationId.ToString());

                if (maybeMatchingPatient.ValidationCode != verificationCode)
                {
                    await this.auditBroker.LogInformationAsync(
                        auditType: "Patient Code",
                        title: "Patient Code Validation Failed",
                        message: "The validation code provided was incorrect.",
                        fileName: null,
                        correlationId: correlationId.ToString());

                    throw new IncorrectValidationCodeException("The validation code provided is incorrect.");
                }
            }
            else
            {
                string ipAddress = await this.securityBroker.GetIpAddressAsync();

                await this.auditBroker.LogInformationAsync(
                    auditType: "Patient Code",
                    title: "Validating Patient Code",
                    message: $"Patient with IP address {ipAddress} is validating a code for patient {nhsNumber}.",
                    fileName: null,
                    correlationId: correlationId.ToString());

                if (maybeMatchingPatient.RetryCount > this.decisionConfigurations.MaxRetryCount)
                {
                    await this.auditBroker.LogInformationAsync(
                        auditType: "Patient Code",
                        title: "Patient Code Validation Failed",

                        message: $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded " +
                           $"for patient {nhsNumber}",

                        fileName: null,
                        correlationId: correlationId.ToString());

                    throw new ExceededMaxRetryCountException(
                        $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded.");
                }

                if (maybeMatchingPatient.ValidationCode != verificationCode)
                {
                    patientToUpdate.RetryCount += 1;
                    await this.patientService.ModifyPatientAsync(patientToUpdate);

                    await this.auditBroker.LogInformationAsync(
                        auditType: "Patient Code",
                        title: "Patient Code Validation Failed",
                        message: "The validation code provided was incorrect.",
                        fileName: null,
                        correlationId: correlationId.ToString());

                    throw new IncorrectValidationCodeException("The validation code provided is incorrect.");
                }

                DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                if (maybeMatchingPatient.ValidationCodeExpiresOn < currentDateTime)
                {
                    string newValidationCode = await this.patientService.GenerateValidationCodeAsync();
                    patientToUpdate.ValidationCode = newValidationCode;
                    patientToUpdate.ValidationCodeMatchedOn = null;
                    patientToUpdate.RetryCount = 0;

                    patientToUpdate.ValidationCodeExpiresOn =
                        currentDateTime.AddMinutes(
                            this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

                    await this.patientService.ModifyPatientAsync(patientToUpdate);

                    await this.auditBroker.LogInformationAsync(
                        auditType: "Patient Code",
                        title: "New Validation Code Generated",
                        message: "The validation code was expired so a new code was issued.",
                        fileName: null,
                        correlationId: correlationId.ToString());

                    throw new RenewedValidationCodeException(
                        "The validation code has expired, but we have issued a new code that will be sent via " +
                        "your prefered contact method");
                }
            }

            patientToUpdate.ValidationCodeMatchedOn = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            await this.patientService.ModifyPatientAsync(patientToUpdate);

            await this.auditBroker.LogInformationAsync(
                auditType: "Patient Code",
                title: "Patient Code Validation Succeeded",
                message: "The validation code provided was valid and successfully verified.",
                fileName: null,
                correlationId: correlationId.ToString());
        }

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
    }
}
