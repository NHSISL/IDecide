// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : RESTFulController
    {
        private readonly IPatientOrchestrationService patientOrchestrationService;
        private readonly IApiPlatformClient apiPlatformClient;

        public PatientSearchController(IPatientOrchestrationService patientOrchestrationService, IApiPlatformClient apiPlatformClient)
        {
            this.patientOrchestrationService = patientOrchestrationService;
            this.apiPlatformClient = apiPlatformClient;
        }

        [HttpPost("PatientSearch")]
        [Authorize]
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
    }
}
