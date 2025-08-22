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
        System.Threading.Timer AutoCloseTimer;
        int dispatchcnt = 0;
        int TotalTime = 30;
        string DestName;
        public AutoCancel(OnFinishIdObj _OnFinished, string destName,
            int _totaltime = 30, string captiontitle = "")
        {
            InitializeComponent();
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _OnFinished?.Invoke(this, 0); };
            TotalTime = _totaltime;
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

        DateTime timereventtime = DateTime.Now;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AutoCloseTimer = new System.Threading.Timer(TimerEvent_Handler, null, 0, 250);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.BtnClose_Click {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void TimerEvent_Handler(object? state)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => TimerEvent_Handler(state));
                    return;
                }
                DateTime nowx = DateTime.Now;
                if (nowx.Subtract(timereventtime).TotalSeconds >= 1)
                {
                    timereventtime = nowx;
                    dispatchcnt--;
                    if (dispatchcnt <= 0)
                    {
                        IsCloseAction = true;
                        Close();
                    }
                    Dispatcher.Invoke(() =>
                    {
                        lblTime.Content = (dispatchcnt <= 1) ? "1" : dispatchcnt.ToString();
                    });

                    if (dispatchcnt == 1)
                    {
                        AutoCloseTimer.Change(0, 10);
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    if (dispatchcnt.ToString() == "" || dispatchcnt < 0)
                    {
                        IsCloseAction = true;
                        Close();
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoCancel.TimerEvent_Handler {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void AutoCloseTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AutoCloseTimer_Tick(sender, e));
                    return;
                }
                dispatchcnt--;
                lblTime.Content = dispatchcnt.ToString();
                if (dispatchcnt == 0)
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
