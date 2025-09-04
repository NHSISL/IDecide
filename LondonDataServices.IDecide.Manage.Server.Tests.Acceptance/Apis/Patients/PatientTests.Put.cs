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
        public async Task ShouldPutPatientAsync()
        {
            // given
            Patient randomPatient = 
                await PostRandomPatientAsync();

            Patient modifiedPatient = 
                UpdatePatientWithRandomValues(randomPatient);

            // when
            await this.apiBroker.PutPatientAsync(modifiedPatient);
            
            Patient actualPatient = await this.apiBroker
                .GetPatientByIdAsync(randomPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(modifiedPatient, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}