// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Bases;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Consumers
{
    public class Consumer : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
