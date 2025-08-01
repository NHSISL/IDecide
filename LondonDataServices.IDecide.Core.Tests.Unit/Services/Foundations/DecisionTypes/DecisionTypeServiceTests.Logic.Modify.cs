// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldModifyDecisionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomEntraUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomModifyDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.UserId);

            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = inputDecisionType.DeepClone();
            storageDecisionType.UpdatedDate = randomDecisionType.CreatedDate;
            DecisionType updatedDecisionType = inputDecisionType;
            DecisionType expectedDecisionType = updatedDecisionType.DeepClone();
            Guid decisionTypeId = inputDecisionType.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(decisionTypeId))
                    .ReturnsAsync(storageDecisionType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateDecisionTypeAsync(inputDecisionType))
                    .ReturnsAsync(updatedDecisionType);

            // when
            DecisionType actualDecisionType =
                await this.decisionTypeService.ModifyDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionType.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(inputDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}