// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using Moq;
using NhsDigitalSearchCriteria = NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.SearchCriteria;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnLookupByNhsNumberIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomString = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomString;
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

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> patientLookupByNhsNumberTask =
                pdsService.PatientLookupByNhsNumberAsync(inputNhsNumber);

            PdsServiceException actualPdsServiceException =
                 await Assert.ThrowsAsync<PdsServiceException>(
                     testCode: patientLookupByNhsNumberTask.AsTask);

            // then
            actualPdsServiceException.Should().BeEquivalentTo(
                expectedpdsServiceException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedpdsServiceException))),
                    Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
