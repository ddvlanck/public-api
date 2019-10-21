namespace Public.Api.Address
{
    using AddressRegistry.Api.Legacy.AddressMatch.Responses;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using System.Threading;
    using System.Threading.Tasks;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressController
    {
        /// <summary>
        /// Voer een adres match vraag uit en krijg de adressen die gematcht worden.
        /// </summary>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de adressen.</param>
        /// <param name="nisCode">Filter op de NisCode van de gemeente.</param>
        /// <param name="postCode">Filter op de postcode van het adres.</param>
        /// <param name="kadStraatcode">Filter op de straatcode van het kadaster.</param>
        /// <param name="rrStraatcode">Filter op de straatcode van het rijksregister.</param>
        /// <param name="straatNaam">Filter op de straatnaam van het adres.</param>
        /// <param name="huisNummer">Filter op het huisnummer van het adres.</param>
        /// <param name="index">Filter op het huisnummer gekend in het rijksregister.</param>
        /// <param name="busNummer">Filter op het busnummer van het adres.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adresmatch")]
        [ProducesResponseType(typeof(AddressMatchCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> AddressMatch(
            [FromQuery] string gemeenteNaam,
            [FromQuery] string nisCode,
            [FromQuery] string postCode,
            [FromQuery] string kadStraatcode,
            [FromQuery] string rrStraatcode,
            [FromQuery] string straatNaam,
            [FromQuery] string huisNummer,
            [FromQuery] string index,
            [FromQuery] string busNummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            CancellationToken cancellationToken = default)
            => await AddressMatchWithFormat(
                null,
                gemeenteNaam,
                nisCode,
                postCode,
                kadStraatcode,
                rrStraatcode,
                straatNaam,
                huisNummer,
                index,
                busNummer,
                actionContextAccessor,
                responseOptions,
                cancellationToken);

        /// <summary>
        /// Voer een adres match vraag uit en krijg de adressen die gematcht worden.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de adressen.</param>
        /// <param name="nisCode">Filter op de NisCode van de gemeente.</param>
        /// <param name="postCode">Filter op de postcode van het adres.</param>
        /// <param name="kadStraatcode">Filter op de straatcode van het kadaster.</param>
        /// <param name="rrStraatcode">Filter op de straatcode van het rijksregister.</param>
        /// <param name="straatNaam">Filter op de straatnaam van het adres.</param>
        /// <param name="huisNummer">Filter op het huisnummer van het adres.</param>
        /// <param name="index">Filter op het huisnummer gekend in het rijksregister.</param>
        /// <param name="busNummer">Filter op het busnummer van het adres.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adresmatch.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(AddressMatchCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> AddressMatchWithFormat(
            [FromRoute] string format,
            [FromQuery] string gemeenteNaam,
            [FromQuery] string nisCode,
            [FromQuery] string postCode,
            [FromQuery] string kadStraatcode,
            [FromQuery] string rrStraatcode,
            [FromQuery] string straatNaam,
            [FromQuery] string huisNummer,
            [FromQuery] string index,
            [FromQuery] string busNummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            var taal = Taal.NL;

            IRestRequest BackendRequest() => CreateBackendMatchRequest(
                taal,
                busNummer,
                huisNummer,
                postCode,
                gemeenteNaam,
                nisCode,
                straatNaam,
                kadStraatcode,
                rrStraatcode,
                index);

            var response = await GetFromBackendAsync(format, BackendRequest, Request.GetTypedHeaders(), CreateDefaultHandleBadRequest(), cancellationToken);

            return BackendListResponseResult.Create(response, Request.Query, string.Empty);
        }

        private static IRestRequest CreateBackendMatchRequest(
            Taal taal,
            string boxNumber,
            string houseNumber,
            string postalCode,
            string municipalityName,
            string nisCode,
            string streetName,
            string kadStreetCode,
            string rrStreetCode,
            string index)
            => new RestRequest(
                    "adresmatch?" +
                    "taal={taal}&" +
                    "busnummer={busnummer}&" +
                    "huisnummer={huisnummer}&" +
                    "postcode={postcode}&" +
                    "gemeentenaam={gemeentenaam}&" +
                    "niscode={niscode}&" +
                    "straatnaam={straatnaam}&" +
                    "kadstraatcode={kadstraatcode}&" +
                    "rrstraatcode={rrstraatcode}&" +
                    "index={index}")
                .AddParameter("taal", taal, ParameterType.UrlSegment)
                .AddParameter("busnummer", boxNumber, ParameterType.UrlSegment)
                .AddParameter("huisnummer", houseNumber, ParameterType.UrlSegment)
                .AddParameter("postcode", postalCode, ParameterType.UrlSegment)
                .AddParameter("gemeentenaam", municipalityName, ParameterType.UrlSegment)
                .AddParameter("niscode", nisCode, ParameterType.UrlSegment)
                .AddParameter("straatnaam", streetName, ParameterType.UrlSegment)
                .AddParameter("kadStreetCode", kadStreetCode, ParameterType.UrlSegment)
                .AddParameter("rrstraatcode", rrStreetCode, ParameterType.UrlSegment)
                .AddParameter("index", index, ParameterType.UrlSegment);
    }
}
