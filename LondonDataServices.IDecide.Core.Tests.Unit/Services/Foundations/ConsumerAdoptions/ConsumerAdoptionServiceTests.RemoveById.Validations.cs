// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidConsumerAdoptionId = Guid.Empty;

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.Id),
                values: "Id is required");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            // when
            ValueTask<ConsumerAdoption> removeConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(invalidConsumerAdoptionId);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    removeConsumerAdoptionByIdTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveByIdIfConsumerAdoptionIsNotFoundAndLogItAsync()
        {
            //given
            Guid someConsumerAdoptionId = Guid.NewGuid();
            ConsumerAdoption noConsumerAdoption = null;

            var notFoundConsumerAdoptionException = new NotFoundConsumerAdoptionException(
                $"Couldn't find consumerAdoption with consumerAdoptionId: {someConsumerAdoptionId}.");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: notFoundConsumerAdoptionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noConsumerAdoption);

            //when
            ValueTask<ConsumerAdoption> retrieveConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RemoveConsumerAdoptionByIdAsync(someConsumerAdoptionId);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    retrieveConsumerAdoptionByIdTask.AsTask);

            //then
            actualConsumerAdoptionValidationException.Should().BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
