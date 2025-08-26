// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions
{
    public class NotFoundConsumerException : Xeption
    {
        public NotFoundConsumerException(string message)
            : base(message)
        { }
    }
}