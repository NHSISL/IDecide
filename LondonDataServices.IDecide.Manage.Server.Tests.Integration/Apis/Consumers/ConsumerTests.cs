// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Consumers
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ConsumerApiTests
    {
        private readonly ApiBroker apiBroker;

        public ConsumerApiTests(ApiBroker apiBroker) =>
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
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(consumer => consumer.CreatedDate).Use(now)
                .OnProperty(consumer => consumer.CreatedBy).Use(user)
                .OnProperty(consumer => consumer.UpdatedDate).Use(now)
                .OnProperty(consumer => consumer.UpdatedBy).Use(user);

            return filler;
        }

        private static Consumer UpdateConsumerWithRandomValues(Consumer inputConsumer)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedConsumer = CreateRandomConsumer();
            updatedConsumer.Id = inputConsumer.Id;
            updatedConsumer.CreatedDate = inputConsumer.CreatedDate;
            updatedConsumer.CreatedBy = inputConsumer.CreatedBy;
            updatedConsumer.UpdatedDate = now;

            return updatedConsumer;
        }

        private async ValueTask<Consumer> PostRandomConsumerAsync()
        {
            Consumer randomConsumer = CreateRandomConsumer();
            return await this.apiBroker.PostConsumerAsync(randomConsumer);
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
    }
}
