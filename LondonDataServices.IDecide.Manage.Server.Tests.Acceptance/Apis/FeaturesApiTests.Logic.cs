// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis
{
    public partial class FeaturesApiTests
    {
        [Fact]
        public async Task ShouldGetFeaturesAsync()
        {
            // Given
            List<string> expectedResult = new List<string>
            {
            };

            // When
            string[] actualResult = await this.apiBroker.GetFeaturesAsync();

            // Then
            actualResult.Should().BeEquivalentTo(expectedResult.ToArray());
        }
    }
}