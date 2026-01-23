// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Attrify.Attributes;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : RESTFulController
    {
        private readonly IPatientService patientService;
        private readonly INhsLoginService nhsLoginService;
        private readonly IPatientOrchestrationService patientOrchestrationService;
        private readonly IConfiguration configuration;

        public PatientsController(
            IPatientService patientService,
            INhsLoginService nhsLoginService,
            IPatientOrchestrationService patientOrchestrationService,
            IConfiguration configuration)
        {
            this.patientService = patientService;
            this.nhsLoginService = nhsLoginService;
            this.patientOrchestrationService = patientOrchestrationService;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpGet("patientInfo")]
        public async Task<IActionResult> GetPatientInfo()
        {
            try
            {
                Core.Models.Foundations.NhsLogins.NhsLoginUserInfo nhsLoginUserInfo =
                    await this.nhsLoginService.NhsLoginAsync();

                return Ok(nhsLoginUserInfo);
            }
            catch (NhsLoginServiceDependencyException nhsLoginServiceDependencyException)
            {
                return InternalServerError(nhsLoginServiceDependencyException);
            }
            catch (NhsLoginServiceServiceException nhsLoginServiceServiceException)
            {
                return InternalServerError(nhsLoginServiceServiceException);
            }
        }

        [HttpPost]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<Patient>> PostPatientAsync([FromBody] Patient patient)
        {
            try
            {
                Patient addedPatient =
                    await this.patientService.AddPatientAsync(patient);

                return Created(addedPatient);
            }
            catch (PatientValidationException patientValidationException)
            {
                return BadRequest(patientValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
               when (patientDependencyValidationException.InnerException is AlreadyExistsPatientException)
            {
                return Conflict(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                return BadRequest(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                return InternalServerError(patientDependencyException);
            }
            catch (PatientServiceException patientServiceException)
            {
                return InternalServerError(patientServiceException);
            }
        }

        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<IQueryable<Patient>>> Get()
        {
            try
            {
                IQueryable<Patient> retrievedPatients =
                    await this.patientService.RetrieveAllPatientsAsync();

                return Ok(retrievedPatients);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                return InternalServerError(patientDependencyException);
            }
            catch (PatientServiceException patientServiceException)
            {
                return InternalServerError(patientServiceException);
            }
        }

        [HttpGet("{patientId}")]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<Patient>> GetPatientByIdAsync(Guid patientId)
        {
            try
            {
                Patient patient = await this.patientService.RetrievePatientByIdAsync(patientId);

                return Ok(patient);
            }
            catch (PatientValidationException patientValidationException)
                when (patientValidationException.InnerException is NotFoundPatientException)
            {
                return NotFound(patientValidationException.InnerException);
            }
            catch (PatientValidationException patientValidationException)
            {
                return BadRequest(patientValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                return BadRequest(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                return InternalServerError(patientDependencyException);
            }
            catch (PatientServiceException patientServiceException)
            {
                return InternalServerError(patientServiceException);
            }
        }

        [HttpPut]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<Patient>> PutPatientAsync([FromBody] Patient patient)
        {
            try
            {
                Patient modifiedPatient =
                    await this.patientService.ModifyPatientAsync(patient);

                return Ok(modifiedPatient);
            }
            catch (PatientValidationException patientValidationException)
                when (patientValidationException.InnerException is NotFoundPatientException)
            {
                return NotFound(patientValidationException.InnerException);
            }
            catch (PatientValidationException patientValidationException)
            {
                return BadRequest(patientValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
               when (patientDependencyValidationException.InnerException is AlreadyExistsPatientException)
            {
                return Conflict(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                return BadRequest(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                return InternalServerError(patientDependencyException);
            }
            catch (PatientServiceException patientServiceException)
            {
                return InternalServerError(patientServiceException);
            }
        }

        [HttpDelete("{patientId}")]
        [InvisibleApi]
        [Authorize(Roles = "LondonDataServices.IDecide.Portal.Server.Administrators")]
        public async ValueTask<ActionResult<Patient>> DeletePatientByIdAsync(Guid patientId)
        {
            try
            {
                Patient deletedPatient =
                    await this.patientService.RemovePatientByIdAsync(patientId);

                return Ok(deletedPatient);
            }
            catch (PatientValidationException patientValidationException)
                when (patientValidationException.InnerException is NotFoundPatientException)
            {
                return NotFound(patientValidationException.InnerException);
            }
            catch (PatientValidationException patientValidationException)
            {
                return BadRequest(patientValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
                when (patientDependencyValidationException.InnerException is LockedPatientException)
            {
                return Locked(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                return BadRequest(patientDependencyValidationException.InnerException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                return InternalServerError(patientDependencyException);
            }
            catch (PatientServiceException patientServiceException)
            {
                return InternalServerError(patientServiceException);
            }
        }
    }
}
