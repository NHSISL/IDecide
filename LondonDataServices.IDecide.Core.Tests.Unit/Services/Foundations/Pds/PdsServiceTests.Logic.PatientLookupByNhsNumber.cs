// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Force.DeepCloner;
using Hl7.Fhir.Model;
using Moq;
using Task = System.Threading.Tasks.Task;

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
            Patient outputFhirPatient = CreateRandomPatient(inputNhsNumber);
            Models.Foundations.Pds.Patient expectedPatient = CreateRandomLocalPatient(outputFhirPatient);

            this.pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByNhsNumberAsync(inputNhsNumber))
                        .ReturnsAsync(outputFhirPatient);

            // when
            Models.Foundations.Pds.Patient actualPatient = 
                await this.pdsService.PatientLookupByNhsNumberAsync(inputNhsNumber);

            //then
            this.pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByNhsNumberAsync(inputNhsNumber),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
