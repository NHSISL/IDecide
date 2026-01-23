using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public async Task ShouldReturnUserInfoOnGetPatientPOWInfoAsync()
        {
            // given
            NhsLoginUserInfo loginUserInfo = CreateRandomNhsLoginUserInfo();
            NhsLoginUserInfo expectedLoginUserInfo = loginUserInfo.DeepClone();

            nhsLoginServiceMock
                .Setup(service => service.NhsLoginAsync())
                .ReturnsAsync(expectedLoginUserInfo);

            var patientsController = new PatientsController(
                this.patientServiceMock.Object,
                this.nhsLoginServiceMock.Object,
                this.patientOrchestrationServiceMock.Object,
                this.configurationMock.Object);

            // when
            ActionResult<NhsLoginUserInfo> actualResult =
                await patientsController.GetPatientInfo();

            // then
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            var actualUserInfo = Assert.IsType<NhsLoginUserInfo>(okResult.Value);

            Assert.Equal(expectedLoginUserInfo.Birthdate, actualUserInfo.Birthdate);
            Assert.Equal(expectedLoginUserInfo.Email, actualUserInfo.Email);
            Assert.Equal(expectedLoginUserInfo.FamilyName, actualUserInfo.FamilyName);
            Assert.Equal(expectedLoginUserInfo.GivenName, actualUserInfo.GivenName);
            Assert.Equal(expectedLoginUserInfo.PhoneNumber, actualUserInfo.PhoneNumber);

            this.nhsLoginServiceMock.Verify(
                service => service.NhsLoginAsync(),
                Times.Once);

            this.nhsLoginServiceMock.VerifyNoOtherCalls();
        }
    }
}