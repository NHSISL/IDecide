// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Securities;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Audits
{
    public partial class AuditService : IAuditService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public AuditService(
            IStorageBroker storageBroker,
            IIdentifierBroker identifierBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.identifierBroker = identifierBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Audit> AddAuditAsync(
            string auditType,
            string title,
            string? message,
            string? fileName,
            string? correlationId,
            string? logLevel = "Information") =>
        TryCatch(async () =>
        {
            DateTimeOffset dateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();

            Audit audit = new Audit
            {
                Id = await this.identifierBroker.GetIdentifierAsync(),
                AuditType = auditType,
                Title = title,
                Message = message,
                CorrelationId = correlationId,
                FileName = fileName,
                LogLevel = logLevel,
                CreatedBy = auditUser?.UserId.ToString() ?? string.Empty,
                CreatedDate = dateTimeOffset,
                UpdatedBy = auditUser?.UserId.ToString() ?? string.Empty,
                UpdatedDate = dateTimeOffset,
            };

            await ValidateAuditOnAddAsync(audit);

            return await this.storageBroker.InsertAuditAsync(audit);
        });

        public ValueTask<Audit> AddAuditAsync(Audit audit) =>
            TryCatch(async () =>
            {
                Audit auditWithAddAuditApplied = await ApplyAddAuditAsync(audit);
                await ValidateAuditOnAddAsync(auditWithAddAuditApplied);

                return await this.storageBroker.InsertAuditAsync(auditWithAddAuditApplied);
            });

        public ValueTask BulkAddAuditsAsync(List<Audit> audits, int batchSize = 10000) =>
        TryCatch(async () =>
        {
            ValidateOnBulkAddAudits(audits);
            await BatchBulkAddAuditsAsync(audits, batchSize);
        });

        public ValueTask<IQueryable<Audit>> RetrieveAllAuditsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllAuditsAsync());

        public ValueTask<Audit> RetrieveAuditByIdAsync(Guid auditId) =>
        TryCatch(async () =>
        {
            ValidateAuditId(auditId);

            Audit maybeAudit = await this.storageBroker
                .SelectAuditByIdAsync(auditId);

            ValidateStorageAudit(maybeAudit, auditId);

            return maybeAudit;
        });

        virtual internal async ValueTask BatchBulkAddAuditsAsync(List<Audit> audits, int batchSize)
        {
            int totalRecords = audits.Count;
            var exceptions = new List<Exception>();

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                try
                {
                    var batch = audits.Skip(i).Take(batchSize).ToList();

                    if (batch.Count != 0)
                    {
                        List<Audit> validatedAudits = await ValidateAuditsAndAssignIdAndAuditAsync(batch);
                        await this.storageBroker.BulkInsertAuditsAsync(validatedAudits);
                    }
                }
                catch (Exception ex)
                {
                    await this.loggingBroker.LogErrorAsync(ex);
                }
            }
        }

        public ValueTask<Audit> ModifyAuditAsync(Audit audit) =>
            TryCatch(async () =>
            {
                Audit auditWithModifyAuditApplied = await ApplyModifyAuditAsync(audit);
                await ValidateAuditOnModifyAsync(auditWithModifyAuditApplied);
                Audit maybeAudit = await this.storageBroker.SelectAuditByIdAsync(audit.Id);
                ValidateStorageAudit(maybeAudit, audit.Id);
                ValidateAgainstStorageAuditOnModify(inputAudit: audit, storageAudit: maybeAudit);

                return await this.storageBroker.UpdateAuditAsync(auditWithModifyAuditApplied);
            });

        public ValueTask<Audit> RemoveAuditByIdAsync(Guid auditId) =>
        TryCatch(async () =>
        {
            ValidateAuditId(auditId);

            Audit maybeAudit = await this.storageBroker
                .SelectAuditByIdAsync(auditId);

            ValidateStorageAudit(maybeAudit, auditId);

            Audit auditWithDeleteAuditApplied =
                await ApplyDeleteAuditAsync(maybeAudit);

            Audit updatedAudit =
                await this.storageBroker.UpdateAuditAsync(auditWithDeleteAuditApplied);

            await ValidateAgainstStorageAuditOnDeleteAsync(
                audit: updatedAudit,
                maybeAudit: auditWithDeleteAuditApplied);

            return await this.storageBroker.DeleteAuditAsync(updatedAudit);
        });

        virtual internal async ValueTask<Audit> ApplyAddAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.CreatedBy = auditUser?.UserId.ToString() ?? string.Empty;
            audit.CreatedDate = auditDateTimeOffset;
            audit.UpdatedBy = auditUser?.UserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }

        virtual internal async ValueTask<Audit> ApplyModifyAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.UpdatedBy = auditUser?.UserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }

        virtual internal async ValueTask<Audit> ApplyDeleteAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.UpdatedBy = auditUser?.UserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }

        virtual internal async ValueTask<List<Audit>> ValidateAuditsAndAssignIdAndAuditAsync(List<Audit> audits)
        {
            List<Audit> validatedAudites = new List<Audit>();

            foreach (Audit address in audits)
            {
                try
                {
                    User currentUser = await this.securityBroker.GetCurrentUserAsync();
                    var currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                    address.Id = await this.identifierBroker.GetIdentifierAsync();
                    address.CreatedDate = currentDateTime;
                    address.CreatedBy = currentUser.UserId;
                    address.UpdatedDate = address.CreatedDate;
                    address.UpdatedBy = address.CreatedBy;
                    await ValidateAuditOnAddAsync(address);
                    validatedAudites.Add(address);
                }
                catch (Exception ex)
                {
                    await this.loggingBroker.LogErrorAsync(ex);
                }
            }

            return await ValueTask.FromResult(validatedAudites);
        }
    }
}