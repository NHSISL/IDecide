// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions
{
    public class InvalidConsumerAdoptionException : Xeption
    {
        public InvalidConsumerAdoptionException(string message)
            : base(message)
        { }
    }
}
