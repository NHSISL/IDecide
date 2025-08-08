// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public class PatientService : IPatientService
    {
        public ValueTask<Patient> AddPatientAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

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