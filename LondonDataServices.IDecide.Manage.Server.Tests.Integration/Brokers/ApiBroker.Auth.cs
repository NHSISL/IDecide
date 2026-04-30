// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string AuthRelativeUrl = "api/auth";

        public async ValueTask<HttpResponseMessage> GetLoginRedirectAsync() =>
            await this.httpClientNoRedirect.GetAsync($"{AuthRelativeUrl}/login");
    }
}
