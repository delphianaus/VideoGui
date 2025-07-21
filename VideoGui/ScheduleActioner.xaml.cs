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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static VideoGui.TitleSelectFrm;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ScheduleActioner.xaml
    /// </summary>
    public partial class ScheduleActioner : Window
    {
        databasehook<Object> ModuleCallBack = null;
        public bool IsClosed = false, IsClosing = false, IsCopy = false;
        Nullable<DateTime> actionDate = null, completeDate = null;
        Nullable<DateOnly> scheduleDate = null;
        Nullable<TimeSpan> scheduleTimeStart = null, scheduleTimeEnd = null;
        public int actionScheduleID = -1;

        public ScheduleActioner(OnFinishIdObj DoOnFinish, databasehook<Object> _ModuleCallBack)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _ModuleCallBack;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(this,-1); };
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
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize(actionScheduleID));
                Width = Width + 7;
                Height = Height + 20;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _selectReleaseSchedule = new SelectReleaseSchedule(selectReleaseSchedule_OnFinish, ModuleCallBack);
                Hide();
                _selectReleaseSchedule.ShowActivated = true;
                _selectReleaseSchedule.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelect_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void selectReleaseSchedule_OnFinish(object sender, int id)
        {
            try
            {
                Show();
                int LastId = -1;
                if (sender is SelectReleaseSchedule srs)
                {
                    txtSchName.Text = (srs.SelectedId != -1) ? srs.SelectedItem : txtSchName.Text;
                    srs = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SchedulingSelectEditor_OnFinish - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

       

        private void BtnSelectAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _frmActionScheduleSelector = new ActionScheduleSelector(ActionScheduleSelector_OnFinish, ModuleCallBack);
                Hide();
                _frmActionScheduleSelector.ShowActivated = true;
                _frmActionScheduleSelector.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnSelectAction_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void ProcessLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnSaveAction.IsEnabled = txtActionName.Text != "" && txtSchName.Text != ""
                    && ReleaseDate.Value.HasValue && ReleaseTimeStart.Value.HasValue &&
                       ReleaseTimeEnd.Value.HasValue && AppliedDate.Value.HasValue &&
                       AppliedTime.Value.HasValue && txtMaxSchedules.Text.ToInt(0) > 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessLostFocus - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void txtActionName_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtActionName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtActionName.Text != "")
                {
                    ProcessLostFocus(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtActionName_KeyUp - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (e.WidthChanged)
                    {
                        grpScheduleDates.Width = e.NewSize.Width - 29;
                        grpScheduleDates2.Width = grpScheduleDates.Width;
                        grpActionName.Width = grpScheduleDates.Width;
                        grpScheduleName.Width = e.NewSize.Width - 146;// 388 - 256 = 132
                        Canvas.SetLeft(grpMaxSchedules, e.NewSize.Width - 145);  // 388 - 261 = 131
                        Canvas.SetLeft(btnSelect, e.NewSize.Width - 189); // 388 - 211 = 
                        Canvas.SetLeft(BtnSelectAction, e.NewSize.Width - 71); // 388 - 211 = 
                        Canvas.SetLeft(BtnSaveAction, e.NewSize.Width - 71 - 36); // 388 - 211 = 
                        txtActionName.Width = e.NewSize.Width - 120; // 388 - 201 = 187
                        txtSchName.Width = e.NewSize.Width - 200; // 388 - 201 = 187
                        var newcenter = (e.NewSize.Width / 2);
                        Canvas.SetLeft(ReleaseTimeStart, newcenter + 20);
                        Canvas.SetLeft(ReleaseTimeEnd, newcenter + 20);
                        Canvas.SetLeft(lblTime, newcenter - 14);
                        Canvas.SetLeft(lblTime_Copy, newcenter - 14);
                        Canvas.SetLeft(ReleaseDate, newcenter - 166);
                        Canvas.SetLeft(lblDate, newcenter - 204);
                        Canvas.SetLeft(AppliedTime, newcenter + 20);
                        Canvas.SetLeft(lblTimeApp, newcenter - 14);
                        Canvas.SetLeft(AppliedDate, newcenter - 166);
                        Canvas.SetLeft(lblDateApp, newcenter - 204);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void ActionScheduleSelector_OnFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is ActionScheduleSelector ast)
                {
                    txtActionName.Text = (ast.PersistId != -1) ? ast.Title : "";
                    if (ast.IsClosing)
                    {
                       Thread.Sleep(100);
                       System.Windows.Forms.Application.DoEvents();
                    }
                    ast = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ActionScheduleSelector_OnFinish - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void BtnSaveAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                actionDate = null;
                if (AppliedDate.Value.HasValue && AppliedTime.Value.HasValue)
                {
                    actionDate = new DateTime(AppliedDate.Value.Value.Year, AppliedDate.Value.Value.Month,
                        AppliedDate.Value.Value.Day, AppliedTime.Value.Value.Hour, AppliedTime.Value.Value.Minute, AppliedTime.Value.Value.Second);
                }
                scheduleDate = null;
                if (ReleaseDate.Value.HasValue)
                {
                    scheduleDate = new DateOnly(ReleaseDate.Value.Value.Year, ReleaseDate.Value.Value.Month, ReleaseDate.Value.Value.Day);
                }
                completeDate = null;

                if (ReleaseTimeStart.Value.HasValue)
                {
                    scheduleTimeStart = new TimeSpan(ReleaseTimeStart.Value.Value.Hour, ReleaseTimeStart.Value.Value.Minute, ReleaseTimeStart.Value.Value.Second);
                }
                if (ReleaseTimeEnd.Value.HasValue)
                {
                    scheduleTimeEnd = new TimeSpan(ReleaseTimeEnd.Value.Value.Hour, ReleaseTimeEnd.Value.Value.Minute, ReleaseTimeEnd.Value.Value.Second);
                }
                var p = txtMaxSchedules.Text.ToInt(0);


                ModuleCallBack?.Invoke(this, new
                    CustomParams_UpdateAction(actionScheduleID, actionDate, scheduleDate,
                    scheduleTimeStart, scheduleTimeEnd, completeDate,
                    txtSchName.Text, txtActionName.Text, txtMaxSchedules.Text.ToInt(0)));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnSaveAction_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
