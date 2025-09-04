// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPatientGenerationRequestIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            GenerateCodeRequest randomGenerateCodeRequest = GetRandomGenerateCodeRequest();
            GenerateCodeRequest inputGenerateCodeRequest = randomGenerateCodeRequest.DeepClone();

            BadRequestObjectResult expectedBadRequestObjectResult =
               BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.RecordPatientInformationAsync(
                    inputGenerateCodeRequest.NhsNumber,
                    inputGenerateCodeRequest.NotificationPreference,
                    inputGenerateCodeRequest.GenerateNewCode))
                        .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.patientSearchController.PostPatientGenerationRequestAsync(inputGenerateCodeRequest);

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

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPatientGenerationRequestIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            GenerateCodeRequest randomGenerateCodeRequest = GetRandomGenerateCodeRequest();
            GenerateCodeRequest inputGenerateCodeRequest = randomGenerateCodeRequest.DeepClone();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
               InternalServerError(serverException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.RecordPatientInformationAsync(
                    inputGenerateCodeRequest.NhsNumber,
                    inputGenerateCodeRequest.NotificationPreference,
                    inputGenerateCodeRequest.GenerateNewCode))
                        .ThrowsAsync(serverException);

            // when
            ActionResult actualActionResult =
                await this.patientSearchController.PostPatientGenerationRequestAsync(inputGenerateCodeRequest);

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
