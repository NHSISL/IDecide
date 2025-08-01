// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public partial class PdsService : IPdsService
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

        public ValueTask<PatientLookup> PatientLookupByDetailsAsync(PatientLookup patientLookup) =>
            TryCatch(async () =>
            {
                SearchCriteria searchCriteria = patientLookup.SearchCriteria;
                PatientBundle patientBundle = await this.pdsBroker.PatientLookupByDetailsAsync(
                    searchCriteria.FirstName,
                    searchCriteria.Surname,
                    searchCriteria.Gender,
                    searchCriteria.Postcode,
                    searchCriteria.DateOfBirth,
                    searchCriteria.DateOfDeath,
                    searchCriteria.RegisteredGpPractice,
                    searchCriteria.Email,
                    searchCriteria.PhoneNumber);

                PatientLookup updatedPatientLookup = new PatientLookup
                {
                    SearchCriteria = searchCriteria,
                    Patients = patientBundle
                };

                return updatedPatientLookup;
            });

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
