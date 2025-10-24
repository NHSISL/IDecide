// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Decisions
{
    public partial class DecisionService
    {
        private async ValueTask ValidateDecisionOnAdd(Decision decision)
        {
            ValidateDecisionIsNotNull(decision);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again."),

                (Rule: IsInvalid(decision.Id), Parameter: nameof(Decision.Id)),
                (Rule: IsInvalid(decision.DecisionChoice), Parameter: nameof(Decision.DecisionChoice)),
                (Rule: IsInvalid(decision.CreatedDate), Parameter: nameof(Decision.CreatedDate)),
                (Rule: IsInvalid(decision.CreatedBy), Parameter: nameof(Decision.CreatedBy)),
                (Rule: IsInvalid(decision.UpdatedDate), Parameter: nameof(Decision.UpdatedDate)),
                (Rule: IsInvalid(decision.UpdatedBy), Parameter: nameof(Decision.UpdatedBy)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonGivenName, 255),
                    Parameter: nameof(Decision.ResponsiblePersonGivenName)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonSurname, 255),
                    Parameter: nameof(Decision.ResponsiblePersonSurname)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonRelationship, 255),
                    Parameter: nameof(Decision.ResponsiblePersonRelationship)),

                (Rule: IsGreaterThan(decision.CreatedBy, 255), Parameter: nameof(Decision.CreatedBy)),
                (Rule: IsGreaterThan(decision.UpdatedBy, 255), Parameter: nameof(Decision.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: decision.UpdatedDate,
                    secondDate: decision.CreatedDate,
                    secondDateName: nameof(Decision.CreatedDate)),
                Parameter: nameof(Decision.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: decision.CreatedBy),
                Parameter: nameof(Decision.CreatedBy)),

                (Rule: IsNotSame(
                    first: decision.UpdatedBy,
                    second: decision.CreatedBy,
                    secondName: nameof(Decision.CreatedBy)),
                Parameter: nameof(Decision.UpdatedBy)),

                (Rule: await IsNotRecentAsync(decision.CreatedDate), Parameter: nameof(Decision.CreatedDate)));
        }

        private async ValueTask ValidateDecisionOnModify(Decision decision)
        {
            ValidateDecisionIsNotNull(decision);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again."),

                (Rule: IsInvalid(decision.Id), Parameter: nameof(Decision.Id)),
                (Rule: IsInvalid(decision.DecisionChoice), Parameter: nameof(Decision.DecisionChoice)),
                (Rule: IsInvalid(decision.CreatedDate), Parameter: nameof(Decision.CreatedDate)),
                (Rule: IsInvalid(decision.CreatedBy), Parameter: nameof(Decision.CreatedBy)),
                (Rule: IsInvalid(decision.UpdatedDate), Parameter: nameof(Decision.UpdatedDate)),
                (Rule: IsInvalid(decision.UpdatedBy), Parameter: nameof(Decision.UpdatedBy)),
                (Rule: IsGreaterThan(decision.CreatedBy, 255), Parameter: nameof(Decision.CreatedBy)),
                (Rule: IsGreaterThan(decision.UpdatedBy, 255), Parameter: nameof(Decision.UpdatedBy)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonGivenName, 255),
                    Parameter: nameof(Decision.ResponsiblePersonGivenName)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonSurname, 255),
                    Parameter: nameof(Decision.ResponsiblePersonSurname)),

                (Rule: IsGreaterThan(decision.ResponsiblePersonRelationship, 255),
                    Parameter: nameof(Decision.ResponsiblePersonRelationship)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: decision.UpdatedBy),
                Parameter: nameof(Decision.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: decision.UpdatedDate,
                    secondDate: decision.CreatedDate,
                    secondDateName: nameof(Decision.CreatedDate)),
                Parameter: nameof(Decision.UpdatedDate)),

                (Rule: await IsNotRecentAsync(decision.UpdatedDate), Parameter: nameof(Decision.UpdatedDate)));
        }

        public void ValidateDecisionId(Guid decisionId) =>
            Validate(
                createException: () => new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again."),
                    
                validations: (Rule: IsInvalid(decisionId), Parameter: nameof(Decision.Id)));

        private static void ValidateStorageDecision(Decision maybeDecision, Guid decisionId)
        {
            if (maybeDecision is null)
            {
                throw new NotFoundDecisionException(
                    message: $"Couldn't find decision type with decisionId: {decisionId}.");
            }
        }

        private static void ValidateDecisionIsNotNull(Decision decision)
        {
            if (decision is null)
            {
                throw new NullDecisionException(message: "Decision is null.");
            }
        }

        private static void ValidateAgainstStorageDecisionOnModify(
            Decision inputDecision,
            Decision storageDecision)
        {
            Validate(
                createException: () => new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again."),

                (Rule: IsNotSame(
                    firstDate: inputDecision.CreatedDate,
                    secondDate: storageDecision.CreatedDate,
                    secondDateName: nameof(Decision.CreatedDate)),
                Parameter: nameof(Decision.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputDecision.CreatedBy,
                    second: storageDecision.CreatedBy,
                    secondName: nameof(Decision.CreatedBy)),
                Parameter: nameof(Decision.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputDecision.UpdatedDate,
                    secondDate: storageDecision.UpdatedDate,
                    secondDateName: nameof(Decision.UpdatedDate)),
                Parameter: nameof(Decision.UpdatedDate)));
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