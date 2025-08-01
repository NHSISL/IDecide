// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public interface IPdsService
    {
        ValueTask<PatientLookup> PatientLookupByDetailsAsync(PatientLookup patientLookup);

        ValueTask<Patient> PatientLookupByNhsNumberAsync(string nhsNumber);
    }
}
