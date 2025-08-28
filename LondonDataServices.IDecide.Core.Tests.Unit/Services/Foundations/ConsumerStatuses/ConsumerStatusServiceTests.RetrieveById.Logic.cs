// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveConsumerStatusByIdAsync()
        {
            // given
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            ConsumerStatus inputConsumerStatus = randomConsumerStatus;
            ConsumerStatus storageConsumerStatus = randomConsumerStatus;
            ConsumerStatus expectedConsumerStatus = storageConsumerStatus.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(inputConsumerStatus.Id))
                    .ReturnsAsync(storageConsumerStatus);

            // when
            ConsumerStatus actualConsumerStatus =
                await this.consumerStatusService.RetrieveConsumerStatusByIdAsync(inputConsumerStatus.Id);

            // then
            actualConsumerStatus.Should().BeEquivalentTo(expectedConsumerStatus);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(inputConsumerStatus.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
