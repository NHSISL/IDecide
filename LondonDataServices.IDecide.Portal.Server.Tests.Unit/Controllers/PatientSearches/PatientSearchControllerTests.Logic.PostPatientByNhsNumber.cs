// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostByNhsNumberAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(randomNhsNumber);
            Patient outputPatient = randomPatient.DeepClone();
            Patient expectedPatient = outputPatient.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<Patient>(expectedObjectResult);

            this.patientOrchestrationServiceMock.Setup(service => 
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            // when
            ActionResult<Patient> actualActionResult = await this.patientSearchController
                .PostPatientByNhsNumberAsync(inputNhsNumber);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service => 
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                   Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
