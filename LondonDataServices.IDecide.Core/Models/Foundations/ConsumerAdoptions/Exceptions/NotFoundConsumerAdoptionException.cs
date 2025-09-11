// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class NotFoundConsumerAdoptionException : Xeption
    {
        public NotFoundConsumerAdoptionException(string message)
            : base(message)
        { }
    }
}