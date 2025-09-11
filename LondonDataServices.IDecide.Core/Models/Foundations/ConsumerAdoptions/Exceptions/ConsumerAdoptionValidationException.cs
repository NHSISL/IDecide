// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class ConsumerAdoptionValidationException : Xeption
    {
        public ConsumerAdoptionValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}