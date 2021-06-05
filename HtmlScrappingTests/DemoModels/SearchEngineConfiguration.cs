namespace HtmlScrappingTests.DemoModels
{
    class SearchEngineConfiguration
    {
        public string Name { get; set; }
        public string SearchUrl { get; set; }
        public string QueryParam { get; set; }
        public string CountParam { get; set; }
        public int MaxCount { get; set; }
        public string OffsetParam { get; set; }
        public string Regex { get; set; }
        public string DivClassTemplate { get; set; }
        public string DivClass { get; set; }

        public string BuildRegexPattern()
            => Regex.Replace(DivClassTemplate, DivClass);
    }
}
