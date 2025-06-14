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
    /// Interaction logic for MultiShortsUploader.xaml
    /// </summary>
    public partial class MultiShortsUploader : Window
    {
        databasehook<object> dbInit = null;
        public bool IsClosing = false, IsClosed = false;
        public MultiShortsUploader(databasehook<object> _dbInit, OnFinish _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                dbInit = _dbInit;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInit?.Invoke(this, new CustomParams_Initialize());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (e.HeightChanged)
                    {
                        grpschedules.Height = e.NewSize.Height - 380;
                        lstActiveScheduleItems.Height = cnvschedules.Height - 34;
                    }
                    if (e.WidthChanged)
                    {
                        lstActiveScheduleItems.Width= cnvschedules.Width - 12;
                        lstActiveSchedulesTitles.Width = lstActiveScheduleItems.Width;
                        lstShorts.Width = lstActiveScheduleItems.Width;
                        lstShortsDirectoryTitles.Width = lstShorts.Width;

                    }
                }    
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                ex.LogWrite($"BtnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }
}
