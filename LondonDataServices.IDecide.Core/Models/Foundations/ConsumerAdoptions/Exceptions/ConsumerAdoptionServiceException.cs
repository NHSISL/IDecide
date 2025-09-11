// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class ConsumerAdoptionServiceException : Xeption
    {
        public ConsumerAdoptionServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}