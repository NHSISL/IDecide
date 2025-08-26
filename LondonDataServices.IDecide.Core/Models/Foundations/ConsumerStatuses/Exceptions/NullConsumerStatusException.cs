// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions
{
    public class NullConsumerStatusException : Xeption
    {
        public NullConsumerStatusException(string message)
            : base(message)
        { }
    }
}