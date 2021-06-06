namespace SearchScraping.Interfaces
{
    public interface ISearchResultCaptureTemplate : IRegexTemplate
    {
        int UrlGroupIndex { get; set; }
        string UrlGroupName { get; set; }
        int HeadingGroupIndex { get; set; }
        string HeadingGroupName { get; set; }
    }
}
