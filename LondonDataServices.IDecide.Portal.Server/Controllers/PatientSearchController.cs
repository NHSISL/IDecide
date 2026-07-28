// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : RESTFulController
    {
        private readonly IPatientOrchestrationService patientOrchestrationService;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public PatientSearchController(
            IPatientOrchestrationService patientOrchestrationService,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.patientOrchestrationService = patientOrchestrationService;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        [HttpPost("PatientSearch")]
        public async ValueTask<ActionResult<Patient>> PostPatientSearchAsync([FromBody] PatientLookup patientLookup)
        {
            try
            {
                string userId = await this.securityAuditBroker.GetCurrentUserIdAsync();

                string searchTerm = !string.IsNullOrWhiteSpace(patientLookup.SearchCriteria?.NhsNumber)
                    ? $"NhsNumber={patientLookup.SearchCriteria.NhsNumber}"
                    : "SearchByDetails";

                await this.loggingBroker.LogInformationAsync(
                    $"[Audit] Patient lookup initiated | User: {userId} | Search: {searchTerm}");

                Patient patient = await this.patientOrchestrationService.PatientLookupAsync(patientLookup);

                await this.loggingBroker.LogInformationAsync(
                    $"[Audit] Patient lookup succeeded | User: {userId} | Search: {searchTerm}");

                return Ok(patient);
            }
            catch (PatientOrchestrationValidationException patientOrchestrationValidationException)
            {
                return BadRequest(patientOrchestrationValidationException.InnerException);
            }
            catch (PatientOrchestrationDependencyValidationException
                patientOrchestrationDependencyValidationException)
            {
                return BadRequest(patientOrchestrationDependencyValidationException.InnerException);
            }
            catch (PatientOrchestrationDependencyException patientOrchestrationDependencyException)
            {
                return InternalServerError(patientOrchestrationDependencyException.InnerException);
            }
            catch (PatientOrchestrationServiceException patientOrchestrationServiceException)
                when (patientOrchestrationServiceException.InnerException is ExternalOptOutPatientOrchestrationException)
            {
                return InternalServerError(patientOrchestrationServiceException.InnerException);
            }
            catch (PatientOrchestrationServiceException patientOrchestrationServiceException)
            {
                return InternalServerError(patientOrchestrationServiceException.InnerException);
            }
        }
    }
}
