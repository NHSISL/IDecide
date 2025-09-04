// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostPatientGenerationRequestAsync()
        {
            // given
            GenerateCodeRequest randomGenerateCodeRequest = GetRandomGenerateCodeRequest();
            GenerateCodeRequest inputGenerateCodeRequest = randomGenerateCodeRequest.DeepClone();
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await this.patientSearchController
                .PostPatientGenerationRequestAsync(inputGenerateCodeRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    inputGenerateCodeRequest.NhsNumber,
                    inputGenerateCodeRequest.NotificationPreference,
                    inputGenerateCodeRequest.GenerateNewCode),
                        Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
