using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.FileProviders;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VideoGui
{
    public class JobListDetails : INotifyPropertyChanged
    {
        private string _RTMP, _ScriptFile, _title, _MultiFile, _fileinfo, _sourcePath, _handle, 
            _VideoInfo, _MultiSourceDir,
            _SourceFile, _FileNoExt, _FileExt, _DestMFile, _StartPos, _EndPos, _PosMode, _MuxData;
        private int _progress, _OwnedByID, _SourceFileIndex, _DeletionFileHandle;
        private double _TotalSeconds;
        //CET MOD
        private bool _IsTwitchStream, _Is5K, _Is4kAdobe, _IsMSJ, _IsNVM, _Complete, 
            _ConversionStarted, _ProbePassed, _KeepSource, _IsMulti,
            _fisheye, _processed, _X264Override, _ComplexMode, _Is720p,
            _Is48K, _IsComplex, _ProbeLock, _IsAc3_2Channel, _IsAc3_6Channel,
            _InProcess, _IsSkipped, _IsShorts,_Mpeg4ASP, _Mpeg4AVC, _Is1080p, 
            _Is4K, _IsInterlaced, _ProbeStarted,    _IsMuxed;
        private FontStyle _ItemFontStyle;
        private Color _ForegroundColor;
        private int _IsCreateShorts = -1;
        private Nullable<DateTime> _twitchschedule;
        private DateTime _JobDate, _ProbeDate, _LastProgressEvent;
        private List<string> ComplexCutList = new List<string>();


        public List<string> GetCutList()
        {
            try
            {
                return ComplexCutList;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{MethodBase.GetCurrentMethod().Name} {ex.Message}");
                return null;
            }
        }
        public void ClearComplexList()
        {
            try
            {
                ComplexCutList.Clear();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        public void AddToComplexList(string cut)
        {
            try
            {
                ComplexCutList.Add(cut);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        public DateTime JobDate { get => _JobDate; set { _JobDate = value; OnPropertyChanged(); } }
        public DateTime ProbeDate { get => _ProbeDate; set { _ProbeDate = value; OnPropertyChanged(); } }
        public DateTime LastProgressEvent { get => _LastProgressEvent; set { _LastProgressEvent = value; OnPropertyChanged(); } }
        public bool IsAc3_2Channel { get => _IsAc3_2Channel; set { _IsAc3_2Channel = value; OnPropertyChanged(); } }
        public bool IsMuxed { get => _IsMuxed; set { _IsMuxed = value; OnPropertyChanged(); } }
        public string MuxData { get => _MuxData; set { _MuxData = value; OnPropertyChanged(); } }
        public bool IsAc3_6Channel { get => _IsAc3_6Channel; set { _IsAc3_6Channel = value; OnPropertyChanged(); } }
        public bool Is48K { get => _Is48K; set { _Is48K = value; OnPropertyChanged(); } }
        public bool KeepSource { get => _KeepSource; set { _KeepSource = value; OnPropertyChanged(); } }
        public bool Is4K { get => _Is4K; set { _Is4K = value; OnPropertyChanged(); } }
        // CET MOD
        public bool IsNVM { get => _IsNVM; set { _IsNVM = value; OnPropertyChanged(); } }

        public bool IsMSJ { get => _IsMSJ; set { _IsMSJ = value; OnPropertyChanged(); } }
        public bool Is5K { get => _Is5K; set { _Is5K = value; OnPropertyChanged(); } }
        public bool IsTwitchStream { get => _IsTwitchStream; set { _IsTwitchStream = value; OnPropertyChanged(); } }
        public bool IsTwitchOut { get => _IsTwitchStream && !_twitchschedule.HasValue; set { _IsTwitchStream = _IsTwitchStream; OnPropertyChanged(); } }


        public bool IsTwitchActive { get => _IsTwitchStream && _twitchschedule.HasValue; set { _IsTwitchStream = _IsTwitchStream; OnPropertyChanged(); } }
        public Nullable<DateTime> twitchschedule { get => _twitchschedule; set { _twitchschedule = value; OnPropertyChanged(); } }
        public string RTMP { get => _RTMP; set { _RTMP = value; OnPropertyChanged(); } }


        public bool IsShorts { get => _IsShorts; set { _IsShorts = value; OnPropertyChanged(); } }
        public int IsCreateShorts { get => _IsCreateShorts; set { _IsCreateShorts = value; OnPropertyChanged(); } }

        public bool Is4KAdobe { get => _Is4kAdobe; set { _Is4kAdobe = value; OnPropertyChanged(); } }

        public bool ProbePassed { get => _ProbePassed; set { _ProbePassed = value; OnPropertyChanged(); } }
        public bool Is720P { get => _Is720p; set { _Is720p = value; OnPropertyChanged(); } }
        public bool ComplexMode { get => _ComplexMode; set { _ComplexMode = value; OnPropertyChanged(); } }
        public bool IsComplex { get => _IsComplex; set { _IsComplex = value; OnPropertyChanged(); } }
        public double TotalSeconds { get => _TotalSeconds; set { _TotalSeconds = value; OnPropertyChanged(); } }
        public string VideoInfo { get => _VideoInfo; set { _VideoInfo = value; OnPropertyChanged(); } }

        public string ScriptFile { get => _ScriptFile; set { _ScriptFile = value; OnPropertyChanged(); } }
        public string SourceFile { get => _SourceFile; set { _SourceFile = value; OnPropertyChanged(); } }
        public string DestMFile { get => _DestMFile; set { _DestMFile = value; OnPropertyChanged(); } }
        public string MultiSourceDir { get => _MultiSourceDir; set { _MultiSourceDir = value; OnPropertyChanged(); } }
        public string StartPos { get => _StartPos; set { _StartPos = value; OnPropertyChanged(); } }
        public string EndPos { get => _EndPos; set { _EndPos = value; OnPropertyChanged(); } }
        public string PosMode { get => _PosMode; set { _PosMode = value; OnPropertyChanged(); } }
        public string MultiFile { get => _MultiFile; set { _MultiFile = value; OnPropertyChanged(); } }
        public string FileExt { get => _FileExt; set { _FileExt = value; OnPropertyChanged(); } }
        public string FileNoExt { get => GetFileLocation(); set { _FileNoExt = value; OnPropertyChanged(); } }
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
        public string Fileinfo { get => _fileinfo; set { _fileinfo = value; OnPropertyChanged(); } }
        public string Handle { get => _handle; set { _handle = value; OnPropertyChanged(); } }
        public string SourcePath { get => _sourcePath; set { _sourcePath = value; OnPropertyChanged(); } }
        public bool Processed { get => _processed; set { _processed = value; OnPropertyChanged(); } }
        public int Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

        public int DeletionFileHandle { get => _DeletionFileHandle; set { _DeletionFileHandle = value; OnPropertyChanged(); } }

        public int SourceFileIndex { get => _SourceFileIndex; set { _SourceFileIndex = value; OnPropertyChanged(); } }

        public bool X264Override { get => _X264Override; set { _X264Override = value; OnPropertyChanged(); } }
        public bool ProbeLock { get => _ProbeLock; set { _ProbeLock = value; OnPropertyChanged(); } }
        public bool Is1080p { get => _Is1080p; set { _Is1080p = value; OnPropertyChanged(); } }
        public bool IsInterlaced { get => _IsInterlaced; set { _IsInterlaced = value; OnPropertyChanged(); } }

        public bool Complete { get => _Complete; set { _Complete = value; OnPropertyChanged(); } }
        public bool IsMulti { get => _IsMulti; set { _IsMulti = value; OnPropertyChanged(); } }
        public bool Mpeg4ASP { get => _Mpeg4ASP; set { _Mpeg4ASP = value; OnPropertyChanged(); } }
        public bool Mpeg4AVC { get => _Mpeg4AVC; set { _Mpeg4AVC = value; OnPropertyChanged(); } }
        public bool InProcess { get => _InProcess; set { _InProcess = value; OnPropertyChanged(); } }
        public FontStyle ItemFontStyle { get => _ItemFontStyle; set { _ItemFontStyle = value; OnPropertyChanged(); } }
        public Color ForegroundColor { get => _ForegroundColor; set { _ForegroundColor = value; OnPropertyChanged(); } }
        public bool IsSkipped { get => _IsSkipped; set { _IsSkipped = value; OnPropertyChanged(); } }
        public bool ConversionStarted { get => _ConversionStarted; set { _ConversionStarted = value; OnPropertyChanged(); } }

        public bool ProbeStarted { get => _ProbeStarted; set { _ProbeStarted = value; OnPropertyChanged(); } }


        public JobListDetails(FbDataReader reader, int cnt)
        {
            try
            {
                Is5K = false;
                RTMP = "";
                bool IsComplexYT = false;
                var srcdir = reader["SRCDIR"].ToString();
                var destfname = reader["DESTFNAME"].ToString();
                Title = Path.GetFileName(destfname);
                Is720P = (reader["B720P"] is Int16 _is720p) ? (Int16)_is720p == 1 : false;
                IsShorts = (reader["BSHORTS"] is Int16 _isShorts) ? (Int16)_isShorts == 1 : false;
                IsMuxed = (reader["ISMUXED"] is Int16 _isMux) ? (Int16)_isMux == 1 : false;
                MuxData = (reader["MUXDATA"] is string _isMuxData) ? _isMuxData : "";
                IsCreateShorts = (reader["BCREATESHORTS"] is Int16 _isCShorts) ? (Int16)_isCShorts : -1; ;
                var IsEncodeTrim = (reader["BENCODETRIM"] is Int16 _isEncodeTrim) ? (Int16)_isEncodeTrim == 1 : false;
                IsTwitchStream = (reader["BTWITCHSTREAM"] is Int16 _isTwitchStream) ? (Int16)_isTwitchStream == 1 : false;
                Nullable<DateTime> TwitchDateOnly = (reader["TWITCHDATE"] is DateTime _TwitchDate) ? (DateTime)_TwitchDate : null;
                Nullable<TimeSpan> TwitchTimeSpan = (reader["TWITCHTIME"] is TimeSpan _TwitchTime) ? (TimeSpan)_TwitchTime : null;
                twitchschedule = null;
                if (TwitchDateOnly is not null && TwitchTimeSpan is not null)
                {
                    twitchschedule = TwitchDateOnly.Value.AtTime(TwitchTimeSpan.Value);
                    if (twitchschedule.Value.Year < 1800)
                    {
                        //IsTwitchStream = false;
                        RTMP = "";
                        twitchschedule = null;
                    }
                    else RTMP = reader["RTMP"].ToString();
                }
                var IsCutTrim = (reader["BCUTTRIM"] is Int16 _isCutTrim) ? (Int16)_isCutTrim == 1 : false;
                long start = reader["STARTPOS"].ToString().ToInt();
                long end = reader["DURATION"].ToString().ToInt();
                _IsMSJ = (reader["BMONITOREDSOURCE"] is Int16 isMonitoredSource) ? (Int16)isMonitoredSource == 1 : false;
                TimeSpan ST1 = TimeSpan.FromMilliseconds(start);
                TimeSpan Dur = TimeSpan.FromMilliseconds(end);
                var _StartPos = ((!Is720P) && (!IsShorts)) ? TimeSpan.FromMilliseconds(start) : TimeSpan.Zero;
                var Durationcut = ((!Is720P) && (!IsShorts)) ? TimeSpan.FromMilliseconds(end) : TimeSpan.Zero;
                StartPos = (_StartPos != TimeSpan.Zero) ? _StartPos.ToString() : "";
                var _Duration = Durationcut != TimeSpan.Zero ? Durationcut : TimeSpan.Zero;


                if (_Duration != TimeSpan.Zero) IsComplexYT = true;
                int ScriptType = 0;
                if (Is720P) ScriptType = 2;
                if (IsShorts) ScriptType = 0;
                if (IsCutTrim) ScriptType = 1;
                if (IsEncodeTrim) ScriptType = 3;
                if (IsCreateShorts != 0) ScriptType = 4;
                if (IsTwitchStream) ScriptType = 5;
                if (IsMuxed) ScriptType = 6;
                string CutFrames = (_Duration != TimeSpan.Zero) ?
                    $"|{_StartPos.ToFFmpeg()}|{_Duration.ToFFmpeg()}|time" : "";
                ScriptFile = $"true|{destfname}|{srcdir}|*.mp4{CutFrames}";
                SourceFileIndex = cnt;
                (_Is1080p, _Is4K) = (false, true);
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var DestDirectoryAdobe4K = key.GetValueStr("DestDirectoryAdobe4k");
                key?.Close();
                Is4KAdobe = (destfname.ToLower().Contains(DestDirectoryAdobe4K.ToLower()));
                (ProbePassed, Complete, ProbeLock, _ProbeStarted, _ConversionStarted, Processed, JobDate, MultiFile, _IsNVM) =
                    (true, false, false, false, false, false, DateTime.Now, Title, true);
                DeletionFileHandle = reader["ID"].ToInt();
                

                IsTwitchStream = ScriptType == 5;
                Is4K = (Is4KAdobe) ? true : Is4K;
                List<string> Commands = _ScriptFile.Split('|').ToList();
                string ks = Commands.FirstOrDefault();
                KeepSource = false;
                if (Commands.Count > 3)
                {
                    KeepSource = ks.ToLower() == "true";
                    Commands.RemoveAt(0);
                }
                DestMFile = Commands.FirstOrDefault();
                FileExt = Path.GetExtension(DestMFile);
                //Title = Path.GetFileNameWithoutExtension(DestMFile);
                _IsMulti = true;
                Commands.RemoveAt(0);
                string sourceDir = Commands.FirstOrDefault();
                MultiSourceDir = sourceDir;
                Commands.RemoveAt(0);
                string ext = Commands.FirstOrDefault();
                Commands.RemoveAt(0);
                if ((Commands.Count >= 2) && (IsTwitchStream || IsComplexYT))
                {
                    StartPos = Commands.FirstOrDefault();
                    EndPos = Commands[1].ToString();
                    PosMode = Commands[2].ToString();
                }
                List<string> Files = Directory.EnumerateFiles(sourceDir, "*" + ext, SearchOption.AllDirectories).ToList();
                ComplexCutList.AddRange(Files.Where(file => File.Exists(file)).Select(file => $"file '{file}'"));
                (_handle, X264Override, Mpeg4ASP, Mpeg4AVC, _IsInterlaced, IsSkipped) = ("", false, false, false, false, false);
                (_Is48K, _IsAc3_2Channel, _IsAc3_6Channel) = (false, false, false);
                ItemFontStyle = (X264Override) ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;
                ForegroundColor = X264Override ? Color.FromScRgb(100, 6, 186, 28) : Color.FromArgb(100, 246, 8, 50);
                Handle = "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public JobListDetails(string _Title, int _SourceFileIndex, int _Autoinssertid, string _ScriptFile,
            int _ScriptType, bool _Is1080p = false, bool _Is4Kp = false, bool _ISMJS = false,
            bool __Is4KAdobe = false, bool __IsShorts = false, int __IsCreateShorts = 0,
            string _FIleInfo = "", bool __IsMuxed = false, string __muxdata = "")
        {
            // _ScriptType - 0 = src, 1 cst, 2 edt
            DeletionFileHandle = -1;
            (Fileinfo, Progress, SourceFileIndex, Is1080p, Is4K, ScriptFile) = (_FIleInfo, 0, _SourceFileIndex, _Is1080p, _Is4Kp, _ScriptFile);
            (ProbePassed, Complete, ProbeLock, _ProbeStarted, _ConversionStarted, Processed) = (true, false, false, false, false, false);
            (JobDate, MultiFile) = (DateTime.Now, _Title);
            _IsNVM = true;
            _IsMuxed = __IsMuxed;
            MuxData = __muxdata;
            Is5K = false;
            IsTwitchStream = false || _ScriptType == 5;
            Is4KAdobe = __Is4KAdobe;
            _IsMSJ = _ISMJS;
            DeletionFileHandle = _Autoinssertid;
            
            IsShorts = __IsShorts | _ScriptType == 0 || _ScriptType == 4;
            IsCreateShorts = _IsCreateShorts;
            Is4K = (Is4KAdobe || IsShorts) ? true : Is4KAdobe;
            List<string> Commands = _ScriptFile.Split('|').ToList();
            string ks = Commands.FirstOrDefault();
            KeepSource = false;
            if (Commands.Count > 3)
            {
                KeepSource = ks.ToLower() == "true";
                Commands.RemoveAt(0);
            }
            DestMFile = Commands.FirstOrDefault();
            FileExt = Path.GetExtension(DestMFile);
            Title = Path.GetFileNameWithoutExtension(DestMFile);
            _IsMulti = true;
            Commands.RemoveAt(0);
            string sourceDir = Commands.FirstOrDefault();
            MultiSourceDir = sourceDir;
            Commands.RemoveAt(0);
            string ext = Commands.FirstOrDefault();
            Commands.RemoveAt(0);
            if ((Commands.Count >= 2) && ( IsTwitchOut))
            {
                StartPos = Commands.FirstOrDefault();
                EndPos = Commands[1].ToString();
                PosMode = Commands[2].ToString();
            }
            if (!IsMuxed)
            {
                List<string> Files = Directory.EnumerateFiles(sourceDir, "*" + ext, SearchOption.AllDirectories).ToList();
                foreach (string file in Files)
                {
                    if (File.Exists(file))
                    {
                        ComplexCutList.Add($"file '{file}'");
                    }
                }
            }
            _handle = "";
            (X264Override, Mpeg4ASP, Mpeg4AVC, _IsInterlaced, IsSkipped) = (false, false, false, false, false);
            (_Is48K, _IsAc3_2Channel, _IsAc3_6Channel) = (false, false, false);
            ItemFontStyle = (X264Override) ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;
            ForegroundColor = X264Override ? Color.FromScRgb(100, 6, 186, 28) : Color.FromArgb(100, 246, 8, 50);
            if (IsMuxed)
            {
                IsMulti = false;
                Title = Path.GetFileNameWithoutExtension(MultiSourceDir);
                SourceFile = MultiSourceDir;
                SourcePath = Path.GetDirectoryName(MultiSourceDir);
            }
        }

        public JobListDetails(string _Title, int _SourceFileIndex, bool _Is1080p = false,
            bool _Is4Kp = false, bool _Is4KAdobe = false, string _FIleInfo = "",
            int _Progress = 0, bool x265Override = false, bool _IsMpeg4ASP = false,
            bool _IsMpeg4AVC = false)
        {
            Fileinfo = _FIleInfo;
            Progress = _Progress;
            RTMP = "";
            twitchschedule = DateTime.Now.AddYears(-100);
            Processed = false;
            SourceFileIndex = _SourceFileIndex;
            DeletionFileHandle = -1;
            Is4KAdobe |= _Is4KAdobe;
            Is1080p = _Is1080p;
            Is4K = _Is4Kp;
            Handle = "";
            if (Is4KAdobe)
            {
                Is4K = true;
            }
            _IsMSJ = false;
            _ConversionStarted = false;
            ProbePassed = true;
            Complete = false;
            JobDate = DateTime.Now;
            ProbeLock = false;
            MultiFile = "";
            _ProbeStarted = false;
            ScriptFile = "";
            IsNVM = false;
            if (!Is1080p && !Is4K && !Is4KAdobe)
            {
                Is720P = true;
            }
            
            string myext = Path.GetExtension(_Title).ToLower();

            var fnx = Path.GetFileNameWithoutExtension(_Title).ToLower();
            IsComplex = (File.Exists(fnx + ".x264") || File.Exists(fnx + ".x265"));
            if (IsComplex)
            {
                string fnp = File.Exists(fnx + ".x264") ? fnx + ".x264" : fnx + ".x265";
                ComplexMode = fnp.Contains(".x265");
                string cutlist = File.ReadAllText(fnp);
                List<string> cuts = cutlist.Split(new char[] { '|' }).ToList();
                if (cuts.Count > 0)
                {
                    ComplexCutList.AddRange(cuts);
                }
            }

            ProbeDate = DateTime.Now.AddYears(-1500);
            LastProgressEvent = DateTime.Now.AddYears(-1500);
            SourceFile = System.IO.Path.GetFileName(_Title);
            SourcePath = System.IO.Path.GetDirectoryName(_Title);
            Title = System.IO.Path.GetFileNameWithoutExtension(_Title);
            FileNoExt = System.IO.Path.GetFileNameWithoutExtension(_Title);
            FileExt = System.IO.Path.GetExtension(_Title);
            X264Override = x265Override;
            Mpeg4ASP = _IsMpeg4ASP;
            Mpeg4AVC = _IsMpeg4AVC;
            _IsMulti = false;
            _IsInterlaced = false;
            IsSkipped = false;// 0 means not locked
            _Is48K = false;
            _IsAc3_2Channel = false;
            _IsAc3_6Channel = false;

            _handle = "";
            ItemFontStyle = (X264Override) ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;
            ForegroundColor = X264Override ? Color.FromScRgb(100, 6, 186, 28) : Color.FromArgb(100, 246, 8, 50);

        }

        public string GetFileLocation()
        {
            return (IsMulti) ? Path.GetFileNameWithoutExtension(_DestMFile) : _FileNoExt;
        }

        public JobListDetails()
        {
            Handle = "";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
