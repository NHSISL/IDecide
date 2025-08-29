// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using Moq;
using Task = System.Threading.Tasks.Task;

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

            pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByDetailsAsync(
                    string.Empty,
                    inputPatientLookup.SearchCriteria.Surname,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty))
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

            pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByDetailsAsync(
                    string.Empty,
                    inputPatientLookup.SearchCriteria.Surname,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsServiceException))),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}