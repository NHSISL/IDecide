// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.ReIdentifications
{
    public class ApprovalRequest
    {
        public Guid ImpersonationContextId { get; set; }
        public bool IsApproved { get; set; }
    }
}
