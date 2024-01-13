namespace MSLearnGPT.API.Models.Search;

public class SearchResponse
{
    public bool ScopeRemoved { get; set; } = false;
    public int Count { get; set; }
    public string NextLink { get; set; } = null!;
    public string SrchEng { get; set; } = null!;
    public bool TermHasSynonyms { get; set; }

    public IEnumerable<Facet> Facets { get; set; }
        = Enumerable.Empty<Facet>();

    public IEnumerable<Result> Results { get; set; }
        = Enumerable.Empty<Result>();
}