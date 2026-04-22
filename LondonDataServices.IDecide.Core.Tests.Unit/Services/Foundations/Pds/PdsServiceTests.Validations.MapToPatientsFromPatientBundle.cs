// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using FluentAssertions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldThrowNullBundleJsonExceptionOnMapToPatientsFromBundleJsonWhenNull()
        {
            // given
            string nullBundleJson = null;

            var expectedArgumentNullException =
                new ArgumentNullException("jsonString", "JSON string is null or empty.");

            // when
            ArgumentNullException actualArgumentNullException =
                 Assert.Throws<ArgumentNullException>(() =>
                    pdsService.MapToPatientsFromBundleJson(nullBundleJson));

            // then
            actualArgumentNullException.Should().BeEquivalentTo(
                expectedArgumentNullException);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}