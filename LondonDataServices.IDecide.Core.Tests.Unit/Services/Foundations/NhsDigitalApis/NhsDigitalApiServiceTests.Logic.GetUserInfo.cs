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
        public async Task ShouldGetUserInfoAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();
            string randomUserInfo = GetRandomString();
            string expectedUserInfo = randomUserInfo.DeepClone();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ReturnsAsync(randomUserInfo);

            // when
            string actualUserInfo =
                await this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    inputState,
                    inputCancellationToken);

            // then
            actualUserInfo.Should().BeEquivalentTo(expectedUserInfo);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
