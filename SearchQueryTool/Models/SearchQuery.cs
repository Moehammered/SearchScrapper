namespace SearchQueryTool.Models
{
    public class SearchQuery
    {
        public string SearchTerm { get; set; }
        public string UrlMatch { get; set; }
        public int ResultLimit { get; set; }
    }
}
