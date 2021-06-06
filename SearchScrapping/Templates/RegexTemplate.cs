using SearchScraping.Interfaces;
using System.Text.RegularExpressions;

namespace SearchScraping.Templates
{
    public class RegexTemplate : IRegexTemplate
    {
        public string Regex { get; set; }
        public string Template { get; set; }
        public string TemplateValue { get; set; }
        public string Pattern => Regex.Replace(Template, TemplateValue);
        public Regex BuildRegex() => new Regex(Pattern);
    }
}
