// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
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

            Validate(
                (Rule: IsInvalid(patient.Id), Parameter: nameof(Patient.Id)),
                (Rule: IsInvalidIdentifier(patient.NhsNumber), Parameter: nameof(Patient.NhsNumber)),
                (Rule: IsInvalidCode(patient.ValidationCode), Parameter: nameof(Patient.ValidationCode)),
                (Rule: IsInvalid(patient.ValidationCodeExpiresOn), Parameter: nameof(Patient.ValidationCodeExpiresOn)),
                (Rule: IsInvalid(patient.RetryCount), Parameter: nameof(Patient.RetryCount)));
        }

        private static void ValidatePatientIsNotNull(Patient patient)
        {
            if (patient is null)
            {
                throw new NullPatientException(message: "Patient is null.");
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
