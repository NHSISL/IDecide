// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeService
    {
        private async ValueTask ValidateDecisionTypeOnAdd(DecisionType decisionType)
        {
            ValidateDecisionTypeIsNotNull(decisionType);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate<InvalidDecisionTypeException>(
                message: "Invalid decisionType. Please correct the errors and try again.",
                (Rule: IsInvalid(decisionType.Id), Parameter: nameof(DecisionType.Id)),
                (Rule: IsInvalid(decisionType.Name), Parameter: nameof(DecisionType.Name)),
                (Rule: IsInvalid(decisionType.CreatedDate), Parameter: nameof(DecisionType.CreatedDate)),
                (Rule: IsInvalid(decisionType.CreatedBy), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsInvalid(decisionType.UpdatedDate), Parameter: nameof(DecisionType.UpdatedDate)),
                (Rule: IsInvalid(decisionType.UpdatedBy), Parameter: nameof(DecisionType.UpdatedBy)),
                (Rule: IsGreaterThan(decisionType.Name, 255), Parameter: nameof(DecisionType.Name)),
                (Rule: IsGreaterThan(decisionType.CreatedBy, 255), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsGreaterThan(decisionType.UpdatedBy, 255), Parameter: nameof(DecisionType.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: decisionType.UpdatedDate,
                    secondDate: decisionType.CreatedDate,
                    secondDateName: nameof(DecisionType.CreatedDate)),
                Parameter: nameof(DecisionType.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: decisionType.CreatedBy),
                Parameter: nameof(DecisionType.CreatedBy)),

                (Rule: IsNotSame(
                    first: decisionType.UpdatedBy,
                    second: decisionType.CreatedBy,
                    secondName: nameof(DecisionType.CreatedBy)),
                Parameter: nameof(DecisionType.UpdatedBy)),

                (Rule: await IsNotRecentAsync(decisionType.CreatedDate), Parameter: nameof(DecisionType.CreatedDate)));
        }

        private async ValueTask ValidateDecisionTypeOnModify(DecisionType decisionType)
        {
            ValidateDecisionTypeIsNotNull(decisionType);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate<InvalidDecisionTypeException>(
                message: "Invalid decisionType. Please correct the errors and try again.",
                (Rule: IsInvalid(decisionType.Id), Parameter: nameof(DecisionType.Id)),
                (Rule: IsInvalid(decisionType.Name), Parameter: nameof(DecisionType.Name)),
                (Rule: IsInvalid(decisionType.CreatedDate), Parameter: nameof(DecisionType.CreatedDate)),
                (Rule: IsInvalid(decisionType.CreatedBy), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsInvalid(decisionType.UpdatedDate), Parameter: nameof(DecisionType.UpdatedDate)),
                (Rule: IsInvalid(decisionType.UpdatedBy), Parameter: nameof(DecisionType.UpdatedBy)),
                (Rule: IsGreaterThan(decisionType.Name, 255), Parameter: nameof(DecisionType.Name)),
                (Rule: IsGreaterThan(decisionType.CreatedBy, 255), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsGreaterThan(decisionType.UpdatedBy, 255), Parameter: nameof(DecisionType.UpdatedBy)),

                 (Rule: IsNotSame(
                    first: currentUserId,
                    second: decisionType.UpdatedBy),
                Parameter: nameof(DecisionType.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: decisionType.UpdatedDate,
                    secondDate: decisionType.CreatedDate,
                    secondDateName: nameof(DecisionType.CreatedDate)),
                Parameter: nameof(DecisionType.UpdatedDate)),

                (Rule: await IsNotRecentAsync(decisionType.UpdatedDate), Parameter: nameof(DecisionType.UpdatedDate)));
        }

        public void ValidateDecisionTypeId(Guid decisionTypeId) =>
            Validate<InvalidDecisionTypeException>(
                message: "Invalid decisionType. Please correct the errors and try again.",
                validations: (Rule: IsInvalid(decisionTypeId), Parameter: nameof(DecisionType.Id)));

        private static void ValidateStorageDecisionType(DecisionType maybeDecisionType, Guid decisionTypeId)
        {
            if (maybeDecisionType is null)
            {
                throw new NotFoundDecisionTypeException(
                    message: $"Couldn't find decision type with decisionTypeId: {decisionTypeId}.");
            }
        }

        private static void ValidateDecisionTypeIsNotNull(DecisionType decisionType)
        {
            if (decisionType is null)
            {
                throw new NullDecisionTypeException(message: "DecisionType is null.");
            }
        }

        private static void ValidateAgainstStorageDecisionTypeOnModify(
            DecisionType inputDecisionType,
            DecisionType storageDecisionType)
        {
            Validate<InvalidDecisionTypeException>(
                message: "Invalid decisionType. Please correct the errors and try again.",
                (Rule: IsNotSame(
                    firstDate: inputDecisionType.CreatedDate,
                    secondDate: storageDecisionType.CreatedDate,
                    secondDateName: nameof(DecisionType.CreatedDate)),
                Parameter: nameof(DecisionType.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputDecisionType.CreatedBy,
                    second: storageDecisionType.CreatedBy,
                    secondName: nameof(DecisionType.CreatedBy)),
                Parameter: nameof(DecisionType.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputDecisionType.UpdatedDate,
                    secondDate: storageDecisionType.UpdatedDate,
                    secondDateName: nameof(DecisionType.UpdatedDate)),
                Parameter: nameof(DecisionType.UpdatedDate)));
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

        private static void Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T), message);

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