// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Users
{
    public class User
    {
        public int Id { get; set; }
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
