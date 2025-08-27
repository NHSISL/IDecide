// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public partial class PatientService : IPatientService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public PatientService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Patient> AddPatientAsync(Patient patient) =>
            TryCatch(async () =>
            {
                patient = await this.securityAuditBroker.ApplyAddAuditValuesAsync(patient);
                await ValidatePatientOnAdd(patient);

                return await this.storageBroker.InsertPatientAsync(patient);
            });

        public ValueTask<IQueryable<Patient>> RetrieveAllPatientsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllPatientsAsync());

        public ValueTask<Patient> RetrievePatientByIdAsync(Guid patientId) =>
            TryCatch(async () =>
            {
                ValidatePatientId(patientId);

                Patient maybePatient = await this.storageBroker
                    .SelectPatientByIdAsync(patientId);

                ValidateStoragePatient(maybePatient, patientId);

                return maybePatient;
            });

        public ValueTask<Patient> ModifyPatientAsync(Patient patient) =>
            TryCatch(async () =>
            {
                patient = await this.securityAuditBroker.ApplyModifyAuditValuesAsync(patient);

                await ValidatePatientOnModify(patient);

                Patient maybePatient =
                    await this.storageBroker.SelectPatientByIdAsync(patient.Id);

                ValidateStoragePatient(maybePatient, patient.Id);

                patient = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(patient, maybePatient);

                ValidateAgainstStoragePatientOnModify(
                    inputPatient: patient,
                    storagePatient: maybePatient);

                return await this.storageBroker.UpdatePatientAsync(patient);
            });

        public ValueTask<Patient> RemovePatientByIdAsync(Guid patientId) =>
            TryCatch(async () =>
            {
                ValidatePatientId(patientId);

                Patient maybePatient = await this.storageBroker
                    .SelectPatientByIdAsync(patientId);

                ValidateStoragePatient(maybePatient, patientId);

                return await this.storageBroker.DeletePatientAsync(maybePatient);
            });

        public async ValueTask<string> GenerateValidationCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(5);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[1];

                for (int i = 0; i < 5; i++)
                {
                    int index;
                    rng.GetBytes(buffer);
                    index = buffer[0] % chars.Length;
                    result.Append(chars[index]);
                }
            }

            return result.ToString();
        }
    }
}