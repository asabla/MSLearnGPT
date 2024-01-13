using System.Web;

using MSLearnGPT.API.Models.Search;

namespace MSLearnGPT.API.Services;

internal interface IMicrosoftLearnSearchService
{
    Task<SearchResponse> SearchAsync(
        string query,
        string scope,
        string locale,
        string[] facets,
        string filter,
        int take = 10,
        bool expandScope = true,
        string partnerId = "LearnSite",
        CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(
        Action<SearchRequest> searchRequest, 
        CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(
        SearchRequest searchRequest, 
        CancellationToken cancellationToken = default);
}

internal class MicrosoftLearnSearchService(
        ILogger<MicrosoftLearnSearchService> logger,
        IHttpClientFactory httpClientFactory)
    : IMicrosoftLearnSearchService
{
    public async Task<SearchResponse> SearchAsync(
            string query,
            string scope,
            string locale,
            string[] facets,
            string filter,
            int take = 10,
            bool expandScope = true,
            string partnerId = "LearnSite",
            CancellationToken cancellationToken = default)
        => await SearchAsync(new SearchRequest
        {
            Query = query,
            Scope = scope,
            Locale = locale,
            Facets = facets,
            Filter = filter,
            Take = take,
            ExpandScope = expandScope,
            PartnerId = partnerId,
        });

    public async Task<SearchResponse> SearchAsync(
        Action<SearchRequest> searchRequest,
        CancellationToken cancellationToken = default)
    {
        SearchRequest request = new();
        searchRequest(request);

        return await SearchAsync(request, cancellationToken);
    }

    public async Task<SearchResponse> SearchAsync(
        SearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        logger.BeginScope($"[{nameof(SearchRequest)}]");
        logger.LogInformation(
            message: "Searching with query '{query}' on MicrosoftLearn",
            searchRequest.Query);

        var httpClient = httpClientFactory.CreateClient(
            name: nameof(MicrosoftLearnSearchService));

        // Make search parameters HTML-safe
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["search"] = searchRequest.Query;
        queryString["scope"] = searchRequest.Scope;
        queryString["locale"] = searchRequest.Locale;

        // TODO: Implement facets

        queryString["$filter"] = searchRequest.Filter;
        queryString["$top"] = searchRequest.Take.ToString();

        queryString["expandScope"] = searchRequest.ExpandScope.ToString();
        queryString["partnerId"] = searchRequest.PartnerId;

        HttpResponseMessage response = await httpClient.GetAsync(
                requestUri: $"api/search?{queryString}",
                cancellationToken: cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            string errMessage = await response.Content.ReadAsStringAsync();
            logger.LogError($"Unable to parse JSON response: {errMessage}");

            throw new Exception(errMessage);
        }

        return await response.Content.ReadFromJsonAsync<SearchResponse>() ?? null!;
    }
}