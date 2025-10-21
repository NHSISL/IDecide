// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string PatientDecisionsRelativeUrl = "api/patientdecision";

        public async ValueTask<List<Decision>> GetPatientDecisionsAsync(
            DateTimeOffset? from = null,
            string? decisionType = null)
        {
            var queryParameters = new List<string>();

            if (from.HasValue)
            {
                queryParameters.Add(
                    $"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
            }

            if (!string.IsNullOrWhiteSpace(decisionType))
            {
                queryParameters.Add(
                    $"decisionType={Uri.EscapeDataString(decisionType)}");
            }

            string url = $"{PatientDecisionsRelativeUrl}/PatientDecision";

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }

            return await this.apiFactoryClient.GetContentAsync<List<Decision>>(url);
        }
    }
}

