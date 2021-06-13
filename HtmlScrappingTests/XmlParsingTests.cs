using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchScraping.Scrapers;
using System;
using System.IO;
using System.Xml.Linq;

namespace HtmlScrapingTests
{
    [TestClass]
    public class XmlParsingTests
    {
        private const string GoogleRobotFile = "google-parsing-tests/google-au_allLinks.html";
        private const string GoogleHumanFile = "google-parsing-tests/google-au_allLinks_spoofedheaders.html";

        private readonly string[] SelfTerminatingTags = new[] {
            "meta", "link", "!", "input", "img", "br", "wbr", "hr"
        };

        private readonly string[] PairedHtmlTags = new[] {
            "script", "noscript", "style"
        };

        private readonly string[] FalseRefPatterns = new[] {
            "nbsp", "middot"
        };

        private string LoadHtml(string file)
            => File.ReadAllText(file);

        [TestMethod]
        public void TestHtmlToXmlParser()
        {
            var humanGoogle = LoadHtml(GoogleHumanFile);
            var parser = new HtmlToXmlParser();
            var spannedDoc = parser.ParseViaSpan(humanGoogle);
            var substringDoc = parser.ParseViaString(humanGoogle);

            Assert.IsNotNull(spannedDoc);
            Assert.IsNotNull(substringDoc);
        }

        [TestMethod]
        public void StripGoogleRobotResultsViaSpan()
        {
            var html = LoadHtml(GoogleRobotFile);
            var stripped = StripHtmlTestViaSpan(html);
            var xdoc = XDocument.Parse(stripped.ToString());
            Assert.IsNotNull(xdoc);
        }

        [TestMethod]
        public void StripGoogleHumanResultsViaSpan()
        {
            var html = LoadHtml(GoogleHumanFile);
            var stripped = StripHtmlTestViaSpan(html);
            var xdoc = XDocument.Parse(stripped.ToString());
            Assert.IsNotNull(xdoc);
        }

        [TestMethod]
        public void ReadSpanStrip()
        {
            var html = LoadHtml("google-parsing-tests/random.org.html");
            var result = StripHtmlTestViaSpan(html);
            var t = result.ToString();
        }

        [TestMethod]
        public void StripRandomOrg()
        {
            var html = LoadHtml("google-parsing-tests/random.org.html");
            StripHtmlTest(html);
        }

        public string StripHtmlTest(string html)
        {
            var copy = html;
            foreach (var tag in SelfTerminatingTags)
            {
                copy = StripTags(copy, tag);
                Assert.IsFalse(copy.Contains($"<{tag}"));
            }

            foreach (var tag in PairedHtmlTags)
            {
                copy = StripTags(copy, tag, false);
                Assert.IsFalse(copy.Contains($"<{tag}") || copy.Contains($"/{tag}>"));
            }

            return copy;
        }

        public ReadOnlySpan<char> StripHtmlTestViaSpan(ReadOnlySpan<char> html)
        {
            var copy = html;
            foreach (var tag in SelfTerminatingTags)
            {
                copy = StripTagsSpan(copy, tag);
                Assert.IsFalse(copy.ToString().Contains($"<{tag}"));
            }

            foreach (var tag in PairedHtmlTags)
            {
                copy = StripTagsSpan(copy, tag, false);
                var t = copy.ToString();
                Assert.IsFalse(t.Contains($"<{tag}") || t.Contains($"/{tag}>"));
            }

            foreach (var r in FalseRefPatterns)
            {
                copy = RemoveFalseReferencesViaSpan(copy, r);
                var t = copy.ToString();
                Assert.IsFalse(t.Contains($"&{r};"));
            }

            return copy;
        }

        private string StripTags(in string original, in string tagName, bool selfTerminating = true)
        {
            var startTag = $"<{tagName}";
            var endingTag = selfTerminating ? ">" : $"/{tagName}>";
            var startInd = original.IndexOf(startTag);
            var copy = original;
            //var lastPlace = startInd;
            var s = copy.AsSpan();

            while (startInd >= 0)
            {
                var start = copy.AsSpan().Slice(startInd);
                var afterInd = start.IndexOf(endingTag);
                if (afterInd >= 0)
                {
                    //var evicted = copy.AsSpan().Slice(startInd, afterInd + endingTag.Length).ToString();
                    copy = copy.Remove(startInd, afterInd + endingTag.Length);
                }
                startInd = copy.IndexOf(startTag);
            }

            return copy;
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
                    //var evicted = start.Slice(afterInd + delimiter.Length).ToString();
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
            //var lastPlace = startInd;

            while (startInd >= 0)
            {
                var start = copy.Slice(startInd);
                var afterInd = start.IndexOf(endingTag);
                if (afterInd >= 0)
                {
                    //var evicted = start.Slice(afterInd + endingTag.Length).ToString();
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
    }
}
