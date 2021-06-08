using System.Collections.Generic;

namespace SearchScraping.Interfaces
{
    public interface IHttpHeaderProvider
    {
        IEnumerable<KeyValuePair<string, string>> ValidatedHeaders { get; }
        IEnumerable<KeyValuePair<string, string>> SpecialHeaders { get; }
    }
}
