// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests : RESTFulController
    {

        private readonly Mock<IConsumerAdoptionService> consumerAdoptionServiceMock;
        private readonly ConsumerAdoptionsController consumerAdoptionsController;

        public ConsumerAdoptionsControllerTests()
        {
            consumerAdoptionServiceMock = new Mock<IConsumerAdoptionService>();
            consumerAdoptionsController = new ConsumerAdoptionsController(consumerAdoptionServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ConsumerAdoptionValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConsumerAdoptionDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ConsumerAdoptionDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConsumerAdoptionServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ConsumerAdoption CreateRandomConsumerAdoption() =>
            CreateConsumerAdoptionFiller().Create();

        private static IQueryable<ConsumerAdoption> CreateRandomConsumerAdoptions()
        {
            return CreateConsumerAdoptionFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<ConsumerAdoption> CreateConsumerAdoptionFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ConsumerAdoption>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                // TODO:  Add your property configurations here

                .OnProperty(consumerAdoption => consumerAdoption.CreatedBy).Use(user)
                .OnProperty(consumerAdoption => consumerAdoption.UpdatedBy).Use(user);

            return filler;
        }
    }
}