// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using Tynamix.ObjectFiller;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using System.Linq.Expressions;
using Xeptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using System.Collections.Generic;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        private readonly Mock<IPdsService> pdsServiceMock = new Mock<IPdsService>();
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly PatientOrchestrationService patientOrchestrationService;

        public PatientOrchestrationServiceTests()
        {
            this.pdsServiceMock = new Mock<IPdsService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.patientOrchestrationService = new PatientOrchestrationService(
                this.pdsServiceMock.Object,
                this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
           new IntRange(min: 2, max: 10).GetValue();

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

        private static List<Patient> GetRandomPatients() =>
            CreatePatientFiller().Create(GetRandomNumber()).ToList();

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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new PdsValidationException(
                    message: "PDS validation errors occured, please try again",
                    innerException),

                new PdsDependencyValidationException(
                    message: "PDS dependency validation occurred, please try again.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new PdsDependencyException(
                    message: "PDS dependency error occurred, please contact support.",
                    innerException),

                new PdsServiceException(
                    message: "PDS service error occurred, please contact support.",
                    innerException),
            };
        }
    }
}
