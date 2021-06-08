using SearchScraping.Interfaces;
using System.Net.Http;

namespace SearchScraping.Utility
{
    public class HttpHeaderUtility : IHttpSetup
    {
        private IHttpHeaderProvider HeaderProvider { get; set; }
        private bool Clear { get; set; }
        public HttpHeaderUtility(IHttpHeaderProvider provider, bool clearHeaders)
            => (HeaderProvider, Clear) = (provider, clearHeaders);

        public void ConfigureHttpClient(HttpClient client)
        {
            if (Clear)
                client.DefaultRequestHeaders.Clear();

            foreach (var header in HeaderProvider.ValidatedHeaders)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

            foreach (var header in HeaderProvider.SpecialHeaders)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
    }
}
