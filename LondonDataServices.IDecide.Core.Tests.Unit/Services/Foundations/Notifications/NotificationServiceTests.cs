// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ISL.Providers.Notifications.Abstractions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationServiceTests
    {
        private readonly Mock<INotificationBroker> notificationBrokerMock;
        private readonly NotificationService notificationService;
        private readonly NotificationConfig notificationConfig;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;

        public NotificationServiceTests()
        {
            this.notificationBrokerMock = new Mock<INotificationBroker>();

            this.notificationConfig = new NotificationConfig
            {
                EmailCodeTemplateId = GetRandomString(),
                SmsCodeTemplateId = GetRandomString(),
                LetterCodeTemplateId = GetRandomString(),
                EmailSubmissionSuccessTemplateId = GetRandomString(),
                SmsSubmissionSuccessTemplateId = GetRandomString(),
                LetterSubmissionSuccessTemplateId = GetRandomString(),
                EmailSubscriberUsageTemplateId = GetRandomString(),
                SmsSubscriberUsageTemplateId = GetRandomString(),
                LetterSubscriberUsageTemplateId = GetRandomString()
            };

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.notificationService = new NotificationService(
                this.notificationBrokerMock.Object, this.notificationConfig, this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static NotificationProviderValidationException GetNotificationProviderValidationException() =>
            (NotificationProviderValidationException)RuntimeHelpers.GetUninitializedObject(
                typeof(NotificationProviderValidationException));

        private static NotificationProviderDependencyException GetNotificationProviderDependencyException() =>
            (NotificationProviderDependencyException)RuntimeHelpers.GetUninitializedObject(
                typeof(NotificationProviderDependencyException));

        private static NotificationProviderServiceException GetNotificationProviderServiceException() =>
            (NotificationProviderServiceException)RuntimeHelpers.GetUninitializedObject(
                typeof(NotificationProviderServiceException));

        public static TheoryData<Xeption> DependencyExceptions()
        {
            return
            [
                GetNotificationProviderDependencyException(),
                GetNotificationProviderServiceException()
            ];
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

        private static Decision CreateRandomDecision() =>
            CreateDecisionFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Patient CreateRandomPatient() =>
            CreatePatientFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static NotificationInfo CreateRandomNotificationInfo()
        {
            return new NotificationInfo
            {
                Decision = CreateRandomDecision(),
                Patient = CreateRandomPatient()
            };
        }

        private static Filler<Decision> CreateDecisionFiller(DateTimeOffset dateTimeOffset, string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(decision => decision.CreatedBy).Use(userId)
                .OnProperty(decision => decision.UpdatedBy).Use(userId)
                .OnProperty(decision => decision.DecisionType).Use(new DecisionType
                {
                    Name = GetRandomString()
                });

            return filler;
        }

        private static Filler<Patient> CreatePatientFiller(DateTimeOffset dateTimeOffset, string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
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

        public Dictionary<string, dynamic> GetCodePersonalisation(NotificationInfo notificationInfo)
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "patient.nhsNumber", notificationInfo.Patient.NhsNumber },
                { "patient.title", notificationInfo.Patient.Title },
                { "patient.givenName", notificationInfo.Patient.GivenName },
                { "patient.surname", notificationInfo.Patient.Surname },
                { "patient.dateOfBirth", notificationInfo.Patient.DateOfBirth },
                { "patient.gender", notificationInfo.Patient.Gender },
                { "patient.email", notificationInfo.Patient.Email },
                { "patient.phone", notificationInfo.Patient.Phone },
                { "patient.address", notificationInfo.Patient.Address },
                { "patient.postCode", notificationInfo.Patient.PostCode },
                { "patient.validationCode", notificationInfo.Patient.ValidationCode },
                { "patient.validationCodeExpiresOn", notificationInfo.Patient.ValidationCodeExpiresOn },
            };

            return personalisation;
        }

        public Dictionary<string, dynamic> GetDecisionPersonalisation(NotificationInfo notificationInfo)
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "patient.nhsNumber", notificationInfo.Patient.NhsNumber },
                { "patient.title", notificationInfo.Patient.Title },
                { "patient.givenName", notificationInfo.Patient.GivenName },
                { "patient.surname", notificationInfo.Patient.Surname },
                { "patient.dateOfBirth", notificationInfo.Patient.DateOfBirth },
                { "patient.gender", notificationInfo.Patient.Gender },
                { "patient.email", notificationInfo.Patient.Email },
                { "patient.phone", notificationInfo.Patient.Phone },
                { "patient.address", notificationInfo.Patient.Address },
                { "patient.postCode", notificationInfo.Patient.PostCode },
                { "patient.validationCode", notificationInfo.Patient.ValidationCode },
                { "patient.validationCodeExpiresOn", notificationInfo.Patient.ValidationCodeExpiresOn },
                { "decision.decisionChoice", notificationInfo.Decision.DecisionChoice },
                { "decision.decisionType.name", notificationInfo.Decision.DecisionType.Name }
            };

            if (!string.IsNullOrWhiteSpace(notificationInfo.Decision.ResponsiblePersonGivenName))

                personalisation.Add(
                    "decision.responsiblePersonGivenName", notificationInfo.Decision.ResponsiblePersonGivenName);

            if (!string.IsNullOrWhiteSpace(notificationInfo.Decision.ResponsiblePersonSurname))

                personalisation.Add(
                    "decision.responsiblePersonSurname", notificationInfo.Decision.ResponsiblePersonSurname);

            if (!string.IsNullOrWhiteSpace(notificationInfo.Decision.ResponsiblePersonRelationship))

                personalisation.Add(
                    "decision.responsiblePersonRelationship", notificationInfo.Decision.ResponsiblePersonRelationship);

            return personalisation;
        }
    }
}
