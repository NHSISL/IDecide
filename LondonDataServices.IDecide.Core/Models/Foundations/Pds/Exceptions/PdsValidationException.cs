// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions
{
    public class PdsValidationException : Xeption
    {
        public PdsValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
