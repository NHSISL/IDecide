// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldSearchPatientPDSAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            string randomResult = GetRandomString();
            string returnedResult = randomResult.DeepClone();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ReturnsAsync(returnedResult);

            // when
            string actualResult =
                await this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            // then
            Assert.Equal(returnedResult, actualResult);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

            }
        }
