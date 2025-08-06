// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System;
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
            throw new NotImplementedException();
        }

        public ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber)
        {
            throw new NotImplementedException();
        }

        virtual internal Patient MapToPatientFromPatientBundle(PatientBundle patientBundle) 
        { 
            throw new NotImplementedException();
        }

        virtual internal Patient MapToPatientFromFhirPatient(Hl7.Fhir.Model.Patient fhirPatient)
        {
            throw new NotImplementedException();
        }
    }
}
