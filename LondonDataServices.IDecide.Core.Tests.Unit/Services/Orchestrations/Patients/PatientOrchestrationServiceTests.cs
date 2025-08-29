// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KellermanSoftware.CompareNetObjects;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly Mock<ISecurityBroker> securityBrokerMock = new Mock<ISecurityBroker>();
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock = new Mock<IDateTimeBroker>();
        private readonly Mock<IPdsService> pdsServiceMock = new Mock<IPdsService>();
        private readonly Mock<IPatientService> patientServiceMock = new Mock<IPatientService>();
        private readonly Mock<INotificationService> notificationServiceMock = new Mock<INotificationService>();
        private readonly DecisionConfigurations decisionConfigurations;
        private readonly PatientOrchestrationService patientOrchestrationService;
        private static readonly int expireAfterMinutes = 1440;
        private static readonly int retryCount = 3;
        private static readonly List<string> decisionWorkflowRoles = new List<string> { "Administrator" };
        private readonly ICompareLogic compareLogic;

        public PatientOrchestrationServiceTests()
        {
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.pdsServiceMock = new Mock<IPdsService>();
            this.patientServiceMock = new Mock<IPatientService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.compareLogic = new CompareLogic();

            this.decisionConfigurations = new DecisionConfigurations
            {
                PatientValidationCodeExpireAfterMinutes = expireAfterMinutes,
                MaxRetryCount = retryCount,
                DecisionWorkflowRoles = decisionWorkflowRoles
            };

            this.patientOrchestrationService = new PatientOrchestrationService(
                loggingBroker: this.loggingBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                pdsService: this.pdsServiceMock.Object,
                patientService: this.patientServiceMock.Object,
                notificationService: this.notificationServiceMock.Object,
                decisionConfigurations: this.decisionConfigurations);

        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
           new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
           new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private PatientLookup GetRandomSearchPatientLookupWithNoNhsNumber(string surname)
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

        private PatientLookup GetRandomSearchPatientLookupWithNhsNumber(string nhsNumber)
        {
            SearchCriteria searchCriteria = new SearchCriteria
            {
                NhsNumber = nhsNumber
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

        private static Patient GetRandomPatientWithNhsNumber(string nhsNumber) =>
            CreatePatientFillerWithNhsNumber(nhsNumber).Create();

        private static Filler<Patient> CreatePatientFillerWithNhsNumber(string nhsNumber = "1234567890")
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(n => n.NhsNumber).Use(nhsNumber);

            return filler;
        }

        private Expression<Func<Patient, bool>> SamePatientAs(Patient expectedPatient) =>
            actualPatient => this.compareLogic.Compare(expectedPatient, actualPatient).AreEqual;

        private Expression<Func<NotificationInfo, bool>> SameNotificationInfoAs(NotificationInfo expectedInfo) =>
           actualInfo => this.compareLogic.Compare(expectedInfo, actualInfo).AreEqual;

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

        public static TheoryData<Xeption> RecordPatientInformationDependencyValidationExceptions()
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

                new PatientValidationException(
                    message: "Patient validation errors occured, please try again",
                    innerException),

                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException),

                new NotificationValidationException(
                    message: "Notification validation errors occured, please try again",
                    innerException),

                new NotificationDependencyValidationException(
                    message: "Notification dependency validation occurred, please try again.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> RecordPatientInformationDependencyExceptions()
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

                new PatientDependencyException(
                    message: "Patient dependency error occurred, please contact support.",
                    innerException),

                new PatientServiceException(
                    message: "Patient service error occurred, please contact support.",
                    innerException),

                new NotificationDependencyException(
                    message: "Notification dependency error occurred, please contact support.",
                    innerException),

                new NotificationServiceException(
                    message: "Notification service error occurred, please contact support.",
                    innerException),
            };
        }
    }
}
