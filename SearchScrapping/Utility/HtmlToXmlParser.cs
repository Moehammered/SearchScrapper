using SearchScraping.Interfaces;
using SearchScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SearchScraping.Scrapers
{
    public class HtmlToXmlParser
    {
        private readonly string[] SelfTerminatingTags = new[] {
            "meta", "link", "!", "input", "img", "br", "wbr", "hr"
        };

        private readonly string[] PairedHtmlTags = new[] {
            "script", "noscript", "style"
        };

        private readonly string[] FalseRefPatterns = new[] {
            "nbsp", "middot"
        };

        public ReadOnlySpan<char> StripHtmlTestViaSpan(ReadOnlySpan<char> html)
        {
            var copy = html;
            foreach (var tag in SelfTerminatingTags)
            {
                copy = StripTagsSpan(copy, tag);
            }

            foreach (var tag in PairedHtmlTags)
            {
                copy = StripTagsSpan(copy, tag, false);
            }

            foreach (var r in FalseRefPatterns)
            {
                copy = RemoveFalseReferencesViaSpan(copy, r);
            }

            return copy;
        }

        public XDocument ParseViaSpan(ReadOnlySpan<char> html)
        {
            var sanitised = StripHtmlTestViaSpan(html);
            return XDocument.Parse(sanitised.ToString());
        }

        public XDocument ParseViaString(in string html)
        {
            var sanitised = StripHtmlTest(html);
            return XDocument.Parse(sanitised);
        }

        private ReadOnlySpan<char> RemoveFalseReferencesViaSpan(ReadOnlySpan<char> original, ReadOnlySpan<char> entityName)
        {
            ReadOnlySpan<char> refPattern = string.Concat("&", entityName);
            var delimiter = ";".AsSpan();
            var startInd = original.IndexOf(refPattern);
            var copy = original;

            while (startInd >= 0)
            {
                var start = copy.Slice(startInd);
                var afterInd = start.IndexOf(delimiter);
                if (afterInd >= 0)
                {
                    var after = start.Slice(afterInd + delimiter.Length);
                    var newlyMade = string.Concat(copy.Slice(0, startInd), after);
                    copy = newlyMade;
                    startInd = copy.IndexOf(refPattern);
                }
                else
                    break;
            }

            return copy;
        }

        private ReadOnlySpan<char> StripTagsSpan(ReadOnlySpan<char> original, ReadOnlySpan<char> tagName, bool selfTerminating = true)
        {
            ReadOnlySpan<char> startTag = string.Concat("<", tagName);
            var endingTag = selfTerminating
                ? ">".AsSpan()
                : string.Concat("/", tagName, ">");

            var startInd = original.IndexOf(startTag);
            var copy = original;

            while (startInd >= 0)
            {
                var start = copy.Slice(startInd);
                var afterInd = start.IndexOf(endingTag);
                if (afterInd >= 0)
                {
                    var after = start.Slice(afterInd + endingTag.Length);
                    var newlyMade = string.Concat(copy.Slice(0, startInd), after);
                    copy = newlyMade;
                    startInd = copy.IndexOf(startTag);
                }
                else
                    break;
            }

            return copy;
        }

        public string StripHtmlTest(string html)
        {
            var copy = html;
            foreach (var tag in SelfTerminatingTags)
            {
                copy = StripTags(copy, tag);
            }

            foreach (var tag in PairedHtmlTags)
            {
                copy = StripTags(copy, tag, false);
            }

            foreach (var r in FalseRefPatterns)
            {
                copy = RemoveFalseReferences(copy, r);
            }

            return copy;
        }

        private string StripTags(in string original, in string tagName, bool selfTerminating = true)
        {
            var startTag = $"<{tagName}";
            var endingTag = selfTerminating ? ">" : $"/{tagName}>";
            var startInd = original.IndexOf(startTag);
            var copy = original;

            while (startInd >= 0)
            {
                var start = copy.AsSpan().Slice(startInd);
                var afterInd = start.IndexOf(endingTag);
                if (afterInd >= 0)
                    copy = copy.Remove(startInd, afterInd + endingTag.Length);
            
                startInd = copy.IndexOf(startTag);
            }

            return copy;
        }

        private string RemoveFalseReferences(in string original, in string entityName)
        {
            var falseEntity = $"&{entityName}";
            var delimiter = ";";
            var startInd = original.IndexOf(falseEntity);
            var copy = original;

            while (startInd >= 0)
            {
                var start = copy.AsSpan().Slice(startInd);
                var afterInd = start.IndexOf(delimiter);
                if (afterInd >= 0)
                    copy = copy.Remove(startInd, afterInd + delimiter.Length);

                startInd = copy.IndexOf(falseEntity);
            }

            return copy;
        }
    }
}
