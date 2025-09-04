// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    public class GenerateCodeRequest
    {
        public string NhsNumber { get; set; }
        public string NotificationPreference { get; set; }
        public string PoaFirstName { get; set; }
        public string PoaSurname { get; set; }
        public string PoaRelationship { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : RESTFulController
    {
        private readonly IPatientOrchestrationService patientOrchestrationService;

        public PatientSearchController(IPatientOrchestrationService patientOrchestrationService)
        {
            this.patientOrchestrationService = patientOrchestrationService;
        }

        [HttpPost("PostPatientSearch")]
        public async ValueTask<ActionResult<Patient>> PostPatientSearchAsync([FromBody] PatientLookup patientLookup)
        {
            try
            {
                Patient patient = await this.patientOrchestrationService.PatientLookupAsync(patientLookup);

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
                return InternalServerError(patientOrchestrationDependencyException);
            }
            catch (PatientOrchestrationServiceException patientOrchestrationServiceException)
            {
                return InternalServerError(patientOrchestrationServiceException);
            }
        }

        [HttpPost("PostPatientByNhsNumber")]
        public async ValueTask<ActionResult<Patient>> PostPatientSearchByNhsNumberAsync([FromBody] GenerateCodeRequest generateCodeRequest)
        {
            try
            {
                await this.patientOrchestrationService.RecordPatientInformationAsync(
                    generateCodeRequest.NhsNumber,
                    generateCodeRequest.NotificationPreference
                );

                return Ok();
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
                return InternalServerError(patientOrchestrationDependencyException);
            }
            catch (PatientOrchestrationServiceException patientOrchestrationServiceException)
            {
                return InternalServerError(patientOrchestrationServiceException);
            }
        }
    }
}
