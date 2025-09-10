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
        public async Task ShouldAddPatientAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Patient randomPatient = CreateRandomPatient(randomDateTimeOffset);
            Patient inputPatient = randomPatient;
            Patient auditAppliedPatient = inputPatient.DeepClone();
            auditAppliedPatient.CreatedBy = randomUserId;
            auditAppliedPatient.CreatedDate = randomDateTimeOffset;
            auditAppliedPatient.UpdatedBy = randomUserId;
            auditAppliedPatient.UpdatedDate = randomDateTimeOffset;
            Patient storagePatient = auditAppliedPatient.DeepClone();
            Patient expectedPatient = storagePatient.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(inputPatient))
                    .ReturnsAsync(auditAppliedPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPatientAsync(auditAppliedPatient))
                    .ReturnsAsync(storagePatient);

            // when
            Patient actualPatient = await this.patientService
                .AddPatientAsync(inputPatient);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(inputPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(auditAppliedPatient),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}