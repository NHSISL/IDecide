// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPropagateCancellationOnSearchPatientPDSWhenCancelledAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken inputCancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ThrowsAsync(new OperationCanceledException(inputCancellationToken));

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                testCode: searchPatientPDSTask.AsTask);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Exception>()),
                    Times.Never);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
