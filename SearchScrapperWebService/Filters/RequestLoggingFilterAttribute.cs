using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchScraperWebService.Filters
{
    public class RequestLoggingFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger Logger;
        public RequestLoggingFilterAttribute(ILogger<RequestLoggingFilterAttribute> logger)
            => Logger = logger;

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var path = context.HttpContext.Request.Path;
            var arguments = Arguments(context.ActionDescriptor.Parameters, context.ActionArguments);

            var msg = $"{context.ActionDescriptor.DisplayName}\n" +
                $"{path}\n{arguments}";
            Logger.LogInformation(msg);
            return base.OnActionExecutionAsync(context, next);
        }

        private string Arguments(IEnumerable<ParameterDescriptor> par, IDictionary<string, object> args)
        {
            var found = par.Where(x => args.ContainsKey(x.Name));

            var supplied = found.Select(x => $"{x.Name}({x.ParameterType.Name}): {args[x.Name]}");
            var missing = par.Except(found).Select(x => $"{x.Name}({x.ParameterType.Name}): null");

            return string.Join(", ", supplied.Union(missing));
        }
    }
}
