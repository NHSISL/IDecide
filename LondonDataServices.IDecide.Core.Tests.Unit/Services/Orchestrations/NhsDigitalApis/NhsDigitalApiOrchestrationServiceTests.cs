// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Threading;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly Mock<INhsDigitalApiService> nhsDigitalApiServiceMock = new Mock<INhsDigitalApiService>();
        private readonly INhsDigitalApiOrchestrationService nhsDigitalApiOrchestrationService;

        public NhsDigitalApiOrchestrationServiceTests()
        {
            this.nhsDigitalApiOrchestrationService = new NhsDigitalApiOrchestrationService(
                loggingBroker: this.loggingBrokerMock.Object,
                nhsDigitalApiService: this.nhsDigitalApiServiceMock.Object);
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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
