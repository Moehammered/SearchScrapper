using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchScraperWebService.Filters
{
    public class ValidatePositiveIntegerParamsAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var badArguments = ExtractUnsignedIntegerArguments(context)
                .Where(x => x.Value == 0);

            if (badArguments.Any())
            {
                var message = string.Join(Environment.NewLine,
                    badArguments.Select(x => $"Please make sure '{x.Key}' is greater than 0."));
                context.Result = new BadRequestObjectResult(message);
            }

            return base.OnActionExecutionAsync(context, next);
        }

        private IDictionary<string, uint> ExtractUnsignedIntegerArguments(ActionExecutingContext context)
        {
            var stringParams = context.ActionDescriptor.Parameters
                .Where(x => x.ParameterType == typeof(uint))
                .Select(x => x.Name);

            return context.ActionArguments
                .Where(x => stringParams.Any(p => p.Equals(x.Key)))
                .ToDictionary(x => x.Key, y => (uint)y.Value);
        }
    }
}
