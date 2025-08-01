// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using FluentAssertions;
using Force.DeepCloner;
using System.Threading.Tasks;
using System;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldRemovePatientByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputPatientId = randomId;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();

            Patient randomPatient = 
                CreateRandomPatient(randomNhsNumber, randomValidationCode);

            Patient storagePatient = randomPatient;
            Patient expectedInputPatient = storagePatient;
            Patient deletedPatient = expectedInputPatient;
            Patient expectedPatient = deletedPatient.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(inputPatientId))
                    .ReturnsAsync(storagePatient);

            this.storageBrokerMock.Setup(broker =>
                broker.DeletePatientAsync(expectedInputPatient))
                    .ReturnsAsync(deletedPatient);

            // when
            Patient actualPatient = await this.patientService
                .RemovePatientByIdAsync(inputPatientId);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(inputPatientId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePatientAsync(expectedInputPatient),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
