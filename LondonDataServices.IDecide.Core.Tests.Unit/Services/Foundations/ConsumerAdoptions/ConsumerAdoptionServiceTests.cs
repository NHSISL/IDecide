// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityAuditBroker> securityAuditBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IConsumerAdoptionService consumerAdoptionService;

        public ConsumerAdoptionServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityAuditBrokerMock = new Mock<ISecurityAuditBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.consumerAdoptionService = new ConsumerAdoptionService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
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

        private static IQueryable<ConsumerAdoption> CreateRandomConsumerAdoptions()
        {
            return CreateConsumerAdoptionFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                .AsQueryable();
        }

        private static ConsumerAdoption CreateRandomModifyConsumerAdoption(
            DateTimeOffset dateTimeOffset,
            string userId = "")
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(dateTimeOffset, userId);
            randomConsumerAdoption.CreatedDate = randomConsumerAdoption.CreatedDate.AddDays(randomDaysInPast);

            return randomConsumerAdoption;
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

        public static TheoryData<int> MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ConsumerAdoption CreateRandomConsumerAdoption() =>
            CreateConsumerAdoptionFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static ConsumerAdoption CreateRandomConsumerAdoption(
            DateTimeOffset dateTimeOffset, string userId = "") =>
            CreateConsumerAdoptionFiller(dateTimeOffset, userId).Create();

        private static Filler<ConsumerAdoption> CreateConsumerAdoptionFiller(
            DateTimeOffset dateTimeOffset,
            string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<ConsumerAdoption>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(consumerAdoption => consumerAdoption.CreatedBy).Use(userId)
                .OnProperty(consumerAdoption => consumerAdoption.UpdatedBy).Use(userId)
                .OnProperty(consumerAdoption => consumerAdoption.Consumer).IgnoreIt()
                .OnProperty(consumerAdoption => consumerAdoption.Decision).IgnoreIt();

            return filler;
        }

        int GetBatchSize(int count, int batchSize)
        {
            if (batchSize <= 0)
            {
                batchSize = 1;
            }

            if (count <= 0)
            {
                return 0;
            }

            return (count + batchSize - 1) / batchSize;
        }

    }
}
