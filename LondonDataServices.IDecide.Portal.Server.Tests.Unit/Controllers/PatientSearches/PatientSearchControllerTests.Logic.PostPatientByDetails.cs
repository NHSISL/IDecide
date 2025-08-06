// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostAsync()
        {
            // given
            string randomString = GetRandomString();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(randomString);
            PatientLookup inputPatientLookup = randomPatientLookup;
            PatientLookup updatedPatientLookup = inputPatientLookup.DeepClone();
            Patient randomPatient = GetRandomPatient(randomString);
            Patient outputPatient = randomPatient.DeepClone();
            Patient expectedPatient = outputPatient.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<Patient>(expectedObjectResult);

            this.patientOrchestrationServiceMock.Setup(service => 
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatient);

            // when
            ActionResult<Patient> actualActionResult = await this.patientSearchController
                .PostPatientByDetailsAsync(randomPatientLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientOrchestrationServiceMock.Verify(service => 
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                   Times.Once);

            this.patientOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
