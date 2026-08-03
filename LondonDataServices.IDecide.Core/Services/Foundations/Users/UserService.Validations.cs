// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private async ValueTask ValidateUserOnAdd(User user)
        {
            ValidateUserIsNotNull(user);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again."),

                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.NhsIdUserUid), Parameter: nameof(User.NhsIdUserUid)),
                (Rule: IsInvalid(user.Name), Parameter: nameof(User.Name)),
                (Rule: IsInvalid(user.Sub), Parameter: nameof(User.Sub)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.CreatedBy), Parameter: nameof(User.CreatedBy)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),
                (Rule: IsInvalid(user.UpdatedBy), Parameter: nameof(User.UpdatedBy)),
                (Rule: IsGreaterThan(user.NhsIdUserUid, 50), Parameter: nameof(User.NhsIdUserUid)),
                (Rule: IsGreaterThan(user.Name, 200), Parameter: nameof(User.Name)),
                (Rule: IsGreaterThan(user.CreatedBy, 255), Parameter: nameof(User.CreatedBy)),
                (Rule: IsGreaterThan(user.UpdatedBy, 255), Parameter: nameof(User.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: user.UpdatedDate,
                    secondDate: user.CreatedDate,
                    secondDateName: nameof(User.CreatedDate)),
                Parameter: nameof(User.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: user.CreatedBy),
                Parameter: nameof(User.CreatedBy)),

                (Rule: IsNotSame(
                    first: user.UpdatedBy,
                    second: user.CreatedBy,
                    secondName: nameof(User.CreatedBy)),
                Parameter: nameof(User.UpdatedBy)),

                (Rule: await IsNotRecentAsync(user.CreatedDate), Parameter: nameof(User.CreatedDate)));
        }

        private async ValueTask ValidateUserOnModify(User user)
        {
            ValidateUserIsNotNull(user);
            string currentUserId = await this.securityAuditBroker.GetCurrentUserIdAsync();

            Validate(
                createException: () => new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again."),

                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.NhsIdUserUid), Parameter: nameof(User.NhsIdUserUid)),
                (Rule: IsInvalid(user.Name), Parameter: nameof(User.Name)),
                (Rule: IsInvalid(user.Sub), Parameter: nameof(User.Sub)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.CreatedBy), Parameter: nameof(User.CreatedBy)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),
                (Rule: IsInvalid(user.UpdatedBy), Parameter: nameof(User.UpdatedBy)),
                (Rule: IsGreaterThan(user.NhsIdUserUid, 50), Parameter: nameof(User.NhsIdUserUid)),
                (Rule: IsGreaterThan(user.Name, 200), Parameter: nameof(User.Name)),
                (Rule: IsGreaterThan(user.CreatedBy, 255), Parameter: nameof(User.CreatedBy)),
                (Rule: IsGreaterThan(user.UpdatedBy, 255), Parameter: nameof(User.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: user.UpdatedBy),
                Parameter: nameof(User.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: user.UpdatedDate,
                    secondDate: user.CreatedDate,
                    secondDateName: nameof(User.CreatedDate)),
                Parameter: nameof(User.UpdatedDate)),

                (Rule: await IsNotRecentAsync(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)));
        }

        private static void ValidateUserId(Guid userId) =>
            Validate(
                createException: () => new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again."),

                validations: (Rule: IsInvalid(userId), Parameter: nameof(User.Id)));

        private static void ValidateStorageUser(User maybeUser, Guid userId)
        {
            if (maybeUser is null)
            {
                throw new NotFoundUserException(
                    message: $"Couldn't find user with userId: {userId}.");
            }
        }

        private static void ValidateUserIsNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException(message: "User is null.");
            }
        }

        private static void ValidateAgainstStorageUserOnModify(User inputUser, User storageUser)
        {
            Validate(
                createException: () => new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again."),

                (Rule: IsNotSame(
                    firstDate: inputUser.CreatedDate,
                    secondDate: storageUser.CreatedDate,
                    secondDateName: nameof(User.CreatedDate)),
                Parameter: nameof(User.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputUser.CreatedBy,
                    second: storageUser.CreatedBy,
                    secondName: nameof(User.CreatedBy)),
                Parameter: nameof(User.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputUser.UpdatedDate,
                    secondDate: storageUser.UpdatedDate,
                    secondDateName: nameof(User.UpdatedDate)),
                Parameter: nameof(User.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
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

        private static void Validate(
            Func<Xeption> createException,
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = createException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: (string)rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
