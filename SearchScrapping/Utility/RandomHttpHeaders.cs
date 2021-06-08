using SearchScraping.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchScraping.Utility
{
    public class RandomHttpHeaders : IHttpHeaderProvider
    {
        public IEnumerable<KeyValuePair<string, string>> ValidatedHeaders => ProduceRandomHeaders();
        public IEnumerable<KeyValuePair<string, string>> SpecialHeaders { get; }

        private string[] AcceptHeaders { get; set; }
        private string[] UserAgents { get; set; }
        private string[] AcceptLanguages { get; set; }
        private const string DefaultLanguage = "en-AU,en;q=0.9";

        private readonly Random Randomiser;

        public RandomHttpHeaders(
            IEnumerable<string> accepts,
            IEnumerable<string> agents,
            IEnumerable<string> languages,
            IEnumerable<KeyValuePair<string, string>> special
            )
        {
            AcceptHeaders = accepts.ToArray();
            UserAgents = agents.ToArray();
            AcceptLanguages = languages.ToArray();
            SpecialHeaders = special;

            Randomiser = new Random();
        }

        private IEnumerable<KeyValuePair<string, string>> ProduceRandomHeaders()
        {
            var accept = new KeyValuePair<string, string>("accept", Pick(AcceptHeaders));
            var agent = new KeyValuePair<string, string>("user-agent", Pick(UserAgents));
            var language = BuildLanguageHeader(AcceptLanguages);

            return new[] { accept, agent, language };
        }

        private KeyValuePair<string, string> BuildLanguageHeader(in string[] values)
        {
            var languages = PickMultiple(values);

            var headerValue = string.Join(",", languages);
            if (!headerValue.Contains(DefaultLanguage))
                headerValue = DefaultLanguage + "," + headerValue;

            return new KeyValuePair<string, string>("accept-language", headerValue);
        }

        private IEnumerable<string> PickMultiple(in string[] values)
        {
            if (values.Any())
            {
                var amount = Randomiser.Next(1, values.Length - 1);
                var permutator = new PermutationRandomiser<string>(values);
                return Enumerable.Range(0, amount).Select(x => permutator.Next());
            }
            else
                return new[] { DefaultLanguage };
        }

        private string Pick(in string[] values)
        {
            var rndInd = Randomiser.Next(0, values.Length - 1);
            return values[rndInd];
        }
    }
}
