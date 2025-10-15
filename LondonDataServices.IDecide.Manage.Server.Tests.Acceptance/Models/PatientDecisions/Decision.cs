// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.PatientDecisions
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
        public Patient Patient { get; set; }
        public DecisionType DecisionType { get; set; }
    }
}
