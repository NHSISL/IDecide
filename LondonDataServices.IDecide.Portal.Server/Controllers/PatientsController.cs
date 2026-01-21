// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Attrify.Attributes;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authentication;
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
        private readonly IPatientOrchestrationService patientOrchestrationService;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public PatientsController(
            IPatientService patientService,
            IPatientOrchestrationService patientOrchestrationService,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            this.patientService = patientService;
            this.patientOrchestrationService = patientOrchestrationService;
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        [Authorize]
        [HttpGet("patientInfo")]
        public async Task<IActionResult> GetPatientInfo()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized();
            }

            using var http = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(
                configuration["NHSLoginOIDC:authority"] + "/userinfo"
            );

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var userInfo = await response.Content.ReadFromJsonAsync<NhsLoginUserInfo>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            return new JsonResult(userInfo);
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
