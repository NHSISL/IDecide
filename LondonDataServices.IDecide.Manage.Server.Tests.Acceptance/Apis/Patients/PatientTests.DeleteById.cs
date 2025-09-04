// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldDeletePatientByIdAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            Patient inputPatient = randomPatient;
            Patient expectedPatient = inputPatient;

            // when
            Patient deletedPatient =
                await this.apiBroker.DeletePatientByIdAsync(inputPatient.Id);

            List<Patient> actualResult =
                await this.apiBroker.GetSpecificPatientByIdAsync(inputPatient.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}