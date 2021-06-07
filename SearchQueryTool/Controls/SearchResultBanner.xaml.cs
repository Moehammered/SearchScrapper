using System.Diagnostics;
using System.Web;
using System.Windows;
using System.Windows.Controls;

namespace SearchQueryTool.Controls
{
    public partial class SearchResultBanner : UserControl
    {
        public SearchResultBanner()
        {
            InitializeComponent();
        }

        private void OpenUrl(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var url = HttpUtility.HtmlDecode(e.Uri.AbsoluteUri);
            var choice = MessageBox.Show($"Would you like to open this link?\n" +
                    $"{url}", "Open URL", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (choice == MessageBoxResult.Yes)
            {
                var processInfo = new ProcessStartInfo()
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(processInfo);
            }
        }
    }
}
