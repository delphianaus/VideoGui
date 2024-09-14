using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{
    public class FileCompares : INotifyPropertyChanged
    {
        private string _SourceFile;
        private string _SourceFilePath;
        private string _Extension;
        private int _SourceFileLength;
        private double _SourceFileSize;
        private bool _DestFileFound;
        private int _DestFileLength;
        private bool _TimesMatch;
        private DateTime _SouceFileDateTime;


        public DateTime SouceFileDateTime { get => _SouceFileDateTime; set { _SouceFileDateTime = value; OnPropertyChanged(); } }
        public string SourceFile { get => _SourceFile; set { _SourceFile = value; OnPropertyChanged(); } }
        public string SourceFilePath { get => _SourceFilePath; set { _SourceFilePath = value; OnPropertyChanged(); } }
        public int SourceFileLength { get => _SourceFileLength; set { _SourceFileLength = value; OnPropertyChanged(); } }
        public double SourceFileSize { get => _SourceFileSize; set { _SourceFileSize = value; OnPropertyChanged(); } }
        public int DestFileLength { get => _DestFileLength; set { _DestFileLength = value; OnPropertyChanged(); } }
        public bool DestFileFound { get => _DestFileFound; set { _DestFileFound = value; OnPropertyChanged(); } }
        public bool TimesMatch { get => _TimesMatch; set { _TimesMatch = value; OnPropertyChanged(); } }

        public string Extension { get => _Extension; set { _Extension = value; OnPropertyChanged(); } }
        public FileCompares(string _SourceFile, string _Extension, int totalseconds, double _sourceFileSize, DateTime _SourceDate)
        {
            Extension = _Extension;
            SourceFile = System.IO.Path.GetFileName(_SourceFile);
            SourceFileLength = totalseconds;
            SourceFileSize = _sourceFileSize;
            SouceFileDateTime = _SourceDate;
            DestFileLength = -1;
            DestFileFound = false;
            TimesMatch = false;
            SourceFilePath= System.IO.Path.GetDirectoryName(_SourceFile);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
