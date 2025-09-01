// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string patientDecisionRelativeUrl = "api/PatientDecision";

        public async ValueTask PostPatientDecision(Decision decision) =>
            await this.apiFactoryClient
                .PostContentAsync($"{patientDecisionRelativeUrl}/PostPatientDecision", decision);
    }
}
