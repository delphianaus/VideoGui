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
using System.Windows.Threading;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for AutoCancel.xaml
    /// </summary>
    /// 

   
    public partial class AutoCancel : Window
    {
        public bool IsCloseAction = false, IsClosed = false, IsClosing = false;
        DispatcherTimer AutoCloseTimer;
        int dispatchcnt = 0;
        int TotalTime = 30;
        string DestName;
        OnFinish DoOnFinished = null;
        public AutoCancel(OnFinish _OnFinished, string destName, int _totaltime = 30, string captiontitle = "")
        {
            InitializeComponent();
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; DoOnFinished?.Invoke(); };
            TotalTime = _totaltime;
            DoOnFinished = _OnFinished;
            dispatchcnt = _totaltime;
            lblTime.Content = TotalTime.ToString();
            DestName = destName;
            lblDestName.Content = DestName;
            lblYouTubeHelper.Content = (captiontitle != "") ? captiontitle : lblYouTubeHelper.Content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsCloseAction = true;
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.BtnClose_Click {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                IsCloseAction = false;
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.BtnClose_Click {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AutoCloseTimer = new DispatcherTimer();
                AutoCloseTimer.Tick += new EventHandler(AutoCloseTimer_Tick);
                AutoCloseTimer.Interval = new TimeSpan(0, 0, 1);
                AutoCloseTimer.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.BtnClose_Click {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void AutoCloseTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                dispatchcnt--;
                lblTime.Content = dispatchcnt.ToString();
                if ( dispatchcnt == 0)
                {
                    IsCloseAction = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.AutoCloseTimer_Tick {MethodBase.GetCurrentMethod().Name}");
            }
        }

        
    }
}
