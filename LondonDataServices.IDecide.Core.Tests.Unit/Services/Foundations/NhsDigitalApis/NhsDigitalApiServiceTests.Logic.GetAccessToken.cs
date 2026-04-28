// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task ShouldGetAccessTokenAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string randomAccessToken = GetRandomString();
            string expectedAccessToken = randomAccessToken.DeepClone();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken))
                    .ReturnsAsync(randomAccessToken);

            // when
            string actualAccessToken =
                await this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            // then
            actualAccessToken.Should().BeEquivalentTo(expectedAccessToken);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
