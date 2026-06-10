// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldReturnRedirectOnLogoutAsync()
        {
            // given
            string expectedUrl = "/";
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
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // when
            IActionResult actualResult =
                await this.authController.Logout(CancellationToken.None);

            // then
            var redirectResult = actualResult as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult!.Url.Should().Be(expectedUrl);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
