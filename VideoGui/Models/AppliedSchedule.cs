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
        private Int16 _StartHour = 0, _EndHour = 0, _Gap = 0, _Day = -1;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int ScheduleId { get => _ScheduleId; set { _ScheduleId = value; OnPropertyChanged(); } }
        public Int16 StartHour { get => _StartHour; set { _StartHour = value; OnPropertyChanged(); } }
        public Int16 EndHour { get => _EndHour; set { _EndHour = value; OnPropertyChanged(); } }
        public Int16 Gap { get => _Gap; set { _Gap = value; OnPropertyChanged(); } }
        public Int16 Days { get => _Day; set { _Day = value; OnPropertyChanged(); } }
        public AppliedSchedule(int ID, int ScheduleId, Int16 StartHour, Int16 EndHour, Int16 Gap, Int16 days)
        {
            try
            {
                this.Id = ID;
                this.ScheduleId = ScheduleId;
                this.StartHour = StartHour;
                this.EndHour = EndHour;
                this.Gap = Gap;
                this.Days = days;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AppliedSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }

            Days = days;
        }

        public bool IsSunday()
        {
            try
            {
                return Days == 1;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsSunday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsMonday()
        {
            try
            {
                return Days == 2;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsMonday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsTuesday()
        {
            try
            {
                return Days == 4;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsTuesday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsWednesday()
        {
            try
            {
                return Days == 8;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsWednesday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsThursday()
        {
            try
            {
                return Days == 16;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsThursday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsFriday()
        {
            try
            {
                return Days == 32;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsFriday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsSaturday()
        {
            try
            {
                return Days == 64;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsSaturday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
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
                Days = (reader["DAYS"] is Int16 DAYS) ? DAYS : (Int16)0;
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
