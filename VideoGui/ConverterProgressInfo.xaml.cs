using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
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
using System.Xml.Linq;
using VideoGui.Models.delegates;

namespace VideoGui
{
    
  
    public partial class ConverterProgressInfo : Window
    {
        public string DestName = "";
        OnFinishByName DoOnFinish = null;
        ListBoxConnect DoListBoxConnect = null;
        AutoCancel DoAutoCancel = null;
        GetDataCount DoGetDataCount = null;
        DispatcherTimer DoDispatcherTimerCancel = null, DoResizeTimer = null;
        public ConverterProgressInfo(string _Name, OnFinishByName _DoOnFinish, ListBoxConnect _DoListBoxConnect, GetDataCount _DoGetDataCount)
        {
            InitializeComponent();
            DestName = _Name;
            DoOnFinish = _DoOnFinish;
            DoListBoxConnect = _DoListBoxConnect;
            DoGetDataCount = _DoGetDataCount;
            Title += $" {_Name}";            
          
        }

        public string GetDestNameNoExt()
        {
            return System.IO.Path.GetFileNameWithoutExtension(DestName);
        }

        public void DoAutoCancelClose()
        {
            try
            {
                if ((DoAutoCancel != null) && (DoAutoCancel.IsCloseAction))
                {
                    Close();
                }
                DoAutoCancel = null;
            }
            catch(Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.DoAutoCancelClose {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void SetupResizeTimer()
        {
            try
            {
                DoResizeTimer = new DispatcherTimer();
                DoResizeTimer.Interval = TimeSpan.FromSeconds(3);
                DoResizeTimer.Tick += DoResizeTimer_Tick;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.SetupResizeTimer {MethodBase.GetCurrentMethod().Name}");
            }

        }

        private void DoResizeTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                DoResizeTimer.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key.SetValue("ProgressHeight", wdwProgress.ActualHeight);
                key.SetValue("ProgressWidth", wdwProgress.ActualWidth);
                key?.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.SetupResizeTimer {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void ShowOnExit()
        {
            try
            {
                DoDispatcherTimerCancel = new DispatcherTimer();
                DoDispatcherTimerCancel.Interval = TimeSpan.FromMilliseconds(250);
                DoDispatcherTimerCancel.Tick += DoDispatcherTimerCancel_Tick;
                DoDispatcherTimerCancel.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ShowOnExit {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void DoDispatcherTimerCancel_Tick(object? sender, EventArgs e)
        {
            try
            {

                DoDispatcherTimerCancel.Stop();
                DoDispatcherTimerCancel = null;

                if (DoAutoCancel == null)
                {
                    DoAutoCancel = new AutoCancel(DoAutoCancelClose, DestName, 30);
                    DoAutoCancel.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ShowOnExit {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetupResizeTimer();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _Height = key.GetValueInt("ProgressHeight", -1);
                var _Width = key.GetValueInt("ProgressWidth", -1);
                key?.Close();
                wdwProgress.Height = (_Height != -1) ? _Height : wdwProgress.Height;
                wdwProgress.Width = (_Width != -1) ? _Height : wdwProgress.Width;
                ItemProgress.ItemsSource = DoListBoxConnect?.Invoke(System.IO.Path.GetFileNameWithoutExtension(DestName));
            }
            catch(Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.Window_Loaded {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DoOnFinish?.Invoke(Name);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.Window_Closing {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void ScrollIntoViewHandler(int count)
        {
            try
            {
                //int index = (DoGetDataCount!= null) ? DoGetDataCount.Invoke(Name) : -1;
                //I//temProgress.TabIndex = (index != -1) ? index : ItemProgress.TabIndex;
                var item = ItemProgress.Items.GetItemAt(count);
                lblstatus.Content = count.ToString();
                ItemProgress.ScrollIntoView(item);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ItemProgress_TargetUpdated {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ItemProgress_TargetUpdated {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void ItemProgress_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                int index = (DoGetDataCount != null) ? DoGetDataCount.Invoke(Name) : -1;
                ItemProgress.TabIndex = (index != -1) ? index : ItemProgress.TabIndex;
                var item = ItemProgress.Items.GetItemAt(index - 1);
                lblstatus.Content = index.ToString();
                ItemProgress.ScrollIntoView(item);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ItemProgress_TargetUpdated {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void wdwProgress_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (DoResizeTimer.IsEnabled)
                    {
                        DoResizeTimer.Stop();
                    }
                    DoResizeTimer.Start();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgressInfo.ItemProgress_TargetUpdated {MethodBase.GetCurrentMethod().Name}");
            }
        }
    }
}
