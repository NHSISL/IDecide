// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ISecurityAuditBroker> securityAuditBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IConsumerStatusService consumerStatusService;

        public ConsumerStatusServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.securityAuditBrokerMock = new Mock<ISecurityAuditBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.consumerStatusService = new ConsumerStatusService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                securityAuditBroker: this.securityAuditBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
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
                    new(type: GetRandomString(), value: GetRandomString())
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

        private static ConsumerStatus CreateRandomConsumerStatus() =>
            CreateConsumerStatusFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static ConsumerStatus CreateRandomConsumerStatus(DateTimeOffset dateTimeOffset, string userId = "") =>
            CreateConsumerStatusFiller(dateTimeOffset, userId).Create();

        private static Filler<ConsumerStatus> CreateConsumerStatusFiller(
            DateTimeOffset dateTimeOffset,
            string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<ConsumerStatus>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(consumerStatus => consumerStatus.CreatedBy).Use(userId)
                .OnProperty(consumerStatus => consumerStatus.UpdatedBy).Use(userId)
                .OnProperty(consumerStatus => consumerStatus.Consumer).IgnoreIt()
                .OnProperty(consumerStatus => consumerStatus.Decision).IgnoreIt();

            return filler;
        }

    }
}
