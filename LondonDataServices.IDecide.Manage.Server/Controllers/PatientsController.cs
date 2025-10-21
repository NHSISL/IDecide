// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : RESTFulController
    {
        private readonly IPatientService patientService;

        public PatientsController(IPatientService patientService) =>
            this.patientService = patientService;

        [HttpPost]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
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
        [EnableQuery]
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Agents")]
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
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Agents")]
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
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
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
        [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators")]
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
