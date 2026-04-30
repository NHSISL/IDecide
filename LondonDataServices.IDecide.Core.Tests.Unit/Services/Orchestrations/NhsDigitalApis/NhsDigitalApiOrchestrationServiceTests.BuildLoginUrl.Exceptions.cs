// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(NhsDigitalApiDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBuildLoginUrlAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(inputCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException
                actualNhsDigitalApiOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                        testCode: buildLoginUrlTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationDependencyValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
