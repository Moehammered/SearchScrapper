using SearchScraping.Interfaces;

namespace SearchScraping.Models
{
    public struct SearchResult : ISearchResult
    {
        public string Heading { get; set; }
        public string Url { get; set; }
        public int Position { get; set; }
    }
}
