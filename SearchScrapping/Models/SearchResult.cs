using SearchScraping.Interfaces;

namespace SearchScraping.Models
{
    public struct SearchResult : ISearchResult
    {
        public int Position { get; set; }
        public string Heading { get; set; }
        public string Url { get; set; }
    }
}
