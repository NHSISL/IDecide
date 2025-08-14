// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService : INotificationService
    {
        private readonly INotificationBroker notificationBroker;

        public NotificationService(INotificationBroker notificationBroker)
        {
            this.notificationBroker = notificationBroker;
        }

        public ValueTask SendCodeNotificationAsync(NotificationInfo notificationInfo) =>
            throw new NotImplementedException();
    }
}
