// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class NotificationDependencyException : Xeption
    {
        public NotificationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
