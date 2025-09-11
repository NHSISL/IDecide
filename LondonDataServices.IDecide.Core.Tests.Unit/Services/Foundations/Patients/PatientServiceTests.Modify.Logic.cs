// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldModifyPatientAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Patient randomPatient = CreateRandomModifyPatient(randomDateTimeOffset);
            Patient inputPatient = randomPatient;
            Patient storagePatient = inputPatient.DeepClone();
            storagePatient.UpdatedDate = randomPatient.CreatedDate;
            Patient auditAppliedPatient = inputPatient.DeepClone();
            auditAppliedPatient.UpdatedBy = randomUserId;
            auditAppliedPatient.UpdatedDate = randomDateTimeOffset;
            Patient auditEnsuredPatient = auditAppliedPatient.DeepClone();
            Patient updatedPatient = inputPatient;
            Patient expectedPatient = updatedPatient.DeepClone();
            Guid patientId = inputPatient.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputPatient))
                    .ReturnsAsync(auditAppliedPatient);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(patientId))
                    .ReturnsAsync(storagePatient);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedPatient, storagePatient))
                    .ReturnsAsync(auditEnsuredPatient);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdatePatientAsync(auditEnsuredPatient))
                    .ReturnsAsync(updatedPatient);

            // when
            Patient actualPatient =
                await this.patientService.ModifyPatientAsync(inputPatient);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputPatient),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(patientId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedPatient, storagePatient),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePatientAsync(auditEnsuredPatient),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}