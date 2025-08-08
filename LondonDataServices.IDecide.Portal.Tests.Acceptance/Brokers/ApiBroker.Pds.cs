// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Models.PatientSearches;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string pdsRelativeUrl = "api/PatientSearch";

        public async ValueTask<Patient> PostPatientByDetailsAsync(PatientLookup patientLookup) =>
            await this.apiFactoryClient.PostContentAsync<PatientLookup, Patient>($"{pdsRelativeUrl}/PostPatientByDetails",  patientLookup);
    }
}
