// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Mvc;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : ControllerBase
    {
        private readonly IPatientOrchestrationService patientOrchestrationService;

        public PatientSearchController(IPatientOrchestrationService patientOrchestrationService)
        {
            this.patientOrchestrationService = patientOrchestrationService;
        }

        [HttpPost("PostPatientByDetails")]
        public async ValueTask<ActionResult<Patient>> PostPatientByDetailsAsync([FromBody] PatientLookup patientLookup)
        {
            Patient patient = await this.patientOrchestrationService.PatientLookupByDetailsAsync(patientLookup);

            return Ok(patient);
        }
    }
}
