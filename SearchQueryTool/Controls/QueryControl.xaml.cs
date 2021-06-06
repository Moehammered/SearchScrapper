using SearchQueryTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SearchQueryTool.Controls
{
    /// <summary>
    /// Interaction logic for QueryControl.xaml
    /// </summary>
    public partial class QueryControl : UserControl
    {
        public SearchQuery Query => DataContext as SearchQuery;
        public QueryControl()
        {
            InitializeComponent();
        }

        private void LimitText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
