using SearchScraping.Models;
using System.Collections.Generic;

namespace SearchScraping.Interfaces
{
    public interface IHtmlParser
    {
        IEnumerable<SearchResult> ParseResults(string html);
    }
}
