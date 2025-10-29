// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations.Schema;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions
{
    public class Decision
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DecisionTypeId { get; set; }
        public string DecisionChoice { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public string ResponsiblePersonGivenName { get; set; }
        public string ResponsiblePersonSurname { get; set; }
        public string ResponsiblePersonRelationship { get; set; }

        [NotMapped]
        public string DecisionTypeName => this.DecisionType?.Name ?? string.Empty;

        [NotMapped]
        public string PatientNhsNumber => this.Patient?.NhsNumber ?? string.Empty;

        public Patient Patient { get; set; }
        public DecisionType DecisionType { get; set; }
    }
}
