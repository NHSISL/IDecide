// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using LondonDataServices.IDecide.Portal.Server.Models.Foundations.Patients;
using LondonDataServices.IDecide.Portal.Server.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Portal.Server.Services.Foundations.Patients;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests : RESTFulController
    {

        private readonly Mock<IPatientService> patientServiceMock;
        private readonly PatientsController patientsController;

        public PatientsControllerTests()
        {
            patientServiceMock = new Mock<IPatientService>();
            patientsController = new PatientsController(patientServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new PatientValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new PatientDependencyValidationException(
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
                new PatientDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new PatientServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Patient CreateRandomPatient() =>
            CreatePatientFiller().Create();

        private static IQueryable<Patient> CreateRandomPatients()
        {
            return CreatePatientFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<Patient> CreatePatientFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<Patient>();
            string name = GetRandomStringWithLengthOf(220);
            string groupName = GetRandomStringWithLengthOf(220);

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(patient => patient.GroupName).Use(() => groupName)
                .OnProperty(patient => patient.Name).Use(() => name)
                .OnProperty(patient => patient.CreatedBy).Use(user)
                .OnProperty(patient => patient.UpdatedBy).Use(user);

            return filler;
        }
    }
}