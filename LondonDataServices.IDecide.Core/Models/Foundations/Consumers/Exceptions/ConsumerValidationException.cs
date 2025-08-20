// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    internal class ConsumerValidationException : Xeption
    {
        public ConsumerValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}