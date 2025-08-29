// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patient;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientApiTests
    {
        private readonly ApiBroker apiBroker;

        public PatientApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static DateTimeOffset GetRandomPastDateTimeOffset()
        {
            DateTime now = DateTimeOffset.UtcNow.Date;
            int randomDaysInPast = GetRandomNegativeNumber();
            DateTime pastDateTime = now.AddDays(randomDaysInPast).Date;

            return new DateTimeRange(earliestDate: pastDateTime, latestDate: now).GetValue();
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GetRandomEmail()
        {
            string randomPrefix = GetRandomStringWithLengthOf(15);
            string emailSuffix = "@email.com";

            return randomPrefix + emailSuffix;
        }

        private static Patient CreateRandomPatient() =>
            CreateRandomPatientFiller().Create();

        private static Filler<Patient> CreateRandomPatientFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Patient>();
            string groupName = GetRandomStringWithLengthOf(220);
            string name = GetRandomStringWithLengthOf(220);

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
            return await this.apiBroker.PostPatientAsync(randomPatient);
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
    }
}
