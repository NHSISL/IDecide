// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class InvalidNotificationInfoException : Xeption
    {
        public InvalidNotificationInfoException(string message)
            : base(message)
        { }
    }
}