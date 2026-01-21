using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public async Task ShouldReturnUserInfoOnGetPatientInfoAsync()
        {
            // given
            string accessToken = "valid-access-token";
            NhsLoginUserInfo loginUserInfo = CreateRandomNhsLoginUserInfo();
            NhsLoginUserInfo expectedLoginUserInfo = loginUserInfo.DeepClone();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";

            // Set up authentication result with token
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var authProperties = new AuthenticationProperties();
            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken
                {
                    Name = "access_token",
                    Value = accessToken
                }
            });
            var authTicket = new AuthenticationTicket(claimsPrincipal, authProperties, "TestScheme");
            var authenticateResult = AuthenticateResult.Success(authTicket);

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(x => x.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>()))
                .ReturnsAsync(authenticateResult);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expectedLoginUserInfo)
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);

            // Mock configuration to return an absolute URI for NHSLoginOIDC:authority
            var configurationMock = new Mock<IConfiguration>();
            configurationMock
                .Setup(x => x["NHSLoginOIDC:authority"])
                .Returns("https://mock-authority");

            // Mock other required dependencies
            var patientServiceMock = new Mock<IPatientService>();
            var patientOrchestrationServiceMock = new Mock<IPatientOrchestrationService>();

            var patientsController = new PatientsController(
                patientServiceMock.Object,
                patientOrchestrationServiceMock.Object,
                configurationMock.Object,
                httpClient);

            patientsController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // when
            IActionResult actualResult = await patientsController.GetPatientInfo();

            // then
            var jsonResult = Assert.IsType<JsonResult>(actualResult);
            var actualUserInfo = Assert.IsType<NhsLoginUserInfo>(jsonResult.Value);

            Assert.Equal(expectedLoginUserInfo.Birthdate, actualUserInfo.Birthdate);
            Assert.Equal(expectedLoginUserInfo.Email, actualUserInfo.Email);
            Assert.Equal(expectedLoginUserInfo.FamilyName, actualUserInfo.FamilyName);
            Assert.Equal(expectedLoginUserInfo.GivenName, actualUserInfo.GivenName);
            Assert.Equal(expectedLoginUserInfo.PhoneNumber, actualUserInfo.PhoneNumber);

            handlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            handlerMock.VerifyNoOtherCalls();
        }
    }
}