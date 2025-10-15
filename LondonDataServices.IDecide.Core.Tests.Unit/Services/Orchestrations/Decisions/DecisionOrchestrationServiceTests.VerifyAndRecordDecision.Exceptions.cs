// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnVerifyAndRecordDecisionAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedDecisionOrchestrationDependencyValidationException =
                new DecisionOrchestrationDependencyValidationException(
                    message: "Decision orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask verifyAndRecordDecisionTask =
                this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationDependencyValidationException
                actualDecisionOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationDependencyValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationDependencyValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationDependencyValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnVerifyAndRecordDecisionAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(dependencyException);

            var expectedDecisionOrchestrationDependencyException =
                new DecisionOrchestrationDependencyException(
                    message: "Decision orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask verifyAndRecordDecisionTask =
                this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationDependencyException
                actualDecisionOrchestrationDependencyException =
                    await Assert.ThrowsAsync<DecisionOrchestrationDependencyException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationDependencyException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationDependencyException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnVerifyAndRecordDecisionIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();
            var serviceException = new Exception();

            var failedServiceDecisionOrchestrationException =
                new FailedDecisionOrchestrationServiceException(
                    message: "Failed decision orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedDecisionOrchestrationServiceException =
                new DecisionOrchestrationServiceException(
                    message: "Decision orchestration service error occurred, contact support.",
                    innerException: failedServiceDecisionOrchestrationException.InnerException as Xeption);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask verifyAndRecordDecisionTask =
                this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationServiceException
                actualDecisionOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationServiceException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationValidationException.Should().BeEquivalentTo(
                expectedDecisionOrchestrationServiceException);

            this.patientServiceMock.Verify(service =>
               service.RetrieveAllPatientsAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionOrchestrationServiceException))),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationServiceException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
