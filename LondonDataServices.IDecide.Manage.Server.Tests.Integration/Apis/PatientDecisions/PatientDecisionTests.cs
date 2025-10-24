// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.PatientDecisions
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
