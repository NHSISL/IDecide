// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.DecisionTypes
{
    [Collection(nameof(ApiTestCollection))]
    public partial class DecisionTypeApiTests
    {
        private readonly ApiBroker apiBroker;

        public DecisionTypeApiTests(ApiBroker apiBroker) =>
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
            DecisionType createdDecisionType = await this.apiBroker.PostDecisionTypeAsync(randomDecisionType);

            return createdDecisionType;
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