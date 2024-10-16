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
        private int _id = -1, _SCHEDULEID = -1;
        private TimeOnly _Time = TimeOnly.FromTimeSpan(TimeSpan.Zero);
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int ScheduleId { get => _SCHEDULEID; set { _SCHEDULEID = value; OnPropertyChanged(); } }
        public TimeOnly Time { get => _Time; set { _Time = value; OnPropertyChanged(); } }
        public VideoSchedule(int ID, int SCHEDULEID, TimeOnly TIME)
        {
            try
            {
                this.Id = ID;
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
                ScheduleId = (reader["SCHEDULEID"] is int SCHEDULEID) ? SCHEDULEID : -1;
                Time = (reader["SCHEDULETIME"] is TimeOnly TIME) ? TIME : TimeOnly.FromTimeSpan(TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"VideoShedule {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
