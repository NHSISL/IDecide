// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
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

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken))
                        .ReturnsAsync(randomJsonResponse);

            // when
            string actualJsonResponse =
                await this.nhsDigitalApiService.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken);

            // then
            actualJsonResponse.Should().BeEquivalentTo(expectedJsonResponse);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    inputSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
