// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<Patient> somePatients = CreateRandomPatients();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Patient>>(expectedInternalServerErrorObjectResult);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Patient>> actualActionResult =
                await this.patientsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
