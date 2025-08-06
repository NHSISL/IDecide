// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tynamix.ObjectFiller;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests : ControllerBase
    {
        private readonly Mock<IPatientOrchestrationService> patientOrchestrationServiceMock;
        private readonly PatientSearchController patientSearchController;

        public PatientSearchControllerTests()
        {
            this.patientOrchestrationServiceMock = new Mock<IPatientOrchestrationService>();

            this.patientSearchController = 
                new PatientSearchController(this.patientOrchestrationServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

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
    }
}
