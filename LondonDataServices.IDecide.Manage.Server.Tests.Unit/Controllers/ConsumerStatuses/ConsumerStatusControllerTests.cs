// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers;
using LondonDataServices.IDecide.Manage.Server.Controllers.ConsumerStatuses;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerStatuses
{
    public partial class ConsumerStatusControllerTests : RESTFulController
    {
        private readonly Mock<IConsumerOrchestrationService> consumerOrchestrationServiceMock;
        private readonly ConsumerStatusController consumerStatusController;

        public ConsumerStatusControllerTests()
        {
            this.consumerOrchestrationServiceMock = new Mock<IConsumerOrchestrationService>();

            this.consumerStatusController =
                new ConsumerStatusController(this.consumerOrchestrationServiceMock.Object);
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

        private static List<Decision> GetRandomDecisions()
        {
            return CreateDecisionFiller()
                .Create(count: GetRandomNumber())
                .ToList();
        }

        private static Filler<Decision> CreateDecisionFiller()
        {
            string userId = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<Decision>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(decision => decision.DecisionChoice).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(decision => decision.CreatedBy).Use(userId)
                .OnProperty(decision => decision.UpdatedBy).Use(userId)
                .OnProperty(decision => decision.DecisionType).IgnoreIt()
                .OnProperty(decision => decision.ConsumerAdoptions).IgnoreIt();

            return filler;
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string randomMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ConsumerOrchestrationValidationException(
                    message: randomMessage,
                    innerException: someInnerException),

                new ConsumerOrchestrationDependencyValidationException(
                    message: randomMessage,
                    innerException: someInnerException)
            };
        }
    }
}
