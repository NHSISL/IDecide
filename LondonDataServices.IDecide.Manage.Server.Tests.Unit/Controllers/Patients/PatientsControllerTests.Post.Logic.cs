// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient inputPatient = randomPatient;
            Patient addedPatient = inputPatient.DeepClone();
            Patient expectedPatient = addedPatient.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<Patient>(expectedObjectResult);

            patientServiceMock
                .Setup(service => service.AddPatientAsync(inputPatient))
                    .ReturnsAsync(addedPatient);

            // when
            ActionResult<Patient> actualActionResult = await patientsController.PostPatientAsync(randomPatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            patientServiceMock
               .Verify(service => service.AddPatientAsync(inputPatient),
                   Times.Once);

            patientServiceMock.VerifyNoOtherCalls();
        }
    }
}