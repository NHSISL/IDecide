// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldGetAllPatientsAsync()
        {
            // given
            List<Patient> randomPatients = await PostRandomPatientsAsync();
            List<Patient> expectedPatients = randomPatients;

            // when
            List<Patient> actualPatients = await this.apiBroker.GetAllPatientsAsync();

            // then
            actualPatients.Should().NotBeNull();

            foreach (Patient expectedPatient in expectedPatients)
            {
                Patient actualPatient = actualPatients
                    .Single(patient => patient.Id == expectedPatient.Id);

                actualPatient.Should().BeEquivalentTo(
                    expectedPatient,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (Patient createdPatient in expectedPatients)
            {
                await this.apiBroker.DeletePatientByIdAsync(createdPatient.Id);
            }
        }
    }
}
