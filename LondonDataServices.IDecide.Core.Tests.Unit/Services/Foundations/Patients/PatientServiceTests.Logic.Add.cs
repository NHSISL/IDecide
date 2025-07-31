// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using FluentAssertions;
using Force.DeepCloner;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldAddPatientAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();
            Patient randomPatient = CreateRandomPatient(randomNhsNumber, randomValidationCode);
            Patient inputPatient = randomPatient;
            Patient storagePatient = inputPatient;
            Patient expectedPatient = storagePatient.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPatientAsync(inputPatient))
                    .ReturnsAsync(storagePatient);

            // when
            Patient actualPatient = await this.patientService
                .AddPatientAsync(inputPatient);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(inputPatient),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
