// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient inputPatient = randomPatient;
            Patient storagePatient = inputPatient.DeepClone();
            Patient expectedPatient = storagePatient.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<Patient>(expectedObjectResult);

            patientServiceMock
                .Setup(service => service.ModifyPatientAsync(inputPatient))
                    .ReturnsAsync(storagePatient);

            // when
            ActionResult<Patient> actualActionResult = await patientsController.PutPatientAsync(randomPatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            patientServiceMock
               .Verify(service => service.ModifyPatientAsync(inputPatient),
                   Times.Once);

            patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
