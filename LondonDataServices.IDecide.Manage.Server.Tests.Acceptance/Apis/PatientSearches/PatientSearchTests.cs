﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Extensions.Patients;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;
using Microsoft.Extensions.Configuration;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientSearches
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientSearchTests
    {
        private readonly ApiBroker apiBroker;

        public PatientSearchTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private PatientLookup GetRandomSearchPatientLookup(string surname)
        {
            SearchCriteria searchCriteria = new SearchCriteria
            {
                Surname = surname
            };

            PatientLookup randomPatientLookup = new PatientLookup
            {
                SearchCriteria = searchCriteria
            };

            return randomPatientLookup;
        }

        private Patient GetPatient(string surname)
        {
            var patients = this.apiBroker.configuration
                .GetSection("FakeFHIRProviderConfigurations:FakePatients")
                .Get<List<FakePatient>>();

            var fakePatient = patients.Where(patient => patient.Surname == surname).First();
            var patient = MapFakePatientToPatient(fakePatient);
            var redactedPatient = patient.Redact();

            return redactedPatient;
        }

        private static Patient MapFakePatientToPatient(FakePatient fakePatient)
        {
            var addressPostcode = fakePatient.Address.Split(",").Last().Trim();

            return new Patient
            {
                Title = fakePatient.Title,
                GivenName = string.Join(", ", fakePatient.GivenNames),
                Surname = fakePatient.Surname,
                DateOfBirth = fakePatient.DateOfBirth,
                Address = string.Join(", ", fakePatient.Address),
                NhsNumber = fakePatient.NhsNumber,
                Gender = fakePatient.Gender,
                Email = fakePatient.Email,
                Phone = fakePatient.PhoneNumber,
                PostCode = addressPostcode
            };
        }
    }
}
