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


    public class ScheduleMapItem : INotifyPropertyChanged
    {
        private int _Id, _Gap = -1, _ScheduleId = -1;
        private TimeSpan _Start = TimeSpan.Zero, _End = TimeSpan.Zero;
        public int ScheduleId { get => _ScheduleId; set { _ScheduleId = value; OnPropertyChanged(); } }
        public TimeSpan Start { get => _Start; set { _Start = value; OnPropertyChanged(); } }
        public TimeSpan End { get => _End; set { _End = value; OnPropertyChanged(); } }
        public int Gap { get => _Gap; set { _Gap = value; OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public string StartDate => Start.ToFFmpeg().Replace(".000", "");
        public string EndDate => End.ToFFmpeg().Replace(".000", "");

        public ScheduleMapItem(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                Start = (reader["SSTART"] is TimeSpan START) ? START : TimeSpan.Zero;
                End = (reader["SEND"] is TimeSpan END) ? END : TimeSpan.Zero;
                Gap = (reader["GAP"] is int GAP) ? GAP : -1;
                ScheduleId = (reader["SCHEDULINGID"] is int SCHEDULEID) ? SCHEDULEID : -1;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleMapItem {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public ScheduleMapItem(TimeSpan _Start, TimeSpan _End, int _Gap, int _ScheduleId)
        {
            try
            {
                Start = _Start;
                End = _End;
                Gap = _Gap;
                ScheduleId = _ScheduleId;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleMapItem {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }


}
