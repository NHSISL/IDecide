// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Services.DecisionTypes;
using Moq;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        private readonly Mock<IStorageBroker> storageBroker;
        private readonly DecisionTypeService decisionTypeService;

        public DecisionTypeServiceTests()
        {
            this.storageBroker = new Mock<IStorageBroker>();
            this.decisionTypeService = new DecisionTypeService(this.storageBroker.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();


        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static DecisionType CreateRandomDecisionType() =>
            CreateRandomDecisionTypeFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Filler<DecisionType> CreateRandomDecisionTypeFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<DecisionType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.UpdatedBy).Use(user);

            return filler;
        }
    }
}
