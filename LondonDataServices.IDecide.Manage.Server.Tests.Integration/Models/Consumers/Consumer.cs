// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers
{
    public class Consumer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
