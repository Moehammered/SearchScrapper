using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SearchScrapperWebService.Filters;
using SearchScraping.Interfaces;
using SearchScraping.Models;
using SearchScraping.Models.Configuration;
using SearchScraping.Scrappers;
using SearchScraping.Services;
using SearchScraping.Templates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScrapperWebService.Controllers
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
        [ValidateSearchParams("query", "resultCount")]
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

        [HttpGet("DumpEngine")]
        public string DumpEngineConfig()
        {
            return GoogleConfig?.ToString() ?? "Google configuration unavailable";
        }
    }
}
