// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.NhsLoginUserInfo;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.Patients
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldGetPatientInfoAsync()
        {
            // given
            var expectedBirthdate = new DateTime(1990, 1, 15);
            var expectedFamilyName = "TestFamilyName";
            var expectedGivenName = "TestGivenName";
            var expectedEmail = "test@example.com";
            var expectedPhoneNumber = "+447887510886";

            // when
            NhsLoginUserInfo actualNhsLoginUserInfo =
                await this.apiBroker.GetPatientInfoAsync();

            // Debug: Print the raw JSON to see what we're getting
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var debugJson = JsonSerializer.Serialize(actualNhsLoginUserInfo, jsonOptions);
            System.Diagnostics.Debug.WriteLine($"Received JSON: {debugJson}");

            // then
            actualNhsLoginUserInfo.Should().NotBeNull();
            actualNhsLoginUserInfo.Birthdate.Should().Be(expectedBirthdate);
            actualNhsLoginUserInfo.FamilyName.Should().Be(expectedFamilyName);
            actualNhsLoginUserInfo.GivenName.Should().Be(expectedGivenName);
            actualNhsLoginUserInfo.Email.Should().Be(expectedEmail);
            actualNhsLoginUserInfo.PhoneNumber.Should().Be(expectedPhoneNumber);
        }
    }
}