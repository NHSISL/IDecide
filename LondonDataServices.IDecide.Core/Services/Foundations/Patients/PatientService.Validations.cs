// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public partial class PatientService
    {
        private async ValueTask ValidatePatientOnAdd(Patient patient)
        {
            ValidatePatientIsNotNull(patient);
            string userId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again."),
                (Rule: IsInvalid(patient.Id), Parameter: nameof(Patient.Id)),
                (Rule: IsInvalid(patient.NhsNumber), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsInvalid(patient.GivenName), Parameter: nameof(Patient.GivenName)),
                (Rule: IsInvalid(patient.Surname), Parameter: nameof(Patient.Surname)),
                (Rule: IsInvalid(patient.DateOfBirth), Parameter: nameof(Patient.DateOfBirth)),
                (Rule: IsInvalid(patient.ValidationCode), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsInvalid(patient.ValidationCodeExpiresOn), Parameter: nameof(Patient.ValidationCodeExpiresOn)),
                (Rule: IsInvalid(patient.Gender), Parameter: nameof(Patient.Gender)),
                (Rule: IsInvalid(patient.CreatedDate), Parameter: nameof(Patient.CreatedDate)),
                (Rule: IsInvalid(patient.CreatedBy), Parameter: nameof(Patient.CreatedBy)),
                (Rule: IsInvalid(patient.UpdatedDate), Parameter: nameof(Patient.UpdatedDate)),
                (Rule: IsInvalid(patient.UpdatedBy), Parameter: nameof(Patient.UpdatedBy)),
                (Rule: IsGreaterThan(patient.NhsNumber, 10), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsGreaterThan(patient.Title, 35), Parameter: nameof(Patient.Title)),
                (Rule: IsGreaterThan(patient.GivenName, 255), Parameter: nameof(Patient.GivenName)),
                (Rule: IsGreaterThan(patient.Surname, 255), Parameter: nameof(Patient.Surname)),
                (Rule: IsGreaterThan(patient.Email, 255), Parameter: nameof(Patient.Email)),
                (Rule: IsGreaterThan(patient.Phone, 15), Parameter: nameof(Patient.Phone)),
                (Rule: IsGreaterThan(patient.PostCode, 8), Parameter: nameof(Patient.PostCode)),
                (Rule: IsGreaterThan(patient.ValidationCode, 5), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsGreaterThan(patient.CreatedBy, 255), Parameter: nameof(Patient.CreatedBy)),
                (Rule: IsGreaterThan(patient.UpdatedBy, 255), Parameter: nameof(Patient.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: patient.UpdatedDate,
                    secondDate: patient.CreatedDate,
                    secondDateName: nameof(Patient.CreatedDate)),
                Parameter: nameof(Patient.UpdatedDate)),

                (Rule: IsNotSame(
                    first: userId,
                    second: patient.CreatedBy),
                Parameter: nameof(Patient.CreatedBy)),

                (Rule: IsNotSame(
                    first: patient.UpdatedBy,
                    second: patient.CreatedBy,
                    secondName: nameof(Patient.CreatedBy)),
                Parameter: nameof(Patient.UpdatedBy)),

                (Rule: await IsNotRecentAsync(patient.CreatedDate), Parameter: nameof(Patient.CreatedDate)));
        }

        private async ValueTask ValidatePatientOnModify(Patient patient)
        {
            ValidatePatientIsNotNull(patient);
            string userId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again."),
                (Rule: IsInvalid(patient.Id), Parameter: nameof(Patient.Id)),
                (Rule: IsInvalid(patient.NhsNumber), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsInvalid(patient.GivenName), Parameter: nameof(Patient.GivenName)),
                (Rule: IsInvalid(patient.Surname), Parameter: nameof(Patient.Surname)),
                (Rule: IsInvalid(patient.DateOfBirth), Parameter: nameof(Patient.DateOfBirth)),
                (Rule: IsInvalid(patient.ValidationCode), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsInvalid(patient.ValidationCodeExpiresOn), Parameter: nameof(Patient.ValidationCodeExpiresOn)),
                (Rule: IsInvalid(patient.Gender), Parameter: nameof(Patient.Gender)),
                (Rule: IsInvalid(patient.CreatedDate), Parameter: nameof(Patient.CreatedDate)),
                (Rule: IsInvalid(patient.CreatedBy), Parameter: nameof(Patient.CreatedBy)),
                (Rule: IsInvalid(patient.UpdatedDate), Parameter: nameof(Patient.UpdatedDate)),
                (Rule: IsInvalid(patient.UpdatedBy), Parameter: nameof(Patient.UpdatedBy)),
                (Rule: IsGreaterThan(patient.NhsNumber, 10), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsGreaterThan(patient.Title, 35), Parameter: nameof(Patient.Title)),
                (Rule: IsGreaterThan(patient.GivenName, 255), Parameter: nameof(Patient.GivenName)),
                (Rule: IsGreaterThan(patient.Surname, 255), Parameter: nameof(Patient.Surname)),
                (Rule: IsGreaterThan(patient.Email, 255), Parameter: nameof(Patient.Email)),
                (Rule: IsGreaterThan(patient.Phone, 15), Parameter: nameof(Patient.Phone)),
                (Rule: IsGreaterThan(patient.PostCode, 8), Parameter: nameof(Patient.PostCode)),
                (Rule: IsGreaterThan(patient.ValidationCode, 5), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsGreaterThan(patient.CreatedBy, 255), Parameter: nameof(Patient.CreatedBy)),
                (Rule: IsGreaterThan(patient.UpdatedBy, 255), Parameter: nameof(Patient.UpdatedBy)),

                (Rule: IsNotSame(
                    first: userId,
                    second: patient.UpdatedBy),
                Parameter: nameof(Patient.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: patient.UpdatedDate,
                    secondDate: patient.CreatedDate,
                    secondDateName: nameof(Patient.CreatedDate)),
                Parameter: nameof(Patient.UpdatedDate)),

                (Rule: await IsNotRecentAsync(patient.UpdatedDate), Parameter: nameof(Patient.UpdatedDate)));
        }

        public void ValidatePatientId(Guid patientId) =>
            Validate(
                createException: () => new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again."),
                validations: (Rule: IsInvalid(patientId), Parameter: nameof(Patient.Id)));

        private static void ValidateStoragePatient(Patient maybePatient, Guid patientId)
        {
            if (maybePatient is null)
            {
                throw new NotFoundPatientException(
                    message: $"Couldn't find patient with patientId: {patientId}.");
            }
        }

        private static void ValidatePatientIsNotNull(Patient patient)
        {
            if (patient is null)
            {
                throw new NullPatientException(message: "Patient is null.");
            }
        }

        private static void ValidateAgainstStoragePatientOnModify(
            Patient inputPatient,
            Patient storagePatient)
        {
            Validate(
                createException: () => new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again."),
                (Rule: IsNotSame(
                    firstDate: inputPatient.CreatedDate,
                    secondDate: storagePatient.CreatedDate,
                    secondDateName: nameof(Patient.CreatedDate)),
                Parameter: nameof(Patient.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputPatient.CreatedBy,
                    second: storagePatient.CreatedBy,
                    secondName: nameof(Patient.CreatedBy)),
                Parameter: nameof(Patient.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputPatient.UpdatedDate,
                    secondDate: storagePatient.UpdatedDate,
                    secondDateName: nameof(Patient.UpdatedDate)),
                Parameter: nameof(Patient.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsGreaterThan(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second) => new
            {
                Condition = first != second,
                Message = $"Expected value to be '{first}' but found '{second}'."
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private static dynamic IsNotSame(
           string first,
           string second,
           string secondName) => new
           {
               Condition = first != second,
               Message = $"Text is not the same as {secondName}"
           };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };
        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastThreshold = 90;
            int futureThreshold = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastThreshold);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureThreshold);
            bool isNotRecent = date < startDate || date > endDate;

            return (isNotRecent, startDate, endDate);
        }

        private static void Validate<T>(
            Func<T> createException,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            T invalidDataException = createException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}