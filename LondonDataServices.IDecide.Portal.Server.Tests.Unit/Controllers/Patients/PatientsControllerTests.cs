// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests : RESTFulController
    {
        private readonly Mock<IPatientService> patientServiceMock;
        private readonly Mock<INhsLoginService> nhsLoginServiceMock;
        private readonly Mock<IPatientOrchestrationService> patientOrchestrationServiceMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
        private readonly HttpClient httpClientMock;
        private readonly PatientsController patientsController;

        public PatientsControllerTests()
        {
            patientServiceMock = new Mock<IPatientService>();
            nhsLoginServiceMock = new Mock<INhsLoginService>();
            patientOrchestrationServiceMock = new Mock<IPatientOrchestrationService>();
            configurationMock = new Mock<IConfiguration>();
            httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

            patientsController = new PatientsController(
                patientServiceMock.Object,
                nhsLoginServiceMock.Object,
                patientOrchestrationServiceMock.Object,
                configurationMock.Object);
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
            string result = new MnemonicString(
                wordCount: 1,
                wordMinLength: length,
                wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Patient CreateRandomPatient() =>
            CreatePatientFiller().Create();

        private static NhsLoginUserInfo CreateRandomNhsLoginUserInfo() =>
            CreateNhsLoginUserInfoFiller().Create();

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

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(patient => patient.NhsNumber).Use(GetRandomStringWithLengthOf(10))
                .OnProperty(patient => patient.Title).Use(GetRandomStringWithLengthOf(35))
                .OnProperty(patient => patient.GivenName).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Surname).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Gender).Use(GetRandomStringWithLengthOf(50))
                .OnProperty(patient => patient.Email).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Phone).Use(GetRandomStringWithLengthOf(15))
                .OnProperty(patient => patient.PostCode).Use(GetRandomStringWithLengthOf(8))
                .OnProperty(patient => patient.ValidationCode).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(patient => patient.CreatedDate).Use(dateTimeOffset)
                .OnProperty(patient => patient.CreatedBy).Use(user)
                .OnProperty(patient => patient.UpdatedDate).Use(dateTimeOffset)
                .OnProperty(patient => patient.UpdatedBy).Use(user);

            return filler;
        }

        private static Filler<NhsLoginUserInfo> CreateNhsLoginUserInfoFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<NhsLoginUserInfo>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(nhsLoginUserInfo => nhsLoginUserInfo.FamilyName).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(nhsLoginUserInfo => nhsLoginUserInfo.Email).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(nhsLoginUserInfo => nhsLoginUserInfo.PhoneNumber).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(nhsLoginUserInfo => nhsLoginUserInfo.GivenName).Use(GetRandomStringWithLengthOf(255));

            return filler;
        }
    }
}