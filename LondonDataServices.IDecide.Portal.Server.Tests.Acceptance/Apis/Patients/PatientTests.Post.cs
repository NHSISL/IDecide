// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldPostPatientAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient inputPatient = randomPatient;
            Patient expectedPatient = inputPatient;

            // when 
            await this.apiBroker.PostPatientAsync(inputPatient);

            Patient actualPatient =
                await this.apiBroker.GetPatientByIdAsync(inputPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}