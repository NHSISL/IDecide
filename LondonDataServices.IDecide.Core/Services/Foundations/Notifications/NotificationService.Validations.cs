// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
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
                (Rule: IsInvalid(notificationInfo.Patient), Parameter: nameof(NotificationInfo.Patient)),
                (Rule: IsInvalid(notificationInfo.Decision), Parameter: nameof(NotificationInfo.Decision)));

        }

        private static void ValidateNotificationInfoIsNotNull(NotificationInfo notificationInfo)
        {
            if (notificationInfo is null)
            {
                throw new NullNotificationInfoException(message: "Notification info is null.");
            }
        }

        private static dynamic IsInvalid(Patient patient) => new
        {
            Condition = patient is null,
            Message = "Patient is required"
        };

        private static dynamic IsInvalid(Decision decision) => new
        {
            Condition = decision is null,
            Message = "Decision is required"
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
