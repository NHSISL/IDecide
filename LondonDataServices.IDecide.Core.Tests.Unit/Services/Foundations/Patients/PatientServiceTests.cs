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

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly PatientService patientService;

        public PatientServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.patientService = new PatientService(
                storageBroker: this.storageBrokerMock.Object,
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

        private static Patient CreateRandomPatient(string nhsNumber, string validationCode)
        {
            return CreateRandomPatientFiller(nhsNumber, validationCode).Create();
        }

        private static Filler<Patient> CreateRandomPatientFiller(string nhsNumber, string validationCode)
        {
            var filler = new Filler<Patient>();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(randomDateTime)
                .OnProperty(patient => patient.NhsNumber).Use(nhsNumber)
                .OnProperty(patient => patient.ValidationCode).Use(validationCode);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }
    }
}
