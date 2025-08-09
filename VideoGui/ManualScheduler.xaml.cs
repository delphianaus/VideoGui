using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        public bool IsClosed = false, IsClosing = false, IsCopy = false, 
            HasValues = false, TestMode = false, RunSchedule = false;
        public Nullable<DateTime> SelectedDate = null;
        public Nullable<TimeOnly> StartTime = null, EndTime = null;
        public Nullable<int> Max = null;
        public bool IsMultiForm = false;
        public Action<object> ShowMultiForm = null;
        private void lblDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ReleaseDate.Value.HasValue)
                {
                    var cts_Date = ReleaseDate.Value.Value;
                    cts_Date = cts_Date.AddDays(1);
                    ReleaseDate.Value = cts_Date;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ManualScheduler.lblDate_MouseDoubleClick {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool IsValidDate = (ReleaseDate.Value.HasValue && ReleaseDate.Value.Value.Date >= DateTime.Now.Date );
                if (IsValidDate)
                {
                    DateOnly date =  DateOnly.FromDateTime(ReleaseDate.Value.Value.Date);
                    TimeOnly start = TimeOnly.FromDateTime(ReleaseTimeStart.Value.Value);
                    var NewDate = date.ToDateTime(start);   
                    if (NewDate < DateTime.Now)
                    {
                        IsValidDate = false;
                    }
                }
                RunSchedule = true && IsValidDate; 
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ManualScheduler.btnAccept_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public ManualScheduler(databasehook<Object> _ModuleCallBack, OnFinishIdObj DoOnFinish)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _ModuleCallBack;
                Closing += (s, e) => { GetValues(); IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(this, -1); };
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
                TestMode = chkSchedule.IsChecked.Value;
                if (HasValues)
                {
                    ModuleCallBack?.Invoke(this, new CustomParams_SaveSchedule(SelectedDate.Value,
                        ReleaseTimeStart.Value.Value.TimeOfDay, 
                        ReleaseTimeEnd.Value.Value.TimeOfDay, txtMaxSchedules.Text.ToInt(0), chkSchedule.IsChecked.Value)) ;
                }
            
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
