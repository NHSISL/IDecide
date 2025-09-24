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

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientSearches
{
    public partial class PatientSearchTests
    {
        [Fact]
        public async Task ShouldRecordPatientInformation()
        {
            // given
            RecordPatientInformationRequest someRecordPatientInformationRequest = GetRecordPatientInformationRequest();

            RecordPatientInformationRequest inputRecordPatientInformationRequest =
                someRecordPatientInformationRequest.DeepClone();

            // when
            await this.apiBroker.RecordPatientInformationAsync(inputRecordPatientInformationRequest);

            // then
            var actualPatients = await this.apiBroker.GetAllPatientsAsync();

            var actualPatient = actualPatients
                .FirstOrDefault(patient => patient.NhsNumber == inputRecordPatientInformationRequest.NhsNumber);

            actualPatient.Should().NotBeNull();
            actualPatient?.NhsNumber.Should().Be(inputRecordPatientInformationRequest.NhsNumber);

            var expectedNotificationPreference = Enum.Parse<NotificationPreference>(
                inputRecordPatientInformationRequest.NotificationPreference, ignoreCase: true);

            actualPatient?.NotificationPreference.Should().Be(expectedNotificationPreference);
        }
    }
}
