using Newtonsoft.Json;
using SearchScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchQueryTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient Client;
        private IEnumerable<SearchResult> Results { get; set; } = new[] 
        {
            new SearchResult() { Heading="1", Position=1, Url="www.1.com.au" },
            new SearchResult() { Heading="2", Position=2, Url="www.2.com.au" },
            new SearchResult() { Heading="3", Position=3, Url="www.3.com.au" },
            new SearchResult() { Heading="4", Position=4, Url="www.4.com.au" },
            new SearchResult() { Heading="5", Position=5, Url="www.5.com.au" },
            new SearchResult() { Heading="6", Position=6, Url="www.6.com.au" },
        };

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
                dataGrid.ItemsSource = JsonConvert.DeserializeObject<IEnumerable<SearchResult>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FakeSearch_Click(object sender, RoutedEventArgs e)
        {
            var query = SearchQueryControl.Query;
            MessageBox.Show($"{query.SearchTerm} - {query.UrlMatch} - {query.ResultLimit}");
            dataGrid.ItemsSource = Results;
        }
    }
}
