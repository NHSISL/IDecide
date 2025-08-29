// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public PatientsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public class PatientLookup
        {
            public SearchCriteria SearchCriteria { get; set; }
            public List<Patient> Patients { get; set; }
        }

        public class Patient
        {
            public Guid Id { get; set; }
            public string NhsNumber { get; set; }
            public string Title { get; set; }
            public string GivenName { get; set; }
            public string Surname { get; set; }
            public DateTimeOffset DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string PostCode { get; set; }
            public string ValidationCode { get; set; }
            public DateTimeOffset ValidationCodeExpiresOn { get; set; }
            public int RetryCount { get; set; }
            public string CreatedBy { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public DateTimeOffset UpdatedDate { get; set; }
        }

        public class SearchCriteria
        {
            public string NhsNumber { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public string Postcode { get; set; } = string.Empty;
            public string DateOfBirth { get; set; } = string.Empty;
            public string DateOfDeath { get; set; } = string.Empty;
            public string RegisteredGpPractice { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
        }

        public class PatientSearchCriteria
        {
            public string NhsNumber { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public string Postcode { get; set; } = string.Empty;
            public string DateOfBirth { get; set; } = string.Empty;
        }

        public class GenerateCodeRequest
        {
            public string NhsNumber { get; set; }
            public string NotificationPreference { get; set; }
            public string PoaFirstName { get; set; }
            public string PoaSurname { get; set; }
            public string PoaRelationship { get; set; }
        }

        public class ConfirmCodeRequest
        {
            public string NhsNumber { get; set; }
            public string Code { get; set; }
        }

        //recapture
        [HttpPost("PostPatientByNhsNumber")]
        public async ValueTask<ActionResult<Patient>> PostPatientByNhsNumberAsync([FromBody] PatientLookup patientLookup)
        {
            var createdPatient = new Patient
            {
                Id = Guid.NewGuid(),
                NhsNumber = patientLookup.SearchCriteria.NhsNumber,
                GivenName = "D****",
                Surname = "H****",
                Gender = "Male",
                Email = "",
                DateOfBirth = new DateTime(1990, 5, 15),
                Address = "9 T** W*********, S**********, S*****",
                Phone = "07*******84",
                ValidationCode = "",
            };

            await Task.CompletedTask;

            return Ok(createdPatient);
        }

        //recapture
        [HttpPost("PostPatientByDetails")]
        public async ValueTask<ActionResult<Patient>> GetPatientByDetails([FromBody] PatientLookup patientLookup)
        {
            var createdPatient = new Patient
            {
                Id = Guid.NewGuid(),
                NhsNumber = patientLookup.SearchCriteria.NhsNumber,
                GivenName = "D****",
                Surname = "H****",
                Gender = "Male",
                Email = "",
                DateOfBirth = new DateTime(1990, 5, 15),
                Address = "9 T** W*********, S**********, S*****",
                Phone = "07*******84",
                ValidationCode = "",
            };

            await Task.CompletedTask;

            return Ok(createdPatient);
        }

        //recapture
        [HttpPut]
        public IActionResult GenerateAndSendCode([FromBody] GenerateCodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NhsNumber) || string.IsNullOrWhiteSpace(request.NotificationPreference))
            {
                return BadRequest("NhsNumber and NotificationPreference are required.");
            }

            return NoContent();
        }

        //recapture
        [HttpPut("confirm-code")]
        public IActionResult ConfirmPatientCode([FromBody] ConfirmCodeRequest request)
        {
            bool isCodeValid = request.Code == "12345";
            if (isCodeValid)
                return Ok(true);
            else
                return Forbid();
        }
    }
}