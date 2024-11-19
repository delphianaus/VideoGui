using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class EditableScheduleEvent : INotifyPropertyChanged
    {
        public EditableScheduleEvent(FbDataReader reader)
        {
            try
            {
                Name = (reader["SP.NAME"] is string _nn ? _nn : "");
                EventStartDate = (reader["ESD.START"] is DateOnly ESD ? ESD : new DateOnly());
                EventStartTime = (reader["ESD.STARTTIME"] is TimeOnly TSD ? TSD : new TimeOnly());
                EventEndDate = (reader["ESD.END"] is DateOnly EED ? EED : new DateOnly());
                EventEndTime = (reader["ESD.ENDTIME"] is TimeOnly EET ? EET : new TimeOnly());
                ScheduleStartDate = (reader["SD.START"] is DateOnly SSD ? SSD : new DateOnly());
                ScheduleStartTime = (reader["SD.STARTTIME"] is TimeOnly sst ? sst : new TimeOnly());
                ScheduleEndDate = (reader["SD.END"] is DateOnly SSE ? SSE : new DateOnly());
                ScheduleEndTime = (reader["SD.ENDTIME"] is TimeOnly sse ? sse : new TimeOnly());
                Source = (reader["SP.SOURCE"] is Int16 src ? src  : -1);
                MaxDaily = (reader["SP.MAXDAILY"] is Int16 MD ? MD : -1);
                MaxEvent = (reader["SP.MAXEVENT"] is Int16 ME ? ME : -1);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"EditableScheduleEvent fb constructor {MethodBase.GetCurrentMethod()?.Name} {this} {ex.Message}");
            }
        }
        private string _Name = string.Empty;
        private int _Source = -1, _MaxDaily = -1, _MaxEvent = -1;
        private DateOnly EventStartDate, EventEndDate, ScheduleStartDate, ScheduleEndDate;
        private TimeOnly EventStartTime, EventEndTime, ScheduleStartTime, ScheduleEndTime;
        public DateTime EventStart { get => EventStartDate.ToDateTime(EventStartTime); set { (EventStartDate,EventStartTime) = ConvertDT(value); OnPropertyChanged(); } }
        public DateTime EventEnd { get => EventEndDate.ToDateTime(EventEndTime); set { (EventEndDate, EventEndTime) = ConvertDT(value); OnPropertyChanged(); } }
        public DateTime ScheduleStart { get => ScheduleStartDate.ToDateTime(ScheduleStartTime); set { (ScheduleStartDate, ScheduleStartTime) = ConvertDT(value); OnPropertyChanged(); } }
        public DateTime ScheduleEnd { get => ScheduleEndDate.ToDateTime(ScheduleEndTime); set { (ScheduleEndDate, ScheduleEndTime) = ConvertDT(value); OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        public int Source { get => _Source; set { _Source = value; OnPropertyChanged(); } }
        public int MaxEvent { get => _MaxEvent; set { _MaxEvent = value; OnPropertyChanged(); } }
        public int MaxDaily { get => _MaxDaily; set { _MaxDaily = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public (DateOnly, TimeOnly) ConvertDT(DateTime dts)
        {
            try
            {
                return (DateOnly.FromDateTime(dts), TimeOnly.FromDateTime(dts));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConvertDT {MethodBase.GetCurrentMethod()?.Name} {this} {ex.Message}");
                return (new DateOnly(),new TimeOnly());
            }
        }
    }
}
