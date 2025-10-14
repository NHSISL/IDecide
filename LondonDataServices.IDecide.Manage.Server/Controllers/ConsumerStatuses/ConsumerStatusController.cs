// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace LondonDataServices.IDecide.Manage.Server.Controllers.ConsumerStatuses
{
    [Authorize(Roles = "LondonDataServices.IDecide.Manage.Server.Administrators,LondonDataServices.IDecide.Manage.Server.Agents")]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerStatusController : RESTFulController
    {
        private readonly IConsumerOrchestrationService consumerOrchestrationService;

        public ConsumerStatusController(IConsumerOrchestrationService consumerOrchestrationService) =>
            this.consumerOrchestrationService = consumerOrchestrationService;

        [HttpPost("AdoptPatientDecisions")]
        public async ValueTask<ActionResult> AdoptPatientDecisionsAsync([FromBody] List<Decision> decisions)
        {
            throw new NotImplementedException();
        }
    }
}
