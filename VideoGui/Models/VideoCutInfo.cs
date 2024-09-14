using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class VideoCutInfo : INotifyPropertyChanged
    {
        public string _FileName = "";
        public TimeSpan _TimeFrom = TimeSpan.Zero, _TimeTo = TimeSpan.Zero;
        public int _CutNo = 0;

        public string FileName { get => _FileName; set { _FileName = value; OnPropertyChanged(); } }
        public TimeSpan TimeFrom { get => _TimeFrom; set { _TimeFrom = value; OnPropertyChanged(); } }
        public TimeSpan TimeTo { get => _TimeTo; set { _TimeTo = value; OnPropertyChanged(); } }
        public int CutTo { get => _CutNo; set { _CutNo = value; OnPropertyChanged(); } }


        public VideoCutInfo(string __FileName,TimeSpan _FromTime, TimeSpan To_Time, int _Cutnum)
        {
            FileName = __FileName;
            _FromTime = _FromTime;
            TimeTo = To_Time;
            TimeFrom = _FromTime;   
            CutTo = _Cutnum;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
