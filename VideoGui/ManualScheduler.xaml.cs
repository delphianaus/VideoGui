using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoGui.Models;
using VideoGui.Models.delegates;
using Wpf.Ui.Controls;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ManualScheduler.xaml
    /// </summary>
    public partial class ManualScheduler : FluentWindow
    {
        databasehook<Object> Invoker = null;
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

        private void PART_Popup_Opened(object sender, EventArgs e)
        {
            var popup = sender as Popup;

            // safer place to edit calendar visuals
        }

        private void ReleaseDate_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Xceed.Wpf.Toolkit.DateTimePicker DTYP && DTYP is not null)
            {


                var calItem = DTYP?.Template.FindName("PART_CalendarItem", DTYP) as CalendarItem;

                if (calItem != null)
                {
                    var header = calItem.Template.FindName("PART_HeaderButton", calItem) as System.Windows.Controls.Button;

                    // 5. Set the FontSize
                    if (header != null)
                    {
                        header.FontSize = 16; // Set desired font size here
                        header.Height = 30;   // Optional: Adjust height for readability
                        header.Width = 120;   // Optional: Adjust width for readability
                    }
                }

            }
        }

        private void ReleaseDate_Initialized(object sender, EventArgs e)
        {
            ;
        }

        private void ReleaseDate_GotFocus_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool IsValidDate = (ReleaseDate.Value.HasValue && ReleaseDate.Value.Value.Date >= DateTime.Now.Date);
                if (IsValidDate)
                {
                    DateOnly date = DateOnly.FromDateTime(ReleaseDate.Value.Value.Date);
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

        public ManualScheduler(databasehook<Object> _Invoker, OnFinishIdObj DoOnFinish)
        {
            try
            {
                InitializeComponent();
                Invoker = _Invoker;
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
                    Invoker?.Invoke(this, new CustomParams_SaveSchedule(SelectedDate.Value,
                        ReleaseTimeStart.Value.Value.TimeOfDay,
                        ReleaseTimeEnd.Value.Value.TimeOfDay, txtMaxSchedules.Text.ToInt(0), chkSchedule.IsChecked.Value));
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
                Invoker?.Invoke(this, new CustomParams_Initialize());
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
                    grpScheduleDates.Height = e.NewSize.Height - 90;
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
