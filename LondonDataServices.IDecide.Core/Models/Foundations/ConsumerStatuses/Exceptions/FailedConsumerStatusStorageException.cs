// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions
{
    public class FailedConsumerStatusStorageException : Xeption
    {
        public FailedConsumerStatusStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
