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
    public class DraftShorts : INotifyPropertyChanged
    {
        private int _id { get; set; } = -1;
        private string _VideoId { get; set; } = "";
        private string _FileName { get; set; } = "";
        public string VideoId { get { return _VideoId; } set { _VideoId = value; OnPropertyChanged(); } }
        public string FileName { get { return _FileName; } set { _FileName = value; OnPropertyChanged(); } }
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public DraftShorts(string vid, string fname)
        {
            VideoId = vid;
            FileName = fname;
        }
        public DraftShorts(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int id) ? id : -1;
                VideoId = (reader["VIDEOID"] is string vid) ? vid : "";
                FileName = (reader["FILENAME"] is string fname) ? fname : "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DraftShorts {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
