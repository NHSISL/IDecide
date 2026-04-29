// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using KellermanSoftware.CompareNetObjects;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.Users;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        private readonly Mock<INhsDigitalApiService> nhsDigitalApiServiceMock;
        private readonly Mock<IUserService> userServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly NhsDigitalApiOrchestrationService nhsDigitalApiOrchestrationService;
        private readonly ICompareLogic compareLogic;

        public NhsDigitalApiOrchestrationServiceTests()
        {
            this.nhsDigitalApiServiceMock = new Mock<INhsDigitalApiService>();
            this.userServiceMock = new Mock<IUserService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.nhsDigitalApiOrchestrationService = new NhsDigitalApiOrchestrationService(
                nhsDigitalApiService: this.nhsDigitalApiServiceMock.Object,
                userService: this.userServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static CancellationToken GetCancellationToken() =>
            new CancellationTokenSource().Token;

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string CreateUserInfoJson(string nhsIdUserUid, string name, string sub) =>
            $"{{\"NhsIdUserUid\":\"{nhsIdUserUid}\",\"Name\":\"{name}\",\"Sub\":\"{sub}\"}}";

        private static Filler<User> CreateUserFiller(bool isAuthorised = true)
        {
            string userId = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnType<DateTime>().Use(dateTimeOffset.UtcDateTime)
                .OnType<DateTime?>().Use(dateTimeOffset.UtcDateTime)
                .OnProperty(user => user.IsAuthorised).Use(isAuthorised)
                .OnProperty(user => user.CreatedBy).Use(userId)
                .OnProperty(user => user.UpdatedBy).Use(userId);

            return filler;
        }

        private static User CreateRandomUser(bool isAuthorised = true) =>
            CreateUserFiller(isAuthorised).Create();

        private Expression<Func<User, bool>> SameUserAs(User expectedUser) =>
            actualUser => this.compareLogic.Compare(expectedUser, actualUser).AreEqual;

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
