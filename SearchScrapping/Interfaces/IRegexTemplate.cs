using System.Text.RegularExpressions;

namespace SearchScraping.Interfaces
{
    public interface IRegexTemplate
    {
        string Regex { get; set; }
        string Template { get; set; }
        string TemplateValue { get; set; }
        string Pattern { get; }
        Regex BuildRegex();
    }
}
