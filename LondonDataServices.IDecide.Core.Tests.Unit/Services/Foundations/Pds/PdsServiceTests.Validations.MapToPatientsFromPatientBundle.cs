// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldThrowNullBundleJsonExceptionOnMapToPatientsFromBundleJsonWhenNull()
        {
            // given
            string nullBundleJson = null;

            var expectedNullPatientBundleException =
                new NullPatientBundleException(message: "Patient bundle is null.");

            // when
            NullPatientBundleException actualNullPatientBundleException =
                 Assert.Throws<NullPatientBundleException>(() =>
                    pdsService.MapToPatientsFromBundleJson(nullBundleJson));

            // then
            actualNullPatientBundleException.Should().BeEquivalentTo(
                expectedNullPatientBundleException);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}