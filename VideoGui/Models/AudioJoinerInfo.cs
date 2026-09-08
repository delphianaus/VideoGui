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
        string _Status = "", _td = "";

        TimeSpan _TimeDataInternal = TimeSpan.Zero;

        public string FileName { get => _FileName; set { _FileName = value; OnPropertyChanged(); } }
        public string TimeData { get => DisplayTimeData(); set { _td = value; OnPropertyChanged(); } }

        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
        public TimeSpan TimeDataInternal { get => _TimeDataInternal; set { _TimeDataInternal = value; DisplayTimeData(); OnPropertyChanged(); } }

        public AudioJoinerInfo(string _FName, string _Status, TimeSpan Data)
        {
            FileName = _FName;
            Status = _Status;
            _TimeDataInternal = Data;
            _td = DisplayTimeData();
        }

        public string DisplayTimeData()
        {
            var r =  _TimeDataInternal.ToCustomTimeString();
            if (r.Length == 5) r = $"00:{r}";
            return r;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            //_td = DisplayTimeData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
