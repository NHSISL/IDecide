// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using System.Threading.Tasks;
using System;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;

namespace LondonDataServices.IDecide.Core.Services.DecisionTypes
{
    public partial class DecisionTypeService
    {
        private async ValueTask ValidateDecisionTypeOnAdd(DecisionType decisionType)
        {
            ValidateDecisionTypeIsNotNull(decisionType);
            User currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(decisionType.Id), Parameter: nameof(DecisionType.Id)),
                (Rule: IsInvalid(decisionType.Name), Parameter: nameof(DecisionType.Name)),
                (Rule: IsInvalid(decisionType.CreatedDate), Parameter: nameof(DecisionType.CreatedDate)),
                (Rule: IsInvalid(decisionType.CreatedBy), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsInvalid(decisionType.UpdatedDate), Parameter: nameof(DecisionType.UpdatedDate)),
                (Rule: IsInvalid(decisionType.UpdatedBy), Parameter: nameof(DecisionType.UpdatedBy)),
                (Rule: IsInvalidLength(decisionType.Name, 255), Parameter: nameof(DecisionType.Name)),
                (Rule: IsInvalidLength(decisionType.CreatedBy, 255), Parameter: nameof(DecisionType.CreatedBy)),
                (Rule: IsInvalidLength(decisionType.UpdatedBy, 255), Parameter: nameof(DecisionType.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: decisionType.UpdatedDate,
                    secondDate: decisionType.CreatedDate,
                    secondDateName: nameof(DecisionType.CreatedDate)),
                Parameter: nameof(DecisionType.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUser.UserId,
                    second: decisionType.CreatedBy),
                Parameter: nameof(DecisionType.CreatedBy)),

                (Rule: IsNotSame(
                    first: decisionType.UpdatedBy,
                    second: decisionType.CreatedBy,
                    secondName: nameof(DecisionType.CreatedBy)),
                Parameter: nameof(DecisionType.UpdatedBy)),

                (Rule: await IsNotRecentAsync(decisionType.CreatedDate), Parameter: nameof(DecisionType.CreatedDate)));
        }

        private static void ValidateDecisionTypeIsNotNull(DecisionType decisionType)
        {
            if (decisionType is null)
            {
                throw new NullDecisionTypeException(message: "DecisionType is null.");
            }
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

        private static dynamic IsInvalidLength(string text, int maxLength) => new
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
            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDecisionTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDecisionTypeException.ThrowIfContainsErrors();
        }
    }
}
