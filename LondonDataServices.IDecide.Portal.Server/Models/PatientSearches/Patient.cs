// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Portal.Server.Models.PatientSearches
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string NhsNumber { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string ValidationCode { get; set; }
        public DateTimeOffset ValidationCodeExpiresOn { get; set; }
        public int RetryCount { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
