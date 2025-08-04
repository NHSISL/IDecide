// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public partial class PdsService : IPdsService
    {
        private readonly ILoggingBroker loggingBroker;

        public PdsService(ILoggingBroker loggingBroker)
        {
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<PatientLookup> PatientLookupByDetailsAsync(PatientLookup patientLookup)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
