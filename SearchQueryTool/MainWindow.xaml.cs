using Newtonsoft.Json;
using SearchScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;

namespace SearchQueryTool
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient Client;

        public MainWindow()
        {
            InitializeComponent();
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44368");
        }

        private async void PerformSearch(object sender, RoutedEventArgs e)
        {
            var restCall = $"/api/GoogleSearch/Search?" +
                $"query={SearchQueryControl.Query.SearchTerm}" +
                $"&resultCount={SearchQueryControl.Query.ResultLimit}";

            try
            {
                var json = await Client.GetStringAsync(restCall);
                var results = JsonConvert.DeserializeObject<IEnumerable<SearchResult>>(json);
                MatchedDisplay.SetResults(results.Where(x => x.Url.Contains(SearchQueryControl.Query.UrlMatch)));
                AllResults.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void FakeSearch_Click(object sender, RoutedEventArgs e)
        {
            var restCall = $"/api/GoogleSearch/DummySearch?" +
                $"query={SearchQueryControl.Query.SearchTerm}" +
                $"&resultCount={SearchQueryControl.Query.ResultLimit}";

            try
            {
                var json = await Client.GetStringAsync(restCall);
                var results = JsonConvert.DeserializeObject<IEnumerable<SearchResult>>(json);
                MatchedDisplay.SetResults(results.Where(x => x.Url.Contains(SearchQueryControl.Query.UrlMatch)));
                AllResults.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
