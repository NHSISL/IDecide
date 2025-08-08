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
            var testPatients = this.apiBroker.configuration
                .GetSection("FakeFHIRProviderConfigurations:FakePatients")
                .Get<List<FakeFhirPatient>>();

            FakeFhirPatient patient = testPatients.Where(patient => patient.Surname == surname).FirstOrDefault();
            Patient redactedPatient = GetRedactedPatient(patient);

            return redactedPatient;
        }

        private static Patient GetRedactedPatient(FakeFhirPatient patient)
        {
            Patient redactedPatient = new Patient
            {
                Address = RedactAddress(patient.Address),
                DateOfBirth = patient.DateOfBirth,
                Email = RedactEmail(patient.Email),
                GivenName = RedactNames(patient.GivenNames),
                NhsNumber = patient.NhsNumber,
                Phone = RedactPhoneNumber(patient.Phone),
                PostCode = RedactPostcode(patient.Address),
                Surname = RedactName(patient.Surname),
            };

            return redactedPatient;
        }

        private static string RedactAddress(string address)
        {
            var tokens = Regex.Matches(address, @"\w+|[^\w\s]+|\s+")
                      .Cast<Match>()
                      .Select(m => m.Value)
                      .ToList();

            return string.Concat(tokens.Select(token =>
            {
                if (Regex.IsMatch(token, @"^\d+$"))
                    return token;

                if (Regex.IsMatch(token, @"^[A-Za-z]+$"))
                    return token.Length > 0 ? token[0] + new string('*', token.Length - 1) : token;

                return token;
            }));
        }

        private static string RedactName(string name)
        {
            string[] words = name.Split(' ');

            string redactedString = string.Join(" ", words.Select(word =>
                word.Length > 0 ? word[0] + new string('*', word.Length - 1) : word));

            return redactedString;
        }

        private static string RedactNames(List<string> names)
        {
            string redactedName = "";
            foreach (string name in names) {
                string[] words = name.Split(' ');

                string redactedString = string.Join(" ", words.Select(word =>
                    word.Length > 0 ? word[0] + new string('*', word.Length - 1) : word));

                redactedName = $"{redactedName} {redactedString}";
            }

            return redactedName;
        }

        private static string RedactEmail(string email)
        {
            string[] parts = email.Split('@');

            if (parts.Length != 2)
            {
                return email;
            }

            string[] usernameParts = parts[0].Split('.');

            string redactedUsername = string.Join(".", usernameParts.Select(word =>
                word.Length > 0 ? word[0] + new string('*', word.Length - 1) : word));

            string providerPart = parts[1];

            string fullRedactedEmail = $"{redactedUsername}@{providerPart}";

            return fullRedactedEmail;
        }

        private static string RedactPhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number) || number.Length < 4)
            {
                return number;
            }

            string redactedPhoneNumber = number.Substring(0, 2) +
                new string('*', number.Length - 4) +
                    number.Substring(number.Length - 2);

            return redactedPhoneNumber;
        }

        private static string RedactPostcode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return address;
            }

            string[] addressLines = address.Split(',');
            string postcode = addressLines.Last();

            if (string.IsNullOrWhiteSpace(postcode) || postcode.Length < 2)
            {
                return postcode;
            }

            string redactedPostcode = postcode.Substring(0, postcode.Length - 2) + "**";

            return redactedPostcode;
        }
    }
}