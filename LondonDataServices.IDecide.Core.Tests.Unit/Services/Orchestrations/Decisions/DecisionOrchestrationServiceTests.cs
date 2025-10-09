// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using KellermanSoftware.CompareNetObjects;
using LondonDataServices.IDecide.Core.Brokers.Audits;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
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
        private readonly Mock<ISecurityBroker> securityBrokerMock = new Mock<ISecurityBroker>();
        private readonly Mock<IAuditBroker> auditBrokerMock = new Mock<IAuditBroker>();
        private readonly Mock<IIdentifierBroker> identifierBrokerMock = new Mock<IIdentifierBroker>();
        private readonly Mock<IPatientService> patientServiceMock = new Mock<IPatientService>();
        private readonly Mock<INotificationService> notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IDecisionService> decisionServiceMock = new Mock<IDecisionService>();
        private readonly Mock<IConsumerService> consumerServiceMock = new Mock<IConsumerService>();
        private readonly DecisionConfigurations decisionConfigurations;
        private readonly SecurityBrokerConfigurations securityBrokerConfigurations;
        private static readonly int maxRetryCount = 3;
        private static readonly int patientValidationCodeExpireAfterMinutes = 1440;
        private static readonly int validatedCodeValidForMinutes = 1440;
        private static readonly List<string> decisionWorkflowRoles = new List<string> { "Administrator" };
        private readonly DecisionOrchestrationService decisionOrchestrationService;
        private readonly ICompareLogic compareLogic;

        public DecisionOrchestrationServiceTests()
        {
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.auditBrokerMock = new Mock<IAuditBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.patientServiceMock = new Mock<IPatientService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.decisionServiceMock = new Mock<IDecisionService>();
            this.consumerServiceMock = new Mock<IConsumerService>();
            this.compareLogic = new CompareLogic();

            this.decisionConfigurations = new DecisionConfigurations
            {
                MaxRetryCount = maxRetryCount,
                PatientValidationCodeExpireAfterMinutes = patientValidationCodeExpireAfterMinutes,
                ValidatedCodeValidForMinutes = validatedCodeValidForMinutes,
                DecisionWorkflowRoles = decisionWorkflowRoles
            };

            this.securityBrokerConfigurations = new SecurityBrokerConfigurations
            {
                ReCaptchaScoreThreshold = 0.8
            };

            this.decisionOrchestrationService = new DecisionOrchestrationService(
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                auditBroker: this.auditBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                patientService: this.patientServiceMock.Object,
                notificationService: this.notificationServiceMock.Object,
                decisionService: this.decisionServiceMock.Object,
                consumerService: this.consumerServiceMock.Object,
                decisionConfigurations: this.decisionConfigurations,
                securityBrokerConfigurations: this.securityBrokerConfigurations);
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

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static IQueryable<Consumer> CreateRandomConsumers()
        {
            return CreateConsumerFiller()
                .Create(count: GetRandomNumber())
                .AsQueryable();
        }

        private static IQueryable<Consumer> CreateRandomConsumersWithMatchingEntraIdEntry(string userId)
        {
            List<Consumer> consumers = CreateConsumerFiller()
                .Create(count: GetRandomNumber())
                .ToList();

            if (!string.IsNullOrWhiteSpace(userId) && consumers.Count > 0)
            {
                consumers[0].EntraId = userId;
            }

            return consumers.AsQueryable();
        }

        private static Filler<Consumer> CreateConsumerFiller()
        {
            string userId = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLengthOf(36))
                .OnProperty(consumer => consumer.CreatedBy).Use(userId)
                .OnProperty(consumer => consumer.UpdatedBy).Use(userId)
                .OnProperty(consumer => consumer.ConsumerAdoptions).IgnoreIt();

            return filler;
        }

        private static Patient CreateRandomPatient() =>
            CreatePatientFiller().Create();

        private static Filler<Patient> CreatePatientFiller()
        {
            string userId = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
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
                .OnProperty(patient => patient.CreatedBy).Use(userId)
                .OnProperty(patient => patient.UpdatedBy).Use(userId)
                .OnProperty(patient => patient.Decisions).IgnoreIt();

            return filler;
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
                .OnProperty(patient => patient.ValidationCodeExpiresOn).Use(validationCodeExpiresOn)
                .OnProperty(patient => patient.NhsNumber).Use(inputNhsNumber)
                .OnProperty(patient => patient.ValidationCode).Use(validationCode)
                .OnProperty(patient => patient.RetryCount).Use(retryCount);

            return filler;
        }

        private static IQueryable<Decision> CreateRandomDecisions()
        {
            return CreateDecisionFiller(CreateRandomPatient())
                .Create(count: GetRandomNumber())
                .AsQueryable();
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

        private static Decision GetRandomDecisionWithNullPatient() =>
            CreateDecisionFillerWithNullPatient().Create();

        private static Filler<Decision> CreateDecisionFillerWithNullPatient()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(decision => decision.Patient).Use((Patient)null);

            return filler;
        }

        private static List<Claim> CreateRandomClaims()
        {
            string randomString = GetRandomString();

            return Enumerable.Range(start: 1, count: GetRandomNumber())
                .Select(_ => new Claim(type: randomString, value: randomString)).ToList();
        }

        private static User CreateRandomUser()
        {
            string randomId = GetRandomStringWithLengthOf(255);
            string randomString = GetRandomString();

            User user = new User(
                userId: randomId,
                givenName: randomString,
                surname: randomString,
                displayName: randomString,
                email: randomString,
                jobTitle: randomString,
                roles: new List<string> { randomString },
                claims: CreateRandomClaims());

            return user;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private Expression<Func<Decision, bool>> SameDecisionAs(Decision expectedDecision) =>
            actualDecision => this.compareLogic.Compare(expectedDecision, actualDecision).AreEqual;

        private Expression<Func<NotificationInfo, bool>> SameNotificationInfoAs(NotificationInfo expectedInfo) =>
           actualInfo => this.compareLogic.Compare(expectedInfo, actualInfo).AreEqual;

        private Expression<Func<Patient, bool>> SamePatientAs(Patient expectedPatient) =>
            actualPatient => this.compareLogic.Compare(expectedPatient, actualPatient).AreEqual;

        public static TheoryData<Xeption> VerifyAndRecordDecisionDependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new PatientValidationException(
                    message: "Patient validation errors occured, please try again",
                    innerException),

                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException),

                new DecisionValidationException(
                    message: "Decision validation errors occured, please try again",
                    innerException),

                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException),

                new NotificationValidationException(
                    message: "Notification validation errors occured, please try again",
                    innerException),

                new NotificationDependencyValidationException(
                    message: "Notification dependency validation occurred, please try again.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> VerifyAndRecordDecisionDependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new PatientDependencyException(
                    message: "Patient dependency error occurred, please contact support.",
                    innerException),

                new PatientServiceException(
                    message: "Patient service error occurred, please contact support.",
                    innerException),

                new DecisionDependencyException(
                    message: "Decision dependency error occurred, please contact support.",
                    innerException),

                new DecisionServiceException(
                    message: "Decision service error occurred, please contact support.",
                    innerException),

                new NotificationDependencyException(
                    message: "Notification dependency error occurred, please contact support.",
                    innerException),

                new NotificationServiceException(
                    message: "Notification service error occurred, please contact support.",
                    innerException),
            };
        }

        public static TheoryData<Xeption>
            RetrieveAllPendingAdoptionDecisionsForConsumerDependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ConsumerValidationException(
                    message: "Consumer validation errors occured, please try again",
                    innerException),

                new ConsumerDependencyValidationException(
                    message: "Consumer dependency validation occurred, please try again.",
                    innerException),

                new DecisionValidationException(
                    message: "Decision validation errors occured, please try again",
                    innerException),

                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException)
            };
        }

        public static TheoryData<Xeption> RetrieveAllPendingAdoptionDecisionsForConsumerDependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, please contact support.",
                    innerException),

                new ConsumerServiceException(
                    message: "Consumer service error occurred, please contact support.",
                    innerException),

                new DecisionDependencyException(
                    message: "Decision dependency error occurred, please contact support.",
                    innerException),

                new DecisionServiceException(
                    message: "Decision service error occurred, please contact support.",
                    innerException)
            };
        }
    }
}
