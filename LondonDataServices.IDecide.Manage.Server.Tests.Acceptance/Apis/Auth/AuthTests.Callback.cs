// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Auth
{
    public partial class AuthTests
    {
        [Fact]
        public async Task ShouldRedirectToHomeOnCallbackWhenUserIsAuthorisedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            var authorisedUser = new User
            {
                Sub = GetRandomString(),
                Name = GetRandomString(),
                NhsIdUserUid = GetRandomString(),
                IsAuthorised = true
            };

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service =>
                    service.ProcessCallbackAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(authorisedUser);

            // when
            HttpResponseMessage response =
                await this.apiBroker.GetCallbackAsync(randomCode, randomState);

            // then
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Be("/home");
        }

        [Fact]
        public async Task ShouldRedirectToUnauthorisedOnCallbackWhenUserIsNotAuthorisedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            var unauthorisedUser = new User
            {
                Sub = GetRandomString(),
                Name = GetRandomString(),
                NhsIdUserUid = GetRandomString(),
                IsAuthorised = false
            };

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service =>
                    service.ProcessCallbackAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(unauthorisedUser);

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // when
            HttpResponseMessage response =
                await this.apiBroker.GetCallbackAsync(randomCode, randomState);

            // then
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Be("/unauthorised");
        }
    }
}
