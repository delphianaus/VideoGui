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
        private Nullable<TimeOnly> _ActionScheduleStart = null, _ActionScheduleEnd = null;
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
        public Nullable<TimeOnly> ActionScheduleStart { get => _ActionScheduleStart; set { _ActionScheduleStart = value; OnPropertyChanged(); } }
        public Nullable<TimeOnly> ActionScheduleEnd { get => _ActionScheduleEnd; set { _ActionScheduleEnd = value; OnPropertyChanged(); }        }
        public string AppliedDateString { get => AppliedAction?.ToString("dd-MM HH:mm:ss tt"); set { _AppliedDateString = value; OnPropertyChanged(); } }
        public string ScheduleDateString { get => ActionSchedule?.ToString("dd-MM HH:mm:ss tt"); set { _ScheduleDateString = value; OnPropertyChanged(); } }
        public string CompletedDateString { get => CompletedScheduledDate?.ToString("dd-MM HH:mm:ss tt"); set { _CompletedDateString = value; OnPropertyChanged(); } }
        public Nullable<DateTime> AppliedAction { get => _AppliedAction; set { _AppliedAction = value; OnPropertyChanged(); } }
        public Nullable<DateTime> CompletedScheduledDate { get => _CompletedScheduledDate; set { _CompletedScheduledDate = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ScheduledActions(int _Id, int _ScheduleId, string _ScheduleName, string _ActionName, int Max,
            ActionType _ActionType, Nullable<DateOnly> _AppliedSchedule, 
            Nullable<TimeOnly> _AppliedScheduleStart, Nullable<TimeOnly> _AppliedScheduleEnd,
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
                Max = (reader["MAX"] is int MX) ? MX : -1;
                VideoActionType = (reader["VIDEOTYPE"] is int VT) ? (ActionType)VT : ActionType.VideoUpload;
                ActionSchedule = (reader["SCHEDULED_DATE"] is DateOnly scheduledDateStart) ? scheduledDateStart : new DateOnly();
                ActionScheduleEnd = (reader["SCHEDULED_TIME_END"] is TimeOnly scheduledTimeEnd) ? scheduledTimeEnd : new TimeOnly();
                ActionScheduleStart = (reader["SCHEDULED_TIME_START"] is TimeOnly scheduledTimeStart) ? scheduledTimeStart : new TimeOnly();
                if (reader["ACTION_DATE"] is DateOnly actiondDate && reader["ACTION_TIME"] is TimeOnly actionTime)
                {
                    AppliedAction = actiondDate.ToDateTime(actionTime);
                }
                if (reader["COMPLETED_DATE"] is DateOnly completedDate && reader["COMPLETED_TIME"] is TimeOnly completedTime)
                {
                    CompletedScheduledDate = completedDate.ToDateTime(completedTime);
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
