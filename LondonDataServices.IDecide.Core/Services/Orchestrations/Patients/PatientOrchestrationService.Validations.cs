// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService
    {
        private static void ValidatePatientLookupIsNotNull(PatientLookup patientLookup)
        {
            if (patientLookup is null)
            {
                throw new NullPatientLookupException("Patient lookup is null.");
            }
        }

        private static void ValidatePatientLookupPatientIsExactMatch(PatientLookup patientLookup)
        {
            if (patientLookup.Patients.Count != 1)
            {
                throw new NoExactPatientFoundException(
                    patientLookup.Patients.Count == 0
                        ? "No matching patient found."
                        : "Multiple matching patients found.");
            }
        }

        private static void ValidatePatientLookupByNhsNumberArguments(string nhsNumber)
        {
            Validate(
                (Rule: IsInvalidIdentifier(nhsNumber),
                Parameter: nameof(nhsNumber)));
        }

        private static void ValidateRecordPatientInformationArguments(
            string nhsNumber,
            string captchaToken,
            string notificationPreference)
        {
            Validate(
                (Rule: IsInvalidIdentifier(nhsNumber),
                Parameter: nameof(nhsNumber)),

                (Rule: IsInvalid(captchaToken),
                Parameter: nameof(captchaToken)),

                (Rule: IsInvalidNotificationPreference(notificationPreference),
                Parameter: nameof(notificationPreference)));
        }

        private static void ValidatePatientIsNotNull(Patient patient)
        {
            if (patient is null)
            {
                throw new NullPatientException("Patient is null.");
            }
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalidNotificationPreference(string notificationPreference)
        {
            return new
            {
                Condition = !(Enum.TryParse<NotificationPreference>(notificationPreference, true, out var preference)
                    && Enum.IsDefined(typeof(NotificationPreference), preference)),
                Message = "Text is not a valid notification preference"
            };
        }

        private static dynamic IsInvalidIdentifier(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name) || IsExactTenDigits(name) is false,
            Message = "Text must be exactly 10 digits."
        };

        private static bool IsExactTenDigits(string input)
        {
            bool result = input.Length == 10 && input.All(char.IsDigit);

            return result;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidPdsException =
                new InvalidPatientOrchestrationArgumentException(
                    message: "Invalid patient orchestration argument. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPdsException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPdsException.ThrowIfContainsErrors();
        }
    }
}
