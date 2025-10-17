// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientCodes
{
    public partial class PatientCodeControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnVerifyPatientCodeIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            PatientCodeRequest randomPatientCodeRequest = GetRandomPatientCodeRequest();
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();
            BadRequestObjectResult expectedBadRequestObjectResult = BadRequest(validationException.InnerException);
            var expectedActionResult = expectedBadRequestObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.VerifyPatientCodeAsync(
                    inputPatientCodeRequest.NhsNumber,
                    inputPatientCodeRequest.VerificationCode))
                        .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.patientCodeController.VerifyPatientCodeAsync(inputPatientCodeRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.VerifyPatientCodeAsync(
                    inputPatientCodeRequest.NhsNumber,
                    inputPatientCodeRequest.VerificationCode),
                    Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnVerifyPatientCodeIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            PatientCodeRequest randomPatientCodeRequest = GetRandomPatientCodeRequest();
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();
            InternalServerErrorObjectResult expectedBadRequestObjectResult = InternalServerError(serverException);
            var expectedActionResult = expectedBadRequestObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.VerifyPatientCodeAsync(
                    inputPatientCodeRequest.NhsNumber,
                    inputPatientCodeRequest.VerificationCode))
                        .ThrowsAsync(serverException);

            // when
            ActionResult actualActionResult =
                await this.patientCodeController.VerifyPatientCodeAsync(inputPatientCodeRequest);

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
