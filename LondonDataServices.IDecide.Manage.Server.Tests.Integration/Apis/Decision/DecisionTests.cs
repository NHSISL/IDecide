// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class DecisionApiTests
    {
        private readonly ApiBroker apiBroker;

        public DecisionApiTests(ApiBroker apiBroker) =>
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

        private static Decision CreateRandomDecision() =>
            CreateRandomDecisionFiller().Create();

        private static Filler<Decision> CreateRandomDecisionFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(decision => decision.DecisionChoice).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(decision => decision.CreatedDate).Use(now)
                .OnProperty(decision => decision.CreatedBy).Use(user)
                .OnProperty(decision => decision.UpdatedDate).Use(now)
                .OnProperty(decision => decision.UpdatedBy).Use(user);

            return filler;
        }

        private static Decision UpdateDecisionWithRandomValues(Decision inputDecision)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedDecision = CreateRandomDecision();
            updatedDecision.Id = inputDecision.Id;
            updatedDecision.CreatedDate = inputDecision.CreatedDate;
            updatedDecision.CreatedBy = inputDecision.CreatedBy;
            updatedDecision.UpdatedDate = now;

            return updatedDecision;
        }

        private async ValueTask<Decision> PostRandomDecisionAsync()
        {
            Decision randomDecision = CreateRandomDecision();
            return await this.apiBroker.PostDecisionAsync(randomDecision);
        }

        private async ValueTask<List<Decision>> PostRandomDecisionsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomDecisions = new List<Decision>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomDecisions.Add(await PostRandomDecisionAsync());
            }

            return randomDecisions;
        }
    }
}
