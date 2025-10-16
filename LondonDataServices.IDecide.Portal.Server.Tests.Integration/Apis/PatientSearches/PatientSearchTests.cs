// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.PatientSearches
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientSearchTests
    {
        private readonly ApiBroker apiBroker;

        public PatientSearchTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private PatientLookup GetRandomSearchPatientLookup(string nhsNumber)
        {
            SearchCriteria searchCriteria = new SearchCriteria
            {
                NhsNumber = nhsNumber
            };

            PatientLookup randomPatientLookup = new PatientLookup
            {
                SearchCriteria = searchCriteria
            };

            return randomPatientLookup;
        }

        private Patient GetExpectedPatient()
        {
            var patient = new Patient
            {
                Address = "1***, D****** R***, E******, E** 7**",
                CreatedBy = null,
                CreatedDate = default,
                DateOfBirth = new DateTimeOffset(1997, 2, 1, 0, 0, 0, TimeSpan.Zero),
                Email = "j***.d***@example.com",
                Gender = "Male",
                GivenName = "J***",
                Id = default,
                NhsNumber = "9449304424",
                NotificationPreference = NotificationPreference.Email,
                Phone = null,
                PostCode = "EN3 7**",
                RetryCount = 0,
                Surname = "Test",
                Title = "Mr",
                UpdatedBy = null,
                UpdatedDate = default,
                ValidationCode = null,
                ValidationCodeExpiresOn = default,
                ValidationCodeMatchedOn = null
            };

            return patient;
        }
    }
}
