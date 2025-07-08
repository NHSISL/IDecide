// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Audits;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddAuditIfServiceErrorOccursAndLogItAsync()
        {
            // given
            List<Audit> someAudits = CreateRandomAudits();
            int randomBatchSize = GetRandomNumber();
            var serviceException = new Exception();

            var failedAuditServiceException =
                new FailedAuditServiceException(
                    message: "Failed audit service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedAuditServiceException =
                new AuditServiceException(
                    message: "Audit service error occurred, please contact support.",
                    innerException: failedAuditServiceException);

            var auditServiceMock = new Mock<AuditService>(
                this.storageBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };


            auditServiceMock.Setup(service =>
                service.BatchBulkAddAuditsAsync(It.IsAny<List<Audit>>(), It.IsAny<int>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask bulkAddAuditTask =
                auditServiceMock.Object.BulkAddAuditsAsync(someAudits);

            AuditServiceException actualAuditServiceException =
                await Assert.ThrowsAsync<AuditServiceException>(
                    bulkAddAuditTask.AsTask);

            // then
            actualAuditServiceException.Should()
                .BeEquivalentTo(expectedAuditServiceException);

            auditServiceMock.Verify(service =>
                service.BatchBulkAddAuditsAsync(It.IsAny<List<Audit>>(), It.IsAny<int>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditServiceException))),
                        Times.Once);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}