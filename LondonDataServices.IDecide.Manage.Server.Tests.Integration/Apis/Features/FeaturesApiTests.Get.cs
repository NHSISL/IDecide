// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    public partial class FeaturesApiTests
    {
        [Fact]
        public async Task ShouldGetAllFeaturesAsync()
        {
            // Given
            List<string> expectedFeatures = new List<string>
            {
                "Configuration",
                "UserAccess",
                "DelegatedUserAccess",
                "Ods",
                "Pds",
                "ReportReidentify",
                "SinglePatientReidentify",
                "CsvReidentify",
                "Projects",
                "CsvWorklist"
            };

            // When
            string[] actualFeatures = await this.apiBroker.GetFeaturesAsync();

            // Then
            actualFeatures.Should().BeEquivalentTo(expectedFeatures);
        }
    }
}