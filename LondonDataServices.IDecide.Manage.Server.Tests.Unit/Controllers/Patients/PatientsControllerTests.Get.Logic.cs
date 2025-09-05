// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Guid inputId = randomPatient.Id;
            Patient storagePatient = randomPatient;
            Patient expectedPatient = storagePatient.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<Patient>(expectedObjectResult);

            patientServiceMock
                .Setup(service => service.RetrievePatientByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storagePatient);

            // when
            ActionResult<Patient> actualActionResult = await patientsController.GetPatientByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            patientServiceMock
                .Verify(service => service.RetrievePatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
