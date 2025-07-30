// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds
{
    public class Patient
    {
        public string NhsNumber { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
