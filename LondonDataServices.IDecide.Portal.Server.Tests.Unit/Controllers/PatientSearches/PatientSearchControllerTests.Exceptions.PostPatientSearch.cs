// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            string randomString = GetRandomString();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(randomString);
            PatientLookup inputPatientLookup = randomPatientLookup;

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedBadRequestObjectResult);

            this.patientOrchestrationServiceMock.Setup(service =>
                service.PatientLookupAsync(inputPatientLookup))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientSearchController.PostPatientSearchAsync(inputPatientLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.PatientLookupAsync(inputPatientLookup),
                    Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            string randomString = GetRandomString();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(randomString);
            PatientLookup inputPatientLookup = randomPatientLookup;

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedInternalServerErrorObjectResult);

            this.patientOrchestrationServiceMock.Setup(service =>
                service.PatientLookupAsync(inputPatientLookup))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientSearchController.PostPatientSearchAsync(inputPatientLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service =>
                service.PatientLookupAsync(inputPatientLookup),
                    Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
