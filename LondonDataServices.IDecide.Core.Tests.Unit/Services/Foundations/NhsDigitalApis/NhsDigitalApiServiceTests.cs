// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        private readonly Mock<INhsDigitalApiBroker> nhsDigitalApiBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly NhsDigitalApiService nhsDigitalApiService;

        public NhsDigitalApiServiceTests()
        {
            this.nhsDigitalApiBrokerMock = new Mock<INhsDigitalApiBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.nhsDigitalApiService = new NhsDigitalApiService(
                nhsDigitalApiBroker: this.nhsDigitalApiBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static CancellationToken GetCancellationToken() =>
            CancellationToken.None;

        private static SearchCriteria CreateRandomSearchCriteria() =>
            new SearchCriteria
            {
                Surname = GetRandomString(),
                FirstName = GetRandomString(),
                DateOfBirth = GetRandomString(),
                Postcode = GetRandomString()
            };
    }
}
