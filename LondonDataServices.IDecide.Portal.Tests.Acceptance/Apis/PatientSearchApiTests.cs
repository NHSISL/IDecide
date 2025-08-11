// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Server.Models.PatientSearches;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Patient = LondonDataServices.IDecide.Portal.Server.Models.PatientSearches.Patient;
using System.Linq;
using System.Text.RegularExpressions;

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

        private Patient GetPatient(string surname)
        {
            var redactedPatient = this.apiBroker.configuration
                .GetSection("FakeFHIRProviderConfigurations:AcceptanceFakePatientRedactedPatient")
                .Get<Patient>();

            return redactedPatient;
        }
    }
}