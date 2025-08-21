// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupUsingByDetailsWhenNoNhsNumberProvidedAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = new List<Patient> { GetRandomPatient(inputSurname) };
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();
            Patient patient = outputPatientLookup.Patients.FirstOrDefault();
            Patient patientToRedact = patient.DeepClone();
            Patient redactedPatient = patientToRedact.Redact();
            Patient expectedPatient = redactedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            // when
            Patient actualPatient =
                await this.patientOrchestrationService.PatientLookupAsync(inputPatientLookup);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPatientLookupUsingByNhsNumberWhenNhsNumberProvidedAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient outputPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            Patient patientToRedact = outputPatient.DeepClone();
            Patient redactedPatient = patientToRedact.Redact();
            Patient expectedPatient = redactedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            // when
            Patient actualPatient =
                await this.patientOrchestrationService.PatientLookupAsync(inputPatientLookup);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
