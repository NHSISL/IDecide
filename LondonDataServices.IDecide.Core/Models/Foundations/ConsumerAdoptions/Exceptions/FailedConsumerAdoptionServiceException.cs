// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class FailedConsumerAdoptionServiceException : Xeption
    {
        public FailedConsumerAdoptionServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}