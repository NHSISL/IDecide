// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using ISL.Providers.PDS.FakeFHIR.Mappers;
using System.Collections.Generic;
using System;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using Tynamix.ObjectFiller;
using System.Linq;
using LondonDataServices.IDecide.Core.Mappers;
using System.Linq.Expressions;
using Xeptions;

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

            return patient;
        }

        private static Patient CreateRandomPatientWithNhsNumber(string nhsNumber)
        {
            var patient = new Patient();

            var nameFiller = new Filler<HumanName>();
            nameFiller.Setup()
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

            patient.Id = nhsNumber;
            patient.Name = new List<HumanName> { nameFiller.Create() };
            patient.Gender = AdministrativeGender.Male;
            patient.BirthDate = GetRandomDateTimeOffset().ToString("yyyy-MM-dd");

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

        private Models.Foundations.Pds.Patient CreateRandomLocalPatient(PatientBundle patientBundle)
        {
            Models.Foundations.Pds.Patient patient = LocalPatientMapper.FromPatientBundle(patientBundle);

            return patient;
        }

        private Models.Foundations.Pds.Patient CreateRandomLocalPatient(Patient fhirPatient)
        {
            Models.Foundations.Pds.Patient patient = LocalPatientMapper.FromFhirPatient(fhirPatient);

            return patient;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
