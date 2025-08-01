// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllPatientsAsync()
        {
            // given
            IQueryable<Patient> randomPatients = CreateRandomPatients();
            IQueryable<Patient> storagePatients = randomPatients;
            IQueryable<Patient> expectedPatients = storagePatients;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPatientsAsync())
                    .ReturnsAsync(storagePatients);

            // when
            IQueryable<Patient> actualPatients =
                await this.patientService.RetrieveAllPatientsAsync();

            // then
            actualPatients.Should().BeEquivalentTo(expectedPatients);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPatientsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
