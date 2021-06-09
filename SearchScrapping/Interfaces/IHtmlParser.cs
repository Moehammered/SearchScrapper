using SearchScraping.Models;
using System.Collections.Generic;

namespace SearchScraping.Interfaces
{
    public interface IHtmlParser
    {
        bool CanParse(string html);
        IEnumerable<SearchResult> ParseResults(string html);
    }
}
