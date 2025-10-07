// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionService
    {
        private async ValueTask ValidateConsumerAdoptionOnAdd(ConsumerAdoption consumerAdoption)
        {
            ValidateConsumerAdoptionIsNotNull(consumerAdoption);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate<InvalidConsumerAdoptionException>(
                message: "Invalid consumerAdoption. Please correct the errors and try again.",
                (Rule: IsInvalid(consumerAdoption.Id), Parameter: nameof(ConsumerAdoption.Id)),
                (Rule: IsInvalid(consumerAdoption.AdoptionDate), Parameter: nameof(ConsumerAdoption.AdoptionDate)),
                (Rule: IsInvalid(consumerAdoption.CreatedDate), Parameter: nameof(ConsumerAdoption.CreatedDate)),
                (Rule: IsInvalid(consumerAdoption.CreatedBy), Parameter: nameof(ConsumerAdoption.CreatedBy)),
                (Rule: IsInvalid(consumerAdoption.UpdatedDate), Parameter: nameof(ConsumerAdoption.UpdatedDate)),
                (Rule: IsInvalid(consumerAdoption.UpdatedBy), Parameter: nameof(ConsumerAdoption.UpdatedBy)),
                (Rule: IsGreaterThan(consumerAdoption.CreatedBy, 255), Parameter: nameof(ConsumerAdoption.CreatedBy)),
                (Rule: IsGreaterThan(consumerAdoption.UpdatedBy, 255), Parameter: nameof(ConsumerAdoption.UpdatedBy)),

                (Rule: IsNotSame(
                        firstDate: consumerAdoption.UpdatedDate,
                        secondDate: consumerAdoption.CreatedDate,
                        secondDateName: nameof(ConsumerAdoption.CreatedDate)),
                    Parameter: nameof(ConsumerAdoption.UpdatedDate)),

                (Rule: IsNotSame(
                        first: currentUserId,
                        second: consumerAdoption.CreatedBy),
                    Parameter: nameof(ConsumerAdoption.CreatedBy)),

                (Rule: IsNotSame(
                        first: consumerAdoption.UpdatedBy,
                        second: consumerAdoption.CreatedBy,
                        secondName: nameof(ConsumerAdoption.CreatedBy)),
                    Parameter: nameof(ConsumerAdoption.UpdatedBy)),

                (Rule: await IsNotRecentAsync(consumerAdoption.CreatedDate),
                    Parameter: nameof(ConsumerAdoption.CreatedDate)));
        }

        private async ValueTask ValidateConsumerAdoptionOnModify(ConsumerAdoption consumerAdoption)
        {
            ValidateConsumerAdoptionIsNotNull(consumerAdoption);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate<InvalidConsumerAdoptionException>(
                message: "Invalid consumerAdoption. Please correct the errors and try again.",
                (Rule: IsInvalid(consumerAdoption.Id), Parameter: nameof(ConsumerAdoption.Id)),
                (Rule: IsInvalid(consumerAdoption.AdoptionDate), Parameter: nameof(ConsumerAdoption.AdoptionDate)),
                (Rule: IsInvalid(consumerAdoption.CreatedDate), Parameter: nameof(ConsumerAdoption.CreatedDate)),
                (Rule: IsInvalid(consumerAdoption.CreatedBy), Parameter: nameof(ConsumerAdoption.CreatedBy)),
                (Rule: IsInvalid(consumerAdoption.UpdatedDate), Parameter: nameof(ConsumerAdoption.UpdatedDate)),
                (Rule: IsInvalid(consumerAdoption.UpdatedBy), Parameter: nameof(ConsumerAdoption.UpdatedBy)),
                (Rule: IsGreaterThan(consumerAdoption.CreatedBy, 255), Parameter: nameof(ConsumerAdoption.CreatedBy)),
                (Rule: IsGreaterThan(consumerAdoption.UpdatedBy, 255), Parameter: nameof(ConsumerAdoption.UpdatedBy)),

                (Rule: IsNotSame(
                        first: currentUserId,
                        second: consumerAdoption.UpdatedBy),
                    Parameter: nameof(ConsumerAdoption.UpdatedBy)),

                (Rule: IsSame(
                        firstDate: consumerAdoption.UpdatedDate,
                        secondDate: consumerAdoption.CreatedDate,
                        secondDateName: nameof(ConsumerAdoption.CreatedDate)),
                    Parameter: nameof(ConsumerAdoption.UpdatedDate)),

                (Rule: await IsNotRecentAsync(consumerAdoption.UpdatedDate), Parameter: nameof(ConsumerAdoption.UpdatedDate)));
        }

        private static void ValidateConsumerAdoptionId(Guid consumerAdoptionId) =>
            Validate<InvalidConsumerAdoptionException>(
                message: "Invalid consumerAdoption. Please correct the errors and try again.",
                validations: (Rule: IsInvalid(consumerAdoptionId), Parameter: nameof(ConsumerAdoption.Id)));

        private static void ValidateStorageConsumerAdoption(ConsumerAdoption maybeConsumerAdoption, Guid consumerAdoptionId)
        {
            if (maybeConsumerAdoption is null)
            {
                throw new NotFoundConsumerAdoptionException(
                    message: $"Couldn't find consumerAdoption with consumerAdoptionId: {consumerAdoptionId}.");
            }
        }

        private static void ValidateOnBulkAddOrModifyConsumerAdoptions(List<ConsumerAdoption> consumerAdoptions)
        {
            Validate<InvalidConsumerAdoptionException>(
                message: "Invalid consumerAdoption. Please correct the errors and try again.",
                validations: (Rule: IsInvalid(consumerAdoptions), Parameter: nameof(consumerAdoptions)));
        }

        private static void ValidateConsumerAdoptionIsNotNull(ConsumerAdoption consumerAdoption)
        {
            if (consumerAdoption is null)
            {
                throw new NullConsumerAdoptionException(message: "ConsumerAdoption is null.");
            }
        }

        private static void ValidateAgainstStorageConsumerAdoptionOnModify(
            ConsumerAdoption inputConsumerAdoption,
            ConsumerAdoption storageConsumerAdoption)
        {
            Validate<InvalidConsumerAdoptionException>(
                message: "Invalid consumerAdoption. Please correct the errors and try again.",
                (Rule: IsNotSame(
                        firstDate: inputConsumerAdoption.CreatedDate,
                        secondDate: storageConsumerAdoption.CreatedDate,
                        secondDateName: nameof(ConsumerAdoption.CreatedDate)),
                    Parameter: nameof(ConsumerAdoption.CreatedDate)),

                (Rule: IsNotSame(
                        first: inputConsumerAdoption.CreatedBy,
                        second: storageConsumerAdoption.CreatedBy,
                        secondName: nameof(ConsumerAdoption.CreatedBy)),
                    Parameter: nameof(ConsumerAdoption.CreatedBy)),

                (Rule: IsSame(
                        firstDate: inputConsumerAdoption.UpdatedDate,
                        secondDate: storageConsumerAdoption.UpdatedDate,
                        secondDateName: nameof(ConsumerAdoption.UpdatedDate)),
                    Parameter: nameof(ConsumerAdoption.UpdatedDate)));
        }

        virtual internal async ValueTask<List<ConsumerAdoption>> ValidateConsumerAdoptionsAndAssignIdAndAuditOnAddAsync(
            List<ConsumerAdoption> consumerAdoptions)
        {
            List<ConsumerAdoption> validatedConsumerAdoptions = new List<ConsumerAdoption>();

            foreach (ConsumerAdoption consumerAdoption in consumerAdoptions)
            {
                try
                {
                    string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();
                    var currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                    consumerAdoption.CreatedDate = currentDateTime;
                    consumerAdoption.CreatedBy = currentUserId;
                    consumerAdoption.UpdatedDate = currentDateTime;
                    consumerAdoption.UpdatedBy = currentUserId;
                    await ValidateConsumerAdoptionOnAdd(consumerAdoption);
                    validatedConsumerAdoptions.Add(consumerAdoption);
                }
                catch (Exception ex)
                {
                    await this.loggingBroker.LogErrorAsync(ex);
                }
            }

            return validatedConsumerAdoptions;
        }

        virtual internal async ValueTask<List<ConsumerAdoption>> ValidateConsumerAdoptionsAndAssignAuditOnModifyAsync(
            List<ConsumerAdoption> consumerAdoptions)
        {
            List<ConsumerAdoption> validatedConsumerAdoptions = new List<ConsumerAdoption>();

            foreach (ConsumerAdoption consumerAdoption in consumerAdoptions)
            {
                try
                {
                    string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();
                    var currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                    consumerAdoption.UpdatedDate = currentDateTime;
                    consumerAdoption.UpdatedBy = currentUserId;
                    await ValidateConsumerAdoptionOnModify(consumerAdoption);
                    validatedConsumerAdoptions.Add(consumerAdoption);
                }
                catch (Exception ex)
                {
                    await this.loggingBroker.LogErrorAsync(ex);
                }
            }

            return validatedConsumerAdoptions;
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

        private static dynamic IsInvalid(List<ConsumerAdoption> consumerAdoptions) => new
        {
            Condition = consumerAdoptions == null,
            Message = "ConsumerAdoptions is required"
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
