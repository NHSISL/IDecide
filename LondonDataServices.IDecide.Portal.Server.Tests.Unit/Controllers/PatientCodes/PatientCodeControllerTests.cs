// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using LondonDataServices.IDecide.Portal.Server.Models;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientCodes
{
    public partial class PatientCodeControllerTests : RESTFulController
    {
        private readonly Mock<IPatientOrchestrationService> patientOrchestrationServiceMock;
        private readonly PatientCodeController patientCodeController;

        public PatientCodeControllerTests()
        {
            this.patientOrchestrationServiceMock = new Mock<IPatientOrchestrationService>();

            this.patientCodeController =
                new PatientCodeController(this.patientOrchestrationServiceMock.Object);
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

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static PatientCodeRequest GetRandomPatientCodeRequest() =>
           CreatePatientCodeRequestFiller().Create();

        private static Filler<PatientCodeRequest> CreatePatientCodeRequestFiller()
        {
            var filler = new Filler<PatientCodeRequest>();

            filler.Setup()
                .OnProperty(pcr => pcr.NhsNumber).Use(GenerateRandom10DigitNumber());

            return filler;
        }
    }
}
