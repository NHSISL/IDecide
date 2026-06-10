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
        public async Task ShouldRedirectBadRequestOrInternalServerErrorOnCallbackAsync()
        {
            // given
            string randomCode = "someCode";
            string randomState = "someState";

            // when
            HttpResponseMessage response =
                await this.apiBroker.GetCallbackAsync(randomCode, randomState);

            // then
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.Redirect,
                HttpStatusCode.BadRequest,
                HttpStatusCode.InternalServerError);
        }
    }
}
