using SearchScraping.Interfaces;

namespace SearchScraping.Models.Configuration
{
    public class SearchEngineConfiguration : ISearchEngineConfiguration
    {
        public string Name { get; set; }
        public string SearchUrl { get; set; }
        public string QueryParam { get; set; }
        public string CountParam { get; set; }
        public uint MaxCount { get; set; }
        public string OffsetParam { get; set; }

        public override string ToString()
            => $"{Name} - {SearchUrl}\n" +
            $"query param: {QueryParam}, offset param: {OffsetParam}\n" +
            $"count param: {CountParam}, max count: {MaxCount}";
    }
}
