// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string patientsRelativeUrl = "api/patients";

        public async ValueTask<Patient> PostPatientAsync(Patient patient) =>
            await this.apiFactoryClient.PostContentAsync(patientsRelativeUrl, patient);

        public async ValueTask<List<Patient>> GetAllPatientsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Patient>>(patientsRelativeUrl);

        public async ValueTask<Patient> GetPatientByIdAsync(Guid patientId) =>
            await this.apiFactoryClient.GetContentAsync<Patient>($"{patientsRelativeUrl}/{patientId}");

        public async ValueTask<Patient> PutPatientAsync(Patient patient) =>
            await this.apiFactoryClient.PutContentAsync(patientsRelativeUrl, patient);

        public async ValueTask<Patient> DeletePatientByIdAsync(Guid patientId) =>
            await this.apiFactoryClient.DeleteContentAsync<Patient>($"{patientsRelativeUrl}/{patientId}");
    }
}