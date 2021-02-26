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

namespace Public.Api.PostalCode
{
    public partial class PostalCodeController
    {

        public readonly int PageSize = 250;

        /// <summary>
        /// Vraag een lijst met postinfo over postcodes op.
        /// </summary>
        /// <param name="page">Optionele nulgebaseerde index van de Linked Data Event Stream. Indien null wordt altijd geredirect naar pagina 1</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met postinfo over postcodes gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo/base", Name = nameof(GetLinkedDataEventStreamPage))]
        public async Task<IActionResult> GetLinkedDataEventStreamPage(
            [FromQuery] string? page,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrEmpty(page))
            {
                return Redirect("?page=1");
            }

            var pageNumber = Int32.Parse(page);
            var offset = (pageNumber - 1) * PageSize;
            var limit = pageNumber * PageSize;
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            var cacheKey = $"legacy/postalinfo-ldes:{pageNumber}";



            RestRequest BackendRequest() => CreateBackendLdesRequest(offset, limit);
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

        private static RestRequest CreateBackendLdesRequest(
            int? offset,
            int? limit)
        {
            var request = new RestRequest("postcodes/base");
            request.AddPagination(offset, limit);
            return request;
        }

    }
}
