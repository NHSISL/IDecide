// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : RESTFulController
    {
        private readonly IPatientService patientService;
        private readonly IPatientOrchestrationService patientOrchestrationService;

        public PatientsController(IPatientService patientService, IPatientOrchestrationService patientOrchestrationService)
        {
            this.patientService = patientService;
            this.patientOrchestrationService = patientOrchestrationService;
        }


        [HttpPost("PatientGenerationNhsLoginRequest")]
        public async ValueTask<ActionResult> PostPatientGenerationNhsLoginRequestAsync(string phoneNumber)
        {
            try
            {
                var nhsnumber = HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "nhs_number")?.Value;
                var firstName = HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "given_name")?.Value;
                var lastName = HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "surname")?.Value;
                var email = HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "email")?.Value;
                var dobString = HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth")?.Value;

                DateTimeOffset.TryParse(dobString, out DateTimeOffset dob);

                var patient = new Patient
                {
                    NhsNumber = nhsnumber,
                    GivenName = firstName,
                    Surname = lastName,
                    DateOfBirth = dob,
                    Email = email,
                    Phone = phoneNumber
                };

                await this.patientOrchestrationService.RecordPatientInformationNhsLoginAsync(patient);

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
                when (patientValidationException.InnerException is Core.Models.Foundations.Patients.Exceptions.NotFoundPatientException)
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
                when (patientValidationException.InnerException is Core.Models.Foundations.Patients.Exceptions.NotFoundPatientException)
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
                when (patientValidationException.InnerException is Core.Models.Foundations.Patients.Exceptions.NotFoundPatientException)
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
