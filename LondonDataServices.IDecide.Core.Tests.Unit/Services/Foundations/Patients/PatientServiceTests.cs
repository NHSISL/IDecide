// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System;
using Tynamix.ObjectFiller;
using Xeptions;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Securities;
using System.Collections.Generic;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly PatientService patientService;

        public PatientServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.patientService = new PatientService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();


        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static string GenerateRandom5DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(10000, 20000).ToString();

            return randomNumber;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Patient CreateRandomPatient(
            string nhsNumber,
            string validationCode)
        {
            return CreateRandomPatientFiller(
                nhsNumber,
                validationCode, 
                GetRandomDateTimeOffset(), 
                GetRandomStringWithLengthOf(255))
                    .Create();
        }

        private static Patient CreateRandomPatient(
            string nhsNumber,
            string validationCode,
            DateTimeOffset dateTimeOffset,
            string userId)
        {
            return CreateRandomPatientFiller(nhsNumber, validationCode, dateTimeOffset, userId).Create();
        }

        private static Filler<Patient> CreateRandomPatientFiller(
            string nhsNumber,
            string validationCode,
            DateTimeOffset dateTimeOffset,
            string userId)
        {
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(user => user.CreatedBy).Use(userId)
                .OnProperty(user => user.UpdatedBy).Use(userId)
                .OnProperty(patient => patient.NhsNumber).Use(nhsNumber)
                .OnProperty(patient => patient.ValidationCode).Use(validationCode);

            return filler;
        }

        private User CreateRandomUser(string userId = "")
        {
            var newUserId = string.IsNullOrWhiteSpace(userId) ? GetRandomStringWithLengthOf(255) : userId;

            return new User(
                userId: newUserId,
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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }
    }
}
