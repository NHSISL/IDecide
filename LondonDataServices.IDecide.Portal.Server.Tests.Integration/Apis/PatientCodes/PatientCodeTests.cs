// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Models;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.PatientCodes
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientCodeTests
    {
        private readonly ApiBroker apiBroker;

        public PatientCodeTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

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

        private static PatientCodeRequest CreateRandomPatientCodeRequest(Patient patient)
        {
            PatientCodeRequest patientCodeRequest = new PatientCodeRequest
            {
                NhsNumber = patient.NhsNumber,
                VerificationCode = patient.ValidationCode,
                NotificationPreference = patient.NotificationPreference.ToString(),
                GenerateNewCode = false
            };

            return patientCodeRequest;
        }

        private async ValueTask<Patient> PostRandomPatientAsync(string? nhsNumber = null)
        {
            Patient randomPatient = CreateRandomPatient(nhsNumber);
            Patient createdPatient = await this.apiBroker.PostPatientAsync(randomPatient);

            return createdPatient;
        }

        private static Patient CreateRandomPatient(string? nhsNumber = null) =>
            CreateRandomPatientFiller(nhsNumber).Create();

        private static Filler<Patient> CreateRandomPatientFiller(string? nhsNumber = null)
        {
            string patientNhsNumber = nhsNumber ?? GenerateRandom10DigitNumber();
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(patient => patient.NhsNumber).Use(patientNhsNumber)
                .OnProperty(patient => patient.Title).Use(GetRandomStringWithLengthOf(35))
                .OnProperty(patient => patient.GivenName).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Surname).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Gender).Use(GetRandomStringWithLengthOf(50))
                .OnProperty(patient => patient.Email).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(patient => patient.Phone).Use(GetRandomStringWithLengthOf(15))
                .OnProperty(patient => patient.PostCode).Use(GetRandomStringWithLengthOf(8))
                .OnProperty(patient => patient.ValidationCode).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(patient => patient.CreatedDate).Use(now)
                .OnProperty(patient => patient.CreatedBy).Use(user)
                .OnProperty(patient => patient.UpdatedDate).Use(now)
                .OnProperty(patient => patient.UpdatedBy).Use(user);

            return filler;
        }
    }
}
