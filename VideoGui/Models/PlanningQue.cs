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

    public class PlanningQue : INotifyPropertyChanged
    {
        private string _Source = "", _SourceDir = "";
        private int _Id = -1;
        public string Source { get => _Source; set { _Source = value; OnPropertyChanged(); } }
        public string SourceDir { get => _SourceDir; set { _SourceDir = value; OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public PlanningQue(FbDataReader reader)
        {
            Id = reader["ID"].ToInt();
            Source = reader["SOURCE"].ToString();
            SourceDir = reader["SOURCEDIR"].ToString();
        }
        public PlanningQue(string _Source, string _SourceDir)
        {
            Source = _Source;
            SourceDir = _SourceDir;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
