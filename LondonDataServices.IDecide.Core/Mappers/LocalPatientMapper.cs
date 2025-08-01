// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using ISL.Providers.PDS.Abstractions.Models;

namespace LondonDataServices.IDecide.Core.Mappers
{
    public class LocalPatientMapper
    {
        public static Models.Foundations.Pds.Patient FromPatientBundle(PatientBundle patientBundle)
        {
            Patient bundlePatient = patientBundle.Patients.FirstOrDefault();
            Models.Foundations.Pds.Patient patient = FromFhirPatient(bundlePatient);

            return patient;
        }

        public static Models.Foundations.Pds.Patient FromFhirPatient(Patient fhirPatient)
        {
            Models.Foundations.Pds.Patient patient = new Models.Foundations.Pds.Patient
            {
                NhsNumber = fhirPatient.Id,
                Address = GetCurrentAddressString(fhirPatient),
                DateOfBirth = DateTimeOffset.Parse(fhirPatient.BirthDate),
                EmailAddress = GetCurrentEmail(fhirPatient),
                FirstName = GetFirstName(fhirPatient),
                Surname = GetSurname(fhirPatient),
                Postcode = GetCurrentPostcode(fhirPatient),
                PhoneNumber = GetCurrentPhoneNumber(fhirPatient)
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

        private static string GetCurrentAddressString(Patient patient)
        {
            string address = patient.Address
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(a => BuildUkAddressString(a))
                .FirstOrDefault();

            return address;
        }

        private static string GetCurrentPostcode(Patient patient)
        {
            string postcode = patient.Address
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(a => a.PostalCode)
                .FirstOrDefault();

            return postcode;
        }

        private static string GetCurrentEmail(Patient patient)
        {
            string email = patient.Telecom
                .Where(t => t.System == ContactPoint.ContactPointSystem.Email)
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(t => t.Value)
                .FirstOrDefault();

            return email;
        }

        private static string GetCurrentPhoneNumber(Patient patient)
        {
            string phoneNumber = patient.Telecom
                .Where(t => t.System == ContactPoint.ContactPointSystem.Phone)
                .OrderByDescending(t => ParseEndDate(t.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(t => t.Value)
                .FirstOrDefault();

            return phoneNumber;
        }

        private static string GetFirstName(Patient patient)
        {
            string firstNameString = patient.Name
                .OrderByDescending(n => ParseEndDate(n.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(n => string.Join(' ', n.Given))
                .FirstOrDefault();

            return firstNameString;
        }

        private static string GetSurname(Patient patient)
        {
            string surname = patient.Name
                .OrderByDescending(n => ParseEndDate(n.Period?.End) ?? DateTimeOffset.MaxValue)
                .Select(n => n.Family)
                .FirstOrDefault();

            return surname;
        }
    }
}