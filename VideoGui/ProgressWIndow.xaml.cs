using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ProgressWIndow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public delegate void CancelScan();
        public CancelScan OnCancelScan;
        public ProgressWindow(CancelScan _OnCancelScan, string Status, string ScanningPath, int Max, int min = 0)
        {
            try
            {

                OnCancelScan = _OnCancelScan;
                InitializeComponent();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lblScanningPath.Content = "Scanning Path";
                    lblScanningFileName.Content = ScanningPath;
                    lblCurrent.Content = min.ToString();
                    lblStatus.Content = Status;
                    lblMax.Content = (Max != -1) ? Max.ToString() : "";
                    if (Max != -1)
                    {
                        ProgressBar1.Maximum = Max;
                        ProgressBar1.Value = min;
                    }
                });

                BtnCancel.IsEnabled = (OnCancelScan != null);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void UpdateStatus(string status)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lblStatus.Content = status;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void UpdateCount(int count)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressBar1.Maximum = count;
                    lblMax.Content = count;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        public void UpdateProgress(int CurrentProgress, string filename)
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    // lblStatus.Content
                    ProgressBar1.Value = CurrentProgress;
                    lblCurrentFile.Content = filename;
                    lblCurrent.Content = CurrentProgress.ToString();

                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCancelScan?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void ProgressWindowStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.DragMove();
                });

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
    }
}
