using System.Diagnostics;
using System.Web;
using System.Windows;

namespace SearchQueryTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Documents.Hyperlink link)
            {
                var url = HttpUtility.HtmlDecode(link.NavigateUri.AbsoluteUri);
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
}
