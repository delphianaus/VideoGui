using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoGui.Models.delegates;

namespace VideoGui.Models
{

    class ScheduledActions : INotifyPropertyChanged
    {
        private int _Id = -1, _ScheduleNameId = -1, _Max = -1;
        private ActionType _VideoActionType = ActionType.VideoUpload;
        private Nullable<DateOnly> _ActionSchedule = null;
        private Nullable<TimeSpan> _ActionScheduleStart = null, _ActionScheduleEnd = null;
        private Nullable<DateTime> _AppliedAction = null, _CompletedScheduledDate = null;
        private string _ScheduleName = "", _ActionName = "", _AppliedDateString, _ScheduleDateString, _CompletedDateString;
        private bool _IsActioned = false;
        public string ActionName { get => _ActionName; set { _ActionName = value; OnPropertyChanged(); } }
        public bool IsActioned { get => _IsActioned; set { _IsActioned = value; OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public int Max { get => _Max; set { _Max = value; OnPropertyChanged(); } }
        public string ScheduleName { get => _ScheduleName; set { _ScheduleName = value; OnPropertyChanged(); } }
        public int ScheduleNameId { get => _ScheduleNameId; set { _ScheduleNameId = value; OnPropertyChanged(); } }
        public ActionType VideoActionType { get => _VideoActionType; set { _VideoActionType = value; OnPropertyChanged(); } }
        public Nullable<DateOnly> ActionSchedule { get => _ActionSchedule; set { _ActionSchedule = value; OnPropertyChanged(); } }
        public Nullable<TimeSpan> ActionScheduleStart { get => _ActionScheduleStart; set { _ActionScheduleStart = value; OnPropertyChanged(); } }
        public Nullable<TimeSpan> ActionScheduleEnd { get => _ActionScheduleEnd; set { _ActionScheduleEnd = value; OnPropertyChanged(); } }
        public string AppliedDateString { get => AppliedAction?.ToString("dd/MM/yyyy hh:mm:ss tt"); set { _AppliedDateString = value; OnPropertyChanged(); } }
        public string ScheduleDateString { get => GetScheduleDate(); set { _ScheduleDateString = value; OnPropertyChanged(); } }

        private string GetScheduleDate()
        {
            try
            {
                string data = "";
                if (ActionSchedule.HasValue && ActionScheduleStart.HasValue && ActionScheduleEnd.HasValue)
                {
                    data = $"{ActionSchedule.Value.ToString("dd/MM/yyyy")}";
                    //{ActionScheduleStart.Value.ToString("HH:mm")} - {ActionScheduleEnd.Value.ToString("HH:mm")}";
                    var start = ActionScheduleStart.Value;
                    var end = ActionScheduleEnd.Value;
                    if (start.Minutes == 00)
                    {
                        data += (start.Hours > 12) ? $" {start.Hours-12}PM-" : $" {start.Hours}AM-";
                    }
                    else
                    {
                        data += $" {start.ToString("HH:mm tt")}-";
                    }

                    if (end.Minutes == 00)
                    {
                        data += (end.Hours > 12) ? $" {end.Hours - 12}PM" : $" {end.Hours}AM";
                    }
                    else
                    {
                        data += $"{end.ToString("HH:mm tt")}";
                    }
                    return data;
                }

                return "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetScheduleDate Constructor {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "INVALID";
            }
        }

        public string CompletedDateString { get => CompletedScheduledDate?.ToString("dd-MM HH:mm:ss tt"); set { _CompletedDateString = value; OnPropertyChanged(); } }
        public Nullable<DateTime> AppliedAction { get => _AppliedAction; set { _AppliedAction = value; OnPropertyChanged(); } }
        public Nullable<DateTime> CompletedScheduledDate { get => _CompletedScheduledDate; set { _CompletedScheduledDate = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public ScheduledActions() 
        { 
        }
        public ScheduledActions(int _Id, int _ScheduleId, string _ScheduleName, string _ActionName, int Max,
            ActionType _ActionType, Nullable<DateOnly> _AppliedSchedule,
            Nullable<TimeSpan> _AppliedScheduleStart, Nullable<TimeSpan> _AppliedScheduleEnd,
            Nullable<DateTime> _AppliedAction, Nullable<DateTime> _CompletedScheduledDate, bool _IsActioned = false)
        {
            try
            {
                Id = _Id;
                ScheduleNameId = _ScheduleId;
                ScheduleName = _ScheduleName;
                ActionName = _ActionName;
                Max = Max;
                VideoActionType = _ActionType;
                ActionSchedule = _AppliedSchedule;
                ActionScheduleStart = _AppliedScheduleStart;
                ActionScheduleEnd = _AppliedScheduleEnd;
                AppliedAction = _AppliedAction;
                CompletedScheduledDate = _CompletedScheduledDate;
                IsActioned = _IsActioned;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduledActions {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public ScheduledActions(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int Idx) ? Idx : -1;
                ScheduleNameId = (reader["SCHEDULENAMEID"] is int SCHNAMEID) ? SCHNAMEID : -1;
                ScheduleName = (reader["SCHEDULENAME"] is string SNAME) ? SNAME : "";
                ActionName = (reader["ACTIONNAME"] is string ANAME) ? ANAME : "";
                Max = (reader["MAXSCHEDULES"] is int MX) ? MX : -1;
                VideoActionType = (reader["VIDEOTYPE"] is int VT) ? (ActionType)VT : ActionType.VideoUpload;
                if (reader["scheduled_date"] is DateTime scheduledDatex)
                { 
                   DateOnly r = new DateOnly(scheduledDatex.Date.Date.Year, scheduledDatex.Date.Date.Month, scheduledDatex.Date.Date.Day);
                   ActionSchedule = r;
                }

                //ActionSchedule = (reader["SCHEDULED_DATE"] is DateTime scheduledDateStart) ? scheduledDateStart.Date : new DateOnly();
                ActionScheduleEnd = (reader["SCHEDULED_TIME_END"] is TimeSpan scheduledTimeEnd) ? scheduledTimeEnd : new TimeSpan();
                ActionScheduleStart = (reader["SCHEDULED_TIME_START"] is TimeSpan scheduledTimeStart) ? scheduledTimeStart : new TimeSpan();
                if (reader["ACTION_DATE"] is DateTime actiondDate && reader["ACTION_TIME"] is TimeSpan actionTime)
                {
                    var z = new TimeSpan(actionTime.Hours, actionTime.Minutes, actionTime.Seconds);
                    AppliedAction = actiondDate.AtTime(new TimeOnly(z.Hours, z.Minutes, z.Seconds));
                }
                if (reader["COMPLETED_DATE"] is DateTime completedDate && reader["COMPLETED_TIME"] is TimeSpan completedTime)
                {
                    var z1 = new TimeSpan(completedTime.Hours, completedTime.Minutes, completedTime.Seconds);
                    CompletedScheduledDate = completedDate.AtTime(new TimeOnly(z1.Hours, z1.Minutes, z1.Seconds));
                }
                IsActioned = (reader["ISACTIONED"] is int IsAct) ? IsAct == 1 : false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduledActions fbReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
