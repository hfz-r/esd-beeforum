using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace beeforum.Infrastructure.Mvc
{
    public class ValidatorActionFilter : IActionFilter
    {
        private readonly ILogger _logger;

        public ValidatorActionFilter(ILogger<ValidatorActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ContentResult();
                var errors = context.ModelState.ToDictionary(valuePair => valuePair.Key,
                    valuePair => valuePair.Value.Errors.Select(x => x.ErrorMessage).ToArray());

                var content = JsonConvert.SerializeObject(new {errors});
                result.Content = content;
                result.ContentType = "application/json";

                context.HttpContext.Response.StatusCode = 422; //un-processable entity;
                context.Result = result;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}