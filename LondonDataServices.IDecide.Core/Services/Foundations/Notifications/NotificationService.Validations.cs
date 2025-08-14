// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private async ValueTask ValidateNotificationInfoOnSendCode(NotificationInfo notificationInfo)
        {
            ValidateNotificationInfoIsNotNull(notificationInfo);
        }

        private static void ValidateNotificationInfoIsNotNull(NotificationInfo notificationInfo)
        {
            if (notificationInfo is null)
            {
                throw new NullNotificationInfoException(message: "Notification info is null.");
            }
        }
    }
}
