using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HtmlScrapingTests
{
    [TestClass]
    public class XmlParsingTests
    {
        private string LoadHtml(string file)
            => File.ReadAllText(file);

        [TestMethod]
        public void StripRandomOrg()
        {
            var html = LoadHtml("google-parsing-tests/random.org.html");
            StripHtmlTest(html);
        }

        public string StripHtmlTest(string html)
        {
            var selfTerminating = new[] {
                "meta", "link", "!"
            };

            var copy = html;
            foreach (var tag in selfTerminating)
            {
                copy = StripTags(copy, tag);
                Assert.IsFalse(copy.Contains($"<{tag}"));
            }

            var paired = new[] {
                "script", "noscript", "style"
            };
            foreach (var tag in paired)
            {
                copy = StripTags(copy, tag, false);
                Assert.IsFalse(copy.Contains($"<{tag}") || copy.Contains($"/{tag}>"));
            }

            return copy;
        }

        private string StripTags(in string original, in string tagName, bool selfTerminating = true)
        {
            var startTag = $"<{tagName}";
            var endingTag = selfTerminating ? ">" : $"/{tagName}>";
            var startInd = original.IndexOf(startTag);
            var copy = original;
            var lastPlace = startInd;

            while (startInd >= 0)
            {
                var start = copy.AsSpan().Slice(startInd);
                var afterInd = start.IndexOf(endingTag);
                if (afterInd >= 0)
                {
                    var evicted = copy.AsSpan().Slice(startInd, afterInd + endingTag.Length).ToString();
                    copy = copy.Remove(startInd, afterInd + endingTag.Length);
                }
                startInd = copy.IndexOf(startTag);
            }

            return copy;
        }
    }
}
