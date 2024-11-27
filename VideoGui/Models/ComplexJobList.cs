using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VideoGuiNetCore.Properties;

namespace VideoGui.Models
{
    public class ComplexJobList : INotifyPropertyChanged
    {
        private TimeSpan _Start, _Duration;
        private string _RTMP, _SourceDirectory, _DestinationDirectory, _Filename;
        private bool _IsTwitchStream, _Is720p, _IsShorts, _IsCutTrim, _IsEncodeTrim,
              _IsDeleteMonitoredSource, _IsPersistentJob, _IsLocked, _IsMuxed;
        private string _Id, _MuxData;
        private Nullable<DateTime> _TwitchSchedule;
        private int _IsCreateShorts = -1;


        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public TimeSpan Start { get => _Start; set { _Start = value; OnPropertyChanged(); } }
        public TimeSpan Duration { get => _Duration; set { _Duration = value; OnPropertyChanged(); } }
        public string SourceDirectory { get => _SourceDirectory; set { _SourceDirectory = value; OnPropertyChanged(); } }
        public string MuxData { get => _MuxData; set { _MuxData = value; OnPropertyChanged(); } }
        public string DestinationDirectory { get => _DestinationDirectory; set { _DestinationDirectory = value; OnPropertyChanged(); } }
        public string Filename { get => _Filename; set { _Filename = value; OnPropertyChanged(); } }

        public Nullable<DateTime> TwitchSchedule { get => _TwitchSchedule; set { _TwitchSchedule = value; OnPropertyChanged(); } }

        public string DestinationFile { get => _DestinationDirectory + "\\" + _Filename; set { _Filename = _Filename; OnPropertyChanged(); } }

        public string _SRC = "", _DEST = "", _Times = "", _ProceessingType = "", _ProcessingActions = "", _RecordAge = "";

        public string SRC { get => _SRC; set { _SRC = value; OnPropertyChanged(); } }
        public string DEST { get => _DEST; set { _DEST = value; OnPropertyChanged(); } }


        public string Times { get => _Times; set { _Times = value; OnPropertyChanged(); } }

        public string ProceessingType { get => _ProceessingType; set { _ProceessingType = value; OnPropertyChanged(); } }

        public string ProcessingActions { get => _ProcessingActions; set { _ProcessingActions = value; OnPropertyChanged(); } }
        public string RecordAge { get => _RecordAge; set { _RecordAge = value; OnPropertyChanged(); } }
        public string RTMP { get => _RTMP; set { _RTMP = value; OnPropertyChanged(); } }

        public bool IsMuxed { get => _IsMuxed; set { _IsMuxed = value; OnPropertyChanged(); } }

