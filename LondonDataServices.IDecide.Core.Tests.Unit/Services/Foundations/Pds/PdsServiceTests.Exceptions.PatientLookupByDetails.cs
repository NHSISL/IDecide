// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using System;
using Moq;
using Task = System.Threading.Tasks.Task;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnLookupByDetailsIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString;
            var serviceException = new Exception();

            var failedServicePdsException =
                new FailedServicePdsException(
                    message: "Failed PDS service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedpdsServiceException =
                new PdsServiceException(
                    message: "PDS service error occurred, please contact support.",
                    innerException: failedServicePdsException);

            pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    inputSurname,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> patientLookupByDetailsTask =
                pdsService.PatientLookupByDetailsAsync(
                    null,
                    inputSurname,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

           PdsServiceException actualPdsServiceException =
                await Assert.ThrowsAsync<PdsServiceException>(
                    testCode: patientLookupByDetailsTask.AsTask);

            // then
            actualPdsServiceException.Should().BeEquivalentTo(
                expectedpdsServiceException);

            pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    inputSurname,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedpdsServiceException))),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
