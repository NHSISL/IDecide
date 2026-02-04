// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Decisions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class DecisionApiTests
    {
        private readonly ApiBroker apiBroker;

        public DecisionApiTests(ApiBroker apiBroker) =>
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

        private static Decision UpdateDecisionWithRandomValues(Decision inputDecision)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var updatedDecision = CreateRandomDecision(
                patientId: inputDecision.PatientId,
                decisionTypeId: inputDecision.DecisionTypeId);

            updatedDecision.Id = inputDecision.Id;
            updatedDecision.CreatedDate = inputDecision.CreatedDate;
            updatedDecision.CreatedBy = inputDecision.CreatedBy;
            updatedDecision.UpdatedDate = now;

            return updatedDecision;
        }

        private async ValueTask<Decision> PostRandomDecisionAsync(Guid patientId, Guid decisionTypeId)
        {
            Decision randomDecision = CreateRandomDecision(patientId, decisionTypeId);
            Decision createdDecision = await this.apiBroker.PostDecisionAsync(randomDecision);

            return createdDecision;
        }

        private async ValueTask<List<Decision>> PostRandomDecisionsAsync(Guid patientId, Guid decisionTypeId)
        {
            int randomNumber = GetRandomNumber();
            var randomDecisions = new List<Decision>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomDecisions.Add(await PostRandomDecisionAsync(patientId, decisionTypeId));
            }

            return randomDecisions;
        }

        private static Decision CreateRandomDecision(Guid patientId, Guid decisionTypeId) =>
            CreateRandomDecisionFiller(patientId, decisionTypeId).Create();

        private static Filler<Decision> CreateRandomDecisionFiller(Guid patientId, Guid decisionTypeId)
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(decision => decision.PatientId).Use(patientId)
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
                .OnProperty(patient => patient.NhsNumber).Use(GetRandomStringWithLengthOf(10))
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