// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using LondonDataServices.IDecide.Manage.Server.Models;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests : RESTFulController
    {
        private readonly Mock<IPatientOrchestrationService> patientOrchestrationServiceMock;
        private readonly PatientSearchController patientSearchController;

        public PatientSearchControllerTests()
        {
            this.patientOrchestrationServiceMock = new Mock<IPatientOrchestrationService>();

            this.patientSearchController =
                new PatientSearchController(this.patientOrchestrationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new PatientOrchestrationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new PatientOrchestrationDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new PatientOrchestrationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new PatientOrchestrationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
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
                .OnProperty(patient => patient.Surname).Use(inputSurname);

            return filler;
        }

        private static RecordPatientInformationRequest GetRecordPatientInformationRequest() =>
            CreateRecordPatientInformationRequestFiller().Create();

        private static Filler<RecordPatientInformationRequest> CreateRecordPatientInformationRequestFiller()
        {
            var filler = new Filler<RecordPatientInformationRequest>();
            filler.Setup();

            return filler;
        }
    }
}
