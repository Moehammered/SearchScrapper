using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchScrapperWebService.Filters
{
    public class ValidateSearchParamsAttribute : ActionFilterAttribute
    {
        private readonly string QueryParamName;
        private readonly string ResultCountParamName;
        private const string BadQueryMessage = "Please make sure the query parameter isn't empty.";
        private const string BadResultCountMessage = "Please make sure the result count parameter is greater than 0.";

        public ValidateSearchParamsAttribute(string queryParam, string resultCountParam)
        {
            QueryParamName = queryParam;
            ResultCountParamName = resultCountParam;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Dictionary<string, string> issues = new Dictionary<string, string>();

            if (context.ActionArguments.ContainsKey(QueryParamName))
            {
                var param = context.ActionArguments[QueryParamName]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(param))
                    issues.Add(QueryParamName, BadQueryMessage);
            }
            else
                issues.Add(QueryParamName, "Parameter missing");

            if (context.ActionArguments.ContainsKey(ResultCountParamName))
            {
                if (context.ActionArguments[ResultCountParamName] is uint number && number == 0)
                    issues.Add(ResultCountParamName, BadResultCountMessage);
            }
            else
                issues.Add(ResultCountParamName, "Parameter missing");

            if(issues.Any())
            {
                var message = string.Join(Environment.NewLine, issues.Select(x => $"{x.Key}: {x.Value}"));
                context.Result = new BadRequestObjectResult(message);
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
