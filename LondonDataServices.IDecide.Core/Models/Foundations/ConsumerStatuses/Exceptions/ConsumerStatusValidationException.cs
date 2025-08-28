// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions
{
    public class ConsumerStatusValidationException : Xeption
    {
        public ConsumerStatusValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}