using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VideoGui.ffmpeg;

namespace VideoGui.Models
{

    public class SourceFileInfo
    {
        private string _FileName;
        private TimeSpan _Start, _Duration;

        public string FileName { get => _FileName; set { _FileName = value; } }

        public TimeSpan Start { get => _Start; set { _Start = value; } }

        public TimeSpan Duration { get => _Duration; set { _Duration = value; } }
        public SourceFileInfo(string __FileName, TimeSpan __Start, TimeSpan __Duration)
        {
            try
            {
                FileName = __FileName;
                _Start = __Start;
                _Duration = __Duration;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
    public class SourceFileCache : INotifyPropertyChanged
    {
        private string _SourceDirectory;
        private DateTime _LastModified;
        private List<SourceFileInfo> _SourceFiles;
        public string SourceDirectory { get => _SourceDirectory; set { _SourceDirectory = value; OnPropertyChanged(); } }
        public DateTime LastModified { get => _LastModified; set { _LastModified = value; OnPropertyChanged(); } }

        public List<SourceFileInfo> SourceFiles
        {
            get => _SourceFiles; set
            {
                _SourceFiles = value; OnPropertyChanged();
            }
        }
        public string[] GetDefaultVideoExts()
        {
            return new string[] { ".avi", ".mkv", ".mp4", ".m2ts" };
        }
        public string GetExePath()
        {
            try
            {
                string res = "";
                string defaultprogramlocation = @"c:\videogui";
                res = (Debugger.IsAttached) ? defaultprogramlocation : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        public async void RebuildList(string _SourceDirectory)
        {
            try
            {
                _SourceFiles.Clear();
                List<string> Files = Directory.EnumerateFiles(_SourceDirectory, "*.*", SearchOption.AllDirectories).
                    Where(s => s.ToLower().EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                BuildList(Files);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public async void BuildList(List<string> Files)
        {
            try
            {
                _LastModified = DateTime.Now;
                var FileConverter = new ffmpegbridge();

                FileConverter.ReadDuration(Files);
                while (!FileConverter.Finished)
                {
                    Thread.Sleep(250);
                }
                List<(string, double)> FileInfos = new List<(string, double)>();
                FileInfos.AddRange(FileConverter.FileInfoList);

                TimeSpan StartPos = TimeSpan.Zero;
                foreach (var file in FileInfos)
                {
                    TimeSpan tspan = TimeSpan.FromMilliseconds(file.Item2);
                    SourceFileInfo TFI = new SourceFileInfo(file.Item1, StartPos, tspan);
                    StartPos += tspan;
                    _SourceFiles.Add(TFI);
                }
                FileConverter = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public SourceFileCache(string _SourceDirectory, bool buildlist = true)
        {
            try
            {
                _SourceFiles = new List<SourceFileInfo>();
                if (buildlist)
                {
                    List<string> Files = Directory.EnumerateFiles(_SourceDirectory, "*.*", SearchOption.AllDirectories).
                        Where(s => s.ToLower().EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                    BuildList(Files);   //TotalSecs = +file.Duration.TotalSeconds;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
