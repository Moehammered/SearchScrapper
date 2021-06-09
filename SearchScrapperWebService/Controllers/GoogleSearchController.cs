using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SearchScraperWebService.Filters;
using SearchScraping.Interfaces;
using SearchScraping.Models;
using SearchScraping.Models.Configuration;
using SearchScraping.Scrapers;
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
        [ProducesResponseType(typeof(IEnumerable<int>), 200)]
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
                var matches = results.Where(x => x.Url.ToLower().Contains(url.ToLower()));
                return Ok(matches.Select(x => x.Position));
            }
        }

        // debugging -- dummy versions
        [HttpGet("DummySearch")]
        [ValidateEmptyStringParams]
        [ProducesResponseType(typeof(IEnumerable<SearchResult>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SearchDummyAsync(CancellationToken ct, string query, uint resultCount = 100)
        {
            var html = await System.IO.File.ReadAllTextAsync("dummy-data/google-au_allLinks.html");
            if (string.IsNullOrWhiteSpace(html))
            {
                return BadRequest($"Search term '{query}' returned nothing when searching Google. " +
                    $"Please wait then try again.");
            }
            else
                return Ok(HtmlParser.ParseResults(html));
        }

        [HttpGet("DummyRanking")]
        [ValidateEmptyStringParams]
        [ValidatePositiveIntegerParams]
        [ProducesResponseType(typeof(IEnumerable<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> FindRankingDummyAsync(CancellationToken ct, string query, string url, uint resultCount = 100)
        {
            var html = await System.IO.File.ReadAllTextAsync("dummy-data/google-au_allLinks.html");
            if (string.IsNullOrWhiteSpace(html))
            {
                return BadRequest($"Search term '{query}' returned nothing when searching Google. " +
                    $"Please wait then try again.");
            }
            else
            {
                var results = HtmlParser.ParseResults(html);
                var matches = results.Where(x => x.Url.ToLower().Contains(url.ToLower()));
                return Ok(matches.Select(x => x.Position));
            }
        }
    }
}
