// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.CsvIdentificationRequests
{
    public class CsvIdentificationRequest
    {
        public Guid Id { get; set; }
        public string RequesterUserId { get; set; } = string.Empty;
        public string RequesterFirstName { get; set; } = string.Empty;
        public string RequesterLastName { get; set; } = string.Empty;
        public string RequesterDisplayName { get; set; } = string.Empty;
        public string RequesterEmail { get; set; } = string.Empty;
        public string RequesterJobTitle { get; set; } = string.Empty;
        public string RecipientUserId { get; set; } = string.Empty;
        public string RecipientFirstName { get; set; } = string.Empty;
        public string RecipientLastName { get; set; } = string.Empty;
        public string RecipientDisplayName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientJobTitle { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Organisation { get; set; } = string.Empty;
        public string Filepath { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string Sha256Hash { get; set; } = string.Empty;
        public int IdentifierColumnIndex { get; set; }
        public bool HasHeaderRecord { get; set; } = false;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
