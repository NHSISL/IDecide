﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LondonDataServices.IDecide.Portal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrontendConfigurationsController : Controller
    {
        private IConfiguration configuration;

        public FrontendConfigurationsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult GetFrontendConfiguration()
        {
            var activeFeatures = configuration.GetSection("FrontendConfiguration").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            return Ok(activeFeatures);
        }
    }
}
