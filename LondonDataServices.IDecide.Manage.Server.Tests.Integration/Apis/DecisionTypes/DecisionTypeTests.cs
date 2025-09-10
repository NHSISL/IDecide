// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.DecisionTypes
{
    [Collection(nameof(ApiTestCollection))]
    public partial class DecisionTypeApiTests
    {
        private readonly ApiBroker apiBroker;

        public DecisionTypeApiTests(ApiBroker apiBroker) =>
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

        private static DecisionType CreateRandomDecisionType() =>
            CreateRandomDecisionTypeFiller().Create();

        private static Filler<DecisionType> CreateRandomDecisionTypeFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset now = DateTime.UtcNow;
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

        private static DecisionType UpdateDecisionTypeWithRandomValues(DecisionType inputDecisionType)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedDecisionType = CreateRandomDecisionType();
            updatedDecisionType.Id = inputDecisionType.Id;
            updatedDecisionType.CreatedDate = inputDecisionType.CreatedDate;
            updatedDecisionType.CreatedBy = inputDecisionType.CreatedBy;
            updatedDecisionType.UpdatedDate = now;

            return updatedDecisionType;
        }

        private async ValueTask<DecisionType> PostRandomDecisionTypeAsync()
        {
            DecisionType randomDecisionType = CreateRandomDecisionType();
            return await this.apiBroker.PostDecisionTypeAsync(randomDecisionType);
        }

        private async ValueTask<List<DecisionType>> PostRandomDecisionTypesAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomDecisionTypes = new List<DecisionType>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomDecisionTypes.Add(await PostRandomDecisionTypeAsync());
            }

            return randomDecisionTypes;
        }
    }
}
