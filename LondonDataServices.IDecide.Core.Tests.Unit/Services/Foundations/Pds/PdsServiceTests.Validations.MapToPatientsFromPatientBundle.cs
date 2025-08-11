// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldThrowNullPatientBundleExceptionOnMapToPatientsFromPatientBundleWhenNull()
        {
            // given
            PatientBundle nullPatientBundle = null;

            var expectedNullPatientBundleException =
                new NullPatientBundleException(message: "Patient bundle is null.");

            // when
            NullPatientBundleException actualNullPatientBundleException =
                 Assert.Throws<NullPatientBundleException>(() => 
                    pdsService.MapToPatientsFromPatientBundle(nullPatientBundle));

            // then
            actualNullPatientBundleException.Should().BeEquivalentTo(
                expectedNullPatientBundleException);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}