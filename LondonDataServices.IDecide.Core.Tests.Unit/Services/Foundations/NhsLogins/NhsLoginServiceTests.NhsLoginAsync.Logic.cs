// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using Moq;
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsLogins
{
    public partial class NhsLoginServiceTests
    {
        [Fact]
        public async Task ShouldCallNhsLoginAsyncAndReturnUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            NhsLoginUserInfo randomUserInfo = new NhsLoginUserInfo
            {
                Birthdate = GetRandomDateTimeOffset().Date,
                FamilyName = GetRandomString(),
                Email = GetRandomString(),
                PhoneNumber = GetRandomString(),
                GivenName = GetRandomString()
            };

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
                    .ReturnsAsync(randomAccessToken);

            this.securityBrokerMock.Setup(broker =>
                broker.GetNhsLoginUserInfoAsync(randomAccessToken))
                    .ReturnsAsync(randomUserInfo);

            // when
            NhsLoginUserInfo actualUserInfo =
                await this.nhsLoginService.NhsLoginAsync();

            // then
            actualUserInfo.Should().BeEquivalentTo(randomUserInfo);

            this.securityBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetNhsLoginUserInfoAsync(randomAccessToken),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}