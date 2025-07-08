// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Bases;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.AccessAudits
{
    public class AccessAudit : IKey, IAudit
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string PseudoIdentifier { get; set; }
        public string UserId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Purpose { get; set; }
        public string Reason { get; set; }
        public string Organisation { get; set; }
        public bool HasAccess { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
