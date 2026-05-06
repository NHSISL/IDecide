// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldReturnNoContentOnLogoutAsync()
        {
            // given
            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // when
            IActionResult actualResult =
                await this.authController.Logout(CancellationToken.None);

            // then
            actualResult.Should().BeOfType<NoContentResult>();

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
