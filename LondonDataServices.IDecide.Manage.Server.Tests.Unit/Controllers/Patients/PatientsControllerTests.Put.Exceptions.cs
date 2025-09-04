// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Patient somePatient = CreateRandomPatient();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedBadRequestObjectResult);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.PutPatientAsync(somePatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Patient somePatient = CreateRandomPatient();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedInternalServerErrorObjectResult);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.PutPatientAsync(somePatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Patient somePatient = CreateRandomPatient();
            string someMessage = GetRandomString();

            var notFoundPatientException =
                new NotFoundPatientException(
                    message: someMessage);

            var patientValidationException =
                new PatientValidationException(
                    message: someMessage,
                    innerException: notFoundPatientException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundPatientException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedNotFoundObjectResult);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(patientValidationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.PutPatientAsync(somePatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsPatientErrorOccurredAsync()
        {
            // given
            Patient somePatient = CreateRandomPatient();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsPatientException =
                new AlreadyExistsPatientException(
                    message: someMessage,
                    innerException: someInnerException);

            var patientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsPatientException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsPatientException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedConflictObjectResult);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(patientDependencyValidationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.PutPatientAsync(somePatient);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
