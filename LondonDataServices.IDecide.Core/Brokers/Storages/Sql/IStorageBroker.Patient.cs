// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<Patient> InsertPatientAsync(Patient patient);
        ValueTask<IQueryable<Patient>> SelectAllPatientsAsync();
        ValueTask<Patient> SelectPatientByIdAsync(Guid patientId);
        ValueTask<Patient> UpdatePatientAsync(Patient patient);
        ValueTask<Patient> DeletePatientAsync(Patient patient);
    }
}
