// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationService
    {
        private static void ValidateVerifyAndRecordDecisionArguments(Decision decision)
        {
            ValidateDecisionIsNotNull(decision);
            ValidateDecisionProperties(decision);
        }

        private static void ValidateDecisionIsNotNull(Decision decision)
        {
            if (decision is null)
            {
                throw new NullDecisionException("Decision is null.");
            }
        }

        private static void ValidateDecisionProperties(Decision decision)
        {
            Validate(
                (Rule: IsPatientNull(decision.Patient),
                Parameter: nameof(decision.Patient)),

                (Rule: IsInvalidIdentifier(decision.Patient?.NhsNumber),
                Parameter: nameof(decision.Patient.NhsNumber)),

                (Rule: IsInvalidIdentifier(decision.PatientNhsNumber),
                Parameter: nameof(decision.PatientNhsNumber)),

                (Rule: IsInvalidValidationCode(decision.Patient?.ValidationCode),
                Parameter: nameof(decision.Patient.ValidationCode)));
        }

        private static void ValidatePatientExists(Patient patient)
        {
            if (patient is null)
            {
                throw new NotFoundPatientException("Patient does not exist");
            }
        }

        private static dynamic IsPatientNull(Patient patient) => new
        {
            Condition = patient is null,
            Message = "Patient cannot be null."
        };

        private static dynamic IsInvalidIdentifier(string identifier) => new
        {
            Condition = String.IsNullOrWhiteSpace(identifier) || IsExactTenDigits(identifier) is false,
            Message = "Text must be exactly 10 digits."
        };

        private static bool IsExactTenDigits(string input)
        {
            bool result = input.Length == 10 && input.All(char.IsDigit);

            return result;
        }

        private static dynamic IsInvalidValidationCode(string validationCode) => new
        {
            Condition = String.IsNullOrWhiteSpace(validationCode) || IsExactFiveCharacters(validationCode) is false,
            Message = "Code must be 5 characters long."
        };

        private static bool IsExactFiveCharacters(string input)
        {
            bool result = input.Length == 5;

            return result;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidPdsException =
                new InvalidDecisionOrchestrationArgumentException(
                    message: "Invalid decision orchestration argument. Please correct the errors and try again.");

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
