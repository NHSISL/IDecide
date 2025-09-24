// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Portal.Server.Models;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientSearches
{
    public partial class PatientSearchTests
    {
        [Fact]
        public async Task ShouldRecordPatientInformation()
        {
            // given
            string inputSurname = "Smith";
            Patient randomPatient = GetPatient(inputSurname);

            RecordPatientInformationRequest someRecordPatientInformationRequest =
                GetRecordPatientInformationRequest(randomPatient.NhsNumber);

            RecordPatientInformationRequest inputRecordPatientInformationRequest =
                someRecordPatientInformationRequest.DeepClone();

            // when
            await this.apiBroker.RecordPatientInformationAsync(inputRecordPatientInformationRequest);

            // then
            var actualPatients = await this.apiBroker.GetAllPatientsAsync();

            var actualPatient = actualPatients
                .First(patient => patient.NhsNumber == inputRecordPatientInformationRequest.NhsNumber);

            actualPatient.Should().NotBeNull();
            actualPatient.NhsNumber.Should().Be(inputRecordPatientInformationRequest.NhsNumber);

            var expectedNotificationPreference = Enum.Parse<NotificationPreference>(
                inputRecordPatientInformationRequest.NotificationPreference, ignoreCase: true);

            actualPatient.NotificationPreference.Should().Be(expectedNotificationPreference);
            actualPatient.ValidationCode.Length.Should().Be(5);
            actualPatient.ValidationCodeExpiresOn.Should().BeAfter(DateTimeOffset.UtcNow);
        }
    }
}
