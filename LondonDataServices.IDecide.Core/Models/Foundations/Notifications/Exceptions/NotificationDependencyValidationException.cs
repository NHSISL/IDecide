// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class NotificationDependencyValidationException : Xeption
    {
        public NotificationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
