// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldReturnConsumerStatuses()
        {
            // given
            IQueryable<ConsumerStatus> randomConsumerStatuses = CreateRandomConsumerStatuses();
            IQueryable<ConsumerStatus> storageConsumerStatuses = randomConsumerStatuses;
            IQueryable<ConsumerStatus> expectedConsumerStatuses = storageConsumerStatuses;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerStatusesAsync())
                    .ReturnsAsync(storageConsumerStatuses);

            // when
            IQueryable<ConsumerStatus> actualConsumerStatuses =
                await this.consumerStatusService.RetrieveAllConsumerStatusesAsync();

            // then
            actualConsumerStatuses.Should().BeEquivalentTo(expectedConsumerStatuses);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerStatusesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
