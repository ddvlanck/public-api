namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";

        private readonly string _validApiKeySet;

        public ApiKeyAuthAttribute(string validApiKeySet)
            => _validApiKeySet = validApiKeySet;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            void SetExceptionFormat(HttpContext httpContext)
            {
                if (!(context.Controller is PublicApiController))
                    return;

                PublicApiController.DetermineAndSetProblemDetailsFormat(
                    string.Empty,
                    httpContext.RequestServices.GetRequiredService<IActionContextAccessor>(),
                    httpContext.Request);
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                SetExceptionFormat(context.HttpContext);
                throw new ApiException("API key verplicht.", StatusCodes.Status401Unauthorized);
            }

            var valiApiKeys = context
                .HttpContext
                .RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection($"ApiKeys:{_validApiKeySet}")
                .GetChildren()
                .Select(c => c.Value)
                .ToArray();

            if (!valiApiKeys.Contains(potentialApiKey.First()))
            {
                SetExceptionFormat(context.HttpContext);
                throw new ApiException("Ongeldige API key.", StatusCodes.Status401Unauthorized);
            }

            await next();
        }
    }
}
