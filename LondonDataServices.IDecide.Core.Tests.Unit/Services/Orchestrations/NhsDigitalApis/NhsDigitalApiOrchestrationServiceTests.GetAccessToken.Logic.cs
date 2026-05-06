// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldGetAccessTokenAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string randomResult = GetRandomString();
            string returnedResult = randomResult.DeepClone();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetAccessTokenAsync(inputCancellationToken))
                    .ReturnsAsync(returnedResult);

            // when
            string actualResult =
                await this.nhsDigitalApiOrchestrationService
                    .GetAccessTokenAsync(inputCancellationToken);

            // then
            Assert.Equal(returnedResult, actualResult);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetAccessTokenAsync(inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
