// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string AuthRelativeUrl = "api/auth";

        public async ValueTask<HttpResponseMessage> GetLoginRedirectAsync() =>
            await this.httpClientNoRedirect.GetAsync($"{AuthRelativeUrl}/login");

        public async ValueTask<HttpResponseMessage> GetSessionAsync() =>
            await this.httpClient.GetAsync($"{AuthRelativeUrl}/session");

        public async ValueTask<HttpResponseMessage> PostLogoutAsync() =>
            await this.httpClientNoRedirect.PostAsync($"{AuthRelativeUrl}/logout", content: null);

        public async ValueTask<HttpResponseMessage> GetCallbackAsync(string code, string state) =>
            await this.httpClientNoRedirect.GetAsync(
                $"{AuthRelativeUrl}/callback?code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}");
    }
}
