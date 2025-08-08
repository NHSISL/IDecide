// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldRetrievePatientByIdAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient inputPatient = randomPatient;
            Patient storagePatient = randomPatient;
            Patient expectedPatient = storagePatient.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(inputPatient.Id))
                    .ReturnsAsync(storagePatient);

            // when
            Patient actualPatient =
                await this.patientService.RetrievePatientByIdAsync(inputPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(inputPatient.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}