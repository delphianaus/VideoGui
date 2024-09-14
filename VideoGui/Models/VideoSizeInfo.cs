using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{
    public class VideoSizeInfo : INotifyPropertyChanged
    {
        private string _SourceFile;
        private string _SourceFilePath;
        private bool _FileExistsInFinishedDir;
        private string _CompletedDir;
        public int _SourceFileRes;
        private bool _ExcludeFile;

        public VideoSizeInfo(string _SourceFile, string _CompletedDir, int _SourceFileRes)
        {
            SourceFile = System.IO.Path.GetFileName(_SourceFile);
            SourceFilePath = System.IO.Path.GetDirectoryName(_SourceFile);
            FileExistsInFinishedDir = false;
            CompletedDir = _CompletedDir;
            SourceFileRes = _SourceFileRes;
            ExcludeFile = false;
        }

        public VideoSizeInfo()
        {

        }
        public string SourceFile { get => _SourceFile; set { _SourceFile = value; OnPropertyChanged(); } }
        public string SourceFilePath { get => _SourceFilePath; set { _SourceFilePath = value; OnPropertyChanged(); } }
        public string CompletedDir { get => _CompletedDir; set { _CompletedDir = value; OnPropertyChanged(); } }

        public int SourceFileRes { get => _SourceFileRes; set { _SourceFileRes = value; OnPropertyChanged(); } }

        public bool FileExistsInFinishedDir { get => _FileExistsInFinishedDir; set { _FileExistsInFinishedDir = value; OnPropertyChanged(); } }

        public bool ExcludeFile { get => _ExcludeFile; set { _ExcludeFile = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
