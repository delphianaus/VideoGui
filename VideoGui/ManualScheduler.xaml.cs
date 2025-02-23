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
    /// Interaction logic for ManualScheduler.xaml
    /// </summary>
    public partial class ManualScheduler : Window
    {
        databasehook<Object> ModuleCallBack = null;
        public bool IsClosed = false, IsClosing = false, IsCopy = false, HasValues = false;
        public Nullable<DateTime> SelectedDate = null;
        public Nullable<TimeOnly> StartTime = null, EndTime = null;
        public Nullable<int> Max = null;
        public ManualScheduler(databasehook<Object> _ModuleCallBack, OnFinish DoOnFinish)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _ModuleCallBack;
                Closing += (s, e) => { GetValues(); IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public void GetValues()
        {
            try
            {
                Max = (txtMaxSchedules.Text == "") ? 0 : txtMaxSchedules.Text.ToInt(0);
                if (ReleaseDate.Value.HasValue)
                {
                    SelectedDate = ReleaseDate.Value.Value.Date;
                }
                if (ReleaseTimeStart.Value.HasValue)
                {
                    StartTime = TimeOnly.FromDateTime(ReleaseTimeStart.Value.Value);
                }
                if (ReleaseTimeEnd.Value.HasValue)
                {
                    EndTime = TimeOnly.FromDateTime(ReleaseTimeEnd.Value.Value);
                }
                HasValues = (Max.HasValue && SelectedDate.HasValue && StartTime.HasValue && EndTime.HasValue);
            }            
            catch (Exception ex)
            {
                ex.LogWrite($"GetValues {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
                Width++;
                Height++;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && e.WidthChanged)
                {
                    grpScheduleDates.Width = e.NewSize.Width - 9;
                    cnvmain.Width = e.NewSize.Width - 9;
                }
                if (IsLoaded && e.HeightChanged)
                {
                    grpScheduleDates.Height = e.NewSize.Height - 10;
                    cnvmain.Height = e.NewSize.Height - 21;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
