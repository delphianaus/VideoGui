using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{

    public class PlanningCuts : INotifyPropertyChanged
    {
        private string _FileName = "";//, _SourceDir = "";
        private int _Id = -1, _PlanningQueId = -1;
        private TimeSpan _Start = TimeSpan.Zero, _Stop = TimeSpan.Zero;
        public string FileName { get => _FileName; set { _FileName = value; OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public int PlanningQueId { get => _PlanningQueId; set { _PlanningQueId = value; OnPropertyChanged(); } }

        public TimeSpan Start { get => _Start; set { _Start = value; OnPropertyChanged(); } }
        public TimeSpan Stop { get => _Stop; set { _Stop = value; OnPropertyChanged(); } }

        public PlanningCuts(FbDataReader reader)
        {
            Id = reader["ID"].ToInt();
            PlanningQueId = reader["PLANINGQUEID"].ToInt();
            FileName = reader["FILENAME"].ToString();
            Start = TimeSpan.FromMilliseconds(reader["START"].ToInt(0));
            Stop = TimeSpan.FromMilliseconds(reader["STOP"].ToInt(0));

        }
        public PlanningCuts(int _PlanningQueId, string _Filename, TimeSpan _Start, TimeSpan _Stop)
        {
            PlanningQueId = _PlanningQueId;
            FileName = _Filename;
            Start = _Start;
            Stop = _Stop;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

