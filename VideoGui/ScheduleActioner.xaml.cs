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
using static VideoGui.TitleSelectFrm;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ScheduleActioner.xaml
    /// </summary>
    public partial class ScheduleActioner : Window
    {
        databasehook<Object> ModuleCallBack = null;
        bool IsClosed = false, IsClosing = false;
        public ScheduleActioner(OnFinish DoOnFinish, databasehook<Object> _ModuleCallBack)
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
                ex.LogWrite($"ScheduleActioner - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
       
    }
}
