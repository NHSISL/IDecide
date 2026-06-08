// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Auth
{
    public partial class AuthApiTests
    {
        [Fact]
        public async Task ShouldReturnOkOrUnauthorizedOnSessionAsync()
        {
            // given / when
            HttpResponseMessage response = await this.apiBroker.GetSessionAsync();

            // then
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK,
                HttpStatusCode.Unauthorized);
        }
    }
}
