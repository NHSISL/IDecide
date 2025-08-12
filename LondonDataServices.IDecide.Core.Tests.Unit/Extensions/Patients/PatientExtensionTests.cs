// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions.Patients
{
    public partial class PatientExtensionTests
    {
        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

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
                Email = "test.family@gmail.com",
                GivenName = "Test Person",
                NhsNumber = GenerateRandom10DigitNumber(),
                Phone = "07123456712",
                PostCode = "AB2 1ZY",
                Surname = "Family",
                CreatedBy = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                Decisions = new List<Decision>(),
                Gender = GetRandomString(),
                Id = Guid.NewGuid(),
                RetryCount = 0,
                Title = GetRandomString(),
                UpdatedBy = GetRandomString(),
                UpdatedDate = GetRandomDateTimeOffset(),
                ValidationCode = GetRandomString(),
                ValidationCodeExpiresOn = GetRandomDateTimeOffset()
            };
        }

        public Patient GetRedactedPatient(Patient patient)
        {
            return new Patient
            {
                Address = "16 R** G****, T********",
                DateOfBirth = patient.DateOfBirth,
                Email = "t***.f*****@gmail.com",
                GivenName = "T*** P*****",
                NhsNumber = patient.NhsNumber,
                Phone = "07*******12",
                PostCode = "AB2 1**",
                Surname = "F*****",
                CreatedBy = patient.CreatedBy,
                CreatedDate = patient.CreatedDate,
                Decisions = patient.Decisions,
                Gender = patient.Gender,
                Id = patient.Id,
                RetryCount = patient.RetryCount,
                Title = patient.Title,
                UpdatedBy = patient.UpdatedBy,
                UpdatedDate= patient.UpdatedDate,
                ValidationCode = patient.ValidationCode,
                ValidationCodeExpiresOn = patient.ValidationCodeExpiresOn
            };
        }

        public Patient GetPatientToRedactWithWhitespace()
        {
            return new Patient
            {
                Address = " ",
                DateOfBirth = GetRandomDateTimeOffset(),
                Email = " ",
                GivenName = " ",
                NhsNumber = GenerateRandom10DigitNumber(),
                Phone = " ",
                PostCode = " ",
                Surname = " ",
                CreatedBy = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                Decisions = new List<Decision>(),
                Gender = GetRandomString(),
                Id = Guid.NewGuid(),
                RetryCount = 0,
                Title = GetRandomString(),
                UpdatedBy = GetRandomString(),
                UpdatedDate = GetRandomDateTimeOffset(),
                ValidationCode = GetRandomString(),
                ValidationCodeExpiresOn = GetRandomDateTimeOffset()
            };
        }

        public Patient GetRedactedPatientWithWhitespace(Patient patient)
        {
            return new Patient
            {
                Address = " ",
                DateOfBirth = patient.DateOfBirth,
                Email = " ",
                GivenName = " ",
                NhsNumber = patient.NhsNumber,
                Phone = " ",
                PostCode = " ",
                Surname = " ",
                CreatedBy = patient.CreatedBy,
                CreatedDate = patient.CreatedDate,
                Decisions = patient.Decisions,
                Gender = patient.Gender,
                Id = patient.Id,
                RetryCount = patient.RetryCount,
                Title = patient.Title,
                UpdatedBy = patient.UpdatedBy,
                UpdatedDate = patient.UpdatedDate,
                ValidationCode = patient.ValidationCode,
                ValidationCodeExpiresOn = patient.ValidationCodeExpiresOn
            };
        }

        public Patient GetPatientToRedactWithPostcodeInAddress()
        {
            return new Patient
            {
                Address = "16 Red Grove, Testville, AB2 1ZY",
                DateOfBirth = GetRandomDateTimeOffset(),
                Email = "test.family@gmail.com",
                GivenName = "Test Person",
                NhsNumber = GenerateRandom10DigitNumber(),
                Phone = "07123456712",
                PostCode = "AB2 1ZY",
                Surname = "Family",
                CreatedBy = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                Decisions = new List<Decision>(),
                Gender = GetRandomString(),
                Id = Guid.NewGuid(),
                RetryCount = 0,
                Title = GetRandomString(),
                UpdatedBy = GetRandomString(),
                UpdatedDate = GetRandomDateTimeOffset(),
                ValidationCode = GetRandomString(),
                ValidationCodeExpiresOn = GetRandomDateTimeOffset()
            };
        }

        public Patient GetRedactedPatientWithPostcodeInAddress(Patient patient)
        {
            return new Patient
            {
                Address = "16 R** G****, T********, A** 1**",
                DateOfBirth = patient.DateOfBirth,
                Email = "t***.f*****@gmail.com",
                GivenName = "T*** P*****",
                NhsNumber = patient.NhsNumber,
                Phone = "07*******12",
                PostCode = "AB2 1**",
                Surname = "F*****",
                CreatedBy = patient.CreatedBy,
                CreatedDate = patient.CreatedDate,
                Decisions = patient.Decisions,
                Gender = patient.Gender,
                Id = patient.Id,
                RetryCount = patient.RetryCount,
                Title = patient.Title,
                UpdatedBy = patient.UpdatedBy,
                UpdatedDate = patient.UpdatedDate,
                ValidationCode = patient.ValidationCode,
                ValidationCodeExpiresOn = patient.ValidationCodeExpiresOn
            };
        }
    }
}
