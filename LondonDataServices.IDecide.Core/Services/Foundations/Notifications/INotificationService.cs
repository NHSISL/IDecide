// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public interface INotificationService
    {
        ValueTask SendCodeNotificationAsync(NotificationInfo notificationInfo);
        ValueTask SendSubscriberUsageNotificationAsync(NotificationInfo notificationInfo);
    }
}
