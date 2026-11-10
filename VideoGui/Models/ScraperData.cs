using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Graphics.Printing.Workflow;

namespace VideoGui.Models
{
    public class ScraperData : INotifyPropertyChanged
    {

        private string _videoId = "";
        private string _filename = "";
        private int _ScheduleNumber = 0;
        private Nullable<DateTime> _ScheduledDate = null;

        public string VideoId { get => _videoId; set { _videoId = value; OnPropertyChanged(); } }
        public string Filename { get => _filename; set { _filename = value; OnPropertyChanged(); } }
        public Nullable<DateTime> ScheduledDate { get => _ScheduledDate; set { _ScheduledDate = value; OnPropertyChanged(); } }
        public int ScheduleNumber { get => _ScheduleNumber; set { _ScheduleNumber = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
