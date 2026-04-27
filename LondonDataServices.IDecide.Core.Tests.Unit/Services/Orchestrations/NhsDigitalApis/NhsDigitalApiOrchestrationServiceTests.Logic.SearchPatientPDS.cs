// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldSearchPatientPDSAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria.DeepClone();
            CancellationToken inputCancellationToken = GetCancellationToken();
            string randomJsonResponse = GetRandomString();
            string expectedJsonResponse = randomJsonResponse.DeepClone();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken))
                        .ReturnsAsync(randomJsonResponse);

            // when
            string actualJsonResponse =
                await this.nhsDigitalApiOrchestrationService.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken);

            // then
            actualJsonResponse.Should().BeEquivalentTo(expectedJsonResponse);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
