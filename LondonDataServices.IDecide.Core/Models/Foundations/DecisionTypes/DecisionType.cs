// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes
{
    public class DecisionType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [BindNever]
        public List<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
