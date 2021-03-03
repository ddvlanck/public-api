using Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Public.Api.Infrastructure;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Public.Api.StreetName
{
    public partial class StreetNameController
    {
        /// <summary>
        /// Vraag een de SHACL shape op van straatnamen
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met info over straatnamen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("base/straatnamen/shape", Name = nameof(GetStreetNameShape))]
        public async Task<IActionResult> GetStreetNameShape(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            var cacheKey = $"legacy/streetname-shape";

            RestRequest BackendRequest() => CreateBackendLdesRequest();
            var value = await (CacheToggle.FeatureEnabled
                 ? GetFromCacheThenFromBackendAsync(
                     contentFormat.ContentType,
                     BackendRequest,
                     cacheKey,
                     CreateDefaultHandleBadRequest(),
                     cancellationToken)
                 : GetFromBackendAsync(
                     contentFormat.ContentType,
                     BackendRequest,
                     CreateDefaultHandleBadRequest(),
                     cancellationToken));


            return new BackendResponseResult(value);
        }

        private static RestRequest CreateBackendLdesRequest()
        {
            var request = new RestRequest("straatnamen/base/shape");
            return request;
        }
    }
}
