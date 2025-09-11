// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveConsumerAdoptionByIdAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            ConsumerAdoption inputConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption storageConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption expectedConsumerAdoption = storageConsumerAdoption.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(inputConsumerAdoption.Id))
                    .ReturnsAsync(storageConsumerAdoption);

            // when
            ConsumerAdoption actualConsumerAdoption =
                await this.consumerAdoptionService.RetrieveConsumerAdoptionByIdAsync(inputConsumerAdoption.Id);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(expectedConsumerAdoption);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(inputConsumerAdoption.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
