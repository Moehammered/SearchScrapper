using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HtmlScrappingTests
{
    [TestClass]
    public class InitialHtmlTests
    {
        private const string QueryResultClass = "kCrYT";
        private readonly string QueryResultPattern = $"<div class=\"{QueryResultClass}\">(?:<)a[^>]*href=\"(.*?)\">.*?<.*?<\\/a>";
        
        [TestMethod]
        public async Task SinglePageRequest()
        {
            var urlQuery = @"https://www.google.com.au/search?num=100&q=conveyancing+software";

            //http request
            var client = new HttpClient();
            var response = await client.GetStringAsync(urlQuery);
            await System.IO.File.WriteAllTextAsync("_allLinks.html", response);

            var matches = System.Text.RegularExpressions.Regex.Matches(response, QueryResultPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var successfulMatches = matches.Cast<System.Text.RegularExpressions.Match>().Where(x => x.Success);
            var joinedLinks = string.Join("\n", successfulMatches.Select(x => x.Value));
            await System.IO.File.WriteAllTextAsync("_allLinks.html_list.txt", joinedLinks);
        }

        [TestMethod]
        public async Task AsyncPageRequestsTest()
        {
            var itemCount = 100;
            var itemsPerPage = 10;

            var baseUrl = @"https://www.google.com.au/search?q=conveyancing+software&";
            var queries = Enumerable.Range(0, itemCount / itemsPerPage)
                .Select(index => $@"{baseUrl}num={itemsPerPage}&start={index*itemsPerPage}");

            var client = new HttpClient();
            var jobs = queries.Select(x => client.GetStringAsync(x));

            var pages = await Task.WhenAll(jobs);
            var fileTasks = pages.Select((x, i) => System.IO.File.WriteAllTextAsync($"{i}.html", x));
            await Task.WhenAll(fileTasks);
        }
    }
}
