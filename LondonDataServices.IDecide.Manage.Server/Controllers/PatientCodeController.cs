// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Manage.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientCodeController : RESTFulController
    {
        private readonly IPatientOrchestrationService patientOrchestrationService;

        public PatientCodeController(IPatientOrchestrationService patientOrchestrationService)
        {
            this.patientOrchestrationService = patientOrchestrationService;
        }

        [HttpPost("PatientGenerationRequest")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Agents")]
        public async ValueTask<ActionResult> PostPatientGenerationRequestAsync(
            [FromBody] PatientCodeRequest patientCodeRequest)
        {
            try
            {
                await this.patientOrchestrationService.RecordPatientInformationAsync(
                    patientCodeRequest.NhsNumber,
                    patientCodeRequest.NotificationPreference,
                    patientCodeRequest.GenerateNewCode
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

        [HttpPost("VerifyPatientCode")]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Agents")]
        public async ValueTask<ActionResult> VerifyPatientCodeAsync([FromBody] PatientCodeRequest patientCodeRequest)
        {
            try
            {
                await this.patientOrchestrationService.VerifyPatientCodeAsync(
                    patientCodeRequest.NhsNumber,
                    patientCodeRequest.VerificationCode
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
