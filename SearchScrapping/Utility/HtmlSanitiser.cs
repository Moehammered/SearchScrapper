using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchScraping.Utility
{
    public class HtmlSanitiser
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

        public string Sanitise(in string html)
        {
            var copy = html;
            foreach (var tag in SelfTerminatingTags)
                copy = SanitiseTags(copy, tag, true);

            foreach (var tag in PairedHtmlTags)
                copy = SanitiseTags(copy, tag, false);

            foreach (var entity in FalseRefPatterns)
                copy = SanitiseAmpersandEntries(copy, entity);

            return copy;
        }

        public string SanitiseAmpersandEntries(in string html, in string entity)
        {
            var locations = FindSections(html, $"&{entity}", ";");
            var builder = new StringBuilder();
            var parsedLength = 0;

            foreach (var item in locations)
            {
                var before = html.Substring(parsedLength, item.Item1 - parsedLength);
                builder.Append(before);
                parsedLength = item.Item2;
            }

            builder.Append(html.Substring(parsedLength));
            return builder.ToString();
        }

        public string SanitiseTags(in string html, in string tag, bool selfTerminating = false)
        {
            var locations = FindTags(html, tag, selfTerminating);
            var builder = new StringBuilder();
            var parsedLength = 0;
            
            foreach (var item in locations)
            {
                var before = html.Substring(parsedLength, item.Item1 - parsedLength);
                builder.Append(before);
                parsedLength = item.Item2;
            }

            builder.Append(html.Substring(parsedLength));
            return builder.ToString();
        }

        public IEnumerable<(int,int)> FindTags(in string html, in string tagName, bool selfTerminating = false)
        {
            var last = (0, 0);
            var locations = new List<(int, int)>(10);
            do
            {
                last = FindTag(html, tagName, last.Item2, selfTerminating);
                locations.Add(last);
            } while (last != (-1, -1));
            locations.Remove((-1, -1));

            return locations;
        }

        public (int, int) FindTag(in string html, in string tagName, int offset = 0, bool selfTerminating = false)
        {
            var startTag = $"<{tagName}";
            var endTag = selfTerminating ? ">" : $"/{tagName}>";

            return FindSection(html, startTag, endTag, offset);
        }

        public IEnumerable<(int, int)> FindSections(in string html, in string entity, in string delimiter)
        {
            var last = (0, 0);
            var locations = new List<(int, int)>(10);
            do
            {
                last = FindSection(html, entity, delimiter, last.Item2);
                locations.Add(last);
            } while (last != (-1, -1));
            locations.Remove((-1, -1));

            return locations;
        }

        private (int,int) FindSection(in string html, in string entity, in string delimiter, int offset = 0)
        {
            var start = html.IndexOf(entity, offset);
            if (start != -1)
            {
                var end = html.IndexOf(delimiter, start);
                return (start, end + delimiter.Length);
            }
            else
                return (-1, -1);
        }
    }
}
