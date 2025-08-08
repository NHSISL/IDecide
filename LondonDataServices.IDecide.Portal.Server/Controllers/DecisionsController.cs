using LondonDataServices.IDecide.Portal.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public DecisionsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Mock Decision model for demonstration
        public class Decision
        {
            public string Id { get; set; }
            public string PatientNhsNumber { get; set; }
            public int? DecisionTypeId { get; set; }
            public string DecisionChoice { get; set; }
            public string Code { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }
        }

        //Recapture
        [HttpPost]
        public ActionResult<Decision> CreateDecision([FromBody] Decision decision)
        {
            // Mocked response - in a real app, save to DB and return the saved entity
            var createdDecision = new Decision
            {
                Id = Guid.NewGuid().ToString(),
                PatientNhsNumber = decision?.PatientNhsNumber ?? "1234567890",
                DecisionTypeId = decision?.DecisionTypeId ?? 1,
                DecisionChoice = decision?.DecisionChoice ?? "optin",
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "system",
                UpdatedDate = DateTime.UtcNow
            };

            return Ok(createdDecision);
        }
    }
}