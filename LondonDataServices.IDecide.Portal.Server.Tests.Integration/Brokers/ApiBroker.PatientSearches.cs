// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string PatientSearchesRelativeUrl = "api/patientsearch";

        public async ValueTask<Patient> PostPatientSearchAsync(PatientLookup patientLookup)
        {
            return await this.anonymousApiFactoryClient.PostContentAsync<PatientLookup, Patient>(
                $"{PatientSearchesRelativeUrl}/PatientSearch", patientLookup);
        }
    }
}