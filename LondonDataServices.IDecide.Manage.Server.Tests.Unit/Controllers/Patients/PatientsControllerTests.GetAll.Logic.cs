// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<Patient> randomPatients = CreateRandomPatients();
            IQueryable<Patient> storagePatients = randomPatients.DeepClone();
            IQueryable<Patient> expectedPatient = storagePatients.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPatient);

            var expectedActionResult =
                new ActionResult<IQueryable<Patient>>(expectedObjectResult);

            patientServiceMock
                .Setup(service => service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(storagePatients);

            // when
            ActionResult<IQueryable<Patient>> actualActionResult = await patientsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            patientServiceMock
               .Verify(service => service.RetrieveAllPatientsAsync(),
                   Times.Once);

            patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
