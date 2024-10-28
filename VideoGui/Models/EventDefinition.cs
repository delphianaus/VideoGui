using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class EventDefinition : INotifyPropertyChanged
    {
        private int _Id = -1, _DayOfWeek = 0, _Max = 0, _Type = -1, _Retries = 0;
        private TimeSpan _EventStart = TimeSpan.Zero, _EventEnd = TimeSpan.Zero;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public int DaysOfWeek { get => _DayOfWeek; set { _DayOfWeek = value; OnPropertyChanged(); } }
        public int Max { get => _Max; set { _Max = value; OnPropertyChanged(); } }
        public int Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
        public int Retries { get => _Retries; set { _Retries = value; OnPropertyChanged(); } }
        public TimeSpan EventStart { get => _EventStart; set { _EventStart = value; OnPropertyChanged(); } }
        public TimeSpan EventEnd { get => _EventEnd; set { _EventEnd = value; OnPropertyChanged(); } }

        public EventDefinition(int _Id, int _DayOfWeek, int _Max, int _Type, int _Retries, TimeSpan _EventStart, TimeSpan _EventEnd)
        {
            try
            {
                Id = _Id;
                DaysOfWeek = _DayOfWeek;
                Max = _Max;
                Type = _Type;
                Retries = _Retries;
                EventStart = _EventStart;
                EventEnd = _EventEnd;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"EventDefinition {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        public EventDefinition(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int idx) ? idx : -1;
                DaysOfWeek = (reader["DAYSOFWEEK"] is int dow) ? dow : 0;
                Max = (reader["MAX"] is int max) ? max : 0;
                Type = (reader["TYPE"] is int type) ? type : -1;
                Retries = (reader["RETRIES"] is int ret) ? ret : 0;
                EventStart = (reader["EVENTSTART"] is TimeSpan ts1) ? ts1 : TimeSpan.Zero;
                EventEnd = (reader["EVENTEND"] is TimeSpan ts2) ? ts2 : TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"EventDefinition {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}