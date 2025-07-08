// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Tests.Integration;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.AccessAudits;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.CsvIdentificationRequests;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.OdsDatas;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.PdsDatas;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.UserAccesses;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [ReleaseCandidateFact]
        public async Task ShouldPostCsvIdentificationRequestsAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            PdsData pdsData = await PostPdsDataAsync(randomOdsData.OrganisationCode, randomOdsData.OrganisationName);
            string securityOid = TestAuthHandler.SecurityOid;

            UserAccess randomUserAccess =
                await PostRandomUserAccessAsync(randomOdsData.OrganisationCode, securityOid);

            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            inputCsvIdentificationRequest.RecipientUserId = securityOid;
            inputCsvIdentificationRequest.HasHeaderRecord = false;
            inputCsvIdentificationRequest.IdentifierColumnIndex = 0;
            inputCsvIdentificationRequest.Data = Encoding.UTF8.GetBytes(pdsData.PseudoNhsNumber);
            string randomString = GetRandomStringWithLengthOf(GetRandomNumber());
            string inputReason = randomString;

            CsvIdentificationRequest exisingCsvIdentificationRequest =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            Guid inputCsvIdentificationRequestId = exisingCsvIdentificationRequest.Id;
            int expectedHasAccessAuditCount = 2;

            // when
            byte[] actualFileContentResult =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequestId, inputReason);

            List<AccessAudit> accessAudits = await this.apiBroker.GetAllAccessAuditsAsync();

            int actualHasAccessAuditsCount = accessAudits.Where(accessAudit => accessAudit.HasAccess == true)
                .Count();

            // then
            actualHasAccessAuditsCount.Should().Be(expectedHasAccessAuditCount);

            List<AccessAudit> requestRelatedAccesAudits =
                accessAudits.Where(accessAudit => accessAudit.RequestId == exisingCsvIdentificationRequest.Id)
                    .ToList();

            foreach (AccessAudit accessAudit in requestRelatedAccesAudits)
            {
                await this.apiBroker.DeleteAccessAuditByIdAsync(accessAudit.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id);
        }
    }
}
