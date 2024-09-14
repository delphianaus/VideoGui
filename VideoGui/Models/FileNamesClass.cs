using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{
    public class FileNamesClass : INotifyPropertyChanged
    {
        public FileNamesClass() // used for JSON IMPORT ONLY
        {

        }

        public FileNamesClass(string _title, string _correntName, string _InitPath)
        {
            Title = System.IO.Path.GetFileName(_title);
            CorrentName = _correntName;
            IdentifiedAs = _IdentifiedAs;
            InitialPath = _InitPath;
            _ComboItems = new();
            _SimpleStringProperty = "";
            _IsEnabled = false;
            _IsCorrect = false ;
            _IdentifiedAs = false;
        }

        private string _title, _correntName , _InitialPath, _SimpleStringProperty;
        private List<string> _ComboItems;
        private bool _IsEnabled, _IsCorrect, _IdentifiedAs;


        public bool IsCorrect { get => _IsCorrect; set { _IsCorrect = value; OnPropertyChanged(); } }
        public bool IsEnabled { get => _IsEnabled; set { _IsEnabled = value; OnPropertyChanged(); } }
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
        public string SimpleStringProperty { get => _SimpleStringProperty; set { _SimpleStringProperty = value; OnPropertyChanged(); } }
        public string CorrentName { get => _correntName; set { _correntName = value; OnPropertyChanged(); } }
        public bool IdentifiedAs { get => _IdentifiedAs; set { _IdentifiedAs = value; OnPropertyChanged(); } }
        public string InitialPath { get => _InitialPath; set { _InitialPath = value; OnPropertyChanged(); } }
        
        public List<string> ComboItems { get => _ComboItems; set { _ComboItems = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
