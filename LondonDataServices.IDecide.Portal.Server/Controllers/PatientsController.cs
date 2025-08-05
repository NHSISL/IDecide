using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

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

        public class Patient
        {
            public string Id { get; set; }
            public string NhsNumber { get; set; }
            public string FirstName { get; set; }
            public string Surname { get; set; }
            public string EmailAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string Postcode { get; set; }
            public string NotificationPreference { get; set; }
            public string VerificationCode { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }
        }

        public class GenerateCodeRequest
        {
            public string NhsNumber { get; set; }
            public string NotificationPreference { get; set; }
            public string PoaFirstName { get; set; }
            public string PoaSurname { get; set; }
            public string PoaRelationship { get; set; }

            //PoA fields
        }

        public class ConfirmCodeRequest
        {
            public string NhsNumber { get; set; }
            public string Code { get; set; }
        }

        //recapture
        [HttpPost("PostPatientByNhsNumber")]
        public ActionResult<Patient> GetPatientByNhsNumber([FromBody] Patient patient)
        {
            var createdPatient = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                NhsNumber = "1234567890",
                FirstName = "D****",
                Surname = "H****",
                EmailAddress = "",
                PhoneNumber = "07*******84",
                Address = "9 T** W*********, S**********, S*****",
                Postcode = "CR2 0**",
                DateOfBirth = patient?.DateOfBirth == default ? new DateTime(1990, 5, 15) : patient.DateOfBirth,
                VerificationCode = "",
                NotificationPreference = ""               
            };

            return Ok(createdPatient);
        }

        //recapture
        [HttpPost("PostPatientByDetails")]
        public ActionResult<Patient> GetPatientByDetails([FromBody] Patient patient)
        {
            var createdPatient = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                NhsNumber = "1234567890",
                FirstName = "D****",
                Surname = "H****",
                EmailAddress = "d****.h***s@googlemail.com",
                PhoneNumber = "07*******84",
                Address = "9 T** W*********, S**********, S*****",
                Postcode = "CR2 0**",
                DateOfBirth = patient?.DateOfBirth == default ? new DateTime(1990, 5, 15) : patient.DateOfBirth,
                VerificationCode = "",
                NotificationPreference = ""
            };

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