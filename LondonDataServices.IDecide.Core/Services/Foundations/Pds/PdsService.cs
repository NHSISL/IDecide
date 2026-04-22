// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;
using System.Threading;
using NhsDigitalSearchCriteria = NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.SearchCriteria;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public partial class PdsService : IPdsService
    {
        private readonly IPdsBroker pdsBroker;
        private readonly INhsDigitalApiBroker nhsDigitalApiBroker;
        private readonly ILoggingBroker loggingBroker;

        public PdsService(
            IPdsBroker pdsBroker,
            INhsDigitalApiBroker nhsDigitalApiBroker,
            ILoggingBroker loggingBroker)
        {
            this.pdsBroker = pdsBroker;
            this.nhsDigitalApiBroker = nhsDigitalApiBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<PatientLookup> PatientLookupByDetailsAsync(PatientLookup patientLookup) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupIsNotNull(patientLookup);
                SearchCriteria searchCriteria = patientLookup.SearchCriteria;

                NhsDigitalSearchCriteria nhsDigitalSearchCriteria =
                    MapToNhsDigitalSearchCriteria(searchCriteria);

                string pdsJsonResponse =
                    await this.nhsDigitalApiBroker.SearchPatientPDSAsync(
                        nhsDigitalSearchCriteria,
                        CancellationToken.None);

                PatientLookup updatedPatientLookup = new PatientLookup
                {
                    SearchCriteria = searchCriteria,
                    Patients = MapToPatientsFromBundleJson(pdsJsonResponse)
                };

                return updatedPatientLookup;
            });

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber) =>
            TryCatch(async () =>
            {
                ValidatePatientLookupByNhsNumberArguments(nhsNumber);

                NhsDigitalSearchCriteria nhsDigitalSearchCriteria = new NhsDigitalSearchCriteria
                {
                    NhsNumber = nhsNumber
                };

                string pdsJsonResponse =
                   await this.nhsDigitalApiBroker.SearchPatientPDSAsync(
                       nhsDigitalSearchCriteria,
                       CancellationToken.None);

                List<Patient> patients = MapToPatientsFromPatientBundle(pdsJsonResponse);
                Patient patient = patients.FirstOrDefault();

                return patient;
            });

        virtual internal List<Patient> MapToPatientsFromPatientBundle(string patientBundle)
        {
            ValidateJsonStringIsNotNull(patientBundle);
            var parser = new FhirJsonParser();
            Hl7.Fhir.Model.Patient fhirPatient = parser.Parse<Hl7.Fhir.Model.Patient>(patientBundle);

            Patient patient = MapToPatientFromFhirPatient(fhirPatient);

            return new List<Patient> { patient };
        }

        virtual internal List<Patient> MapToPatientsFromBundleJson(string bundleJson)
        {
            ValidateJsonStringIsNotNull(bundleJson);
            var parser = new FhirJsonParser();
            Bundle bundle = parser.Parse<Bundle>(bundleJson);

            List<Patient> patients = new List<Patient>();

            foreach (Bundle.EntryComponent entry in bundle.Entry)
            {
                Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient)entry.Resource;
                Patient patient = MapToPatientFromFhirPatient(fhirPatient);
                patients.Add(patient);
            }

            return patients;
        }

        virtual internal Patient MapToPatientFromFhirPatient(Hl7.Fhir.Model.Patient fhirPatient)
        {
            ValidateFhirPatientIsNotNull(fhirPatient);

            var isSensitivePatient = fhirPatient.Meta?.Security?.FirstOrDefault()?.Code == "R";

            if (isSensitivePatient)
            {
                Patient partialPatient = new Patient
                {
                    GivenName = GetFirstName(fhirPatient),
                    Surname = GetSurname(fhirPatient),
                    IsSensitive = true
                };

                return partialPatient;
            }

            Patient patient = new Patient
            {
                NhsNumber = fhirPatient.Id,
                Address = GetCurrentAddressString(fhirPatient),
                DateOfBirth = DateTimeOffset.Parse(fhirPatient.BirthDate),
                Email = GetCurrentEmail(fhirPatient),
                GivenName = GetFirstName(fhirPatient),
                Surname = GetSurname(fhirPatient),
                PostCode = GetCurrentPostcode(fhirPatient),
                Phone = GetCurrentPhoneNumber(fhirPatient),
                Gender = GetPatientGender(fhirPatient),
                Title = GetPatientTitle(fhirPatient),
                IsSensitive = false
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

        private static string GetCurrentAddressString(Hl7.Fhir.Model.Patient patient)
        {
            string address = patient.Address
                .Where(address => address.Use == Address.AddressUse.Home)
                .OrderByDescending(address => ParseEndDate(address.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(address => address.Period?.Start)
                .Select(a => BuildUkAddressString(a))
                .FirstOrDefault();

            return address;
        }

        private static string GetCurrentPostcode(Hl7.Fhir.Model.Patient patient)
        {
            string postcode = patient.Address
                .Where(address => address.Use == Address.AddressUse.Home)
                .OrderByDescending(address => ParseEndDate(address.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(address => address.Period?.Start)
                .Select(address => address.PostalCode)
                .FirstOrDefault();

            return postcode;
        }

        private static string GetCurrentEmail(Hl7.Fhir.Model.Patient patient)
        {
            string email = patient.Telecom
                .Where(telecom => telecom.System == ContactPoint.ContactPointSystem.Email)
                .OrderByDescending(telecom => ParseEndDate(telecom.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(telecom => telecom.Period?.Start)
                .Select(telecom => telecom.Value)
                .FirstOrDefault();

            return email;
        }

        private static string GetCurrentPhoneNumber(Hl7.Fhir.Model.Patient patient)
        {
            string phoneNumber = patient.Telecom
                .Where(telecom => telecom.System == ContactPoint.ContactPointSystem.Phone)
                .Where(telecom => telecom.Use == ContactPoint.ContactPointUse.Mobile)
                .OrderByDescending(telecom => ParseEndDate(telecom.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(telecom => telecom.Period?.Start)
                .Select(telecom => telecom.Value)
                .FirstOrDefault();

            return phoneNumber;
        }

        private static string GetFirstName(Hl7.Fhir.Model.Patient patient)
        {
            string firstNameString = patient.Name
                .Where(name => name.Use == HumanName.NameUse.Usual)
                .OrderByDescending(name => ParseEndDate(name.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(name => name.Period?.Start)
                .Select(name => string.Join(' ', name.Given))
                .FirstOrDefault();

            return firstNameString;
        }

        private static string GetSurname(Hl7.Fhir.Model.Patient patient)
        {
            string surname = patient.Name
                .Where(name => name.Use == HumanName.NameUse.Usual)
                .OrderByDescending(name => ParseEndDate(name.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(name => name.Period?.Start)
                .Select(name => name.Family)
                .FirstOrDefault();

            return surname;
        }

        private static string GetPatientGender(Hl7.Fhir.Model.Patient patient)
        {
            string gender = patient.Gender.ToString();

            return gender;
        }

        private static string GetPatientTitle(Hl7.Fhir.Model.Patient patient)
        {
            string title = patient.Name
                .Where(name => name.Use == HumanName.NameUse.Usual)
                .OrderByDescending(name => ParseEndDate(name.Period?.End) ?? DateTimeOffset.MaxValue)
                .OrderByDescending(name => name.Period?.Start)
                .Select(name => string.Join(' ', name.Prefix))
                .FirstOrDefault();

            return title;
        }

        virtual internal NhsDigitalSearchCriteria MapToNhsDigitalSearchCriteria(SearchCriteria searchCriteria)
        {
            NhsDigitalSearchCriteria nhsDigitalSearchCriteria = new NhsDigitalSearchCriteria
            {
                NhsNumber = searchCriteria.NhsNumber,
                FirstName = searchCriteria.FirstName,
                Surname = searchCriteria.Surname,
                Postcode = searchCriteria.Postcode,
                DateOfBirth = searchCriteria.DateOfBirth,
            };

            return nhsDigitalSearchCriteria;
        }
    }
}
