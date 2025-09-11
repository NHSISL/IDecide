// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerAdoptions
{
    public class ConsumerAdoption
    {
        public Guid Id { get; set; }
        public Guid ConsumerId { get; set; }
        public Guid DecisionId { get; set; }
        public DateTimeOffset AdoptionDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
