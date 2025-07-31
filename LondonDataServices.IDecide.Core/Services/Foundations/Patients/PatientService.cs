// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using System.Linq;
using System.Threading.Tasks;
using System;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Brokers.Loggings;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public partial class PatientService : IPatientService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public PatientService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Patient> AddPatientAsync(Patient patient) =>
            TryCatch(async () =>
            {
                await ValidatePatientOnAdd(patient);

                return await this.storageBroker.InsertPatientAsync(patient);
            });

        public ValueTask<Patient> ModifyPatientAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Patient> RemovePatientByIdAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IQueryable<Patient>> RetrieveAllPatientsAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<Patient> RetrievePatientByIdAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }
    }
}
