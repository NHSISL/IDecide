// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    internal class LockedConsumerException : Xeption
    {
        public LockedConsumerException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}