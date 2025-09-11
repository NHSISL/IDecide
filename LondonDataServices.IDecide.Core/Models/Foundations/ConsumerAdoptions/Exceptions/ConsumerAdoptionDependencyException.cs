// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class ConsumerAdoptionDependencyException : Xeption
    {
        public ConsumerAdoptionDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
