// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    internal class InvalidConsumerException : Xeption
    {
        public InvalidConsumerException(string message)
            : base(message)
        { }
    }
}
