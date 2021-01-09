using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRequestTelemetryDemo.Filters
{
    public class TelemetryFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Initialize the RequestTelemetry object
            RequestTelemetry reqTelemetry = context.HttpContext?.Features.Get<RequestTelemetry>();
            if (reqTelemetry != null)
            {
                // Add the request parameters
                var requestParameters = context.ActionArguments;
                foreach (var param in requestParameters)
                {
                    reqTelemetry.Properties.Add(param.Key, JsonConvert.SerializeObject(param.Value));
                }
            }
            
            // Await the action result
            var result = await next();

            // Cast the result to an Mvc ObjectResult and add the value to the telemtry
            var response = (ObjectResult)result.Result;
            if (reqTelemetry != null)
            {
                reqTelemetry.Properties.Add("Result", JsonConvert.SerializeObject(response.Value));
            }
        }
    }
}
