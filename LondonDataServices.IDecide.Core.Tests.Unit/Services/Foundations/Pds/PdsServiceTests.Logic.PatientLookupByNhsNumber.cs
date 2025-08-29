// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupByNhsNumberAsync()
        {
            // given
            string randomIdentifier = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomIdentifier.DeepClone();
            Hl7.Fhir.Model.Patient outputFhirPatient = CreateRandomPatientWithNhsNumber(inputNhsNumber);
            Patient expectedPatient = GeneratePatientFromFhirPatient(outputFhirPatient);

            var pdsServiceMock = new Mock<PdsService>(
                this.pdsBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            pdsServiceMock.Setup(service =>
                service.MapToPatientFromFhirPatient(outputFhirPatient))
                    .Returns(expectedPatient);

            this.pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputFhirPatient);

            PdsService pdsService = pdsServiceMock.Object;

            // when
            Patient actualPatient =
                await pdsService.PatientLookupByNhsNumberAsync(inputNhsNumber);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            pdsServiceMock.Verify(service =>
                service.MapToPatientFromFhirPatient(outputFhirPatient),
                    Times.Once());

            this.pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
