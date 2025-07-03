﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Clients.AuditClient.Exceptions
{
    public class AuditClientDependencyException : Xeption
    {
        public AuditClientDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
