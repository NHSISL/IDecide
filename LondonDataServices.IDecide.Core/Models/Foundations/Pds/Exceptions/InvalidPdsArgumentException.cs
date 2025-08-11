// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions
{
    public class InvalidPdsArgumentException : Xeption
    {
        public InvalidPdsArgumentException(string message)
            : base(message)
        { }
    }
}
