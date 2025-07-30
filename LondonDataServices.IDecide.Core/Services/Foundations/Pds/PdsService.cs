// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Mappers;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public class PdsService : IPdsService
    {
        private readonly IPdsBroker pdsBroker;
        private readonly ILoggingBroker loggingBroker;

        public PdsService(
            IPdsBroker pdsBroker,
            ILoggingBroker loggingBroker)
        {
            this.pdsBroker = pdsBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Patient> PatientLookupByDetailsAsync(
            string givenName = null,
            string familyName = null,
            string gender = null,
            string postcode = null,
            string dateOfBirth = null,
            string dateOfDeath = null,
            string registeredGpPractice = null,
            string email = null,
            string phoneNumber = null)
        {
            PatientBundle patientBundle = await this.pdsBroker.PatientLookupByDetailsAsync(
                givenName,
                familyName,
                gender,
                postcode,
                dateOfBirth,
                dateOfDeath,
                registeredGpPractice,
                email,
                phoneNumber);

            Patient patient = LocalPatientMapper.FromPatientBundle(patientBundle);

            return patient;
        }

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
