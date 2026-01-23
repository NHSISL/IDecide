// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;

public class NhsLoginServiceDependencyException : Xeption
{
    public NhsLoginServiceDependencyException(string message, Xeption innerException)
        : base(message, innerException)
    { }
}
