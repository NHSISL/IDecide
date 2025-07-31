// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string NhsNumber { get; set; }
        public string ValidationCode { get; set; }
        public DateTimeOffset ValidationCodeExpiresOn { get; set; }
        public int RetryCount { get; set; }

        [BindNever]
        public List<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
