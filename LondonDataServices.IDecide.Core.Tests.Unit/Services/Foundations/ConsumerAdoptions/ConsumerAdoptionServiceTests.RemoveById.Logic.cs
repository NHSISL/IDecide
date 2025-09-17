// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveConsumerAdoptionByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputConsumerAdoptionId = randomId;
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            ConsumerAdoption storageConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption expectedInputConsumerAdoption = storageConsumerAdoption;
            ConsumerAdoption deletedConsumerAdoption = expectedInputConsumerAdoption;
            ConsumerAdoption expectedConsumerAdoption = deletedConsumerAdoption.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(inputConsumerAdoptionId))
                    .ReturnsAsync(storageConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteConsumerAdoptionAsync(expectedInputConsumerAdoption))
                    .ReturnsAsync(deletedConsumerAdoption);

            // when
            ConsumerAdoption actualConsumerAdoption = await this.consumerAdoptionService
                .RemoveConsumerAdoptionByIdAsync(inputConsumerAdoptionId);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(expectedConsumerAdoption);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(inputConsumerAdoptionId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerAdoptionAsync(expectedInputConsumerAdoption),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
