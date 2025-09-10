// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
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
            ActionResult<Patient> actualActionResult = await this.patientSearchController
                .RecordPatientInformationAsync(randomRecordPatientInformationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

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
