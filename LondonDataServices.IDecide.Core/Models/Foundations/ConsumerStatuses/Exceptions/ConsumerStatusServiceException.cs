// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions
{
    internal class ConsumerStatusServiceException : Xeption
    {
        public ConsumerStatusServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}