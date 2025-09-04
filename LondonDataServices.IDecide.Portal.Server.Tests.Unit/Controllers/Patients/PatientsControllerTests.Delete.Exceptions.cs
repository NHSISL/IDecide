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

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedBadRequestObjectResult);

            this.patientServiceMock.Setup(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.DeletePatientByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedBadRequestObjectResult);

            this.patientServiceMock.Setup(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.DeletePatientByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemovePatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(patientValidationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.DeletePatientByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedPatientException =
                new LockedPatientException(
                    message: someMessage,
                    innerException: someInnerException);

            var patientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: someMessage,
                    innerException: lockedPatientException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedPatientException);

            var expectedActionResult =
                new ActionResult<Patient>(expectedConflictObjectResult);

            this.patientServiceMock.Setup(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(patientDependencyValidationException);

            // when
            ActionResult<Patient> actualActionResult =
                await this.patientsController.DeletePatientByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.patientServiceMock.Verify(service =>
                service.RemovePatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.patientServiceMock.VerifyNoOtherCalls();
        }
    }
}
