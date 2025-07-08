// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class InvalidArgumentsNotificationException : Xeption
    {
        public InvalidArgumentsNotificationException(string message)
            : base(message)
        { }
    }
}