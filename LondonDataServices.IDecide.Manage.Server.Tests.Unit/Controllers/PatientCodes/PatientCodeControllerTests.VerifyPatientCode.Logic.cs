// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientCodes
{
    public partial class PatientCodeControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnVerifyPatientCodeAsync()
        {
            // given
            PatientCodeRequest randomPatientCodeRequest = GetRandomPatientCodeRequest();
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await this.patientCodeController
                .VerifyPatientCodeAsync(inputPatientCodeRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.VerifyPatientCodeAsync(
                    inputPatientCodeRequest.NhsNumber,
                    inputPatientCodeRequest.VerificationCode),
                        Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
