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

    public class VideoSchedule : INotifyPropertyChanged
    {
        private int _id = -1, _SCHEDULEID = -1, _Day = -1;
        private TimeOnly _Time = TimeOnly.FromTimeSpan(TimeSpan.Zero);
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int ScheduleId { get => _SCHEDULEID; set { _SCHEDULEID = value; OnPropertyChanged(); } }
        public int Days { get => _Day; set { _Day = value; OnPropertyChanged(); } }

        public TimeOnly Time { get => _Time; set { _Time = value; OnPropertyChanged(); } }
        public VideoSchedule(int ID, int SCHEDULEID, TimeOnly TIME, int DAY)
        {
            try
            {
                this.Id = ID;
                this.Days = DAY;
                this.ScheduleId = SCHEDULEID;
                this.Time = TIME;
            }
            catch(Exception ex)
            {
                ex.LogWrite($"VideoShedule {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        public VideoSchedule(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                Days = (reader["DAY"] is int DAY) ? DAY : -1;
                ScheduleId = (reader["SCHEDULEID"] is int SCHEDULEID) ? SCHEDULEID : -1;
                Time = (reader["SCHEDULETIME"] is TimeOnly TIME) ? TIME : TimeOnly.FromTimeSpan(TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"VideoShedule {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
