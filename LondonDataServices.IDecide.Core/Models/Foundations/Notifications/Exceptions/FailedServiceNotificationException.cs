// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions
{
    public class FailedServiceNotificationException : Xeption
    {
        public FailedServiceNotificationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
