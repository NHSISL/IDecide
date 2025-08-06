// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditServiceTests
    {
        [Fact]
        public async Task ShouldRemoveAuditByIdAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset, randomUser.UserId);
            Guid inputAuditId = randomAudit.Id;
            Audit storageAudit = randomAudit;
            Audit ingestionTrackingWithDeleteAuditApplied = storageAudit.DeepClone();
            ingestionTrackingWithDeleteAuditApplied.UpdatedBy = randomUser.UserId.ToString();
            ingestionTrackingWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            Audit updatedAudit = storageAudit;
            Audit deletedAudit = updatedAudit;
            Audit expectedAudit = deletedAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAuditByIdAsync(inputAuditId))
                    .ReturnsAsync(storageAudit);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateAuditAsync(randomAudit))
                    .ReturnsAsync(updatedAudit);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteAuditAsync(updatedAudit))
                    .ReturnsAsync(deletedAudit);

            // when
            Audit actualAudit = await this.auditService
                .RemoveAuditByIdAsync(inputAuditId);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAuditId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAuditAsync(randomAudit),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAuditAsync(updatedAudit),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}