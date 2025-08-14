// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private async ValueTask ValidateNotificationInfoOnSendCode(NotificationInfo notificationInfo)
        {
            ValidateNotificationInfoIsNotNull(notificationInfo);

            Validate<InvalidNotificationInfoException>(
                message: "Invalid notification info. Please correct the errors and try again.",

(Rule: IsInvalid(
                    notificationInfo.Patient.NhsNumber),
                    Parameter: nameof(NotificationInfo.Patient.NhsNumber)),

                (Rule: IsInvalid(notificationInfo.Patient.Title), Parameter: nameof(NotificationInfo.Patient.Title)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.GivenName),
                    Parameter: nameof(NotificationInfo.Patient.GivenName)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.Surname),
                    Parameter: nameof(NotificationInfo.Patient.Surname)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.DateOfBirth),
                    Parameter: nameof(NotificationInfo.Patient.DateOfBirth)),

                (Rule: IsInvalid(notificationInfo.Patient.Gender), Parameter: nameof(NotificationInfo.Patient.Gender)),
                (Rule: IsInvalid(notificationInfo.Patient.Email), Parameter: nameof(NotificationInfo.Patient.Email)),
                (Rule: IsInvalid(notificationInfo.Patient.Phone), Parameter: nameof(NotificationInfo.Patient.Phone)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.Address),
                    Parameter: nameof(NotificationInfo.Patient.Address)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.PostCode),
                    Parameter: nameof(NotificationInfo.Patient.PostCode)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.ValidationCode),
                    Parameter: nameof(NotificationInfo.Patient.ValidationCode)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.ValidationCodeExpiresOn),
                    Parameter: nameof(NotificationInfo.Patient.ValidationCodeExpiresOn)),

                (Rule: IsInvalid(
                    notificationInfo.Patient.NotificationPreference),
                    Parameter: nameof(NotificationInfo.Patient.NotificationPreference)),

                (Rule: IsInvalid(
                    notificationInfo.Decision.DecisionChoice),
                    Parameter: nameof(NotificationInfo.Decision.DecisionChoice)),

                (Rule: IsInvalid(
                    notificationInfo.Decision.DecisionType.Name),
                    Parameter: nameof(NotificationInfo.Decision.DecisionType.Name)));

        }

        private static void ValidateNotificationInfoIsNotNull(NotificationInfo notificationInfo)
        {
            if (notificationInfo is null)
            {
                throw new NullNotificationInfoException(message: "Notification info is null.");
            }
        }

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

        private static dynamic IsInvalid(NotificationPreference notificationPreference) => new
        {
            Condition = notificationPreference == default,
            Message = "Value is required"
        };

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
