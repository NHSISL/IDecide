// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string pdsRelativeUrl = "api/PatientSearch";

        public async ValueTask<Patient> PostPatientSearchAsync(PatientLookup patientLookup)
        {
            return await this.apiFactoryClient.PostContentAsync<PatientLookup, Patient>(
                $"{pdsRelativeUrl}/PostPatientSearch",  patientLookup);

        }
    }
}
