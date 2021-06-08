using SearchQueryViewModels.Base;
using SearchQueryViewModels.Commands;
using SearchScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SearchQueryViewModels.Search
{
    public class SeoViewModel : ReactiveViewModel
    {
        private IEnumerable<SearchResultViewModel> results { get; set; } = new SearchResultViewModel[0];
        public IEnumerable<SearchResultViewModel> Results
        {
            get => results;
            set
            {
                results = value;
                RaisePropertyChanged();
                Matches = results.Where(x => x.Url.ToLower().Contains(SearchQuery.Url.ToLower()));
            }
        }

        private IEnumerable<SearchResultViewModel> matches { get; set; } = new SearchResultViewModel[0];
        public IEnumerable<SearchResultViewModel> Matches
        {
            get => matches;
            private set
            {
                matches = value;
                RaisePropertyChanged();
            }
        }

        public SearchQueryViewModel SearchQuery { get; set; } = new SearchQueryViewModel()
        {
            ResultLimit = "100",
            SearchTerm = "conveyancing software",
            Url = "www.smokeball.com.au"
        };

        public ICommand FetchResultsAsync { get; set; }
        public bool Ready { get; set; } = true;
        public string WebServer { get; set; } = "https://localhost:44368";

        private readonly HttpClient Client;
        private readonly JsonSerializerOptions SerialisationOptions;

        public SeoViewModel()
        {
            Client = new HttpClient();
            FetchResultsAsync = new AsyncGenericCommand<object>(PerformSearchAsync, (o) => Ready);
            SerialisationOptions = new JsonSerializerOptions() 
            { 
                PropertyNameCaseInsensitive = true 
            };
        }

        private async Task PerformSearchAsync(object param)
        {
            Ready = false;
            RaisePropertyChanged(nameof(Ready));

            var restCall = $"{WebServer}/api/GoogleSearch/Search?" +
                    $"query={SearchQuery.SearchTerm}" +
                    $"&resultCount={SearchQuery.ResultLimit}";
            var json = await Client.GetStringAsync(restCall);

            var results = JsonSerializer.Deserialize<IEnumerable<SearchResult>>(json, SerialisationOptions);
            Results = results.Select(x => new SearchResultViewModel(x));

            Ready = true;
            RaisePropertyChanged(nameof(Ready));
        }
    }
}
