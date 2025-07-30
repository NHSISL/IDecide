// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Force.DeepCloner;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupByDetailsAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            Bundle randomBundle = CreateRandomBundle(inputSurname);
            PatientBundle outputPatientBundle = CreateRandomPatientBundle(randomBundle);
            Models.Foundations.Pds.Patient expectedPatient = CreateRandomLocalPatient(outputPatientBundle);

            this.pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    inputSurname,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null))
                        .ReturnsAsync(outputPatientBundle);

            // when
            Models.Foundations.Pds.Patient actualPatient = await this.pdsService.PatientLookupByDetailsAsync(
                null,
                inputSurname,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            //then
            this.pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByDetailsAsync(
                    null,
                    inputSurname,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
