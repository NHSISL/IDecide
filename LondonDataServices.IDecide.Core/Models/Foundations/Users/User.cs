// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Bases;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Users
{
    public class User : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string NhsIdUserUid { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Sub { get; set; } = default!;
        public string RawUserInfo { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsAuthorised { get; set; } = false;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = default!;
        public DateTimeOffset UpdatedDate { get; set; }
    }
}