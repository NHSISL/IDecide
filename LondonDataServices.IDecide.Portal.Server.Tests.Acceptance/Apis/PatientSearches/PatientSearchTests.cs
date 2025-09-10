// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientSearches
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientSearchTests
    {
        private readonly ApiBroker apiBroker;

        public PatientSearchTests(ApiBroker apiBroker) =>
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
    }
}
