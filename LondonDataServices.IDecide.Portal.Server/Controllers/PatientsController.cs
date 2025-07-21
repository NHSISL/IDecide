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
            public DateTime DateOfBirth { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }
        }

        // GET: api/patients
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAllPatients()
        {
            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    NhsNumber = "1234567890",
                    FirstName = "John",
                    Surname = "Doe",
                    EmailAddress = "john.doe@example.com",
                    Address = "123 Main St",
                    Postcode = "AB12 3CD",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    CreatedBy = "system",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "system",
                    UpdatedDate = DateTime.UtcNow
                }
            };
            return Ok(patients);
        }

        // GET: api/patients/{id}
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatientById(string id)
        {
            var patient = new Patient
            {
                Id = id,
                NhsNumber = "9876543210",
                FirstName = "Jane",
                Surname = "Smith",
                EmailAddress = "jane.smith@example.com",
                Address = "456 High St",
                Postcode = "CD34 5EF",
                DateOfBirth = new DateTime(1990, 5, 15),
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };
            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public ActionResult<Patient> CreatePatient([FromBody] Patient patient)
        {
            var createdPatient = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                NhsNumber = patient?.NhsNumber ?? "N/A",
                FirstName = "Jane",
                Surname = "Smith",
                EmailAddress = "jane.smith@example.com",
                PhoneNumber = patient?.PhoneNumber ?? "0000000000",
                Address = "456 High St",
                Postcode = "CD34 5EF",
                DateOfBirth = patient?.DateOfBirth == default ? new DateTime(1990, 5, 15) : patient.DateOfBirth,
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(createdPatient);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id}")]
        public ActionResult<Patient> UpdatePatient(string id, [FromBody] Patient patient)
        {
            patient.Id = id;
            patient.UpdatedDate = DateTime.UtcNow;
            patient.UpdatedBy = "system";
            return Ok(patient);
        }
    }
}