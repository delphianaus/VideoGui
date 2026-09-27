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
using System.Windows.Shapes;
using Windows.UI.Composition;
using Wpf.Ui.Controls;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for TestListboxResizer.xaml
    /// </summary>
    public partial class TestListboxResizer : FluentWindow
    {
        public TestListboxResizer()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
