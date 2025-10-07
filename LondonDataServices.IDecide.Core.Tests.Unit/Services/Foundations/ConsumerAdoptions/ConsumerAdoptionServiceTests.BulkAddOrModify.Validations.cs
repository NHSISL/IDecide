// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
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
        public async Task ShouldThrowValidationExceptionOnBulkAddOrModifyIfConsumerAdoptionIsInvalidAndLogItAsync()
        {
            // given
            List<ConsumerAdoption> invalidConsumerAdoptions = null;

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: "consumerAdoptions",
                values: "ConsumerAdoptions is required");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            // when
            ValueTask bulkAddConsumerAdoptionTask =
                this.consumerAdoptionService.BulkAddOrModifyConsumerAdoptionsAsync(
                    consumerAdoptions: invalidConsumerAdoptions);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(bulkAddConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
