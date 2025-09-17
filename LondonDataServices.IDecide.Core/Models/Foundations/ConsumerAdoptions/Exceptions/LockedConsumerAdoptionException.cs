// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class LockedConsumerAdoptionException : Xeption
    {
        public LockedConsumerAdoptionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}