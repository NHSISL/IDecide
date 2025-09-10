// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    public class PatientCodeRequest
    {
        public string NhsNumber { get; set; }
        public string VeriicationCode { get; set; }
        public string NotificationPreference { get; set; }
    }

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
        public async ValueTask<ActionResult<Patient>> PostPatientGenerationRequestAsync([FromBody] PatientCodeRequest patientCodeRequest)
        {
            try
            {
                await this.patientOrchestrationService.RecordPatientInformationAsync(
                    patientCodeRequest.NhsNumber,
                    patientCodeRequest.NotificationPreference
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
        public async ValueTask<ActionResult> VerifyPatientCodeAsync([FromBody] PatientCodeRequest patientCodeRequest)
        {
            return Ok(true);
            //try
            //{
            //    await this.patientOrchestrationService.VerifyPatientCodeAsync(
            //        patientCodeRequest.NhsNumber,
            //        patientCodeRequest.VeriicationCode
            //    );

            //    return Ok();
            //}
            //catch (PatientOrchestrationValidationException patientOrchestrationValidationException)
            //{
            //    return BadRequest(patientOrchestrationValidationException.InnerException);
            //}
            //catch (PatientOrchestrationDependencyValidationException
            //    patientOrchestrationDependencyValidationException)
            //{
            //    return BadRequest(patientOrchestrationDependencyValidationException.InnerException);
            //}
            //catch (PatientOrchestrationDependencyException patientOrchestrationDependencyException)
            //{
            //    return InternalServerError(patientOrchestrationDependencyException);
            //}
            //catch (PatientOrchestrationServiceException patientOrchestrationServiceException)
            //{
            //    return InternalServerError(patientOrchestrationServiceException);
            //}
        }
    }
}
