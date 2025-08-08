// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldThrowNullFhirPatientExceptionOnMapToPatientFromFhirPatientWhenNull()
        {
            // given
            Hl7.Fhir.Model.Patient nullFhirPatient = null;

            var expectedNullFhirPatientException =
                new NullFhirPatientException(message: "FHIR patient is null.");

            // when
            NullFhirPatientException actualNullFhirPatientException =
                 Assert.Throws<NullFhirPatientException>(() => 
                    pdsService.MapToPatientFromFhirPatient(nullFhirPatient));

            // then
            actualNullFhirPatientException.Should().BeEquivalentTo(
                expectedNullFhirPatientException);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}