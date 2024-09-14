using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class Uploads : INotifyPropertyChanged
    {
        private bool _IsClicked = false;
        private string _filename = "", _status = "";
        public string FileName { get => _filename; set { _filename = value; OnPropertyChanged(); } }
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }
        public bool IsClicked { get => _IsClicked; set { _IsClicked = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsCounted()
        {
            return (!Status.ToLower().Contains("waiting") && !Status.ToLower().Contains("daily"));
        }

        public Uploads(string _filename, string _status)
        {
            FileName = _filename;
            Status = _status;
        }
    }
}
