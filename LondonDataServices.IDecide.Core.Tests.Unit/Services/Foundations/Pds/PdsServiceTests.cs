// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using Tynamix.ObjectFiller;
using System.Linq.Expressions;
using Xeptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using ISL.Providers.PDS.FakeFHIR.Mappers;
using System.Collections.Generic;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;
using System.Linq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        private readonly Mock<IPdsBroker> pdsBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly PdsService pdsService;

        public PdsServiceTests()
        {
            this.pdsBrokerMock = new Mock<IPdsBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.pdsService = new PdsService(
                pdsBroker: pdsBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static Hl7.Fhir.Model.Patient CreateRandomPatient(string surname = "Test")
        {
            var patient = new Hl7.Fhir.Model.Patient();
            patient.Name = new List<HumanName> { CreateHumanNameFiller().Create() };
            patient.Gender = AdministrativeGender.Male;
            patient.BirthDate = GetRandomDateTimeOffset().ToString("yyyy-MM-dd");
            patient.Address = new List<Address> { CreateAddressFiller().Create() };

            patient.Telecom = new List<ContactPoint> { 
                CreateContactPointFiller(ContactPoint.ContactPointSystem.Phone).Create(), 
                CreateContactPointFiller(ContactPoint.ContactPointSystem.Email).Create()
            };

            return patient;
        }

        private static Hl7.Fhir.Model.Patient CreateRandomPatientWithNhsNumber(string nhsNumber)
        {
            var patient = new Hl7.Fhir.Model.Patient();

            patient.Id = nhsNumber;
            patient.Name = new List<HumanName> { CreateHumanNameFiller().Create() };
            patient.Gender = AdministrativeGender.Male;
            patient.BirthDate = GetRandomDateTimeOffset().ToString("yyyy-MM-dd");
            patient.Address = new List<Address> { CreateAddressFiller().Create() };

            patient.Telecom = new List<ContactPoint> {
                CreateContactPointFiller(ContactPoint.ContactPointSystem.Phone).Create(),
                CreateContactPointFiller(ContactPoint.ContactPointSystem.Email).Create()
            };

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

            Hl7.Fhir.Model.Patient patient = CreateRandomPatient(surname);

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

        private static Patient GetRandomPatient(string inputSurname) =>
            CreatePatientFiller(inputSurname).Create();

        private static Filler<Patient> CreatePatientFiller(string inputSurname = "Test")
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(n => n.Surname).Use(inputSurname);

            return filler;
        }

        private static Filler<HumanName> CreateHumanNameFiller(string surname = "Test")
        {
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

            return nameFiller;
        }

        private static Filler<Address> CreateAddressFiller()
        {
            var addressFiller = new Filler<Address>();

            addressFiller.Setup()
                .OnProperty(a => a.Children).IgnoreIt()
                .OnProperty(a => a.CityElement).IgnoreIt()
                .OnProperty(a => a.CountryElement).IgnoreIt()
                .OnProperty(a => a.DistrictElement).IgnoreIt()
                .OnProperty(a => a.Extension).IgnoreIt()
                .OnProperty(a => a.LineElement).IgnoreIt()
                .OnProperty(a => a.NamedChildren).IgnoreIt()
                .OnProperty(a => a.Period).IgnoreIt()
                .OnProperty(a => a.PostalCodeElement).IgnoreIt()
                .OnProperty(a => a.StateElement).IgnoreIt()
                .OnProperty(a => a.TextElement).IgnoreIt()
                .OnProperty(a => a.TypeElement).IgnoreIt()
                .OnProperty(a => a.UseElement).IgnoreIt();

            return addressFiller;
        }

        private static Filler<ContactPoint> CreateContactPointFiller(ContactPoint.ContactPointSystem contactPointSystem)
        {
            var contactPointFiller = new Filler<ContactPoint>();

            contactPointFiller.Setup()
                .OnProperty(cp => cp.System).Use(contactPointSystem)
                .OnProperty(cp => cp.Children).IgnoreIt()
                .OnProperty(cp => cp.Extension).IgnoreIt()
                .OnProperty(cp => cp.NamedChildren).IgnoreIt()
                .OnProperty(cp => cp.Period).IgnoreIt()
                .OnProperty(cp => cp.RankElement).IgnoreIt()
                .OnProperty(cp => cp.SystemElement).IgnoreIt()
                .OnProperty(cp => cp.UseElement).IgnoreIt()
                .OnProperty(cp => cp.ValueElement).IgnoreIt();

            return contactPointFiller;
        }

        private static Patient GeneratePatientFromFhirPatient(Hl7.Fhir.Model.Patient fhirPatient) =>
            CreatePatientFillerFromFhirPatient(fhirPatient).Create();

        private static Filler<Patient> CreatePatientFillerFromFhirPatient(Hl7.Fhir.Model.Patient fhirPatient)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Patient>();
            string givenName = fhirPatient.Name.Select(n => string.Join(' ', n.Given)).First();
            string surname = fhirPatient.Name.Select(n => n.Family).First();
            string address = fhirPatient.Address.Select(a => BuildUkAddressString(a)).First();
            DateTimeOffset dateOfBirth = DateTimeOffset.Parse(fhirPatient.BirthDate);
            string postcode = fhirPatient.Address.Select(a => a.PostalCode).First();
            string gender = fhirPatient.Gender.ToString();
            string title = fhirPatient.Name.Select(n => string.Join(' ', n.Prefix)).First();

            string email = fhirPatient.Telecom
                .Where(t => t.System == ContactPoint.ContactPointSystem.Email)
                .Select(t => t.Value)
                .First();

            string phone = fhirPatient.Telecom
               .Where(t => t.System == ContactPoint.ContactPointSystem.Phone)
               .Select(t => t.Value)
               .First();


            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(n => n.NhsNumber).Use(fhirPatient.Id)
                .OnProperty(n => n.GivenName).Use(givenName)
                .OnProperty(n => n.Surname).Use(surname)
                .OnProperty(n => n.Address).Use(address)
                .OnProperty(n => n.DateOfBirth).Use(dateOfBirth)
                .OnProperty(n => n.PostCode).Use(postcode)
                .OnProperty(n => n.Gender).Use(gender)
                .OnProperty(n => n.Title).Use(title)
                .OnProperty(n => n.Email).Use(email)
                .OnProperty(n => n.Phone).Use(phone)
                .OnProperty(n => n.Id).IgnoreIt()
                .OnProperty(n => n.ValidationCode).IgnoreIt()
                .OnProperty(n => n.ValidationCodeExpiresOn).IgnoreIt()
                .OnProperty(n => n.RetryCount).IgnoreIt()
                .OnProperty(n => n.CreatedBy).IgnoreIt()
                .OnProperty(n => n.CreatedDate).IgnoreIt()
                .OnProperty(n => n.UpdatedBy).IgnoreIt()
                .OnProperty(n => n.UpdatedDate).IgnoreIt()
                .OnProperty(n => n.Decisions).IgnoreIt();

            return filler;
        }

        private static string BuildUkAddressString(Address address)
        {
            var parts = new[]
            {
                string.Join(", ", address.Line),
                address.City,
                address.District,
                address.PostalCode,
                address.Country
            };

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}