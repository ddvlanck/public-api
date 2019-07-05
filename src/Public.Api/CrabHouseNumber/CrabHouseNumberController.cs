namespace Public.Api.CrabHouseNumber
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "CRAB Huisnummers")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class CrabHouseNumberController : RegistryApiController<CrabHouseNumberController>
    {
        private const string Registry = "AddressRegistry";

        public CrabHouseNumberController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<CrabHouseNumberController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}