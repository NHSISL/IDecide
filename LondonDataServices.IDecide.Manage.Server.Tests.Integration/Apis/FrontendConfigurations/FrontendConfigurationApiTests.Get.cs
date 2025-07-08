// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    public partial class FrontendConfigurationsApiTests
    {
        [Fact]
        public async Task ShouldGetFrontendConfigurationsAsync()
        {
            // when
            Dictionary<string, string> actualConfigurations =
                await this.apiBroker.GetFrontendConfigurationsAsync();

            // then
            actualConfigurations.Should().NotBeNull();
            actualConfigurations.Should().NotBeEmpty();
        }
    }
}