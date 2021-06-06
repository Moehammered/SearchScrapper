using SearchScraping.Interfaces;
using SearchScraping.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SearchScraping.Scrappers
{
    public class SearchResultRegex : IHtmlParser
    {
        private Regex HtmlRegex { get; set; }
        private ISearchResultCaptureTemplate CaptureConfig { get; set; }

        public SearchResultRegex(ISearchResultCaptureTemplate captureConfig)
        {
            CaptureConfig = captureConfig;
            HtmlRegex = CaptureConfig.BuildRegex();
        }

        public IEnumerable<SearchResult> ParseResults(string html)
        {
            var index = 0;
            var match = HtmlRegex.Match(html);

            while (match.Success)
            {
                ++index;

                if (match.Groups.Count > 1)
                    yield return new SearchResult()
                    {
                        Position = index,
                        Url = match.Groups[CaptureConfig.UrlGroupIndex].Value,
                        Heading = match.Groups[CaptureConfig.HeadingGroupIndex].Value
                    };

                match = match.NextMatch();
            }
        }
    }
}
