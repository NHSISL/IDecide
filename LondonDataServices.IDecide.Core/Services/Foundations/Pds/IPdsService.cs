// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public interface IPdsService
    {
        ValueTask<PatientLookup> PatientLookupByDetailsAsync(PatientLookup patientLookup);
        ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber);
    }
}
