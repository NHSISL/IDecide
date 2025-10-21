// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerStatuses;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string ConsumerStatusesRelativeUrl = "api/consumerstatus";

        public async ValueTask AdoptPatientDecisionsAsync(List<Decision> decisions) =>
            await this.apiFactoryClient.PostContentAsync(
                $"{ConsumerStatusesRelativeUrl}/AdoptPatientDecisions", decisions);
    }
}
