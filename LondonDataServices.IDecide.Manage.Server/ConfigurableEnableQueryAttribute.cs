// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LondonDataServices.IDecide.Manage.Server
{
    public class ConfigurableEnableQueryAttribute : EnableQueryAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            int pageSize;

            var configuration = actionContext.HttpContext.RequestServices
                .GetRequiredService<IConfiguration>();

            var environment = actionContext.HttpContext.RequestServices
                .GetRequiredService<IHostEnvironment>();

            if (environment.IsDevelopment())
            {
                pageSize = configuration.GetValue<int>("OData:PageSize_Debug", 5000);
            }
            else
            {
                pageSize = configuration.GetValue<int>("OData:PageSize_Release", 50);
            }

            this.PageSize = pageSize;
            base.OnActionExecuting(actionContext);
        }
    }
}
