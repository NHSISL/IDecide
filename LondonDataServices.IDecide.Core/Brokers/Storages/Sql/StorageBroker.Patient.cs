// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        public DbSet<Patient> Patients { get; set; }

        public async ValueTask<Patient> InsertPatientAsync(Patient patient) =>
            await InsertAsync(patient);

        public async ValueTask<IQueryable<Patient>> SelectAllPatientsAsync() =>
            await SelectAllAsync<Patient>();

        public async ValueTask<Patient> SelectPatientByIdAsync(Guid patientId) =>
            await SelectAsync<Patient>(patientId);

        public async ValueTask<Patient> UpdatePatientAsync(Patient patient) =>
            await UpdateAsync(patient);

        public async ValueTask<Patient> DeletePatientAsync(Patient patient) =>
            await DeleteAsync(patient);
    }
}
