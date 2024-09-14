using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{

   
    public class FileCache : INotifyPropertyChanged
    {
        private string _SourceFile;
        private string _SourceFilePath;
        private int _SourceFileLength;
        private DateTime _SouceFileDateTime;
        private double _SourceFileSize;
    

       
        public string SourceFile { get => _SourceFile; set { _SourceFile = value; OnPropertyChanged(); } }
        public string SourceFilePath { get => _SourceFilePath; set { _SourceFilePath = value; OnPropertyChanged(); } }
        public int SourceFileLength { get => _SourceFileLength; set { _SourceFileLength = value; OnPropertyChanged(); } }
        public DateTime SouceFileDateTime { get => _SouceFileDateTime; set { _SouceFileDateTime = value; OnPropertyChanged(); } }
        public double SourceFileSize { get => _SourceFileSize; set { _SourceFileSize = value; OnPropertyChanged(); } }

        public FileCache()
        {

        }
        public FileCache(string _SourceFile, int _SourceFileLength, DateTime _SouceFileDateTime, double _SourceFileSize)
        {
            SourceFile = System.IO.Path.GetFileName(_SourceFile);
            SourceFileLength = _SourceFileLength;
            SouceFileDateTime = _SouceFileDateTime;
            SourceFileSize = _SourceFileSize;
            SourceFilePath = System.IO.Path.GetDirectoryName(_SourceFile);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
