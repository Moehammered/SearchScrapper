using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchScraping.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HtmlScrapingTests
{
    [TestClass]
    public class HtmlSanitisationTests
    {
        private const string HtmlFolder = "google-parsing-tests";
        private string RandomOrgSite => LoadHtml("random.org.html");
        private string GoogleHumanSite => LoadHtml("google-au_allLinks_spoofedheaders.html");
        private readonly HtmlSanitiser Sanitiser = new HtmlSanitiser();
        private readonly string[] SelfTerminatingTags = new[] {
            "meta", "link", "!", "input", "img", "br", "wbr", "hr"
        };

        private readonly string[] PairedHtmlTags = new[] {
            "script", "noscript", "style"
        };

        private readonly string[] FalseRefPatterns = new[] {
            "nbsp", "middot"
        };

        [TestMethod]
        public void FindTagsTest()
        {
            var tags = Sanitiser.FindTag(RandomOrgSite, "link", 0, true);
            Assert.AreEqual(181, tags.Item1);
        }

        [TestMethod]
        public void BuildTagLocationList()
        {
            var site = RandomOrgSite;
            var target = "link";
            var locations = Sanitiser.FindTags(site, target, true);

            Assert.IsTrue(locations.Any());
            foreach(var item in locations)
            {
                var str = site.Substring(item.Item1, item.Item2 - item.Item1);
                Assert.IsTrue(str.StartsWith($"<{target}"));
                Assert.IsTrue(str.EndsWith($">"));
            }
        }

        [TestMethod]
        public void StringBuildTagsTest()
        {
            var site = RandomOrgSite;
            var target = "link";
            var sanitised = Sanitiser.SanitiseTags(site, target, true);
            Assert.IsFalse(sanitised.Contains($"<{target}"));
        }

        [TestMethod]
        public void RemoveAllTagsTest()
        {
            string parse(in string site)
            {
                var copy = site;
                foreach (var tag in SelfTerminatingTags)
                {
                    copy = Sanitiser.SanitiseTags(copy, tag, true);
                    Assert.IsFalse(copy.Contains($"<{tag}"));
                }

                foreach (var tag in PairedHtmlTags)
                {
                    copy = Sanitiser.SanitiseTags(copy, tag, false);
                    Assert.IsFalse(copy.Contains($"<{tag}"));
                }

                foreach(var entity in FalseRefPatterns)
                {
                    copy = Sanitiser.SanitiseAmpersandEntries(copy, entity);
                    Assert.IsFalse(copy.Contains($"&{entity};"));
                }

                return copy;
            }
            var sanitised = Sanitiser.Sanitise(RandomOrgSite);
            var parsed = parse(RandomOrgSite);
            Assert.AreEqual(parsed, sanitised);

            var google = Sanitiser.Sanitise(GoogleHumanSite);
            var gparsed = parse(GoogleHumanSite);
            Assert.AreEqual(gparsed, google);
        }

        private string LoadHtml(string name)
        {
            var path = System.IO.Path.Combine(HtmlFolder, name);
            return System.IO.File.ReadAllText(path);
        }
    }
}
