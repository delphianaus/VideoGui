using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    
    public class AudioJoinerInfo : INotifyPropertyChanged
    {
        string _FileName = "";
        string _Status= "";

        TimeSpan _TimeData = TimeSpan.Zero;

        public string FileName { get => _FileName; set { _FileName = value; OnPropertyChanged(); } }
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
        public TimeSpan TimeData { get => _TimeData; set { _TimeData = value; OnPropertyChanged(); } }

        public AudioJoinerInfo(string _FName, string _Status, TimeSpan Data)
        {
            FileName = _FName;
            Status = _Status;
            TimeData = Data;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
