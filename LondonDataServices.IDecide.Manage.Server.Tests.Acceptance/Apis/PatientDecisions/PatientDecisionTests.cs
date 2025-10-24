// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientDecisions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientDecisionTests
    {
        private readonly ApiBroker apiBroker;

        public PatientDecisionTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static string GetRandomEmailAddress() =>
            new EmailAddresses().GetValue();

        private static string GetRandomLocalMobileNumber()
        {
            Random random = new Random();
            var randomNumberEnd = random.Next(100000000, 200000000).ToString();
            string randomNumber = $"07{randomNumberEnd}";

            return randomNumber;
        }

        private async ValueTask<List<Decision>> PostRandomDecisionsAsync(
            Patient patient,
            DecisionType decisionType)
        {
            List<Decision> randomDecisions = CreateRandomDecisions(patient, decisionType);
            List<Decision> createdDecisions = new List<Decision>();

            foreach (Decision decision in randomDecisions)
            {
                Decision createdDecision = await apiBroker.PostDecisionAsync(decision);
                createdDecisions.Add(createdDecision);
            }

            return createdDecisions;
        }

        private static List<Decision> CreateRandomDecisions(Patient patient, DecisionType decisionType)
        {
            return CreateRandomDecisionFiller(patient, decisionType)
                .Create(count: GetRandomNumber())
                .ToList();
        }

        private static Decision CreateRandomDecision(Patient patient, DecisionType decisionType) =>
            CreateRandomDecisionFiller(patient, decisionType).Create();

        private static Filler<Decision> CreateRandomDecisionFiller(Patient patient, DecisionType decisionType)
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(decision => decision.Patient).Use(patient)
                .OnProperty(decision => decision.PatientId).Use(patient.Id)
                .OnProperty(decision => decision.DecisionType).Use(decisionType)
                .OnProperty(decision => decision.DecisionTypeId).Use(decisionType.Id)
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
                .OnProperty(patient => patient.Email).Use(GetRandomEmailAddress())
                .OnProperty(patient => patient.Phone).Use(GetRandomLocalMobileNumber())
                .OnProperty(patient => patient.PostCode).Use("E15 1DA")
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

        private static Consumer CreateRandomConsumerWithMatchingEntraIdEntry(string userId)
        {
            Consumer consumer = CreateConsumerFiller().Create();
            consumer.EntraId = userId;

            return consumer;
        }

        private async ValueTask<Consumer> PostRandomConsumerWithMatchingEntraIdEntryAsync(string userId)
        {
            Consumer randomConsumer = CreateRandomConsumerWithMatchingEntraIdEntry(userId);
            Consumer createdConsumer = await this.apiBroker.PostConsumerAsync(randomConsumer);

            return createdConsumer;
        }

        private static Filler<Consumer> CreateConsumerFiller()
        {
            string userId = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(consumer => consumer.EntraId).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.CreatedBy).Use(userId)
                .OnProperty(consumer => consumer.UpdatedBy).Use(userId);

            return filler;
        }
    }
}
