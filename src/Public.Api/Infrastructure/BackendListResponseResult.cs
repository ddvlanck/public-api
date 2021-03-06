namespace Public.Api.Infrastructure
{
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Http;

    public class BackendListResponseResult : BackendResponseResult
    {
        private BackendListResponseResult(BackendResponse response)
            : base(response) { }

        public static BackendListResponseResult Create(
            BackendResponse response,
            IQueryCollection requestQuery,
            string nextPageUrlTemplate)
        {
            var nonPagedQueryCollection = new NonPagedQueryCollection(requestQuery);

            response.UpdateNextPageUrlWithQueryParameters(nonPagedQueryCollection, nextPageUrlTemplate);

            return new BackendListResponseResult(response);
        }
    }
}
