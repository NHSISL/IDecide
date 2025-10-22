// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.ConsumerStatuses;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string consumerStatusesRelativeUrl = "api/consumerstatus";

        public async ValueTask AdoptPatientDecisionsAsync(List<Decision> decisions) =>
            await this.apiFactoryClient.PostContentAsync(
                $"{consumerStatusesRelativeUrl}/AdoptPatientDecisions", decisions);
    }
}
