using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class ProcessTargets : INotifyPropertyChanged
    {
        private int _LinkedId = -1, _TitleId = -1, _DescId = -1;
        private string _VideoId = "", _Title = "", _Description = "";

        public int LinkedId { get { return _LinkedId; } set { _LinkedId = value; OnPropertyChanged(); } }
        public int TitleId { get { return _TitleId; } set { _TitleId = value; OnPropertyChanged(); } }
        public int DescId { get { return _DescId; } set { _DescId = value; OnPropertyChanged(); } }
        public string VideoId { get { return _VideoId; } set { _VideoId = value; OnPropertyChanged(); } }
        public string Title { get { return _Title; } set { _Title = value; OnPropertyChanged(); } }
        public string Description { get { return _Description; } set { _Description = value; OnPropertyChanged(); } }

        public ProcessTargets() 
        {
            
        }

        public ProcessTargets(string _VideoId, int _Linkedid, string _Title, int _TitleId, 
            string _Description, int _DescId)
        {
            LinkedId = _Linkedid;
            VideoId = _VideoId;
            Title = _Title;
            TitleId = _TitleId;
            DescId = _DescId;
            Description = _Description;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
