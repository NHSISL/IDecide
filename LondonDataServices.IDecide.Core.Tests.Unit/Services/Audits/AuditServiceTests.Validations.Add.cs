// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Services.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditIsNullAndLogItAsync()
        {
            // given
            Audit nullAudit = null;

            var nullAuditException =
                new NullAuditException(message: "Audit is null.");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation errors occurred, please try again.",
                    innerException: nullAuditException);

            // when
            ValueTask<Audit> addAuditTask =
                this.auditService.AddAuditAsync(nullAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditsIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            DateTimeOffset randomDataTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            var invalidAudit = new Audit
            {
                AuditType = invalidText,
                Title = invalidText,
            };

            var auditServiceMock = new Mock<AuditService>(
                storageBrokerMock.Object,
                identifierBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDataTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.Id),
                values: "Id is required");

            invalidAuditException.AddData(
                key: nameof(Audit.AuditType),
                values: "Text is required");

            invalidAuditException.AddData(
                key: nameof(Audit.Title),
                values: "Text is required");

            invalidAuditException.AddData(
                 key: nameof(Audit.CreatedDate),
                 values:
                 [
                    "Date is required",
                    $"Date is not recent"
                 ]);

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedBy),
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomUser.UserId}' but found '{invalidAudit.CreatedBy}'."
                ]);

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: "Date is required");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: "Text is required");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation errors occurred, please try again.",
                    innerException: invalidAuditException);

            // when
            ValueTask<Audit> addAuditTask =
                auditServiceMock.Object.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            auditServiceMock.Verify(service =>
                service.ApplyAddAuditAsync(invalidAudit),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            Audit randomAudit =
                CreateRandomAudit(randomDateTimeOffset, randomUser.UserId);

            Audit invalidAudit = randomAudit;
            invalidAudit.CreatedDate = GetRandomDateTimeOffset();
            invalidAudit.UpdatedDate = GetRandomDateTimeOffset();

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: $"Date is not the same as {nameof(Audit.CreatedDate)}");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedDate),
                values: $"Date is not recent");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation errors occurred, please try again.",
                    innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                storageBrokerMock.Object,
                identifierBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Audit> addAuditTask =
                auditServiceMock.Object.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            auditServiceMock.Verify(service =>
                service.ApplyAddAuditAsync(invalidAudit),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            Audit randomAudit =
                CreateRandomAudit(randomDateTimeOffset, randomUser.UserId);

            Audit invalidAudit = randomAudit;
            invalidAudit.CreatedBy = GetRandomString();
            invalidAudit.UpdatedBy = GetRandomString();

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedBy),
                values: $"Expected value to be '{randomUser.UserId}' " +
                    $"but found '{invalidAudit.CreatedBy}'.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: $"Text is not the same as {nameof(Audit.CreatedBy)}");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation errors occurred, please try again.",
                    innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                storageBrokerMock.Object,
                identifierBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Audit> addAuditTask =
                auditServiceMock.Object.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            auditServiceMock.Verify(service =>
                service.ApplyAddAuditAsync(invalidAudit),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();
            DateTimeOffset invalidDateTime = randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            Audit randomAudit =
                CreateRandomAudit(invalidDateTime, randomUser.UserId);

            Audit invalidAudit = randomAudit;

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedDate),
                values: "Date is not recent");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation errors occurred, please try again.",
                    innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                storageBrokerMock.Object,
                identifierBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Audit> addAuditTask =
                auditServiceMock.Object.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            auditServiceMock.Verify(service =>
                service.ApplyAddAuditAsync(invalidAudit),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}