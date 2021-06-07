using SearchScraping.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SearchQueryTool.Controls
{
    public partial class SearchResultsList : UserControl
    {
        public SearchResultsList()
        {
            InitializeComponent();
        }

        public void SetResults(IEnumerable<SearchResult> results)
        {
            listBox.ItemsSource = results;
        }
    }
}
