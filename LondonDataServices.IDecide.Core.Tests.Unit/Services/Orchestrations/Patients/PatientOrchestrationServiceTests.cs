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

        private Models.Foundations.Pds.Patient GetRedactedPatient(Models.Foundations.Pds.Patient patient)
        {
            return patient;
        }
    }
}
