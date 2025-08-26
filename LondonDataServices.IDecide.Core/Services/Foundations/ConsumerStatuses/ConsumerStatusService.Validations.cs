// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusService
    {
        private async ValueTask ValidateConsumerStatusOnAdd(ConsumerStatus consumerStatus)
        {
            ValidateConsumerStatusIsNotNull(consumerStatus);
            User currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate<InvalidConsumerStatusException>(
                message: "Invalid consumer status. Please correct the errors and try again.",
                (Rule: IsInvalid(consumerStatus.Id), Parameter: nameof(ConsumerStatus.Id)),
                (Rule: IsInvalid(consumerStatus.CreatedDate), Parameter: nameof(ConsumerStatus.CreatedDate)),
                (Rule: IsInvalid(consumerStatus.CreatedBy), Parameter: nameof(ConsumerStatus.CreatedBy)),
                (Rule: IsInvalid(consumerStatus.UpdatedDate), Parameter: nameof(ConsumerStatus.UpdatedDate)),
                (Rule: IsInvalid(consumerStatus.UpdatedBy), Parameter: nameof(ConsumerStatus.UpdatedBy)),
                (Rule: IsGreaterThan(consumerStatus.CreatedBy, 255), Parameter: nameof(ConsumerStatus.CreatedBy)),
                (Rule: IsGreaterThan(consumerStatus.UpdatedBy, 255), Parameter: nameof(ConsumerStatus.UpdatedBy)),

                (Rule: IsNotSame(
                        firstDate: consumerStatus.UpdatedDate,
                        secondDate: consumerStatus.CreatedDate,
                        secondDateName: nameof(ConsumerStatus.CreatedDate)),
                    Parameter: nameof(ConsumerStatus.UpdatedDate)),

                (Rule: IsNotSame(
                        first: currentUser.UserId,
                        second: consumerStatus.CreatedBy),
                    Parameter: nameof(ConsumerStatus.CreatedBy)),

                (Rule: IsNotSame(
                        first: consumerStatus.UpdatedBy,
                        second: consumerStatus.CreatedBy,
                        secondName: nameof(ConsumerStatus.CreatedBy)),
                    Parameter: nameof(ConsumerStatus.UpdatedBy)),

                (Rule: await IsNotRecentAsync(consumerStatus.CreatedDate),
                    Parameter: nameof(ConsumerStatus.CreatedDate)));
        }

        private static void ValidateConsumerStatusIsNotNull(ConsumerStatus consumerStatus)
        {
            if (consumerStatus is null)
            {
                throw new NullConsumerStatusException(message: "ConsumerStatus is null.");
            }
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

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsGreaterThan(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(string first, string second) => new
        {
            Condition = first != second,
            Message = $"Expected value to be '{first}' but found '{second}'."
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(string first, string second, string secondName) => new
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
