using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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

        // Mock Patient model for demonstration
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

        [HttpPost("GetPatientByNhsNumber")]
        public ActionResult<Patient> GetPatientByNhsNumber([FromBody] Patient patient)
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
                NotificationPreference = "",
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(createdPatient);
        }

        [HttpPost("GetPatientByDetails")]
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
                NotificationPreference = "",
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(createdPatient);
        }

        [HttpPut]
        public ActionResult<Patient> UpdatePatientNotificationPreference([FromBody] Patient patient)
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
                NotificationPreference = "",
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(createdPatient);
        }

        [HttpPost("confirm-code")]
        public IActionResult ConfirmPatientCode([FromBody] ConfirmCodeRequest request)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                NhsNumber = request.NhsNumber,
                FirstName = "D****",
                Surname = "H****",
                EmailAddress = "d****.h***s@googlemail.com",
                PhoneNumber = "07*******84",
                Address = "9 T** W*********, S**********, S*****",
                Postcode = "CR2 0**",
                VerificationCode = "12345",
                NotificationPreference = "SMS",
                DateOfBirth = new DateTime(1990, 5, 15),
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(patient);
        }

        [HttpPut("{nhsNumber}")]
        public ActionResult<Patient> UpdatePatient(string nhsNumber, [FromBody] Patient patient)
        {
            var updatedPatient = new Patient
            {
                Id = patient.Id ?? Guid.NewGuid().ToString(),
                NhsNumber = "1234567890",
                FirstName = "D****",
                Surname = "H****",
                EmailAddress = "d****.h***s@googlemail.com",
                PhoneNumber = "07*******84",
                Address = "9 T** W*********, S**********, S*****",
                Postcode = "CR2 0**",
                NotificationPreference = patient.NotificationPreference ?? "",
                VerificationCode = "12345",
                DateOfBirth = patient.DateOfBirth == default ? new DateTime(1990, 5, 15) : patient.DateOfBirth,
                CreatedBy = patient.CreatedBy ?? "system",
                CreatedDate = patient.CreatedDate == default ? DateTime.UtcNow : patient.CreatedDate,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };
            return Ok(updatedPatient);
        }
    }
}