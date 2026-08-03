// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldRedirectToHomeOnCallbackWhenUserIsAuthorisedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string expectedUrl = "/home";

            var authorisedUser = new User
            {
                Sub = GetRandomString(),
                Name = GetRandomString(),
                NhsIdUserUid = GetRandomString(),
                IsAuthorised = true
            };

            var mockSession = new Mock<ISession>();
            var mockAuthService = new Mock<IAuthenticationService>();

            mockAuthService
                .Setup(a => a.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service =>
                    service.ProcessCallbackAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(authorisedUser);

            // when
            IActionResult actualResult =
                await this.authController.Callback(randomCode, randomState, CancellationToken.None);

            // then
            var redirectResult = actualResult as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult!.Url.Should().Be(expectedUrl);

            mockAuthService.Verify(
                a => a.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "bff-cookie",
                    It.Is<ClaimsPrincipal>(p =>
                        p.FindFirstValue(ClaimTypes.NameIdentifier) == authorisedUser.Sub &&
                        p.FindFirstValue(ClaimTypes.Name) == authorisedUser.Name &&
                        p.FindFirstValue(ClaimTypes.Upn) == authorisedUser.NhsIdUserUid),
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.ProcessCallbackAsync(
                    randomCode,
                    randomState,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRedirectToUnauthorisedOnCallbackWhenUserIsNotAuthorisedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string expectedUrl = "/unauthorised";

            var unauthorisedUser = new User
            {
                Sub = GetRandomString(),
                Name = GetRandomString(),
                NhsIdUserUid = GetRandomString(),
                IsAuthorised = false
            };

            var mockSession = new Mock<ISession>();
            var mockAuthService = new Mock<IAuthenticationService>();

            mockAuthService
                .Setup(a => a.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service =>
                    service.ProcessCallbackAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(unauthorisedUser);

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // when
            IActionResult actualResult =
                await this.authController.Callback(randomCode, randomState, CancellationToken.None);

            // then
            var redirectResult = actualResult as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult!.Url.Should().Be(expectedUrl);

            mockAuthService.Verify(
                a => a.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    "bff-cookie",
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.ProcessCallbackAsync(
                    randomCode,
                    randomState,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
