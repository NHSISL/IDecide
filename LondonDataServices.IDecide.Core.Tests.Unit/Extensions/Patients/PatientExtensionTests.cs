// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions.Patients
{
    public partial class PatientExtensionTests
    {
        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        public Patient GetPatientToRedact()
        {
            return new Patient
            {
                Address = "16 Red Grove, Testville",
                DateOfBirth = GetRandomDateTimeOffset(),
                EmailAddress = "test.family@gmail.com",
                FirstName = "Test Person",
                NhsNumber = GenerateRandom10DigitNumber(),
                PhoneNumber = "07123456712",
                Postcode = "AB2 1ZY",
                Surname = "Family"
            };
        }

        public Patient GetRedactedPatient(Patient patient)
        {
            return new Patient
            {
                Address = "16 R** G****, T********",
                DateOfBirth = patient.DateOfBirth,
                EmailAddress = "t***.f*****@gmail.com",
                FirstName = "T*** P*****",
                NhsNumber = patient.NhsNumber,
                PhoneNumber = "07*******12",
                Postcode = "AB2 1**",
                Surname = "F*****"
            };
        }
    }
}
