// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientApiTests
    {
        private readonly ApiBroker apiBroker;

        public PatientsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Patient UpdatePatientWithRandomValues(Patient inputPatient)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedPatient = CreateRandomPatient();
            updatedPatient.Id = inputPatient.Id;
            updatedPatient.CreatedDate = inputPatient.CreatedDate;
            updatedPatient.CreatedBy = inputPatient.CreatedBy;
            updatedPatient.UpdatedDate = now;

            return updatedPatient;
        }

        private async ValueTask<Patient> PostRandomPatientAsync()
        {
            Patient randomPatient = CreateRandomPatient();
            Patient createdPatient = await this.apiBroker.PostPatientAsync(randomPatient);

            return createdPatient;
        }

        private async ValueTask<List<Patient>> PostRandomPatientsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomPatients = new List<Patient>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomPatients.Add(await PostRandomPatientAsync());
            }

            return randomPatients;
        }

        private static Patient CreateRandomPatient() =>
            CreateRandomPatientFiller().Create();

        private static Filler<Patient> CreateRandomPatientFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            string name = GetRandomStringWithLengthOf(220);
            string groupName = GetRandomStringWithLengthOf(220);
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(patient => patient.GroupName).Use(() => groupName)
                .OnProperty(patient => patient.Name).Use(() => name)
                .OnProperty(patient => patient.CreatedDate).Use(now)
                .OnProperty(patient => patient.CreatedBy).Use(user)
                .OnProperty(patient => patient.UpdatedDate).Use(now)
                .OnProperty(patient => patient.UpdatedBy).Use(user);

            return filler;
        }
    }
}