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

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditServiceTests
    {
        [Fact]
        public async Task ShouldModifyAuditAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            Audit randomAudit =
                CreateRandomModifyAudit(randomDateTimeOffset, randomUser.UserId);

            Audit inputAudit = randomAudit;
            Audit storageAudit = inputAudit.DeepClone();
            storageAudit.UpdatedDate = randomAudit.CreatedDate;
            Audit updatedAudit = inputAudit;
            Audit expectedAudit = updatedAudit.DeepClone();
            Guid auditId = inputAudit.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAuditByIdAsync(auditId))
                    .ReturnsAsync(storageAudit);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateAuditAsync(inputAudit))
                    .ReturnsAsync(updatedAudit);

            // when
            Audit actualAudit =
                await this.auditService.ModifyAuditAsync(inputAudit);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAuditAsync(inputAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}