using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace SearchScraperWebService.Filters
{
    public class ExceptionLoggerFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger Logger;
        public ExceptionLoggerFilterAttribute(ILogger<ExceptionLoggerFilterAttribute> logger)
            => Logger = logger;

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var arguments = Arguments(context.ActionDescriptor);
            var exceptionDescription = $"{context.Exception.GetType().Name} -- {context.Exception.Message}";
            var actionName = context.ActionDescriptor.DisplayName;
            var requestPath = context.HttpContext.Request.Path;

            var errorMessage = $"{exceptionDescription}\n" +
                $"{requestPath}\n" +
                $"{actionName}\n" +
                $"{arguments}";

            Logger.LogError(context.Exception, errorMessage);
            context.ExceptionHandled = true;
            context.Result = new BadRequestObjectResult("An unexpected error occurred.");
            return base.OnExceptionAsync(context);
        }

        private string Arguments(ActionDescriptor action)
            => string.Join(", ", action.RouteValues.Select(x => $"{x.Key}: {x.Value}"));
    }
}
