using SearchQueryViewModels.Utility;
using SearchScraping.Models;
using System.Web;

namespace SearchQueryViewModels.Search
{
    public class SearchResultViewModel
    {
        private readonly SearchResult Result;
        public string Position => PlaceFormat.NumberToPlace(Result.Position);
        public string Url => HttpUtility.UrlDecode(Result.Url);
        public string Heading => Result.Heading;

        public SearchResultViewModel()
            => Result = new SearchResult();

        public SearchResultViewModel(SearchResult result)
            => Result = result;
    }
}
