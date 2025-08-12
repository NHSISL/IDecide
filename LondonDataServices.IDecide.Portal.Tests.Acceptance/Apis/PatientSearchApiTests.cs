// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Tests.Acceptance.Brokers;
using Microsoft.Extensions.Configuration;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using System.Collections.Generic;
using System.Linq;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Brokers.Pds;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientSearchApiTests
    {
        private readonly ApiBroker apiBroker;

        public PatientSearchApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

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
            var addressWithoutPostcode = fakePatient.Address.Split(",").SkipLast(1).Select(a => a.Trim());
            var addressPostcode = fakePatient.Address.Split(",").Last().Trim();

            return new Patient
            {
                Title = fakePatient.Title,
                GivenName = string.Join(", ", fakePatient.GivenNames),
                Surname = fakePatient.Surname,
                DateOfBirth = fakePatient.DateOfBirth,
                Address = string.Join(", ", addressWithoutPostcode),
                NhsNumber = fakePatient.NhsNumber,
                Gender = fakePatient.Gender,
                Email = fakePatient.Email,
                Phone = fakePatient.PhoneNumber,
                PostCode = addressPostcode
            };
        }
    }
}