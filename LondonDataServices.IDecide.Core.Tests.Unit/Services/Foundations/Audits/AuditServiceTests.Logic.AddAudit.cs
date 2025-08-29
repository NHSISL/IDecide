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
        public async Task ShouldAddAuditLogAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();
            Guid randomIdentifier = Guid.NewGuid();
            string randomAuditType = GetRandomString();
            string randomAuditTitle = GetRandomString();
            string randomMesssage = GetRandomString();
            string randomFileName = GetRandomString();
            string randomLogLevel = GetRandomString();

            Audit randomAudit = new Audit
            {
                Id = randomIdentifier,
                AuditType = randomAuditType,
                Title = randomAuditTitle,
                Message = randomMesssage,
                CorrelationId = randomIdentifier.ToString(),
                FileName = randomFileName,
                LogLevel = randomLogLevel,
                CreatedBy = randomUser.UserId,
                CreatedDate = randomDateTimeOffset,
                UpdatedBy = randomUser.UserId,
                UpdatedDate = randomDateTimeOffset,
            };

            Audit inputAudit = randomAudit;
            Audit storageAudit = inputAudit;
            Audit expectedAudit = inputAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomIdentifier);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertAuditAsync(It.Is(SameAuditAs(inputAudit))))
                    .ReturnsAsync(storageAudit);

            // when
            Audit actualAudit = await this.auditService
                .AddAuditAsync(
                    auditType: randomAuditType,
                    title: randomAuditTitle,
                    message: randomMesssage,
                    fileName: randomFileName,
                    correlationId: randomIdentifier.ToString(),
                    logLevel: randomLogLevel);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAuditAsync(It.Is(SameAuditAs(inputAudit))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}