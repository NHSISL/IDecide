// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class NullNotificationInfoException : Xeption
    {
        public NullNotificationInfoException(string message)
            : base(message: message)
        {
        }
    }
}