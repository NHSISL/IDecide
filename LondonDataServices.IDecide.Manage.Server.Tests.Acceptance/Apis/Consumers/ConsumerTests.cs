// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Consumers
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ConsumerApiTests
    {
        private readonly ApiBroker apiBroker;

        public ConsumerApiTests(ApiBroker apiBroker) =>
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

        private static Consumer UpdateConsumerWithRandomValues(Consumer inputConsumer)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedConsumer = CreateRandomConsumer();
            updatedConsumer.Id = inputConsumer.Id;
            updatedConsumer.EntraId = inputConsumer.EntraId;
            updatedConsumer.CreatedDate = inputConsumer.CreatedDate;
            updatedConsumer.CreatedBy = inputConsumer.CreatedBy;
            updatedConsumer.UpdatedDate = now;

            return updatedConsumer;
        }

        private async ValueTask<Consumer> PostRandomConsumerAsync()
        {
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer createdConsumer = await this.apiBroker.PostConsumerAsync(randomConsumer);

            return createdConsumer;
        }

        private async ValueTask<List<Consumer>> PostRandomConsumersAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomConsumers = new List<Consumer>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomConsumers.Add(await PostRandomConsumerAsync());
            }

            return randomConsumers;
        }

        private static Consumer CreateRandomConsumer() =>
            CreateRandomConsumerFiller().Create();

        private static Filler<Consumer> CreateRandomConsumerFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(consumer => consumer.EntraId).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.CreatedDate).Use(now)
                .OnProperty(consumer => consumer.CreatedBy).Use(user)
                .OnProperty(consumer => consumer.UpdatedDate).Use(now)
                .OnProperty(consumer => consumer.UpdatedBy).Use(user);

            return filler;
        }
    }
}