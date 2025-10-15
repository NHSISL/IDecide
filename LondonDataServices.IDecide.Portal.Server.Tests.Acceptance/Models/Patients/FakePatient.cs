// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Patients
{
    public class FakePatient
    {
        public string Title { get; set; }
        public List<string> GivenNames { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Address { get; set; }
        public string NhsNumber { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset DateOfDeath { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RegisteredGpPractice { get; set; }
    }
}
