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
    public class AppliedSchedule : INotifyPropertyChanged
    {
        private int _id = -1, _ScheduleId = -1;
        private Int16 _StartHour = 0, _EndHour = 0, _Gap = 0;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int ScheduleId { get => _ScheduleId; set { _ScheduleId = value; OnPropertyChanged(); } }
        public Int16 StartHour { get => _StartHour; set { _StartHour = value; OnPropertyChanged(); } }
        public Int16 EndHour { get => _EndHour; set { _EndHour = value; OnPropertyChanged(); } }
        public Int16 Gap { get => _Gap; set { _Gap = value; OnPropertyChanged(); } }
        public AppliedSchedule(int ID, int ScheduleId, Int16 StartHour, Int16 EndHour, Int16 Gap)
        {
            try
            {
                this.Id = ID;
                this.ScheduleId = ScheduleId;
                this.StartHour = StartHour;
                this.EndHour = EndHour;
                this.Gap = Gap;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AppliedSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public AppliedSchedule(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                ScheduleId = (reader["SCHEDULEID"] is int SCHEDULEID) ? SCHEDULEID : -1;
                StartHour = (reader["STARTHOUR"] is Int16 STARTHOUR) ? STARTHOUR : (Int16)0;
                EndHour = (reader["ENDHOUR"] is Int16 ENDHOUR) ? ENDHOUR : (Int16)0;
                Gap = (reader["GAP"] is Int16 GAP) ? GAP : (Int16)0;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AppliedSchedule reader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
