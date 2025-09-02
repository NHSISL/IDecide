// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests : RESTFulController
    {
        private readonly Mock<IDecisionTypeService> decisionTypeServiceMock;
        private readonly DecisionTypesController decisionTypesController;

        public DecisionTypesControllerTests()
        {
            decisionTypeServiceMock = new Mock<IDecisionTypeService>();
            decisionTypesController = new DecisionTypesController(decisionTypeServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new DecisionTypeValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new DecisionTypeDependencyValidationException(
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
                new DecisionTypeDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new DecisionTypeServiceException(
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

        private static DecisionType CreateRandomDecisionType() =>
            CreateDecisionTypeFiller().Create();

        private static IQueryable<DecisionType> CreateRandomDecisionTypes()
        {
            return CreateDecisionTypeFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<DecisionType> CreateDecisionTypeFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<DecisionType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(decisionType => decisionType.CreatedBy).Use(user)
                .OnProperty(decisionType => decisionType.UpdatedBy).Use(user);

            return filler;
        }
    }
}