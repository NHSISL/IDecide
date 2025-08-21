// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    public class NullConsumerException : Xeption
    {
        public NullConsumerException(string message)
            : base(message)
        { }
    }
}