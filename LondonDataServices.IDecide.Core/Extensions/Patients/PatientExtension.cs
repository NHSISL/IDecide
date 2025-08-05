// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using Hl7.FhirPath.Sprache;
using System.Text.RegularExpressions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Extensions.Patients
{
    public static class PatientExtension
    {
        public static Patient Redact(this Patient patient)
        {
            Patient redactedPatient = new Patient
            {
                Address = RedactAddress(patient.Address),
                DateOfBirth = patient.DateOfBirth,
                EmailAddress = RedactEmail(patient.EmailAddress),
                FirstName = RedactNames(patient.FirstName),
                NhsNumber = patient.NhsNumber,
                PhoneNumber = RedactPhoneNumber(patient.PhoneNumber),
                Postcode = RedactPostcode(patient.Postcode),
                Surname = RedactNames(patient.Surname)
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

        private static string RedactNames(string name)
        {
            string[] words = name.Split(' ');

            string redactedString = string.Join(" ", words.Select(word =>
                word.Length > 0 ? word[0] + new string('*', word.Length - 1) : word));

            return redactedString;
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

        private static string RedactPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode) || postcode.Length < 2)
            {
                return postcode;
            }

            string redactedPostcode = postcode.Substring(0, postcode.Length - 2) + "**";

            return redactedPostcode;
        }
    }
}
