// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using Moq;
using Task = System.Threading.Tasks.Task;
using NhsDigitalSearchCriteria = NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.SearchCriteria;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnLookupByDetailsIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(randomString);
            PatientLookup inputPatientLookup = randomPatientLookup;
            var serviceException = new Exception();

            var failedServicePdsException =
                new FailedServicePdsException(
                    message: "Failed PDS service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedPdsServiceException =
                new PdsServiceException(
                    message: "PDS service error occurred, please contact support.",
                    innerException: failedServicePdsException);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<PatientLookup> patientLookupByDetailsTask =
                pdsService.PatientLookupByDetailsAsync(inputPatientLookup);

            PdsServiceException actualPdsServiceException =
                 await Assert.ThrowsAsync<PdsServiceException>(
                     testCode: patientLookupByDetailsTask.AsTask);

            // then
            actualPdsServiceException.Should().BeEquivalentTo(
                expectedPdsServiceException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsServiceException))),
                        Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}