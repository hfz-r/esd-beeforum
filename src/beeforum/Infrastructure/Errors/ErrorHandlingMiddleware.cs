using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace beeforum.Infrastructure.Errors
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IStringLocalizer<ErrorHandlingMiddleware> _localizer;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IStringLocalizer<ErrorHandlingMiddleware> localizer)
        {
            _next = next;
            _logger = logger;
            _localizer = localizer;
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger<ErrorHandlingMiddleware> logger,
            IStringLocalizer<ErrorHandlingMiddleware> localizer)
        {
            object errors = null;

            switch (exception)
            {
                case RestException re:
                    errors = re.Errors;
                    context.Response.StatusCode = (int) re.Code;
                    break;
                case Exception e:
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            var result = JsonConvert.SerializeObject(new
            {
                errors
            });

            await context.Response.WriteAsync(result);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger, _localizer);
            }
        }
    }
}