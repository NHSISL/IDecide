// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public partial class PatientService
    {
        private async ValueTask ValidatePatientOnAdd(Patient patient)
        {
            ValidatePatientIsNotNull(patient);
            User currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(patient.Id), Parameter: nameof(Patient.Id)),
                (Rule: IsInvalidIdentifier(patient.NhsNumber), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsInvalidCode(patient.ValidationCode), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsInvalid(patient.ValidationCodeExpiresOn), Parameter: nameof(Patient.ValidationCodeExpiresOn)),
                (Rule: IsInvalid(patient.RetryCount), Parameter: nameof(Patient.RetryCount)),
                (Rule: IsInvalid(patient.Title), Parameter: nameof(Patient.Title)),
                (Rule: IsInvalid(patient.GivenName), Parameter: nameof(Patient.GivenName)),
                (Rule: IsInvalid(patient.Surname), Parameter: nameof(Patient.Surname)),
                (Rule: IsInvalid(patient.DateOfBirth), Parameter: nameof(Patient.DateOfBirth)),
                (Rule: IsInvalid(patient.Gender), Parameter: nameof(Patient.Gender)),
                (Rule: IsInvalid(patient.Email), Parameter: nameof(Patient.Email)),
                (Rule: IsInvalid(patient.Phone), Parameter: nameof(Patient.Phone)),
                (Rule: IsInvalid(patient.Address), Parameter: nameof(Patient.Address)),
                (Rule: IsInvalid(patient.PostCode), Parameter: nameof(Patient.PostCode)),
                (Rule: IsInvalid(patient.CreatedDate), Parameter: nameof(Patient.CreatedDate)),
                (Rule: IsInvalid(patient.CreatedBy), Parameter: nameof(Patient.CreatedBy)),
                (Rule: IsInvalid(patient.UpdatedDate), Parameter: nameof(Patient.UpdatedDate)),
                (Rule: IsInvalid(patient.UpdatedBy), Parameter: nameof(Patient.UpdatedBy)),
                (Rule: IsInvalidLength(patient.Title, 255), Parameter: nameof(Patient.Title)),
                (Rule: IsInvalidLength(patient.GivenName, 255), Parameter: nameof(Patient.GivenName)),
                (Rule: IsInvalidLength(patient.Surname, 255), Parameter: nameof(Patient.Surname)),
                (Rule: IsInvalidLength(patient.Gender, 255), Parameter: nameof(Patient.Gender)),
                (Rule: IsInvalidLength(patient.Email, 255), Parameter: nameof(Patient.Email)),
                (Rule: IsInvalidLength(patient.Phone, 255   ), Parameter: nameof(Patient.Phone)),
                (Rule: IsInvalidLength(patient.Address, 255), Parameter: nameof(Patient.Address)),
                (Rule: IsInvalidLength(patient.PostCode, 255), Parameter: nameof(Patient.PostCode)),

                (Rule: IsNotSame(
                    firstDate: patient.UpdatedDate,
                    secondDate: patient.CreatedDate,
                    secondDateName: nameof(Patient.CreatedDate)),
                Parameter: nameof(Patient.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUser.UserId,
                    second: patient.CreatedBy),
                Parameter: nameof(Patient.CreatedBy)),

                (Rule: IsNotSame(
                    first: patient.UpdatedBy,
                    second: patient.CreatedBy,
                    secondName: nameof(Patient.CreatedBy)),
                Parameter: nameof(Patient.UpdatedBy)),

                (Rule: await IsNotRecentAsync(patient.CreatedDate), Parameter: nameof(Patient.CreatedDate)));
        }

        private static void ValidatePatientIsNotNull(Patient patient)
        {
            if (patient is null)
            {
                throw new NullPatientException(message: "Patient is null.");
            }
        }

        public void ValidatePatientId(Guid patientId) =>
            Validate((Rule: IsInvalid(patientId), Parameter: nameof(Patient.Id)));

        private static void ValidateStoragePatient(Patient maybePatient, Guid patientId)
        {
            if (maybePatient is null)
            {
                throw new NotFoundPatientException(
                    $"Couldn't find patient with patientId: {patientId}.");
            }
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

        private static dynamic IsInvalidCode(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name) || IsExact5Digits(name) is false,
            Message = "Text must be exactly 5 digits."
        };

        private static bool IsExact5Digits(string input)
        {
            bool result = input.Length == 5 && input.All(char.IsDigit);

            return result;
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalid(int number) => new
        {
            Condition = number <= 0,
            Message = "Number is invalid"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPatientException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPatientException.ThrowIfContainsErrors();
        }
    }
}
