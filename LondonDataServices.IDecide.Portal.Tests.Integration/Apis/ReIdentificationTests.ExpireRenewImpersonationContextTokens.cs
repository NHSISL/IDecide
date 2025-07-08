// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Tests.Integration;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.Accesses;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.ImpersonationContexts;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [ReleaseCandidateFact]
        public async Task ShouldExpireRenewImpersonationContextTokensAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = await PostRandomImpersonationContextAsync();
            Guid inputImpersonationContextId = randomImpersonationContext.Id;

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextGenerateTokensAsync(inputImpersonationContextId);

            // then
            actualAccessRequest.ImpersonationContext.InboxSasToken.Should().NotBeNullOrEmpty();
            actualAccessRequest.ImpersonationContext.OutboxSasToken.Should().NotBeNullOrEmpty();
            actualAccessRequest.ImpersonationContext.ErrorsSasToken.Should().NotBeNullOrEmpty();
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }
    }
}
