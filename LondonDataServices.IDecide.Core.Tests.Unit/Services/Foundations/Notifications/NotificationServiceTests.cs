// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Securities;
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

        public NotificationServiceTests()
        {
            this.notificationBrokerMock = new Mock<INotificationBroker>();

            this.notificationService = new NotificationService(
                notificationBroker: this.notificationBrokerMock.Object);
        }

        private User CreateRandomUser(string userId = "")
        {
            userId = string.IsNullOrWhiteSpace(userId) ? GetRandomStringWithLengthOf(255) : userId;

            return new User(
                userId: userId,
                givenName: GetRandomString(),
                surname: GetRandomString(),
                displayName: GetRandomString(),
                email: GetRandomString(),
                jobTitle: GetRandomString(),
                roles: new List<string> { GetRandomString() },

                claims: new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(type: GetRandomString(), value: GetRandomString())
                });
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

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

        private static Decision CreateRandomDecision(DateTimeOffset dateTimeOffset, string userId = "") =>
            CreateDecisionFiller(dateTimeOffset, userId).Create();

        private static Patient CreateRandomPatient() =>
            CreatePatientFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Patient CreateRandomPatient(DateTimeOffset dateTimeOffset, string userId = "") =>
            CreatePatientFiller(dateTimeOffset, userId).Create();

        private static NotificationInfo CreateRandomNotificationInfo()
        {
            return new NotificationInfo
            {
                Decision = CreateRandomDecision(),
                Patient = CreateRandomPatient()
            };
        }

        private static NotificationInfo CreateRandomNotificationInfo(DateTimeOffset dateTimeOffset, string userId = "")
        {
            return new NotificationInfo
            {
                Decision = CreateRandomDecision(dateTimeOffset, userId),
                Patient = CreateRandomPatient(dateTimeOffset, userId)
            };
        }

        private static Filler<Decision> CreateDecisionFiller(DateTimeOffset dateTimeOffset, string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(decision => decision.CreatedBy).Use(userId)
                .OnProperty(decision => decision.UpdatedBy).Use(userId)
                .OnProperty(decision => decision.DecisionType).IgnoreIt();

            return filler;
        }

        private static Filler<Patient> CreatePatientFiller(DateTimeOffset dateTimeOffset, string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
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
    }
}
