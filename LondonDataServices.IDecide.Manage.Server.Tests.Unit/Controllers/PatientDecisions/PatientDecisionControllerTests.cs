// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests : RESTFulController
    {
        private readonly Mock<IDecisionOrchestrationService> decisionOrchestrationServiceMock;
        private readonly PatientDecisionController patientDecisionController;

        public PatientDecisionControllerTests()
        {
            this.decisionOrchestrationServiceMock = new Mock<IDecisionOrchestrationService>();

            this.patientDecisionController =
                new PatientDecisionController(this.decisionOrchestrationServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
           new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Patient GetRandomPatient(
            DateTimeOffset validationCodeExpiresOn,
            string inputNhsNumber = "1234567890",
            string validationCode = "A1B2C",
            int retryCount = 0) =>
            CreatePatientFiller(validationCodeExpiresOn, inputNhsNumber, validationCode, retryCount).Create();

        private static Filler<Patient> CreatePatientFiller(
            DateTimeOffset validationCodeExpiresOn,
            string inputNhsNumber = "1234567890",
            string validationCode = "A1B2C",
            int retryCount = 0)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(patient => patient.ValidationCodeExpiresOn).Use(validationCodeExpiresOn)
                .OnProperty(patient => patient.NhsNumber).Use(inputNhsNumber)
                .OnProperty(patient => patient.ValidationCode).Use(validationCode)
                .OnProperty(patient => patient.RetryCount).Use(retryCount);

            return filler;
        }

        private static Decision GetRandomDecision(Patient patient) =>
            CreateDecisionFiller(patient).Create();

        private static Filler<Decision> CreateDecisionFiller(Patient patient)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(decision => decision.Patient).Use(patient);

            return filler;
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new DecisionOrchestrationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new DecisionOrchestrationDependencyValidationException(
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
                new DecisionOrchestrationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new DecisionOrchestrationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }
    }
}
