using SearchScraping.Interfaces;
using System.Text.RegularExpressions;

namespace SearchScraping.Templates
{
    public class SearchResultCaptureTemplate : ISearchResultCaptureTemplate
    {
        public string Regex { get; set; }
        public string Template { get; set; }
        public string TemplateValue { get; set; }
        public string Pattern => Regex?.Replace(Template, TemplateValue) ?? "";

        public int UrlGroupIndex { get; set; }
        public string UrlGroupName { get; set; }
        public int HeadingGroupIndex { get; set; }
        public string HeadingGroupName { get; set; }

        public Regex BuildRegex() => new Regex(Pattern);

        public override string ToString()
            => $"{Pattern}\n" +
            $"Url Capture Group: [{UrlGroupIndex}]'{UrlGroupName}'\n" +
            $"Heading Capture Group: [{HeadingGroupName}]'{HeadingGroupName}'";
    }
}
