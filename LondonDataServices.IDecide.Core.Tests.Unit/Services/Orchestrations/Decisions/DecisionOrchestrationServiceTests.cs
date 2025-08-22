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
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock = new Mock<IDateTimeBroker>();
        private readonly Mock<IPatientService> patientServiceMock = new Mock<IPatientService>();
        private readonly Mock<INotificationService> notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IDecisionService> decisionServiceMock = new Mock<IDecisionService>();
        private readonly DecisionOrchestrationConfigurations decisionOrchestrationConfigurations;
        private static readonly int maxRetryCount = 3;
        private readonly DecisionOrchestrationService decisionOrchestrationService;
        private readonly ICompareLogic compareLogic;

        public DecisionOrchestrationServiceTests()
        {
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.patientServiceMock = new Mock<IPatientService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.decisionServiceMock = new Mock<IDecisionService>();
            this.compareLogic = new CompareLogic();

            this.decisionOrchestrationConfigurations = new DecisionOrchestrationConfigurations
            {
                MaxRetryCount = maxRetryCount,
            };

            this.decisionOrchestrationService = new DecisionOrchestrationService(
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                patientService: this.patientServiceMock.Object,
                notificationService: this.notificationServiceMock.Object,
                decisionService: this.decisionServiceMock.Object,
                decisionOrchestrationConfigurations: this.decisionOrchestrationConfigurations);
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

        private static List<Patient> GetRandomPatients(DateTimeOffset validationCodeExpiresOn) =>
            CreatePatientFiller(validationCodeExpiresOn).Create(GetRandomNumber()).ToList();

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
                .OnProperty(n => n.ValidationCodeExpiresOn).Use(validationCodeExpiresOn)
                .OnProperty(n => n.NhsNumber).Use(inputNhsNumber)
                .OnProperty(n => n.ValidationCode).Use(validationCode)
                .OnProperty(n => n.RetryCount).Use(retryCount);

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
                .OnProperty(n => n.Patient).Use(patient)
                .OnProperty(n => n.PatientNhsNumber).Use(patient.NhsNumber);

            return filler;
        }

        private static Decision GetRandomDecisionWithNullPatient() =>
            CreateDecisionFillerWithNullPatient().Create();

        private static Filler<Decision> CreateDecisionFillerWithNullPatient()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private Expression<Func<Decision, bool>> SameDecisionAs(Decision expectedDecision) =>
            actualDecision => this.compareLogic.Compare(expectedDecision, actualDecision).AreEqual;

        private Expression<Func<NotificationInfo, bool>> SameNotificationInfoAs(NotificationInfo expectedInfo) =>
           actualInfo => this.compareLogic.Compare(expectedInfo, actualInfo).AreEqual;
    }
}
