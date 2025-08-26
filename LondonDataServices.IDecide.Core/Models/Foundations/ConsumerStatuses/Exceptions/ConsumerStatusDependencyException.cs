// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions
{
    public class ConsumerStatusDependencyException : Xeption
    {
        public ConsumerStatusDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
