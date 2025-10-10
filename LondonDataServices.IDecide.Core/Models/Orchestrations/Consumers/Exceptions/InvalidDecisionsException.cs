// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions
{
    public class InvalidDecisionsException : Xeption
    {
        public InvalidDecisionsException(string message)
            : base(message)
        { }
    }
}
