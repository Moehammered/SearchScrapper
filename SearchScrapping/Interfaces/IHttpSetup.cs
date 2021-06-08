using System.Net.Http;

namespace SearchScraping.Interfaces
{
    public interface IHttpSetup
    {
        void ConfigureHttpClient(HttpClient client);
    }
}
