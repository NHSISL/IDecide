// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldReturnPatients()
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
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}