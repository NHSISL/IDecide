// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using ISL.Providers.PDS.FakeFHIR.Mappers;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System.Collections.Generic;
using System;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;
using Tynamix.ObjectFiller;
using Patient = Hl7.Fhir.Model.Patient;
using LondonDataServices.IDecide.Core.Mappers;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        private readonly Mock<IPdsService> pdsServiceMock = new Mock<IPdsService>();
        private readonly PatientOrchestrationService patientOrchestrationService;

        public PatientOrchestrationServiceTests()
        {
            this.pdsServiceMock = new Mock<IPdsService>();

            this.patientOrchestrationService = new PatientOrchestrationService(
                this.pdsServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        public static string GenerateRandomMobileNumber()
        {
            Random random = new Random();
            string prefix = "07";
            string number = random.Next(100000000, 999999999).ToString();

            return prefix + number;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Patient CreateRandomPatient(string surname)
        {
            var patient = new Patient();

            var nameFiller = new Filler<HumanName>();
            nameFiller.Setup()
                .OnProperty(n => n.Family).Use(surname)
                .OnProperty(n => n.Children).IgnoreIt()
                .OnProperty(n => n.Extension).IgnoreIt()
                .OnProperty(n => n.FamilyElement).IgnoreIt()
                .OnProperty(n => n.GivenElement).IgnoreIt()
                .OnProperty(n => n.NamedChildren).IgnoreIt()
                .OnProperty(n => n.Period).IgnoreIt()
                .OnProperty(n => n.PrefixElement).IgnoreIt()
                .OnProperty(n => n.SuffixElement).IgnoreIt()
                .OnProperty(n => n.TextElement).IgnoreIt()
                .OnProperty(n => n.UseElement).IgnoreIt();

            patient.Name = new List<HumanName> { nameFiller.Create() };
            patient.Gender = AdministrativeGender.Male;
            patient.BirthDate = GetRandomDateTimeOffset().ToString("yyyy-MM-dd");
            patient.Address = new List<Address> { CreateRandomAddress() };
            patient.Id = GenerateRandom10DigitNumber();
            patient.Telecom = new List<ContactPoint> { CreateRandomEmail(), CreateRandomPhoneNumber() };

            return patient;
        }

        private Bundle CreateRandomBundle(string surname)
        {
            var bundle = new Bundle
            {
                Type = Bundle.BundleType.Searchset,
                Total = 1,
                Timestamp = DateTimeOffset.UtcNow
            };

            Patient patient = CreateRandomPatient(surname);

            bundle.Entry = new List<Bundle.EntryComponent>{
                new Bundle.EntryComponent
                {
                    FullUrl = $"https://api.service.nhs.uk/personal-demographics/FHIR/R4/Patient/{patient.Id}",
                    Search = new Bundle.SearchComponent { Score = 1 },
                    Resource = patient
                }
            };

            return bundle;
        }

        private PatientBundle CreateRandomPatientBundle(Bundle bundle)
        {
            return PatientBundleMapper.FromBundle(bundle);
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

        private Models.Foundations.Pds.Patient GetPatientFromFhirPatient(Patient fhirPatient)
        {
            Models.Foundations.Pds.Patient patient = LocalPatientMapper.FromFhirPatient(fhirPatient);

            return patient;
        }

        private static Address CreateRandomAddress() =>
            CreateAddressFiller().Create();

        private static Filler<Address> CreateAddressFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            DateTimeOffset dateTimeOffsetEnd = dateTimeOffset.AddDays(1);
            FhirDateTime periodStart = new FhirDateTime(dateTimeOffset);
            FhirDateTime periodEnd = new FhirDateTime(dateTimeOffsetEnd);
            Period addressPeriod = new Period(periodStart, periodEnd);
            var filler = new Filler<Address>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(n => n.Period).Use(addressPeriod)
                .OnProperty(n => n.Children).IgnoreIt()
                .OnProperty(n => n.Extension).IgnoreIt()
                .OnProperty(n => n.NamedChildren).IgnoreIt()
                .OnProperty(n => n.TextElement).IgnoreIt()
                .OnProperty(n => n.CityElement).IgnoreIt()
                .OnProperty(n => n.CountryElement).IgnoreIt()
                .OnProperty(n => n.DistrictElement).IgnoreIt()
                .OnProperty(n => n.LineElement).IgnoreIt()
                .OnProperty(n => n.PostalCodeElement).IgnoreIt()
                .OnProperty(n => n.StateElement).IgnoreIt()
                .OnProperty(n => n.TextElement).IgnoreIt()
                .OnProperty(n => n.TypeElement).IgnoreIt()
                .OnProperty(n => n.UseElement).IgnoreIt();

            return filler;
        }

        private static ContactPoint CreateRandomEmail()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            DateTimeOffset dateTimeOffsetEnd = dateTimeOffset.AddDays(1);
            FhirDateTime periodStart = new FhirDateTime(dateTimeOffset);
            FhirDateTime periodEnd = new FhirDateTime(dateTimeOffsetEnd);
            Period emailPeriod = new Period(periodStart, periodEnd);

            return new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Period = emailPeriod,
                Value = GetRandomString()
            };
        }

        private static ContactPoint CreateRandomPhoneNumber()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            DateTimeOffset dateTimeOffsetEnd = dateTimeOffset.AddDays(1);
            FhirDateTime periodStart = new FhirDateTime(dateTimeOffset);
            FhirDateTime periodEnd = new FhirDateTime(dateTimeOffsetEnd);
            Period phoneNumberPeriod = new Period(periodStart, periodEnd);

            return new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Period = phoneNumberPeriod,
                Value = GenerateRandomMobileNumber()
            };
        }
    }
}
