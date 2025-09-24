// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.PatientDecisions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientDecisions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientDecisionTests
    {
        private readonly ApiBroker apiBroker;

        public PatientDecisionTests(ApiBroker apiBroker) =>
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

        private static Decision CreateRandomDecision(Patient patient, Guid decisionTypeId) =>
            CreateRandomDecisionFiller(patient, decisionTypeId).Create();

        private static Filler<Decision> CreateRandomDecisionFiller(Patient patient, Guid decisionTypeId)
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(decision => decision.Patient).Use(patient)
                .OnProperty(decision => decision.PatientId).Use(patient.Id)
                .OnProperty(decision => decision.DecisionTypeId).Use(decisionTypeId)
                .OnProperty(decision => decision.DecisionChoice).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(decision => decision.CreatedDate).Use(now)
                .OnProperty(decision => decision.CreatedBy).Use(user)
                .OnProperty(decision => decision.UpdatedDate).Use(now)
                .OnProperty(decision => decision.UpdatedBy).Use(user);

            return filler;
        }

        private async ValueTask<Patient> PostRandomPatientAsync()
        {
            Patient randomPatient = CreateRandomPatient();
            Patient createdPatient = await this.apiBroker.PostPatientAsync(randomPatient);

            return createdPatient;
        }

        private static Patient CreateRandomPatient() =>
            CreateRandomPatientFiller().Create();

        private static Filler<Patient> CreateRandomPatientFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Patient>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(patient => patient.NhsNumber).Use(GenerateRandom10DigitNumber())
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

        private async ValueTask<DecisionType> PostRandomDecisionTypeAsync()
        {
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType createdDecisionType = await this.apiBroker.PostDecisionTypeAsync(randomDecisionType);

            return createdDecisionType;
        }

        private static DecisionType CreateRandomDecisionType() =>
            CreateRandomDecisionTypeFiller().Create();

        private static Filler<DecisionType> CreateRandomDecisionTypeFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<DecisionType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(decisionType => decisionType.Name).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(decisionType => decisionType.CreatedDate).Use(now)
                .OnProperty(decisionType => decisionType.CreatedBy).Use(user)
                .OnProperty(decisionType => decisionType.UpdatedDate).Use(now)
                .OnProperty(decisionType => decisionType.UpdatedBy).Use(user);

            return filler;
        }
    }
}
