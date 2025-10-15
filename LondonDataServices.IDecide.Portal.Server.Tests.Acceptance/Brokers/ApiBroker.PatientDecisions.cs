// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.PatientDecisions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string patientDecisionsRelativeUrl = "api/patientdecision";

        public async ValueTask<Decision> PostPatientDecisionAsync(Decision decision) =>
            await this.apiFactoryClient.PostContentAsync($"{patientDecisionsRelativeUrl}/PatientDecision", decision);
    }
}
