using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class FileInfoGoPro
    {
        string _FileName = "";
        string _NewFile = "";

        TimeSpan _TimeData = TimeSpan.Zero;

        public string FileName { get => _FileName; set { _FileName = value; OnPropertyChanged(); } }
        public string NewFile { get => _NewFile; set { _NewFile = value; OnPropertyChanged(); } }
        public TimeSpan TimeData { get => _TimeData; set { _TimeData = value; OnPropertyChanged(); } }

        public FileInfoGoPro(string _FName, string _NewName, TimeSpan Data)
        {
            FileName = _FName;
            NewFile = _NewName;
            TimeData = Data;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
