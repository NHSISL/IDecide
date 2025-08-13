// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.PowersOfAttorney
{
    public class PowerOfAttorney
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string RelationshipType { get; set; }

        [BindNever]
        public Patient Patient { get; set; } = null!;
    }
}
