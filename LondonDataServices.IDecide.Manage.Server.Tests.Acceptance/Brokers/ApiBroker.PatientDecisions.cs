// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.PatientDecisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string patientDecisionsRelativeUrl = "api/patientdecision";

        public async ValueTask<Decision> PostPatientDecisionAsync(Decision decision) =>
            await this.apiFactoryClient.PostContentAsync($"{patientDecisionsRelativeUrl}/PatientDecision", decision);


        public async ValueTask<List<Decision>> GetPatientDecisionsAsync(DateTimeOffset from, string decisionType)
        {
            string url =
                $"{patientDecisionsRelativeUrl}/PatientDecision?" +
                $"from={Uri.EscapeDataString(from.ToString("o"))}&" +
                $"decisionType={Uri.EscapeDataString(decisionType)}";

            return await this.apiFactoryClient.GetContentAsync<List<Decision>>(url);
        }
    }
}
