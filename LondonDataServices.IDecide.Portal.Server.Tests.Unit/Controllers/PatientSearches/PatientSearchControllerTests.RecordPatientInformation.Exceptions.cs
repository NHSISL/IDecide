// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
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
        public async Task ShouldReturnBadRequestOnRecordPatientInformationIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            RecordPatientInformationRequest randomRecordPatientInformationRequest =
                GetRecordPatientInformationRequest();

            RecordPatientInformationRequest inputRecordPatientInformationRequest =
                randomRecordPatientInformationRequest;

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode))
                        .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.patientSearchController.RecordPatientInformationAsync(inputRecordPatientInformationRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode),
                        Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnRecordPatientInformationIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            RecordPatientInformationRequest randomRecordPatientInformationRequest =
                GetRecordPatientInformationRequest();

            RecordPatientInformationRequest inputRecordPatientInformationRequest =
                randomRecordPatientInformationRequest;

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult = expectedInternalServerErrorObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode))
                    .ThrowsAsync(serverException);

            // when
            ActionResult actualActionResult =
                await this.patientSearchController.RecordPatientInformationAsync(inputRecordPatientInformationRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode),
                    Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorOnExternalOptOutPatientIfServerErrorOccurredAsync()
        {
            // given
            RecordPatientInformationRequest randomRecordPatientInformationRequest =
                GetRecordPatientInformationRequest();

            RecordPatientInformationRequest inputRecordPatientInformationRequest =
                randomRecordPatientInformationRequest;

            PatientOrchestrationServiceException inputPatientOrchestrationServiceException =
                new PatientOrchestrationServiceException(
                    message: "Service error occurred, please contact support.",
                    innerException:
                        new ExternalOptOutPatientOrchestrationException("The patient is marked as sensitive."));

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(inputPatientOrchestrationServiceException);

            var expectedActionResult = expectedInternalServerErrorObjectResult;

            this.patientOrchestrationServiceMock.Setup(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode))
                    .ThrowsAsync(inputPatientOrchestrationServiceException);

            // when
            ActionResult actualActionResult =
                await this.patientSearchController.RecordPatientInformationAsync(inputRecordPatientInformationRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    inputRecordPatientInformationRequest.NhsNumber,
                    inputRecordPatientInformationRequest.NotificationPreference,
                    inputRecordPatientInformationRequest.GenerateNewCode),
                    Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
