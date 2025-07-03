// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class ServerNotificationException : Xeption
    {
        public ServerNotificationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
