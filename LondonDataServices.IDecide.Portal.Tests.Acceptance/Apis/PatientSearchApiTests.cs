// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Server.Models.PatientSearches;
using Microsoft.Extensions.Configuration;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientSearchApiTests
    {
        private readonly ApiBroker apiBroker;

        public PatientSearchApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private PatientLookup GetRandomSearchPatientLookup(string surname)
        {
            SearchCriteria searchCriteria = new SearchCriteria
            {
                Surname = surname
            };

            PatientLookup randomPatientLookup = new PatientLookup
            {
                SearchCriteria = searchCriteria
            };

            return randomPatientLookup;
        }

        private Patient GetPatient()
        {
            var redactedPatient = this.apiBroker.configuration
                .GetSection("FakeFHIRProviderConfigurations:AcceptanceFakePatientRedactedPatient")
                .Get<Patient>();

            return redactedPatient;
        }
    }
}