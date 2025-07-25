// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions
{
    public class DecisionTypeDependencyException : Xeption
    {
        public DecisionTypeDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}