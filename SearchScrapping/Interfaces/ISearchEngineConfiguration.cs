namespace SearchScraping.Interfaces
{
    public interface ISearchEngineConfiguration
    {
        public string Name { get; set; }
        public string SearchUrl { get; set; }
        public string QueryParam { get; set; }
        public string CountParam { get; set; }
        public uint MaxCount { get; set; }
        public string OffsetParam { get; set; }
    }
}
