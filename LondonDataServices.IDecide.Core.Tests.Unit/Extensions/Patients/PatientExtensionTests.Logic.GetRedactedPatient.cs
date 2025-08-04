// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions.Patients
{
    public partial class PatientExtensionTests
    {
        [Fact]
        public void ShouldReturnRedactedPatientWhenGetRedactedPatient()
        {
            // given
            Patient somePatient = GetPatientToRedact();
            Patient inputPatient = somePatient.DeepClone();
            Patient expectedRedactedPatient = GetRedactedPatient(inputPatient);

            // when
            Patient actualResult = inputPatient.GetRedactedPatient();

            // then
            actualResult.Should().BeEquivalentTo(expectedRedactedPatient);
        }
    }
}
