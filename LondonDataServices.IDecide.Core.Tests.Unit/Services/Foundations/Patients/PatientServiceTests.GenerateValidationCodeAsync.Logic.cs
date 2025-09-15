// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldGenerateValidationCodeAsync()
        {
            //given
            int validationCodeLength = 5;

            // when
            string actualValidationCode = await this.patientService.GenerateValidationCodeAsync();

            // then
            actualValidationCode.Should().HaveLength(validationCodeLength);
            actualValidationCode.Should().MatchRegex("^[A-Z0-9]+$");

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}