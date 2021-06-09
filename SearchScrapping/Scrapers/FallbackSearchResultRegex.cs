using SearchScraping.Interfaces;
using SearchScraping.Models;
using System.Collections.Generic;

namespace SearchScraping.Scrapers
{
    public class FallbackSearchResultRegex : IHtmlParser
    {
        private SearchResultRegex Primary { get; set; }
        private SearchResultRegex Fallback { get; set; }

        public FallbackSearchResultRegex(ISearchResultCaptureTemplate primary, ISearchResultCaptureTemplate fallback)
        {
            Primary = new SearchResultRegex(primary);
            Fallback = new SearchResultRegex(fallback);
        }

        public IEnumerable<SearchResult> ParseResults(string html)
        {
            if (Primary.CanParse(html))
                return Primary.ParseResults(html);
            else if (Fallback.CanParse(html))
                return Fallback.ParseResults(html);
            else
                return new SearchResult[0];
        }

        public bool CanParse(string html)
            => Primary.CanParse(html) || Fallback.CanParse(html);
    }
}
