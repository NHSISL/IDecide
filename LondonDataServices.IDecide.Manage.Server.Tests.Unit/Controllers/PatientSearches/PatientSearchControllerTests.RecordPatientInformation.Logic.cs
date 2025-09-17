// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnRecordPatientInformationAsync()
        {
            // given
            RecordPatientInformationRequest randomRecordPatientInformationRequest =
                GetRecordPatientInformationRequest();

            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await this.patientSearchController
                .RecordPatientInformationAsync(randomRecordPatientInformationRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.RecordPatientInformationAsync(
                    randomRecordPatientInformationRequest.NhsNumber,
                    randomRecordPatientInformationRequest.NotificationPreference,
                    randomRecordPatientInformationRequest.GenerateNewCode),
                   Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
