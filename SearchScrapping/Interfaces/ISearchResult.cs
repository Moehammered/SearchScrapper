namespace SearchScraping.Interfaces
{
    public interface ISearchResult
    {
        string Heading { get; set; }
        string Url { get; set; }
        int Position { get; set; }
    }
}
