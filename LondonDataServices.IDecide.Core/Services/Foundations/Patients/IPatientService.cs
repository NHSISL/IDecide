// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public interface IPatientService
    {
        ValueTask<Patient> AddPatientAsync(Patient patient);
        ValueTask<IQueryable<Patient>> RetrieveAllPatientsAsync();
        ValueTask<Patient> RetrievePatientByIdAsync(Guid patientId);
        ValueTask<Patient> ModifyPatientAsync(Patient patient);
        ValueTask<Patient> RemovePatientByIdAsync(Guid patientId);
    }
}