        public bool IsTwitchStream { get => _IsTwitchStream; set { _IsTwitchStream = value; OnPropertyChanged(); } }
        public bool IsLocked { get => _IsLocked; set { _IsLocked = value; OnPropertyChanged(); } }
        public bool Is720p { get => _Is720p; set { _Is720p = value; OnPropertyChanged(); } }
        public bool IsShorts { get => _IsShorts; set { _IsShorts = value; OnPropertyChanged(); } }
        public int IsCreateShorts { get => _IsCreateShorts; set { _IsCreateShorts = value; OnPropertyChanged(); } }
        public bool IsCutTrim { get => _IsCutTrim; set { _IsCutTrim = value; OnPropertyChanged(); } }
        public bool IsEncodeTrim { get => _IsEncodeTrim; set { _IsEncodeTrim = value; OnPropertyChanged(); } }
        public bool IsDeleteMonitoredSource { get => _IsDeleteMonitoredSource; set { _IsDeleteMonitoredSource = value; OnPropertyChanged(); } }
        public bool IsPersistentJob { get => _IsPersistentJob; set { _IsPersistentJob = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ComplexJobList(FbDataReader reader)
        {
            try
            {
                IsLocked = false;
                IsShorts = false;
                IsCreateShorts = -1;
                IsCutTrim = false;
                IsEncodeTrim = false;
                Id = reader["Id"].ToString();
                SourceDirectory = reader["SRCDIR"].ToString();
                var destfname = reader["DESTFNAME"].ToString();
                DestinationDirectory = Path.GetDirectoryName(destfname);
                Filename = Path.GetFileName(destfname);
                IsMuxed = (reader["IsMuxed"] is Int16 _ismuxed) ? _ismuxed == 1 : false;
                MuxData = (reader["MuxData"] is string _MuxData) ? _MuxData : "";
                Is720p = (reader["B720P"] is Int16 _is720p) ? _is720p == 1 : false;
                IsShorts = (reader["BSHORTS"] is Int16 _isShorts) ? (Int16)_isShorts == 1 : false;
                IsCreateShorts = (reader["BCREATESHORTS"] is Int16 _isCreateShorts) ? (Int16)_isCreateShorts : -1;
                IsEncodeTrim = (reader["BENCODETRIM"] is Int16 _isEncodeTrim) ? (Int16)_isEncodeTrim == 1 : false;
                IsCutTrim = (reader["BCUTTRIM"] is Int16 _isCutTrim) ? (Int16)_isCutTrim == 1 : false;
                IsDeleteMonitoredSource = (reader["BMONITOREDSOURCE"] is Int16 _isMonitoredSource) ? (Int16)_isMonitoredSource == 1 : false;
                IsPersistentJob = (reader["BPERSISTENTJOB"] is Int16 _isPersistentJob) ? (Int16)_isPersistentJob == 1 : false;
                IsTwitchStream = (reader["BTWITCHSTREAM"] is Int16 _isTwitchStream) ? (Int16)_isTwitchStream == 1 : false;
                Nullable<DateTime> TwitchDateOnly = (reader["TWITCHDATE"] is DateTime _TwitchDate) ? (DateTime)_TwitchDate : null;
                Nullable<TimeSpan> TwitchTimeSpan = (reader["TWITCHTIME"] is TimeSpan _TwitchTime) ? (TimeSpan)_TwitchTime : null;
                TwitchSchedule = null;
                if (TwitchDateOnly is not null && TwitchTimeSpan is not null)
                {
                    TwitchSchedule = TwitchDateOnly.Value.AtTime(TimeOnly.FromTimeSpan(TwitchTimeSpan.Value));//.ToDateTime(TimeOnly.FromTimeSpan(TwitchTimeSpan.Value));
                    if (TwitchSchedule.Value.Year < 1800)
                    {
                        IsTwitchStream = false;
                        RTMP = "";
                        TwitchSchedule = null;
                    }
                    else RTMP = reader["RTMP"].ToString();
                }
                long start = reader["STARTPOS"].ToString().ToInt();
                long end = reader["DURATION"].ToString().ToInt();
                //
                TimeSpan ST1 = TimeSpan.FromMilliseconds(start);
                TimeSpan Dur = TimeSpan.FromMilliseconds(end);
                var stype = (IsCreateShorts == 1) ? "Short FMT " : (IsCreateShorts == 2) ? "Long FMT " : "";
                var StartPos = ((!Is720p) && (!IsShorts)) ? TimeSpan.FromMilliseconds(start) : TimeSpan.Zero;
                var Durationcut = ((!Is720p) && (!IsShorts)) ? TimeSpan.FromMilliseconds(end) : TimeSpan.Zero;
                Start = StartPos != TimeSpan.Zero ? StartPos : TimeSpan.Zero;
                Duration = Durationcut != TimeSpan.Zero ? Durationcut : TimeSpan.Zero;
                string StartTime = "", EndTime = "";
                StartTime = (!Is720p && !IsShorts) ? Start.ToFFmpeg().Replace(".000", "") : "";
                EndTime = (Duration != TimeSpan.Zero && !Is720p && !IsShorts) ? Duration.ToFFmpeg().Replace(".000", "") : "";
                Times = (!Is720p && !IsShorts) ? $"{StartTime}-{EndTime}" : "";
                ProceessingType = (Is720p) ? "720p Edit File" : (IsShorts) ? "Shorts Master File" :
                    (IsCutTrim) ? "Non Encoded Trim" : (IsEncodeTrim) ? "Encoded Trim" : (IsMuxed) ? "Muxing Job" : "";
                ProcessingActions = (IsCreateShorts > 0) ? $"Creating {stype}Shorts" : (IsShorts) ? "Creating Shorts Master" :
                    (IsPersistentJob && IsDeleteMonitoredSource) ? "Monitored Persistent Job" :
                    (IsPersistentJob) ? "Persistent Job" : (IsDeleteMonitoredSource) ? "Monitored Source" :
                    (IsMuxed) ? "Muxing Action" : "Standard Actions";
                SRC = SourceDirectory.Split("\\").ToList().LastOrDefault();
                DEST = Path.GetFileNameWithoutExtension(Filename);

            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public ComplexJobList(string srcdir, string destfname, TimeSpan StartPos, TimeSpan Durationcut,
            bool b720p, bool bShorts, int bCreateShorts, bool bEncodeTrim, bool bCutTrim, bool bMonitoredSource,
            bool bPersistentJob, int id, bool _IsMuxed, string _MuxData)
        {
            try
            {
                SourceDirectory = srcdir;
                IsLocked = false;
                DestinationDirectory = Path.GetDirectoryName(destfname);
                Filename = Path.GetFileName(destfname);
                Start = StartPos != TimeSpan.Zero ? StartPos : TimeSpan.Zero;
                Duration = Durationcut != TimeSpan.Zero ? Durationcut : TimeSpan.Zero;
                Is720p = b720p;
                IsMuxed = _IsMuxed;
                MuxData = _MuxData;
                IsShorts = bShorts;
                RTMP = "";
                TwitchSchedule = DateTime.Now.AddYears(-500);
                IsTwitchStream = false;
                IsEncodeTrim = bEncodeTrim;
                IsCutTrim = bCutTrim;
                Id = (id != -1) ? id.ToString() : "";
                IsDeleteMonitoredSource = bMonitoredSource;
                IsPersistentJob = bPersistentJob;
                IsCreateShorts = bCreateShorts;
                string StartTime = "", EndTime = "";
                StartTime = (!Is720p && !IsShorts) ? Start.ToFFmpeg().Replace(".000", "") : "";
                EndTime = (Duration != TimeSpan.Zero && !Is720p && !IsShorts) ? Duration.ToFFmpeg().Replace(".000", "") : "";
                Times = (!Is720p && !IsShorts) ? $"{StartTime}-{EndTime}" : "";
                ProceessingType = (Is720p) ? "720p Edit File" : (IsShorts) ? "Shorts Master File" :
                    (IsCutTrim) ? "Non Encoded Trim" : (IsEncodeTrim) ? "Encoded Trim" : (IsMuxed) ? "Muxing Job" : "";
                ProcessingActions = (IsPersistentJob && IsDeleteMonitoredSource) ? "Monited Persistent Job" :
                    (IsPersistentJob) ? "Persistent Job" : (IsDeleteMonitoredSource) ? "Monitored Source" :
                    (IsMuxed) ? "Muxing Action" : "Standard Actions";

                SRC = SourceDirectory.Split("\\").ToList().LastOrDefault();
                DEST = Path.GetFileNameWithoutExtension(Filename);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
