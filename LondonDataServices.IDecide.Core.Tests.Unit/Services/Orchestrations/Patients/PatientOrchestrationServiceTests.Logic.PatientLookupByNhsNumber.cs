// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Moq;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupByNhsNumberAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            Patient outputPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            Patient patientToRedact = outputPatient.DeepClone();
            Patient redactedPatient = patientToRedact.Redact();
            Patient expectedPatient = redactedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            // when
            Patient actualPatient = 
                await this.patientOrchestrationService.PatientLookupByNhsNumberAsync(inputNhsNumber);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.pdsServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
