// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using LondonDataServices.IDecide.Core.Models.Bases;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers
{
    public class Consumer : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [BindNever]
        public List<ConsumerStatus> ConsumerStatuses { get; set; } = new List<ConsumerStatus>();
    }
}
