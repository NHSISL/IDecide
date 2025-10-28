﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Decisions
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

        [BindNever]
        [NotMapped]
        public string DecisionTypeName => this.DecisionType?.Name ?? string.Empty;

        [BindNever]
        [NotMapped]
        public string PatientNhsNumber => this.Patient?.NhsNumber ?? string.Empty;

        [BindNever]
        public DecisionType DecisionType { get; set; } = null!;

        [BindNever]
        public Patient Patient { get; set; } = null!;

        [BindNever]
        public List<ConsumerAdoption> ConsumerAdoptions { get; set; } = new List<ConsumerAdoption>();
    }
}
