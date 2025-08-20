// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerIsNullAndLogItAsync()
        {
            // given
            Consumer nullConsumer = null;

            var nullConsumerException =
                new NullConsumerException(message: "Consumer is null.");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: nullConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumer))
                .ReturnsAsync(nullConsumer);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(nullConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(() =>
                    addConsumerTask.AsTask());

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumer),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedConsumerValidationException))),
                Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
