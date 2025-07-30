// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions
{
    public class PdsServiceException : Xeption
    {
        public PdsServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
