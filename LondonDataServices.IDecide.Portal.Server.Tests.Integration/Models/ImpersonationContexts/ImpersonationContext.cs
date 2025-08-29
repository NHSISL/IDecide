// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.ImpersonationContexts
{
    public class ImpersonationContext
    {
        public Guid Id { get; set; }
        public string RequesterUserId { get; set; } = string.Empty;
        public string RequesterFirstName { get; set; } = string.Empty;
        public string RequesterLastName { get; set; } = string.Empty;
        public string RequesterDisplayName { get; set; } = string.Empty;
        public string RequesterEmail { get; set; } = string.Empty;
        public string RequesterJobTitle { get; set; } = string.Empty;
        public string ResponsiblePersonUserId { get; set; } = string.Empty;
        public string ResponsiblePersonFirstName { get; set; } = string.Empty;
        public string ResponsiblePersonLastName { get; set; } = string.Empty;
        public string ResponsiblePersonDisplayName { get; set; } = string.Empty;
        public string ResponsiblePersonEmail { get; set; } = string.Empty;
        public string ResponsiblePersonJobTitle { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Organisation { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string InboxSasToken { get; set; } = string.Empty;
        public string OutboxSasToken { get; set; } = string.Empty;
        public string ErrorsSasToken { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string IdentifierColumn { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
