using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchScraperWebService.Filters
{
    public class ValidateEmptyStringParamsAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var badArguments = ExtractStringArguments(context)
                .Where(x => string.IsNullOrWhiteSpace(x.Value));

            if(badArguments.Any())
            {
                var message = string.Join(Environment.NewLine, 
                    badArguments.Select(x => $"Please make sure '{x.Key}' is not empty."));
                context.Result = new BadRequestObjectResult(message);
            }

            return base.OnActionExecutionAsync(context, next);
        }

        private IDictionary<string, string> ExtractStringArguments(ActionExecutingContext context)
        {
            var stringParams = context.ActionDescriptor.Parameters
                .Where(x => x.ParameterType == typeof(string))
                .Select(x => x.Name);

            return context.ActionArguments
                .Where(x => stringParams.Any(p => p.Equals(x.Key)))
                .ToDictionary(x => x.Key, y => y.Value as string);
        }
    }
}
