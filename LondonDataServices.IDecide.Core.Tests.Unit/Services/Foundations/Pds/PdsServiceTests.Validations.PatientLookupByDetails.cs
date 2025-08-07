// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Moq;
using Task = System.Threading.Tasks.Task;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenNullAndLogItAsync()
        {
            // given
            PatientLookup nullPatientLookup = null;

            var nullPatientLookupException =
                new NullPatientLookupException(message: "Patient lookup is null.");

            var expectedPdsValidationException =
                new PdsValidationException(
                    message: "PDS validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullPatientLookupException);

            pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null))
                    .ThrowsAsync(nullPatientLookupException);

            // when
            ValueTask<PatientLookup> patientLookupByDetailsTask =
                pdsService.PatientLookupByDetailsAsync(nullPatientLookup);

            PdsValidationException actualPdsValidationException =
                 await Assert.ThrowsAsync<PdsValidationException>(
                     testCode: patientLookupByDetailsTask.AsTask);

            // then
            actualPdsValidationException.Should().BeEquivalentTo(
                expectedPdsValidationException);

            pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null),
                        Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsValidationException))),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}