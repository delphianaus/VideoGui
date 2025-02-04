using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ActionScheduleSelector.xaml
    /// </summary>
    public partial class ActionScheduleSelector : Window
    {
        databasehook<Object> ModuleCallBack = null;
        public bool IsClosed = false, IsClosing = false;
        public ActionScheduleSelector(OnFinish DoOnFinish, databasehook<Object> _ModuleCallBack)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _ModuleCallBack;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ActionScheduleSelector - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (e.WidthChanged)
                    {
                        lstHeader.Width = e.NewSize.Width - 17;// 800-783 = 17
                        lstItems.Width = e.NewSize.Width - 17;
                    }
                    if (e.HeightChanged)
                    {
                        lstItems.Height = e.NewSize.Height - 123;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

    }
}
