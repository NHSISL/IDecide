// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Bases;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses
{
    public class ConsumerStatus : IKey, IAudit
    {
        public Guid Id { get; set; }
        public Guid ConsumerId { get; set; }
        public Guid DecisionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [BindNever]
        public Consumer Consumer { get; set; } = null!;

        [BindNever]
        public Decision Decision { get; set; } = null!;
    }
}
