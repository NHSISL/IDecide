// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;

namespace LondonDataServices.IDecide.Core.Mappers
{
    public class LocalPatientMapper
    {
        public static Models.Foundations.Pds.Patient FromPatientBundle(PatientBundle patientBundle)
        {
            Hl7.Fhir.Model.Patient bundlePatient = patientBundle.Patients.FirstOrDefault();

            string address = bundlePatient.Address
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(a => BuildUkAddressString(a))
                .FirstOrDefault();

            string postcode = bundlePatient.Address
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(a => a.PostalCode)
                .FirstOrDefault();

            string email = bundlePatient.Telecom
                .Where(t => t.System == ContactPoint.ContactPointSystem.Email)
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(t => t.Value)
                .FirstOrDefault();

            string phoneNumber = bundlePatient.Telecom
                .Where(t => t.System == ContactPoint.ContactPointSystem.Phone)
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(t => t.Value)
                .FirstOrDefault();

            string firstNameString = bundlePatient.Name
                .OrderByDescending(n => ParseEndDate(n.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(n => string.Join(' ', n.Given))
                .FirstOrDefault();

            string surname = bundlePatient.Name
                .OrderByDescending(n => ParseEndDate(n.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(n => n.Family)
                .FirstOrDefault();

            Models.Foundations.Pds.Patient patient = new Models.Foundations.Pds.Patient
            {
                NhsNumber = bundlePatient.Id,
                Address = address,
                DateOfBirth = DateTimeOffset.Parse(bundlePatient.BirthDate),
                EmailAddress = email,
                FirstName = firstNameString,
                Surname = surname,
                Postcode = postcode,
                PhoneNumber = phoneNumber
            };

            return patient;
        }

        private static DateTimeOffset? ParseEndDate(string end)
        {
            if (string.IsNullOrWhiteSpace(end)) return null;
            return DateTimeOffset.TryParse(end, out var dto) ? dto : null;
        }

        private static string BuildUkAddressString(Address address)
        {
            var parts = new[]
            {
                string.Join(", ", address.Line),
                address.City,
                address.District,
                address.PostalCode,
                address.Country
            };

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
