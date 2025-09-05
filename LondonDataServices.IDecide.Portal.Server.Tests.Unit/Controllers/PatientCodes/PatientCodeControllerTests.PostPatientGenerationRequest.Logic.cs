// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientCodes
{
    public partial class PatientCodeControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostPatientGenerationRequestAsync()
        {
            // given
            PatientCodeRequest randomPatientCodeRequest = GetRandomPatientCodeRequest();
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await this.patientCodeController
                .PostPatientGenerationRequestAsync(inputPatientCodeRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    inputPatientCodeRequest.NhsNumber,
                    inputPatientCodeRequest.NotificationPreference,
                    inputPatientCodeRequest.GenerateNewCode),
                        Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
