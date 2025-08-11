// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions.Patients
{
    public partial class PatientExtensionTests
    {
        [Fact]
        public void ShouldReturnRedactedPatientWhenRedact()
        {
            // given
            Patient somePatient = GetPatientToRedact();
            Patient inputPatient = somePatient.DeepClone();
            Patient expectedRedactedPatient = GetRedactedPatient(inputPatient);

            // when
            Patient actualResult = inputPatient.Redact();

            // then
            actualResult.Should().BeEquivalentTo(expectedRedactedPatient);
        }

        [Fact]
        public void ShouldReturnRedactedPatientWhenRedactWithWhitespace()
        {
            // given
            Patient somePatient = GetPatientToRedactWithWhitespace();
            Patient inputPatient = somePatient.DeepClone();
            Patient expectedRedactedPatient = GetRedactedPatientWithWhitespace(inputPatient);

            // when
            Patient actualResult = inputPatient.Redact();

            // then
            actualResult.Should().BeEquivalentTo(expectedRedactedPatient);
        }
    }
}
