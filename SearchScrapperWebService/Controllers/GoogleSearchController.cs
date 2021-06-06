using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SearchScraperWebService.Filters;
using SearchScraping.Interfaces;
using SearchScraping.Models;
using SearchScraping.Models.Configuration;
using SearchScraping.Scrappers;
using SearchScraping.Services;
using SearchScraping.Templates;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScraperWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSearchController : ControllerBase
    {
        private ISearchEngineConfiguration GoogleConfig { get; set; }
        private ISearchService SearchSvc { get; set; }
        private IHtmlParser HtmlParser { get; set; }

        public GoogleSearchController(
            IOptions<SearchResultCaptureTemplate> regex,
            IOptions<SearchEngineConfiguration> engineConfigs,
            ISearchService service
            )
        {
            GoogleConfig = engineConfigs.Value;
            SearchSvc = service;
            HtmlParser = new SearchResultRegex(regex.Value);
        }

        [HttpGet("Search")]
        [ValidateEmptyStringParams]
        [ProducesResponseType(typeof(IEnumerable<SearchResult>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SearchAsync(CancellationToken ct, string query, uint resultCount = 100)
        {
            var html = await SearchSvc.FetchQueryResultsAsync(GoogleConfig, query, resultCount, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                return BadRequest($"Search term '{query}' returned nothing when searching Google. " +
                    $"Please wait then try again.");
            }
            else
                return Ok(HtmlParser.ParseResults(html));
        }

        [HttpGet("Ranking")]
        [ValidateEmptyStringParams]
        [ValidatePositiveIntegerParams]
        [ProducesResponseType(typeof(SearchResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> FindRankingAsync(CancellationToken ct, string query, string url, uint resultCount = 100)
        {
            var html = await SearchSvc.FetchQueryResultsAsync(GoogleConfig, query, resultCount, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                return BadRequest($"Search term '{query}' returned nothing when searching Google. " +
                    $"Please wait then try again.");
            }
            else
            {
                var results = HtmlParser.ParseResults(html);
                var result = results.FirstOrDefault(x => x.Url.ToLower().Contains(url.ToLower()));
                return Ok(result);
            }
        }
    }
}
