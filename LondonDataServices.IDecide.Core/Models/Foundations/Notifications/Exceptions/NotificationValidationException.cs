// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class NotificationValidationException : Xeption
    {
        public NotificationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
