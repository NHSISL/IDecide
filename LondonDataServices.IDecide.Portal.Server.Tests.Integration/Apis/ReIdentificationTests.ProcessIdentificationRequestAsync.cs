// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Tests.Integration;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.AccessAudits;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.Accesses;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.OdsDatas;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.PdsDatas;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.UserAccesses;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [ReleaseCandidateFact]
        public async Task ShouldPostIdentificationRequestsAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            PdsData pdsData = await PostPdsDataAsync(randomOdsData.OrganisationCode, randomOdsData.OrganisationName);
            string securityOid = TestAuthHandler.SecurityOid;

            UserAccess randomUserAccess =
                await PostRandomUserAccessAsync(randomOdsData.OrganisationCode, securityOid);

            AccessRequest randomAccessRequest =
                CreateIdentificationRequestAccessRequestGivenPsuedoId(securityOid, pdsData.PseudoNhsNumber);

            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();
            int expectedHasAccessAuditCount = 2;
            int expectedHasAccessIdentificationItemsCount = 1;

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostIdentificationRequestsAsync(inputAccessRequest);

            int actualHasAccessIdentificationItemsCount =
                actualAccessRequest.IdentificationRequest.IdentificationItems
                    .Where(item => item.HasAccess == true).Count();

            List<AccessAudit> accessAudits = await this.apiBroker.GetAllAccessAuditsAsync();

            int actualHasAccessAuditsCount = accessAudits.Where(accessAudit => accessAudit.HasAccess == true)
                .Count();

            // then
            actualHasAccessIdentificationItemsCount.Should().Be(expectedHasAccessIdentificationItemsCount);
            actualHasAccessAuditsCount.Should().Be(expectedHasAccessAuditCount);

            List<AccessAudit> requestRelatedAccesAudits =
                accessAudits.Where(accessAudit => accessAudit.RequestId == randomAccessRequest.IdentificationRequest.Id)
                    .ToList();

            foreach (AccessAudit accessAudit in requestRelatedAccesAudits)
            {
                await this.apiBroker.DeleteAccessAuditByIdAsync(accessAudit.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);
        }
    }
}
