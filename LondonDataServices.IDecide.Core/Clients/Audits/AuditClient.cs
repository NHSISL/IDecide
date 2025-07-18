﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Clients.AuditClient.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits.Exceptions;
using LondonDataServices.IDecide.Core.Services.Audits;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Clients.Audits
{
    public class AuditClient : IAuditClient
    {
        private readonly IAuditService auditService;

        public AuditClient(IAuditService auditService) =>
            this.auditService = auditService;

        public async ValueTask<Audit> LogAuditAsync(
            string auditType,
            string title,
            string message,
            string fileName,
            string correlationId,
            string logLevel = "Information")
        {
            try
            {
                return await auditService
                    .AddAuditAsync(auditType, title, message, fileName, correlationId, logLevel);
            }
            catch (AuditValidationException auditValidationException)
            {
                throw new AuditClientValidationException(
                    message: "Audit client validation error occurred, fix errors and try again.",
                    innerException: auditValidationException.InnerException as Xeption);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                throw new AuditClientValidationException(
                    message: "Audit client validation error occurred, fix errors and try again.",
                    innerException: auditDependencyValidationException.InnerException as Xeption);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                throw new AuditClientDependencyException(
                    message: "Audit client dependency error occurred, please contact support.",
                    innerException: auditDependencyException.InnerException as Xeption);
            }
            catch (AuditServiceException auditServiceException)
            {
                throw new AuditClientServiceException(
                    message: "Audit client service error occurred, fix errors and try again.",
                    auditServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask BulkLogAuditsAsync(List<Audit> audits)
        {
            try
            {
                await auditService.BulkAddAuditsAsync(audits);
            }
            catch (AuditValidationException auditValidationException)
            {
                throw new AuditClientValidationException(
                    message: "Audit client validation error occurred, fix errors and try again.",
                    innerException: auditValidationException.InnerException as Xeption);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                throw new AuditClientValidationException(
                    message: "Audit client validation error occurred, fix errors and try again.",
                    innerException: auditDependencyValidationException.InnerException as Xeption);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                throw new AuditClientDependencyException(
                    message: "Audit client dependency error occurred, please contact support.",
                    innerException: auditDependencyException.InnerException as Xeption);
            }
            catch (AuditServiceException auditServiceException)
            {
                throw new AuditClientServiceException(
                    message: "Audit client service error occurred, fix errors and try again.",
                    auditServiceException.InnerException as Xeption);
            }
        }
    }
}
