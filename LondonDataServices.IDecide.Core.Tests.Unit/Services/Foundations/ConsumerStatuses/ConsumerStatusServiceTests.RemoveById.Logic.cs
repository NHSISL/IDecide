// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveConsumerStatusByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputConsumerStatusId = randomId;
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus();
            ConsumerStatus storageConsumerStatus = randomConsumerStatus;
            ConsumerStatus expectedInputConsumerStatus = storageConsumerStatus;
            ConsumerStatus deletedConsumerStatus = expectedInputConsumerStatus;
            ConsumerStatus expectedConsumerStatus = deletedConsumerStatus.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectConsumerStatusByIdAsync(inputConsumerStatusId))
                .ReturnsAsync(storageConsumerStatus);

            this.storageBrokerMock.Setup(broker =>
                    broker.DeleteConsumerStatusAsync(expectedInputConsumerStatus))
                .ReturnsAsync(deletedConsumerStatus);

            // when
            ConsumerStatus actualConsumerStatus = await this.consumerStatusService
                .RemoveConsumerStatusByIdAsync(inputConsumerStatusId);

            // then
            actualConsumerStatus.Should().BeEquivalentTo(expectedConsumerStatus);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectConsumerStatusByIdAsync(inputConsumerStatusId),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteConsumerStatusAsync(expectedInputConsumerStatus),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
