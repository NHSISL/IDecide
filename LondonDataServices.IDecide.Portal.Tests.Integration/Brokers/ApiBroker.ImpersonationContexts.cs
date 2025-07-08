// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.ImpersonationContexts;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string impersonationContextsRelativeUrl = "api/impersonationContexts";

        public async ValueTask<ImpersonationContext> PostImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PostContentAsync(impersonationContextsRelativeUrl, impersonationContext);

        public async ValueTask<ImpersonationContext> GetImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient
                .GetContentAsync<ImpersonationContext>($"{impersonationContextsRelativeUrl}/{impersonationContextId}");

        public async ValueTask<ImpersonationContext> DeleteImpersonationContextByIdAsync(Guid impersonationContextId)
        {
            return await this.apiFactoryClient.DeleteContentAsync<ImpersonationContext>(
                $"{impersonationContextsRelativeUrl}/{impersonationContextId}");
        }
    }
}
