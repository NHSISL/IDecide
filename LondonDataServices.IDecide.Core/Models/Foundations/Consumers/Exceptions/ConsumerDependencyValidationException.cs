// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    public class ConsumerDependencyValidationException : Xeption
    {
        public ConsumerDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}