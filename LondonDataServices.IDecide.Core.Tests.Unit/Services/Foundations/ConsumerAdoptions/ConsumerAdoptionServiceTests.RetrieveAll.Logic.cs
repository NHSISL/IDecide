// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldReturnConsumerAdoptions()
        {
            // given
            IQueryable<ConsumerAdoption> randomConsumerAdoptions = CreateRandomConsumerAdoptions();
            IQueryable<ConsumerAdoption> storageConsumerAdoptions = randomConsumerAdoptions;
            IQueryable<ConsumerAdoption> expectedConsumerAdoptions = storageConsumerAdoptions;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerAdoptionsAsync())
                    .ReturnsAsync(storageConsumerAdoptions);

            // when
            IQueryable<ConsumerAdoption> actualConsumerAdoptions =
                await this.consumerAdoptionService.RetrieveAllConsumerAdoptionsAsync();

            // then
            actualConsumerAdoptions.Should().BeEquivalentTo(expectedConsumerAdoptions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerAdoptionsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
