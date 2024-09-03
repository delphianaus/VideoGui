using CliWrap;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using FolderBrowserEx;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VideoGui.ffmpeg;
using VideoGui.ffmpeg.Events;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Video;
using Application = System.Windows.Application;
using File = System.IO.File;
using Path = System.IO.Path;
using Window = System.Windows.Window;
using System.IO.Packaging;
using Microsoft.Extensions.FileProviders;
using FirebirdSql.Data.FirebirdClient;
using VideoGui.Models;
using System.Drawing;
using VideoGui.Models.delegates;
using Nancy.ViewEngines;
using System.Data.Common;
using System.CodeDom;
using System.Security.Principal;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Data.SqlTypes;
using Microsoft.VisualBasic;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Net.Sockets;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Controls.Ribbon;
using SevenZip;
using System.DirectoryServices.ActiveDirectory;
using AsyncAwaitBestPractices;
using System.Collections;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using MediaInfo.Model;
using System.Formats.Tar;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using System.Security.Cryptography.Pkcs;
using Microsoft.WindowsAPICodePack.Shell;
using Nancy.Extensions;
using static System.Net.Mime.MediaTypeNames;
using Nancy.TinyIoc;
using System.Xml.Linq;
using System.Collections.Immutable;
using System.Windows.Media.Imaging;


namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// C:\VideoGui\src\VideoGui\MainWindow.xaml.cs  // ver 280 merge from 279 server ver
    public class Stats
    {
        public string filename;
        public float fps, bitrate;
        public int frames;
        public TimeSpan Duration, TotalTime, EstTime, ProcessTime;
        public double Percent, TotalPercent;
        public DateTime StartTime;
        public int CurrentFrame;
        public Stats(string Filename)
        {
            this.filename = Filename;
            this.StartTime = DateTime.Now;
            this.CurrentFrame = -1;
        }

    }

    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        public _StatsHandler ThreadStatsHandler;
        bool ShiftActiveWindowClosing = false;
        public _UpdateSpeed ThreadUpdateSpeed;
        public _UpdateProgress ThreadUpdateProgress;
        public _UpdateTime ThreadUpdateTime;
        public _StatsHandlerDateTimeSetter ThreadStartTimeSet;
        public _StatsHandlerDateTimeGetter ThreadStartTimeGet;
        public _StatsHandlerBool ThreadStatsHandlerBool;
        public _StatsHandlerExtra ThreadStatsHandlerXtra;
        public IsFileInUse OnIsFileInUse;
        public Models.delegates.CompairFinished OnCompairFinished;
        public SourceDestComp compareform;
        public VideoSizeChecker videoResCompare;
        public VideoCardSelector videoCardDetailsSelector;
        public ScraperModule scraperModule = null;
        public SelectShortUpload selectShortUpload = null;
        public ShowMatcher Swm;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource ProcessingCancellationTokenSource = new CancellationTokenSource();
        double frames, LRF, RRF, LLC, RLC;
        DateTime totaltimeprocessed = DateTime.Now;
        object lookuplocked = new object();
        private System.Object thisLock = new Object();
        private Object thisfLock = new Object();
        private Object thispLock = new Object();
        private Object thissLock = new Object();
        public ShortsCreator frmShortsCreator = null;
        public object statslocker = new(), ProcessingJobslocker = new(), ProbeLock = new();
        string Video = "";
        public int LockedDeviceID = 0, SourceIndex = 0;
        public object StatsLock = new();
        List<Task> PageDownloadTasks = new();
        bool SystemSetup = false, InTray = false;
        public bool canclose = false;
        int failed = 0, Passed = 0, InitTitleLength = 0, cnt4K = 0, cnt = 0, cnt1080p = 0, currentjob = 0, trayiconnum = 0;
        private string[] FilesInProcess = Array.Empty<string>();
        string DestFile, filename_pegpeg, link, ProbeData;
        bool ffmpegdone = false, processing = false;
        string Root, ffmpeg_ver = "", ffmpeg_gitver = "";
        string MinBitRate = "", MaxBitRate = "", BitRateBuffer = "", un = "", up = "";
        string VideoHeight, VideoWidth, ArRounding, ArModulas;
        bool ArRoundEnable = true, VSyncEnable, ResizeEnable = true, ArScalingEnabled = true;
        List<string> SourceList = new List<string>();
        StatsHandler Stats_Handler = new StatsHandler();
        DateTime ProcessingTime;
        System.Windows.Forms.Timer FileQueChecker, FormResizerEvent;
        DispatcherTimer TrayIcon;
        public ConverterProgressInfo frmConverterProgressInfo = null;
        public ObservableCollection<JobListDetails> ProcessingJobs = new ObservableCollection<JobListDetails>();
        public ObservableCollection<SourceFileCache> SourceFileInfos = new ObservableCollection<SourceFileCache>();
        public CollectionViewSource HistoricCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource CurrentCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource ImportCollectionViewSource = new CollectionViewSource();
        public MediaImporter GoProMediaImporter = null;
        public ObservableCollection<ComplexJobList> ComplexProcessingJobList = new ObservableCollection<ComplexJobList>();
        public ObservableCollection<ComplexJobHistory> ComplexProcessingJobHistory = new ObservableCollection<ComplexJobHistory>();
        public ObservableCollection<FileInfoGoPro> FileRenamer = new ObservableCollection<FileInfoGoPro>();

        public ObservableCollection<PlanningQue> PlanningQueList = new ObservableCollection<PlanningQue>();
        public ObservableCollection<PlanningCuts> PlanningCutsList = new ObservableCollection<PlanningCuts>();

        public List<ShortsProcessor> ShortsProcessors = new List<ShortsProcessor>();

        VideoCutsEditor frmVCE = null;

        public List<string> ComparitorList = new();
        string DestDirectory720p = string.Empty, DestDirectoryAdobe4K = string.Empty, DestDirectory4K = string.Empty, DestDirectory1080p = string.Empty, backupun = "", DestDirectoryTwitch = "";
        string fileprogress = "", DoneDirectory720p = string.Empty, DoneDirectory1080p = string.Empty, DoneDirectory4K = string.Empty, DoneDirectoryAdobe4K = string.Empty;
        string ErrorDirectory = string.Empty, SourceDirectory4K = string.Empty, SourceDirectoryAdobe4K = string.Empty, SourceDirectory1080p = string.Empty, SourceDirectory720p = string.Empty;
        TimeSpan ProcessingTimeGlobal;
        DateTime StartTime = DateTime.Now, Start2 = DateTime.Now, Start3 = DateTime.Now;
        bool ffmpegready = false, usetorrents = true, CanUpdate = true, ShowAudioControlDialog = true, UseFisheyeRemoval;
        int totaltasks = 4, total1080ptasks = 2, total4kTasks = 3;
        string backupserver = "", backupDone1080p = "", backupDone = "", backupCompleted = "";
        string txtDestPath = "", txtDonepath = "", txtErrorPath = "";
        string CurrentLogFile = "", defaultprogramlocation = "";
        CancellationTokenSource ffmpeg_download_cancellationToken;
        CancellationTokenSource app_download_cancellationToken;
        private readonly SynchronizationContext _syncContext;
        List<Task> mytasklist = new List<Task>();
        List<string> _720PFiles = new(), _1080PFiles = new(), _4KFiles = new();
        List<ConverterProgressInfo> ProgressInfoDisplay = new List<ConverterProgressInfo>();
        public AudioJoiner AutoJoinerFrm = null;
        ComplexSchedular complexfrm = null;
        int CurrentDbId = -1;
        string connectionString = "";
        private ConverterProgress ConverterProgressEventHandler = new ConverterProgress();
        private ObservableCollectionFilters ObservableCollectionFilter;
        //public List<MediaTranscoder?> transcoders = new List<MediaTranscoder?>();
        public class ProgressForegroundConverter2
        {

        }
        public class ProgressForegroundConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                double progress = (double)value;
                System.Drawing.Brush foreground = System.Drawing.Brushes.Green;

                if (progress >= 90d)
                {
                    foreground = System.Drawing.Brushes.Red;
                }
                else if (progress >= 60d)
                {
                    foreground = System.Drawing.Brushes.Yellow;
                }

                return foreground;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public void SetObservableLists(int id)
        {
            try
            {
                if (ObservableCollectionFilter is not null)
                {
                    switch (id)
                    {
                        case 0:
                            {
                                ObservableCollectionFilter.CurrentCollectionViewSource.Source = ComplexProcessingJobList;
                                break;
                            }
                        case 1:
                            {
                                ObservableCollectionFilter.HistoricCollectionViewSource.Source = ComplexProcessingJobHistory;
                                break;
                            }
                        case 2:
                            {
                                ObservableCollectionFilter.ImportCollectionViewSource.Source = FileRenamer;
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public async Task<bool> ConnectI()
        {
            try
            {
                bool res = false;
                if (ObservableCollectionFilter is not null)
                {
                    FileRenamer.Clear();
                    ObservableCollectionFilter.ImportCollectionViewSource.Source = FileRenamer;
                    ObservableCollectionFilter.ImportCollectionViewSource.View.Refresh();
                    if (GoProMediaImporter is not null && GoProMediaImporter.IsLoaded)
                    {
                        GoProMediaImporter.lstSchedules.ItemsSource = ObservableCollectionFilter.ImportCollectionViewSource.View;

                        res = true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public void ShowScraper()
        {
            try
            {
                if (scraperModule is not null)
                {
                    if (!scraperModule.IsClosed && !scraperModule.IsClosing)
                    {
                        scraperModule.Close();
                    }
                    while (true)
                    {
                        if (!scraperModule.IsClosed && scraperModule.IsClosing)
                        {
                            Thread.Sleep(250);
                        }
                        if (scraperModule.IsClosed) break;
                    }
                    scraperModule = null;
                }
                WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                string gUrl = webAddressBuilder.Dashboard().Address;
                
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                key?.Close();
                int MaxShorts = MaxUploads.ToInt(80);
                int MaxPerSlot = uploadsnumber.ToInt(100);
                scraperModule = new ScraperModule(ModuleCallback, FinishScraper,
                    gUrl, false, true, MaxShorts, MaxPerSlot);
                Hide();
                scraperModule.ShowActivated = true;
                scraperModule.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ShowScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void ModuleCallback(object ThisForm, object tld)
        {
            try
            {
                switch(ThisForm)
                {
                    case ScraperModule scraperModule:
                        {
                            scraperModule_Handler(ThisForm, tld);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ModuleCallback {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }

        private void scraperModule_Handler(object thisForm, object tld)
        {
            try
            {
        
            }
            catch (Exception ex)
            {
                ex.LogWrite($"scraperModule_Handler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void FinishScraper()
        {
            try
            {
                Show();
                
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (!scraperModule.IsClosed && scraperModule.IsClosing)
                        {
                            Thread.Sleep(250);
                        }
                        if (scraperModule.IsClosed) break;
                    }
                    scraperModule = null;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FinishScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public async Task<bool> ConnectH()
        {
            try
            {
                bool res = false;
                if (ObservableCollectionFilter is not null)
                {
                    ObservableCollectionFilter.HistoricCollectionViewSource.Source = ComplexProcessingJobHistory;
                    if (complexfrm is not null && complexfrm.IsLoaded)
                    {
                        complexfrm.lstSchedules.ItemsSource = ObservableCollectionFilter.HistoricCollectionViewSource.View;
                        res = true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public async Task<bool> ConnectC()
        {
            try
            {
                if (ObservableCollectionFilter is not null)
                {
                    ObservableCollectionFilter.CurrentCollectionViewSource.Source = ComplexProcessingJobList;
                    if (complexfrm is not null && complexfrm.IsLoaded)
                    {
                        complexfrm.lstSchedules.ItemsSource = ObservableCollectionFilter.CurrentCollectionViewSource.View;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public void ConnnectLists(int Id = 0)
        {
            try
            {
                switch (Id)
                {
                    case 0:
                        {
                            ConnectC().ConfigureAwait(false);
                            break;
                        }
                    case 1:
                        {
                            ConnectH().ConfigureAwait(false);
                            break;
                        }
                    case 2:
                        {
                            ConnectI().ConfigureAwait(false);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public bool IsFileActive(string filename, int threadid)
        {
            lock (lookuplocked)
            {
                try
                {
                    bool found = false;
                    for (int i = 0; i < FilesInProcess.Length; i++)
                    {
                        if (i != threadid)
                        {
                            if (FilesInProcess[i] == filename)
                            {
                                found = true;
                            }
                        }
                    }
                    if (!found)
                    {
                        FilesInProcess[threadid] = filename;
                    }
                    return found;
                }
                catch (Exception ex)
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
        }
        private void SetupTicker()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => SetupTicker());
                    return;
                }


                FileQueChecker.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private readonly IHttpClientFactory httpClientFactory;
        public async Task ParseAndDeleteLogs()
        {
            try
            {

                string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                if (AppPath.ToLower().Contains("debug"))
                {
                    AppPath = defaultprogramlocation;
                }
                List<string> LogFiles = Directory.EnumerateFiles(AppPath, "*-log.txt", SearchOption.AllDirectories).ToList<string>();
                foreach (string sPath in LogFiles)
                {
                    if (sPath.Contains("-log"))
                    {
                        string sp = Path.GetFileName(sPath);
                        int index = sp.IndexOf("-log");
                        string LogName = sp.Substring(0, index);
                        LogName = LogName.Replace("_", "/").Trim() + " 00:00:00";
                        DateTime rt = DateTime.Now.AddYears(-100);
                        string fmt = "dd/MM/yyyy hh:mm:ss";
                        //if (DateTime.TryParseExact(LogName, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out rt))
                        //{
                        //    TimeSpan AS = DateTime.Now - rt;
                        //    if (AS.TotalDays > 4)
                        //    {
                        //        File.Delete(sPath);
                        //
                        //
                        //    }
                        //}
                        //else
                        //{/
                        //    string s = "Invalid Date";
                        // }

                    }
                }
                DateTime LogDate = DateTime.Now.AddYears(-100);
                List<string> LogFiles2 = Directory.EnumerateFiles(AppPath, "*-log.txt", SearchOption.AllDirectories).ToList<string>();
                foreach (string sPath in LogFiles2)
                {
                    string sp = Path.GetFileName(sPath);
                    int index = sp.IndexOf("-log");
                    string LogName = sp.Substring(0, index);
                    LogName = LogName.Replace("_", "/").Trim() + " 00:00:00";
                    DateTime rt = DateTime.Now.AddYears(-100);
                    string fmt = "dd/MM/yyyy hh:mm:ss";
                    if (DateTime.TryParseExact(LogName, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out rt))
                    {
                        if (rt > LogDate)
                        {
                            CurrentLogFile = sPath;
                            LogDate = rt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        public string GetDesinationFromLog(string Source)
        {
            try
            {
                string res = "";

                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select Destination from ProcessingLog where Source = @P0";
                    using (var command = new FbCommand(sql.ToUpper(), connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@p0", Source);
                        object result = command.ExecuteScalar();
                        if (result is string idxx)
                        {
                            return idxx;
                        }
                        else return "";
                    }
                    connection.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        public int InsertIntoProcessingLog(string Source, string Destination)
        {
            try
            {
                int res = -1;

                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "insert into ProcessingLog(Source,Destination) values(@P0,@p1) returning Id";
                    using (var command = new FbCommand(sql.ToUpper(), connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@p0", Source);
                        command.Parameters.AddWithValue("@p1", Destination);
                        object result = command.ExecuteScalar();
                        if (result is int idxx)
                        {
                            return idxx;
                        }
                        else return -1;
                    }
                    connection.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        public int GetIdFromProcessingLog(string Source, string Destination)
        {
            try
            {
                int res = -1;

                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select Id From ProcessingLog where Source = @P0 and Destination = @p1";
                    using (var command = new FbCommand(sql.ToUpper(), connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@p0", Source);
                        command.Parameters.AddWithValue("@p1", Destination);
                        object result = command.ExecuteScalar();
                        if (result is int idxx)
                        {
                            return idxx;
                        }
                        else return -1;
                    }
                    connection.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        public int ModifyRecordInDb(int idx, string SourceDir, string DestinationFName, TimeSpan StartPos, TimeSpan Duration,
                                     bool Is720p, bool IsShorts, bool IsCreateShorts, bool IsEncodeTrim, bool IsCutTrim, bool IsMonitoredSource,
                                     bool IsPersisentJob, bool ismuxed, string muxdata)
        {
            try
            {
                int res = -1;

                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "update AutoInsert set srcdir=@p1, StartPos=@p2, Duration=@p3, b720p=@p4, bShorts=@p5, bCreateShorts=@p6," +
                            "bEncodeTrim=@p7, bCutTrim=@p8, bMonitoredSource=@p90, bPersistentJob=@p10, RunId=@p11, IsMuxed = @p12, MuxData = @p13) " +
                            "where Id = @p99 returning Id";
                    using (var command = new FbCommand(sql, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@p0", SourceDir);
                        command.Parameters.AddWithValue("@p2", StartPos.TotalMilliseconds);
                        command.Parameters.AddWithValue("@p3", Duration.TotalMilliseconds);
                        command.Parameters.AddWithValue("@p4", Is720p);
                        command.Parameters.AddWithValue("@p5", IsShorts);
                        command.Parameters.AddWithValue("@p6", IsCreateShorts);
                        command.Parameters.AddWithValue("@p7", IsEncodeTrim);
                        command.Parameters.AddWithValue("@p8", IsCutTrim);
                        command.Parameters.AddWithValue("@p9", IsMonitoredSource);
                        command.Parameters.AddWithValue("@p10", IsPersisentJob);
                        command.Parameters.AddWithValue("@p11", CurrentDbId);
                        command.Parameters.AddWithValue("@p12", ismuxed);
                        command.Parameters.AddWithValue("@p13", muxdata);
                        command.Parameters.AddWithValue("@p99", idx);
                        object result = command.ExecuteScalar();
                        if (result is int idxx)
                        {
                            return idxx;
                        }
                        else return -1;
                    }
                    connection.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }

        public int ModifyHistory(int idx, DateOnly DELETIONDATE)
        {
            try
            {
                int res = -1;

                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();

                    string sql = "update AutoInsertHistory set DELETIONDATE=@p0 where Id = @p1 returning Id";
                    using (var command = new FbCommand(sql, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@p0", DELETIONDATE);
                        command.Parameters.AddWithValue("@p1", idx);
                        object result = command.ExecuteScalar();
                        if (result is int idxx)
                        {
                            return idxx;
                        }
                        else return -1;
                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }

        public int InsertRecordIntoDb(string SourceDir, string DestinationFName, TimeSpan StartPos, TimeSpan Duration,
                                     bool Is720p, bool IsShorts, bool IsCreateShorts, bool IsEncodeTrim, bool IsCutTrim, bool IsMonitoredSource,
                                     bool IsPersisentJob, Nullable<DateTime> TwitchSchedule = null,
                                     string RTMP = "", bool IsTwitchStream = false, bool IsMuxed = false, string MuxData = "")
        {
            try
            {
                int res = -1;
                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO AUTOINSERT(SRCDIR, DESTFNAME, STARTPOS, DURATION, B720P, BSHORTS, BCREATESHORTS, BENCODETRIM, " +
                            "BCUTTRIM, BMONITOREDSOURCE, BPERSISTENTJOB, RUNID, TWITCHDATE, TWITCHTIME, RTMP, BTWITCHSTREAM, ISMUXED,MUXDATA) " +
                            "VALUES(@P0,@P1,@P2,@P3,@P4,@P5,@P6,@P7,@P8,@P9,@P10,@P11,@P12,@P13,@P14,@P15,@P16,@P17) RETURNING ID";
                    DateTime _TwitchSchedule = DateTime.Now.AddYears(-500);
                    DateOnly _TwichDate = DateOnly.FromDateTime(_TwitchSchedule);
                    TimeOnly _TwichTime = TimeOnly.FromDateTime(_TwitchSchedule);

                    if (TwitchSchedule.HasValue)
                    {
                        _TwichDate = DateOnly.FromDateTime(TwitchSchedule.Value);
                        _TwichTime = TimeOnly.FromDateTime((DateTime)TwitchSchedule.Value);
                    }

                    using (var command = new FbCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@P0", SourceDir);
                        command.Parameters.AddWithValue("@P1", DestinationFName);
                        command.Parameters.AddWithValue("@P2", StartPos.TotalMilliseconds);
                        command.Parameters.AddWithValue("@P3", Duration.TotalMilliseconds);
                        command.Parameters.AddWithValue("@P4", Is720p);
                        command.Parameters.AddWithValue("@P5", IsShorts);
                        command.Parameters.AddWithValue("@P6", IsCreateShorts);
                        command.Parameters.AddWithValue("@P7", IsEncodeTrim);
                        command.Parameters.AddWithValue("@P8", IsCutTrim);
                        command.Parameters.AddWithValue("@P9", IsMonitoredSource);
                        command.Parameters.AddWithValue("@P10", IsPersisentJob);
                        command.Parameters.AddWithValue("@P11", CurrentDbId);
                        command.Parameters.AddWithValue("@P12", _TwichDate);
                        command.Parameters.AddWithValue("@P13", _TwichTime);
                        command.Parameters.AddWithValue("@P14", RTMP);
                        command.Parameters.AddWithValue("@P15", IsTwitchStream);
                        command.Parameters.AddWithValue("@P16", IsMuxed);
                        command.Parameters.AddWithValue("@P17", MuxData);

                        object result = command.ExecuteScalar();
                        if (result is int idxx)
                        {
                            return idxx;
                        }
                        else return -1;
                    }
                    connection.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        public void DbInit()
        {
            try
            {
                string exepath = GetExePath() + "\\VideoGui.fdb";
                connectionString = GetConectionString();

                if (!File.Exists(exepath))
                {
                    FbConnection.CreateDatabase(connectionString);
                }
                string Id = "id integer generated by default as identity primary key";
                string sqlstring = $"create table AutoInsert({Id},srcdir varchar(500), destfname varchar(500),StartPos BIGINT,Duration BIGINT,b720p SMALLINT," +
                    "bShorts SMALLINT,bEncodeTrim SMALLINT,bCutTrim SMALLINT,bMonitoredSource SMALLINT,bPersistentJob SMALLINT, RUNID INTEGER,"+
                     "ISMUXED SMALLINT, MUXDATA VARCHAR(256)); ";

                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("AutoInsert", "ISMUXED","SMALLINT", 0);
                connectionString.AddFieldToTable("AutoInsert", "MUXDATA", "VARCHAR(256)", "");


                sqlstring = $"create table AutoInsertHistory({Id},srcdir varchar(500), destfname varchar(500),StartPos BIGINT,Duration BIGINT,b720p SMALLINT," +
                    "bShorts SMALLINT,bEncodeTrim SMALLINT,bCutTrim SMALLINT,bMonitoredSource SMALLINT,bPersistentJob SMALLINT, RUNID INTEGER,"+
                    "ISMUXED SMALLINT, MUXDATA VARCHAR(256)); ";

                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("AutoInsertHistory", "ISMUXED", "SMALLINT", 0);
                connectionString.AddFieldToTable("AutoInsertHistory", "MUXDATA", "VARCHAR(256)", "");

                // autoinert modify & autoinsert history modify

                List<string> Fields = new List<string>() { "BCREATESHORTS", "BTWITCHSTREAM", "TWITCHDATE", "TWITCHTIME", "RTMP" };
                List<string> FieldType = new List<string>() { "SMALLINT", "SMALLINT", "DATE", "TIME", "VARCHAR(250)" };
                for (int i = 0; i < Fields.Count; i++)
                {

                    sqlstring = $"SELECT RDB$FIELD_NAME AS FIELD_NAME FROM RDB$RELATION_FIELDS WHERE RDB$RELATION_NAME='AUTOINSERT'AND RDB$FIELD_NAME = '{Fields[i]}';";
                    var tablename = sqlstring.RunExecuteScalar(connectionString);
                    if (tablename is null)
                    {
                        sqlstring = $"ALTER TABLE AUTOINSERT ADD {Fields[i]} {FieldType[i]};";
                        sqlstring.RunExecuteScalar(connectionString);
                    }

                    sqlstring = $"SELECT RDB$FIELD_NAME AS FIELD_NAME FROM RDB$RELATION_FIELDS WHERE RDB$RELATION_NAME='AUTOINSERTHISTORY'AND RDB$FIELD_NAME = '{Fields[i]}';";
                    var xtablename = sqlstring.RunExecuteScalar(connectionString);
                    if (xtablename is null)
                    {
                        sqlstring = $"ALTER TABLE AUTOINSERTHISTORY ADD {Fields[i]} {FieldType[i]};";
                        sqlstring.RunExecuteScalar(connectionString);
                    }
                }


                sqlstring = $"create table MonitoredDeletion({Id},DestinationFile varchar(500))";
                connectionString.CreateTableIfNotExists(sqlstring);

                sqlstring = $"create table PLANINGQUES({Id},SOURCE varchar(500),SourceDir varchar(500))";
                connectionString.CreateTableIfNotExists(sqlstring);

                sqlstring = $"create table PLANINGCUTS({Id}, PLANNINGQUEID INTEGER,BIGINT START ,BIGINT END , " +
                    "SMALLINT CUTNO , FILENAME VARCHAR(500))";
                connectionString.CreateTableIfNotExists(sqlstring);


                sqlstring = $"create table ProcessingLog({Id},Source varchar(500),Destination varchar(500),InProcess SmallInt);";
                connectionString.CreateTableIfNotExists(sqlstring);

                sqlstring = $"create table AutoEdits({Id},Source varchar(500),Destination varchar(500),Threshhold varchar(255)," +
                    "Segment varchar(255),Target varchar(255));".ToUpper();
                connectionString.CreateTableIfNotExists(sqlstring);

                sqlstring = $"delete from ProcessingLog where InProcess = 1;";
                sqlstring.ExecuteNonQuery(connectionString);

                connectionString.CreateTableIfNotExists($"CREATE TABLE RUNNINGID({Id}, ACTIVE SMALLINT)");
                sqlstring = $"INSERT INTO RUNNINGID(ACTIVE) VALUES(0) RETURNING ID;";
                int idx = sqlstring.RunExecuteScalar(connectionString, -1);
                CurrentDbId = (idx != -1) ? idx : CurrentDbId;
                LoadFromDb();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DeleteFromAutoInsertTable(int filename)
        {
            try
            {
                string sql = $"DELETE FROM AUTOINSERT WHERE ID = {filename};";
                int idx = sql.RunExecuteScalar(connectionString, -1);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void DeleteFromDeletionTable(string filename)
        {
            try
            {
                if (!DoesDeletionFileExist(filename))
                {
                    string sql = $"delete fromm MonitoredDeletion where DestinationFile = {filename};";
                    int idx = connectionString.RunExecuteScalar(sql, -1);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void InsertIntoDeletionTable(string filename)
        {
            try
            {
                if (!DoesDeletionFileExist(filename))
                {
                    string sql = $"insert into MonitoredDeletion(DestinationFile) values({filename});";
                    int idx = connectionString.RunExecuteScalar(sql.ToUpper(), -1);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public bool DoesDeletionFileExist(string filename)
        {
            try
            {
                string sqlstring = $"select Id from MonitoredDeletion where DestinationFile = {filename}";
                int idx = connectionString.RunExecuteScalar(sqlstring, -1);
                return (idx != -1) ? true : false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public bool DoesAutoInsertExists(string filename)
        {
            try
            {
                string sqlstring = $"select Id from AutoInsert where destfname = {filename}";
                int idx = connectionString.RunExecuteScalar(sqlstring, -1);
                return (idx != -1) ? true : false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }


        public void CloseDialogComplexEditor()
        {

            try
            {
                if (complexfrm != null)
                {
                    Show();
                    complexfrm = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public void LoadFromDb()
        {
            try
            {
                connectionString.ExecuteReader("SELECT * FROM AUTOINSERT", OnReadAutoInsert);
                connectionString.ExecuteReader("SELECT * FROM AUTOINSERTHISTORY Order By DELETIONDATE desc", OnReadAutoInsertHistory);
                connectionString.ExecuteReader("SELECT * FROM PLANINGQUES", OnReadPlanningQues);
                connectionString.ExecuteReader("SELECT * FROM PLANINGCUTS", OnReadPlanningCuts);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void OnReadPlanningCuts(FbDataReader reader)
        {
            PlanningCutsList.Add(new PlanningCuts(reader));
        }

        private void OnReadPlanningQues(FbDataReader reader)
        {
            PlanningQueList.Add(new PlanningQue(reader));
        }

        private void OnReadAutoInsertHistory(FbDataReader reader)
        {
            ComplexProcessingJobHistory.Add(new ComplexJobHistory(reader));
        }

        private void OnReadAutoInsert(FbDataReader reader)
        {
            try
            {
                ComplexProcessingJobList.Add(new ComplexJobList(reader));

                var InMemoryJob = new JobListDetails(reader, ProcessingJobs.Count + 1);
                if (InMemoryJob.IsMulti && InMemoryJob.GetCutList().Count > 0)
                {

                    if (InMemoryJob.GetCutList().Count > 0)
                    {
                        ProcessingJobs.Add(InMemoryJob);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }


        public void AddRecord(bool IsElapsed, bool Is720P, bool IsShorts, bool IsCreateShorts,
        bool IsTrimEncode, bool IsCutEncode, bool IsDeleteMonitored, bool IsPersistantSource,
        bool IsAdobe, string textstart, string textduration, string sourcedirectory, string destFilename,
           Nullable<DateTime> twitchschedule = null, string RTMP = "", bool IsTwitchStream = false,
           bool ismuxed = false, string muxdata = "")
        {
            try
            {
                bool found = false;
                if (!IsTwitchStream && (!twitchschedule.HasValue || RTMP == "") && (!destFilename.EndsWith(".mp4")))
                {
                    destFilename += ".mp4";
                }
                foreach (ComplexJobList jp in ComplexProcessingJobList.
                    Where(record => $"{record.DestinationDirectory}\\{record.DestinationFile}" == destFilename))
                {
                    found = true;
                    jp.Is720p = Is720P;
                    jp.IsShorts = IsShorts;
                    jp.IsMuxed = ismuxed;
                    jp.MuxData = muxdata;
                    jp.IsCreateShorts = IsCreateShorts;
                    jp.IsPersistentJob = IsPersistantSource;
                    jp.SourceDirectory = sourcedirectory;
                    jp.IsDeleteMonitoredSource = IsDeleteMonitored;
                    jp.Duration = textduration.FromStrToTimeSpan();
                    jp.Start = textstart.FromStrToTimeSpan();
                    jp.IsCutTrim = IsCutEncode;
                    jp.IsEncodeTrim = IsTrimEncode;
                    jp.RTMP = RTMP;
                    if (!ismuxed)
                    {
                        jp.IsTwitchStream = IsTwitchStream;
                        jp.TwitchSchedule = twitchschedule.Value;
                        if (twitchschedule.HasValue && RTMP != "")
                        {
                            jp.IsTwitchStream = true;
                        }
                    }
                    //DbEdit
                    if (jp.Id != "")
                    {
                        int iddx = -1;
                        iddx = ModifyRecordInDb(jp.Id.ToInt(), jp.SourceDirectory, jp.DestinationDirectory, jp.Start, jp.Duration,
                            jp.Is720p, jp.IsShorts, jp.IsCreateShorts, jp.IsEncodeTrim, jp.IsCutTrim, jp.IsDeleteMonitoredSource, 
                            jp.IsPersistentJob, jp.IsMuxed, jp.MuxData);
                        jp.Id = (iddx != -1) ? iddx.ToString() : jp.Id;
                        string Title = Path.GetFileName(destFilename);
                        int ScriptType = 0;
                        if (Is720P) ScriptType = 2;
                        if (IsShorts) ScriptType = 0;
                        if (IsCreateShorts) ScriptType = 4;
                        if (IsTwitchStream) ScriptType = 5;
                        if (ismuxed) ScriptType = 6;
                        TimeSpan Final = TimeSpan.Zero;
                        if (!IsElapsed)
                        {
                            TimeSpan st = textstart.FromStrToTimeSpan();
                            TimeSpan Dur = textduration.FromStrToTimeSpan();
                            if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                            {
                                Dur -= st;
                                Final = Dur;
                            }
                            else Final = textduration.FromStrToTimeSpan();
                        }
                        else Final = textduration.FromStrToTimeSpan();
                        if (IsCutEncode) ScriptType = 1;
                        string CutFrames = ((textstart != "") || (textduration != "")) ? $"|{textstart}|{Final.ToFFmpeg()}|time" : "";
                        string ScriptFile = $"true|{destFilename}|{sourcedirectory}|*.mp4{CutFrames}";
                        JobListDetails InMemoryJob = new JobListDetails(Title, SourceIndex++, iddx, ScriptFile, ScriptType, false, true,
                            IsPersistantSource, IsAdobe, IsShorts, IsCreateShorts, "", ismuxed, muxdata);
                        if (InMemoryJob.GetCutList().Count > 0 || RTMP != "")
                        {
                            if (RTMP != "" && twitchschedule.HasValue && IsTwitchStream)
                            {
                                InMemoryJob.RTMP = RTMP;
                                InMemoryJob.IsTwitchStream = IsTwitchStream;
                                InMemoryJob.twitchschedule = twitchschedule.Value;
                            }
                            ProcessingJobs.Add(InMemoryJob);
                        }
                    }
                    break;
                }

                if (!found)
                {
                    bool Insert = false;
                    TimeSpan Final = TimeSpan.Zero;
                    if (!IsElapsed)
                    {
                        TimeSpan st = textstart.FromStrToTimeSpan();
                        TimeSpan Dur = textduration.FromStrToTimeSpan();
                        if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                        {
                            Dur -= st;
                            Final = Dur;
                        }
                        else Final = textduration.FromStrToTimeSpan();
                    }
                    else Final = textduration.FromStrToTimeSpan();
                    int idx = -1;
                    idx = InsertRecordIntoDb(sourcedirectory, destFilename, textstart.FromStrToTimeSpan(), Final,
                        Is720P, IsShorts, IsCreateShorts, IsTrimEncode, IsCutEncode,
                        IsDeleteMonitored, IsPersistantSource, twitchschedule, RTMP, IsTwitchStream, ismuxed, muxdata);
                    if (idx != -1)
                    {

                        ComplexJobList cjl = new ComplexJobList(sourcedirectory, destFilename,
                            textstart.FromStrToTimeSpan(), Final, Is720P, IsShorts, IsCreateShorts,
                            IsTrimEncode, IsCutEncode, IsDeleteMonitored, IsPersistantSource, idx, ismuxed, muxdata);
                        if (RTMP != "" && twitchschedule.HasValue)
                        {
                            cjl.RTMP = RTMP;
                            cjl.IsTwitchStream = true;
                            cjl.TwitchSchedule = twitchschedule.Value;
                        }
                        cjl.IsTwitchStream = IsTwitchStream;
                        ComplexProcessingJobList.Add(cjl);
                        string Title = Path.GetFileName(destFilename);
                        int ScriptType = 0;
                        if (Is720P) ScriptType = 2;
                        if (IsShorts) ScriptType = 0;
                        if (IsCreateShorts) ScriptType = 4;
                        if (IsCutEncode) ScriptType = 1;
                        if (IsTrimEncode) ScriptType = 3;
                        if (IsTwitchStream) ScriptType = 5;
                        string CutFrames = ((textstart != "") && (textduration != "")) ? $"|{textstart}|{Final.ToFFmpeg()}|time" : "";
                        string ScriptFile = $"true|{destFilename}|{sourcedirectory}|*.mp4{CutFrames}";
                        JobListDetails InMemoryJob = new JobListDetails(Title, SourceIndex++, idx, ScriptFile, ScriptType,
                            false, true, IsPersistantSource, IsAdobe, IsShorts, IsCreateShorts, "", ismuxed, muxdata);
                        if (InMemoryJob.GetCutList().Count > 0 || RTMP != "" || ismuxed)
                        {
                            if (RTMP != "" && twitchschedule.HasValue)
                            {
                                InMemoryJob.RTMP = RTMP;
                                InMemoryJob.IsTwitchStream = IsTwitchStream;
                                InMemoryJob.twitchschedule = twitchschedule.Value;
                            }
                            ProcessingJobs.Add(InMemoryJob);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public string GetConectionString()
        {
            try
            {
                string exepath = GetExePath() + "\\VideoGui.fdb";

                var connectionString = new FbConnectionStringBuilder
                {
                    Database = exepath,
                    DataSource = "localhost",
                    ServerType = FbServerType.Embedded,
                    UserID = "SYSDBA",
                    Dialect = 3,
                    ConnectionLifeTime = 15,
                    Port = 3050,
                    Pooling = true,
                    MaxPoolSize = 50,
                    PacketSize = 8192,
                    Password = "masterkey",
                    Role = "",
                    ClientLibrary = GetExePath() + "\\fbclient.dll"
                }.ToString();
                return connectionString;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public void DeleteRecord(int RecordId, bool CheckLocked = false, bool KillRecord = false)
        {
            try
            {
                foreach (var vp in from vp in ComplexProcessingJobList
                                   where vp.Id.ToInt() == RecordId
                                   select vp)
                {
                    if (vp.IsLocked == false || CheckLocked)
                    {
                        ComplexProcessingJobList.Remove(vp);
                        for (int i = 0; i < ProcessingJobs.Count; i++)
                        {
                            if (ProcessingJobs[i].DeletionFileHandle == vp.Id.ToInt())
                            {
                                ProcessingJobs[i].IsSkipped = true;
                                ProcessingJobs[i].Complete = true;
                                ProcessingJobs[i].Progress = 100;
                                if (KillRecord)
                                {
                                    string ID = ProcessingJobs[i].Handle;
                                    if (ID != "")
                                    {
                                        var rpt = Process.GetProcessById(ID.ToInt());
                                        rpt.Kill();
                                    }
                                    ProcessingJobs.RemoveAt(i);
                                }
                                break;
                            }
                        }
                        DeleteFromAutoInsertTable(vp.Id.ToInt());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void LocalSetFilterAge(int a, int b)
        {
            try
            {
                ObservableCollectionFilter?.SetHistoricAges(a, b);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void LocalSetFilterString(string Filter, FilterTypes FilterType, FilterClass FilterClassIs)
        {
            try
            {
                if (FilterClassIs is FilterClass.Current)
                {
                    switch (FilterType)
                    {
                        case FilterTypes.DestinationFileName:
                            {
                                ObservableCollectionFilter.SetCurrentContainsFileName(Filter);
                                break;
                            }
                        case FilterTypes.DestinationDirectory:
                            {
                                ObservableCollectionFilter.SetCurrentContainsPath(Filter);
                                break;
                            }
                        case FilterTypes.SourceDirectory:
                            {
                                ObservableCollectionFilter.SetCurrentContainsSourceDirectory(Filter);
                                break;
                            }
                    }
                }
                else
                {
                    switch (FilterType)
                    {
                        case FilterTypes.DestinationFileName:
                            {
                                ObservableCollectionFilter.SetHistoricContainsFileName(Filter);
                                break;
                            }
                        case FilterTypes.DestinationDirectory:
                            {
                                ObservableCollectionFilter.SetHistoricContainsPath(Filter);
                                break;
                            }
                        case FilterTypes.SourceDirectory:
                            {
                                ObservableCollectionFilter.SetHistoricContainsSourceDirectory(Filter);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public (int, int) GetFilterAges()
        {
            try
            {
                if (ObservableCollectionFilter is not null)
                {
                    return ObservableCollectionFilter.GetFilterAges();
                }
                else return (-1, -1);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return (-1, -1);
            }
        }

        public string GetFilterString(FilterTypes Filter, FilterClass Active)
        {
            try
            {
                string res = "";
                if (ObservableCollectionFilter is not null)
                {
                    return ObservableCollectionFilter.GetFilterString(Filter, Active);
                }
                return "";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public void DoComplexEditor()
        {
            try
            {
                if (complexfrm == null)
                {
                    complexfrm = new ComplexSchedular(AddRecord, DeleteRecord, CloseDialogComplexEditor,
                        ConnnectLists, LocalSetFilterAge, LocalSetFilterString, GetFilterAges, GetFilterString);
                    Hide();
                    complexfrm.ShowDialog();
                    Show();
                    complexfrm = null;

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public bool IsClosed = false, IsClosing = false;
        public MainWindow(OnFinish DoOnFinish)
        {
            try
            {
                Closing += (s, e) => 
                { 
                    IsClosing = true;
                    DoOnFinish?.Invoke();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke();
                };
                _syncContext = SynchronizationContext.Current;
                ThreadStatsHandler = new(StatsHandledForThreads);
                ThreadStatsHandlerBool = new(StatsHandledForThreadsBool);
                ThreadUpdateSpeed = new(UpdateSpeeds);
                ThreadUpdateProgress = new(UpdateProgress);
                ThreadUpdateTime = new(UpdateTime);
                ThreadStatsHandlerXtra = new _StatsHandlerExtra(UpdateFileInfo);
                ThreadStartTimeSet = new _StatsHandlerDateTimeSetter(UpdateStartTime);
                ThreadStartTimeGet = new _StatsHandlerDateTimeGetter(GetStartTime);
                Stats_Handler._UpdateETA += new StatsHandler.UpdateETA(EtaUpdate);
                Stats_Handler.TotalDuration += new StatsHandler.UpdateTwoStrings(TotalDuration);
                Stats_Handler.ProgressValues += new StatsHandler.UpdatePercents(percents);
                Stats_Handler.TotalTime += new StatsHandler.UpdateSIngleString(totalTIme);
                Stats_Handler.UpdateTotalSpeeds += new StatsHandler.UpdateSpeeds(SpeedUpdate);
                //  ProcessingJobs = new ConcurrentQueue<JobListDetails>();
                Content += " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                OnIsFileInUse = new IsFileInUse(IsFileActive);
                InitializeComponent();

                MainWindowX.KeyDown += Window_KeyDown_EventHandler;
                Loadsettings();
                DbInit();

                string UploadPath = @"d:\shorts\VLINE Southern Cross To Swanhill 270624";
                var Files = Directory.EnumerateFiles(UploadPath, "*.mp4", SearchOption.AllDirectories).Take(5).ToList();
                foreach ( var rfile in Files )
                {
                    //File.Delete(rfile);
                }

                this.httpClientFactory = httpClientFactory;
                Task.Run(() => { KillOrphanProcess(); });
                Task.Run(() => { KillOrphanProcess("avidemux_cli.exe"); });

                var isok = SanityCheck().GetAwaiter().GetResult();
                if (!isok)
                {
                    string err = "ffmpeg or ffprobe Issue , Deleting";
                    err.WriteLog();
                    string AppPath = GetExePath();
                    DeleteIfExists(AppPath + "\\ffprobe.exe");
                    DeleteIfExists(AppPath + "\\ffmpeg.exe");
                }
                lstBoxJobs.ItemsSource = ProcessingJobs;
                SetupHandlers().ConfigureAwait(false);
                //Task.Run(() => Loadsettings());
                ParseAndDeleteLogs().ConfigureAwait(false);
                CheckForGraphicsCard();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                totaltasks = key.GetValueInt("maxthreads", 10);
                total1080ptasks = key.GetValueInt("max1080pthreads", 1);
                total4kTasks = key.GetValueInt("max4Kthreads", 3);// 1 for laptop. 2 for desktop
                UseFisheyeRemoval = key.GetValueBool("Fisheye", false);
                LRF = key.GetValueFloat("FisheyeRemoval_LRF", 0.5f);
                LLC = key.GetValueFloat("FisheyeRemoval_LLC", 0.5f);
                RRF = key.GetValueFloat("FisheyeRemoval_RRF", -0.335f);
                RLC = key.GetValueFloat("FisheyeRemoval_RLC", 0.097f);
                txtMaxShorts.Text = key.GetValueStr("MaxShorts", "80");
                key?.Close();
                FileQueChecker = new System.Windows.Forms.Timer();
                FileQueChecker.Tick += new EventHandler(FileQueChecker_Tick);
                FileQueChecker.Interval = (int)new TimeSpan(0, 0, 1).TotalMilliseconds;
                Task.Run(async () => SetupThreadProcessorAsync());
                Thread.Sleep(100);
                SetupTicker();
                ObservableCollectionFilter = new ObservableCollectionFilters();
                FormResizerEvent = new System.Windows.Forms.Timer();
                FormResizerEvent.Tick += new EventHandler(FormResizerEvent_Tick);
                FormResizerEvent.Interval = (int)new TimeSpan(0, 0, 2).TotalSeconds;
                FormResizerEvent.Start();
                lstBoxJobs.AllowDrop = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_KeyDown_EventHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                ShiftActiveWindowClosing = (e.Key == Key.LeftShift || e.Key == Key.RightShift);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private async Task<bool> SanityCheck()
        {
            try
            {
                StringBuilder StdErr = new StringBuilder();
                StringBuilder StdOut = new StringBuilder();
                string appPath = GetExePath();
                await Cli.Wrap(appPath + "\\ffprobe.exe").
                     WithArguments("-version").WithWorkingDirectory(appPath).
                      WithStandardErrorPipe(PipeTarget.ToStringBuilder(StdErr)).
                      WithStandardOutputPipe(PipeTarget.ToStringBuilder(StdOut)).
                      ExecuteAsync().ConfigureAwait(false);
                StdErr.Clear();
                StdOut.Clear();
                StringBuilder StdErr2 = new StringBuilder();
                StringBuilder StdOut2 = new StringBuilder();
                await Cli.Wrap(appPath + "\\ffmpeg.exe").
                    WithArguments("-version").WithWorkingDirectory(appPath).
                    WithStandardErrorPipe(PipeTarget.ToStringBuilder(StdErr2)).
                    WithStandardOutputPipe(PipeTarget.ToStringBuilder(StdOut2)).
                    ExecuteAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        private void FormResizerEvent_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (MainWindowX.IsLoaded)
                {
                    FormResizerEvent.Stop();
                    int deviceID = 0;
                    double WW = -1, HH = -1;
                    string selectedcard = "", card = "", WindowState = "";
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    if (key != null)
                    {
                        HH = key.GetValueDouble("Height", -1);
                        WW = key.GetValueDouble("Width", -1);
                        int ws = key.GetValueInt("WindowState", 0);

                        if (ws != -1)
                        {
                            switch (ws)
                            {
                                case 0:
                                    {
                                        MainWindowX.WindowState = System.Windows.WindowState.Normal;
                                        break;
                                    }
                                case 1:
                                    {
                                        MainWindowX.WindowState = System.Windows.WindowState.Maximized;
                                        break;
                                    }
                                case 2:
                                    {
                                        MainWindowX.WindowState = System.Windows.WindowState.Normal;
                                        break;
                                    }

                            }
                        }
                        selectedcard = key.GetValueStr("selectedcard");
                        MainWindowX.Height = HH != -1 ? HH : MainWindowX.Height;
                        MainWindowX.Width = WW != -1 ? WW : MainWindowX.Width;

                        lstboxresize();
                        key.Close();
                        if (selectedcard != "")
                        {
                            Video = selectedcard;
                            lbAccelHW.AutoSizeLabel(Video);
                            ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");
                            foreach (ManagementObject mo in searcher.Get())
                            {
                                PropertyData description = mo.Properties["Description"];
                                PropertyData VideoProcessor = mo.Properties["VideoProcessor"];
                                if ((description != null) && (VideoProcessor.Value != null))
                                {
                                    card = description.Value.ToString();
                                    if (card == selectedcard)
                                    {
                                        LockedDeviceID = deviceID;
                                        break;
                                    }
                                    else deviceID++;
                                }
                            }
                        }
                        else selectedcard = CheckForGraphicsCard();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private async void LoadSelectForm()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => LoadSelectForm());
                    return;
                }
                // LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " Button_ClickAsync");
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                string SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);

                key?.Close();
                SelectFiles(SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void StartTicker()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => StartTicker());
                    return;
                }
                FileQueChecker.Start();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private DateTime GetStartTime(string filename)
        {
            try
            {
                DateTime result = DateTime.Now.AddYears(-100);
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        result = Stats_Handler.GetStartTime(filename);
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return DateTime.Now.AddYears(-100);
            }
        }
        private void UpdateStartTime(string filename, DateTime start)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        Stats_Handler.UpdateStartTime(filename, start);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void UpdateFileInfo(string record, string filename)
        {
            try
            {
                lock (ProbeLock)
                {
                    var Job = ProcessingJobs.Where(s => s.FileNoExt == record).FirstOrDefault();
                    if (Job != null)
                    {
                        bool found = false;
                        for (int i = 0; i < ProcessingJobs.Count; i++)
                        {
                            if (ProcessingJobs[i].FileNoExt == record)
                            {
                                Thread.Sleep(100);
                                ProcessingJobs[i].Fileinfo = filename;
                                string pr = record + "|" + ProcessingJobs[i].Fileinfo;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            bool rt = false;
                            if (rt)
                            {

                            }
                        }
                    }
                    else
                    {
                        bool t = true;
                        if (t)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void UpdateTime(int mode, string filename, TimeSpan eta)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        if (mode == 0) Stats_Handler.UpdateTime(filename, eta);
                        if (mode == 1) Stats_Handler.UpdateProcessingTime(filename, eta);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void UpdateProgress(string filename, double Progress, TimeSpan Duration, TimeSpan Total)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        Stats_Handler.UpdateProgress(filename, Progress, Duration, Total);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void UpdateSpeeds(string filename, float framess, float totalb, int framecalc, string frames1080p)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        Stats_Handler.UpdateSpeed(filename, framess, totalb, framecalc, frames1080p);
                    }

                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public bool StatsHandledForThreadsBool(int mode, string filename)
        {
            try
            {
                bool thisresult = false;
                this.Dispatcher.Invoke(() =>
                {
                    lock (StatsLock)
                    {
                        if (mode == 0) thisresult = Stats_Handler.FindFilename(filename);
                        //if (mode == 2) thisresult = KillTorrent(true, filename);
                    }
                });
                return thisresult;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        public void StatsHandledForThreads(int mode, string filename)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (StatsLock)
                    {
                        if (mode == 0) Stats_Handler.AddNewStats(filename);
                        if (mode == 1) Stats_Handler.RemoveStats(filename);
                    }
                });

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void removejob(int jobid)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => removejob(jobid));
                    return;
                }
                ProcessingJobs.RemoveAt(jobid);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public string[] GetDefaultVideoExts()
        {
            return new string[] { ".avi", ".mkv", ".mp4", ".m2ts", ".src", ".cst", ".edt" };
        }
        public async Task SetupThreadProcessorAsync()
        {
            int LineNum = 0;
            try
            {
                int threadid = 0, Index = 0;
                this.Dispatcher.Invoke(() =>
                {
                    lstBoxJobs.ItemsSource = ProcessingJobs;
                    string currentpath2 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    if (!File.Exists(currentpath2 + "\\ffmpeg.exe"))
                    {
                        string parentpath = System.IO.Directory.GetParent(currentpath2).FullName;
                        for (int i = 0; i < 3; i++)
                        {
                            parentpath = System.IO.Directory.GetParent(parentpath).FullName;
                        }
                        List<string> PathListFF = Directory.EnumerateFiles(parentpath, "ffmpeg.exe", SearchOption.AllDirectories).
                                   Where(s => s.EndsWith(".exe")).ToList<string>();
                        foreach (string sPath in PathListFF)
                        {
                            if ((sPath.Contains("ffmpeg.exe") && (sPath.Contains("win-x64"))))
                            {
                                currentpath2 = sPath;
                                break;
                            }
                        }
                    }
                });
                LineNum = 1;
                string TwitchStreamKey = string.Empty;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                DestDirectory720p = key.GetValueStr("DestDirectory720p");
                DestDirectory1080p = key.GetValueStr("DestDirectory1080p");
                DestDirectoryTwitch = key.GetValueStr("DestDirectoryTwitch");
                DestDirectory4K = key.GetValueStr("DestDirectory4k");
                TwitchStreamKey = key.GetValueStr("TwitchStreamKey", "live_1061414984_Vu5NrETzHYqB1f4bZO12dxaCOfUkxf");
                DestDirectoryAdobe4K = key.GetValueStr("DestDirectoryAdobe4k");
                DoneDirectory720p = key.GetValueStr("CompDirectory720p");
                DoneDirectory1080p = key.GetValueStr("CompDirectory1080p");
                DoneDirectory4K = key.GetValueStr("CompDirectory4K");
                DoneDirectoryAdobe4K = key.GetValueStr("CompDirectory4KAdobe");
                SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p");
                SourceDirectory720p = key.GetValueStr("SourceDirectory720p");
                SourceDirectory4K = key.GetValueStr("SourceDirectory4K");
                SourceDirectoryAdobe4K = key.GetValueStr("SourceDirectory4KAdobe");
                ErrorDirectory = key.GetValueStr("ErrorDirectory");
                totaltasks = key.GetValueInt("maxthreads", 10);
                total1080ptasks = key.GetValueInt("max1080pthreads", 1);
                total4kTasks = key.GetValueInt("max4Kthreads", 3);// 1 for laptop. 2 for desktop
                key?.Close();
                LineNum = 2;
                List<JobListDetails> NewProcessingList = new();
                List<JobListDetails> NewProcessingListTemp = new();
                DateTime Start = DateTime.Now;
                DateTime Start2 = DateTime.Now;
                string LastFile = "";
                while (true)
                {

                    int xcnt1080p = 0, xcnt4K = 0, xcnt = 0;
                    if (ProcessingJobs.Count > 0)
                    {
                        for (int xp = ProcessingJobs.Count - 1; xp >= 0; xp--)
                        {
                            if ((ProcessingJobs[xp] is not null) && (ProcessingJobs[xp].Handle == "") &&
                                (ProcessingJobs[xp].Is4K) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt4K++;
                            }
                            if ((ProcessingJobs[xp] is not null) && (ProcessingJobs[xp].Handle == "") &&
                                (ProcessingJobs[xp].IsMuxed) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt4K++;
                            }
                            if ((ProcessingJobs[xp] is not null) && (ProcessingJobs[xp].Handle == "") &&
                                (ProcessingJobs[xp].Is1080p || ProcessingJobs[xp].IsTwitchOut) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt1080p++;
                            }
                            if ((ProcessingJobs[xp] is not null) && (ProcessingJobs[xp].Handle == "") &&
                                (ProcessingJobs[xp].Is720P) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt++;
                            }
                        }
                    }
                    LineNum = 3;
                    NewProcessingList.Clear();
                    NewProcessingListTemp.Clear();
                    if (xcnt4K > 0 && ProcessingJobs.Count > 0)
                    {
                        NewProcessingListTemp.AddRange(ProcessingJobs.Where(s => (s.Is4K)).Where(x => !x.Fileinfo.IsNullStr().Contains("Allready Processed")).Where(x => x.Handle == "" || x.Handle is null && !x.Processed && !x.IsSkipped));
                        NewProcessingList.AddRange(NewProcessingListTemp);
                    }
                    LineNum = 4;
                    if (xcnt1080p > 0)
                    {
                        if (ProcessingJobs.Where(s => (s.Is1080p || s.IsTwitchOut) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(total1080ptasks).Count() > 0)
                        {
                            NewProcessingListTemp.AddRange(ProcessingJobs.Where(s => (s.Is1080p || s.IsTwitchOut) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(total1080ptasks));
                            NewProcessingList.AddRange(NewProcessingListTemp);
                        }
                    }
                    LineNum = 5;
                    NewProcessingListTemp.Clear();
                    if (xcnt > 0)
                    {
                        if (ProcessingJobs.Where(s => (s.Is720P) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(totaltasks).Count() > 0)
                        {
                            NewProcessingListTemp.AddRange(ProcessingJobs.Where(s => (s.Is720P) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(totaltasks));
                            NewProcessingList.AddRange(NewProcessingListTemp);
                        }
                    }

                    LineNum = 6;
                    foreach (JobListDetails Jobr in NewProcessingList)
                    {
                        if (ProcessingCancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        var Job = ProcessingJobs.Where(s => s.FileNoExt == Jobr.FileNoExt).FirstOrDefault();
                        if (Job != null)
                        {
                            bool found = false;
                            Process[] psa = Process.GetProcessesByName("ffmpeg");
                            List<string> ProcessIDs = (psa.Select(pid => pid.Id.ToString())).ToList();
                            if (Job.Handle.ContainsAny(ProcessIDs))
                            {
                                Thread.Sleep(250);
                                continue;
                            }
                            LineNum = 7;

                            while (Process.GetProcessesByName("ffmpeg").Count() > 0)
                            {
                                //CountFFMPEGs(NewProcessingList);
                                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                int t720p = key2.GetValueInt("maxthreads", 5);
                                int t1080p = key2.GetValueInt("max1080pthreads", 2);
                                totaltasks = key2.GetValueInt("maxthreads", 10);
                                total1080ptasks = key2.GetValueInt("max1080pthreads", 1);
                                total4kTasks = key2.GetValueInt("max4Kthreads", 3);// 1 for laptop. 2 for desktop
                                key2?.Close();
                                t720p = totaltasks;
                                totaltasks = (_4KFiles.Count > 0) ? 4 : t720p;
                                total1080ptasks = (_4KFiles.Count > 0) ? 1 : t1080p;
                                LineNum = 8;
                                foreach (var j in ProcessingJobs.Where(j => _4KFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _4KFiles.Remove(j.SourceFile);
                                }
                                foreach (var j in ProcessingJobs.Where(j => _1080PFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _1080PFiles.Remove(j.SourceFile);
                                }
                                foreach (var j in ProcessingJobs.Where(j => _720PFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _720PFiles.Remove(j.SourceFile);
                                }
                                //(Stats_Handler.count720p, Stats_Handler.count1080p, Stats_Handler.count4k)
                                if (((Job.Is1080p || Job.IsTwitchOut) && (_1080PFiles.Count >= total1080ptasks)) ||
                                    ((Job.Is4K||Job.IsMuxed) && (_4KFiles.Count >= total4kTasks)) ||
                                    ((!Job.Is720P) && (_720PFiles.Count >= totaltasks)))
                                {
                                    Thread.Sleep(200);
                                    continue;
                                }
                                if ((Job.Is1080p || Job.IsTwitchOut) && (_1080PFiles.Count < total1080ptasks))
                                {
                                    break;
                                }
                                if (Job.IsTwitchActive)
                                {
                                    if (DateTime.Now >= Job.twitchschedule && !Job.InProcess && !Job.Processed)
                                    {
                                        break;
                                    }
                                    else continue;
                                }
                                if ((Job.Is4K||Job.IsMuxed) && (_4KFiles.Count < total4kTasks))
                                {
                                    break;
                                }
                                if ((Job.Is720P) && (_720PFiles.Count < totaltasks))
                                {
                                    break;
                                }
                                Thread.Sleep(150);
                            }
                            LineNum = 10;

                            if (Job.IsMuxed)
                            {
                                Job.Title = Path.GetFileNameWithoutExtension(Job.MultiSourceDir);
                                Job.SourcePath = Path.GetDirectoryName(Job.MultiSourceDir);
                            }

                            while (true && Job.IsTwitchActive)
                            {

                                if (DateTime.Now >= Job.twitchschedule && !Job.InProcess && !Job.Processed)
                                {
                                    string sql = "DELETE FROM AUTOINSERT WHERE BTWITCHSTREAM = 1 AND SRCDIR = @P1";
                                    string SourceDir = Job.MultiSourceDir;
                                    using (var connection = new FbConnection(connectionString))
                                    {
                                        connection.Open();
                                        using (var command = new FbCommand(sql.ToUpper(), connection))
                                        {
                                            command.Parameters.Clear();
                                            command.Parameters.AddWithValue("@P1", SourceDir);
                                            object result = command.ExecuteScalar();
                                            if (result is string idxx)
                                            {

                                            }
                                        }
                                        connection.Close();
                                    }
                                    break;
                                }
                                else continue;
                            }
                            if ((!Job.IsTwitchStream && Job.twitchschedule.HasValue) && (Process.GetProcessesByName("ffmpeg").Count() == 0) && (NewProcessingList.Count > 1))
                            {
                                while (true && NewProcessingList.Count > 1)
                                {
                                    if (((Job.Is1080p || Job.IsTwitchOut) && (_1080PFiles.Count >= total1080ptasks)) ||
                                       ((Job.Is4K || Job.IsMuxed) && (_4KFiles.Count >= total4kTasks)) ||
                                     ((!Job.Is720P) && (_720PFiles.Count >= totaltasks)))
                                    {
                                        Thread.Sleep(100);
                                        continue;
                                    }
                                    if ((Job.Is1080p || Job.IsTwitchOut) && (_1080PFiles.Count < total1080ptasks))
                                    {
                                        break;
                                    }
                                    if (Job.IsTwitchActive)
                                    {
                                        if (DateTime.Now >= Job.twitchschedule && !Job.InProcess && !Job.Processed)
                                        {
                                            break;
                                        }
                                        else continue;
                                    }
                                    if ((Job.Is4K || Job.IsMuxed) && (_4KFiles.Count < total4kTasks))
                                    {
                                        break;
                                    }
                                    if ((Job.Is720P) && (_720PFiles.Count < totaltasks))
                                    {
                                        break;
                                    }
                                }

                            }
                            LineNum = 11;

                            string zprocessingfile = "";
                            List<string> PathListFFS = new List<string>();
                            if (Job.IsMuxed)
                            {
                                zprocessingfile = Job.MultiSourceDir;
                            }
                            else
                            {
                                if (Job.IsMulti)
                                {
                                    string fname = $"sourcefiles{Job.SourceFileIndex}.txt";
                                    PathListFFS = Directory.EnumerateFiles(Job.MultiSourceDir, fname, SearchOption.AllDirectories).ToList<string>();
                                }
                                else
                                {
                                    PathListFFS = Directory.EnumerateFiles(Job.SourcePath, Job.SourceFile, SearchOption.AllDirectories).
                                            Where(s => s.EndsWith(Job.FileExt)).ToList<string>();
                                }
                                if (PathListFFS.Count > 0)
                                {
                                    zprocessingfile = PathListFFS.FirstOrDefault<string>();
                                }
                                LineNum = 12;
                            }
                            if (!Job.IsMulti && !Job.IsCST && zprocessingfile != "")
                            {

                                List<Process> Processes = Win32Processes.GetProcessesLockingFile(zprocessingfile);
                                if (Processes.Count > 0)
                                {
                                    LoadSelectForm();
                                    Thread.Sleep(250);
                                    Job.ProbeDate = DateTime.Now.AddYears(-500);
                                    continue;
                                }
                            }
                            LineNum = 13;
                            string DestFile = "", SourceDirectory = Job.SourcePath, sep = Job.X264Override ? "\\x264\\" : "\\";
                            DestFile = (Job.IsTwitchStream && !Job.twitchschedule.HasValue) ? DestDirectoryTwitch : (Job.Is4KAdobe) ? DestDirectoryAdobe4K : (Job.Is4K) ? DestDirectory4K : (Job.Is1080p) ? DestDirectory1080p : DestDirectory720p;
                            if (Job.IsMuxed)
                            {
                                string dfile = Path.GetDirectoryName(zprocessingfile);
                                string ff = dfile.Split('\\').ToList().LastOrDefault();
                                string newd = dfile.Replace(ff, "Filtered");
                                DestFile = Path.Combine(newd, Path.GetFileNameWithoutExtension(zprocessingfile)+".mp4");
                            }

                            else
                            {
                                if ((Job.ScriptFile is not null) && (Job.ScriptFile != ""))
                                {
                                    if (Job.IsMulti && Job.ScriptFile != "")
                                    {
                                        var p = Job.ScriptFile.Split("|").ToList();
                                        p.RemoveAt(0);
                                        DestFile = p.FirstOrDefault();
                                    }
                                }
                                else
                                {

                                    DestFile += @"\" + Job.FileNoExt;
                                    DestFile += (Job.Is4KAdobe) ? ".mp4" : ".mkv";
                                }
                            }



                            if (this.IsChecked("ChkReEncode") && (File.Exists(DestFile)))
                            {
                                bool doSwap = false;
                                doSwap = ((this.IsChecked("ChkAutoAAC")) && (!DestFile.EndsWith("[AAC].mkv"))) || doSwap;
                                doSwap = ((!this.IsChecked("ChkAutoAAC")) && (!DestFile.EndsWith("[NEW].mkv"))) || doSwap;
                                if (doSwap)
                                {
                                    DestFile = this.IsChecked("ChkAutoAAC") ? DestFile.Replace(".mkv", "[AAC].mkv") : DestFile.Replace(".mkv", "[NEW].mkv");
                                    Job.Title = Path.GetFileName(DestFile);
                                }
                            }
                            LineNum = 152;
                            string mysourcefiles = "";
                            if (Job.IsMuxed)
                            {
                                mysourcefiles = zprocessingfile;
                                SourceDirectory = Path.GetDirectoryName(zprocessingfile);

                            }
                            if (!Job.IsMulti && !Job.IsMuxed)
                            {
                                mysourcefiles = SourceDirectory + "\\" + Job.SourceFile;
                                string filenames = Path.GetFileName(mysourcefiles);
                                if ((!System.IO.File.Exists(mysourcefiles)) && (SourceDirectory != ""))
                                {
                                    List<string> PathList = Directory.EnumerateFiles(SourceDirectory, filenames, SearchOption.AllDirectories).
                                           Where(s => s.ToLower().EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                                    foreach (var ess in PathList.Where(ess => System.IO.File.Exists(ess)))
                                    {
                                        mysourcefiles = ess;
                                    }
                                }
                            }
                            LineNum = 15;
                            // if destfile locked continue;
                            if (!Job.IsTwitchActive)
                            {
                                List<Process> Processesx = Win32Processes.GetProcessesLockingFile(DestFile);
                                if (Processesx.Count > 0)
                                {
                                    continue;
                                }

                                if (System.IO.File.Exists(DestFile))
                                {
                                    double fs = new System.IO.FileInfo(DestFile).Length;
                                    if (fs == 0)
                                    {
                                        File.Delete(DestFile);
                                    }
                                }
                            }
                            else
                            {
                                DestFile = TwitchStreamKey;
                            }
                            LineNum = 16;
                            if ((!Job.InProcess) && (!Job.ProbeLock) && (Job.ProbeDate.Year < 1900))
                            {
                                Job.ProbeDate = DateTime.Now;
                                Job.ProbeLock = true;
                                LineNum = 17;
                                AsyncProbe(DestFile, mysourcefiles, SourceDirectory, Job, Index).ConfigureAwait(false);
                            }
                            else
                            {
                                LineNum = 18;
                                if (Job.ProbeDate.Year > 1900)
                                {
                                    TimeSpan pw = DateTime.Now - Job.ProbeDate;
                                    if (Math.Abs(pw.TotalMinutes) > 5)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] errorStr = new string[] { "Collection", "was", "modified", "enumeration", "operation", "may", "not", "execute" };
                if (errorStr.All(s => s.Contains(ex.Message)))
                {
                    Task.Run(async () => SetupThreadProcessorAsync());
                }
                else ex.LogWrite($"SetupThreadProcessorAsync Line{LineNum}{MethodBase.GetCurrentMethod().Name} {ex.Message}");

            }
            finally
            {
                Task.Run(async () => SetupThreadProcessorAsync());
            }
        }
        public bool FileIncompleteTimes(JobListDetails Job, string DestinationFile)
        {
            try
            {
                TimeSpan Start = Job.StartPos.FromStrToTimeSpan();
                TimeSpan End = Job.EndPos.FromStrToTimeSpan();
                bool res = false;
                if ((File.Exists(DestinationFile) && (Win32Processes.GetProcessesLockingFile(DestinationFile).Count == 0)))
                {
                    ffmpegbridge FileIndexer = new ffmpegbridge();
                    FileIndexer.ReadDuration(DestinationFile);
                    TimeSpan time1 = TimeSpan.Zero;
                    TimeSpan time2 = TimeSpan.Zero;
                    while (!FileIndexer.Finished)
                    {
                        Thread.Sleep(100);
                    }
                    time1 = FileIndexer.GetDuration();
                    FileIndexer = null;
                    time2 = TimeSpan.FromSeconds(Job.TotalSeconds);
                    if (Job.EndPos != "")
                    {
                        if (Job.StartPos != "")
                        {
                            Start = Job.StartPos.FromStrToTimeSpan();
                            time2 = End - Start;
                        }
                    }
                    if (Math.Abs(time2.TotalSeconds - time1.TotalSeconds) < 15)
                    {
                        (Job.ProbeLock, Job.Fileinfo) = (false, "Allready Processed");
                        return true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public async Task AsyncProbe(string DestFile, string mysourcefiles, string SourceDirectory, JobListDetails Job, int Index)
        {
            int LineNum = 0;
            try
            {

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(async () => AsyncProbe(DestFile, mysourcefiles, SourceDirectory, Job, Index));
                    return;
                }
                bool IsOkay = false;
                if ((!Job.IsShorts) && (!Job.IsTwitchStream) && (System.IO.File.Exists(DestFile) || (Job.Handle != "")))
                {
                    try
                    {
                        LineNum = 1;
                        List<Process> Procxx = Win32Processes.GetProcessesLockingFile(DestFile);
                        if (Procxx.Count == 0)
                        {

                            var Task = await IsfinishedFileAsync(DestFile);
                            LineNum = 2;
                            Job.ProbeLock = false;
                            if (Task && FileIncompleteTimes(Job, DestFile))
                            {
                                (Job.ProbeLock, Job.Fileinfo) = (false, "Allready Processed");
                                string dDir = (Job.Is4K) ? DoneDirectory4K : (Job.Is1080p) ? DoneDirectory1080p : DoneDirectory720p;
                                if (Job.Is4KAdobe) dDir = DoneDirectoryAdobe4K;
                                if (Job.IsMulti) dDir = Path.GetDirectoryName(Job.DestMFile);
                                string movefile = dDir + "\\" + Path.GetFileName(DestFile);
                                currentjob++; Passed++;
                                LineNum = 3;
                                lblFailpass.AutoSizeLabel(Passed.ToString() + "/" + failed.ToString());
                                this.SetLabelContent("lblQueInfo", (ProcessingJobs.Count - currentjob).ToString());
                                string ffname = mysourcefiles + Job.FileExt.Replace(".", "");
                                LineNum = 4;
                                ffname = SourceDirectory + "\\" + Job.SourceFile;
                                if (MovedIfExists(ffname, movefile, "A"))
                                {
                                    if (Job.Handle != "")
                                    {
                                        Job.Fileinfo = $"Allready Processed,Moved";
                                    }
                                    Job.Processed = true;
                                }
                                LineNum = 5;
                                ThreadStatsHandler?.Invoke(1, (Job.IsMulti) ? Path.GetFileNameWithoutExtension(Job.DestMFile) : Job.Title);
                                IsOkay = false;
                                LineNum = 6;
                            }
                            else
                            {

                                LineNum = 7;
                                List<Process> Procx = Win32Processes.GetProcessesLockingFile(DestFile);
                                if (Procx.Count == 0)
                                {
                                    File.Delete(DestFile);
                                    IsOkay = true;
                                    (Job.Fileinfo, Job.ProbeDate, Job.ProbeLock) = ("", DateTime.Now.AddYears(-500), false);
                                }
                                LineNum = 8;
                            }

                        }
                    }
                    catch (Exception exx)
                    {
                        Job.Fileinfo = "ERROR SEE LOG";// [File is Locked Exception " + exx.Message;
                        exx.LogWrite(exx.Message);
                        ThreadStatsHandler?.Invoke(1, Job.FileNoExt);
                        Job.ProbeDate = DateTime.Now.AddYears(-500);
                        this.SetValue("Progressbar2", Index);
                        IsOkay = false;
                    }
                }
                else (IsOkay, Job.ProbeLock) = (true, false);

                LineNum = 9;
                if ((IsOkay) && (!Job.ProbeLock))
                {
                    IsOkay = false;
                    Process[] ps1 = Process.GetProcessesByName("ffmpeg");
                    LineNum = 10;
                    foreach (var p in ps1.Where(p => Job.Handle == p.Id.ToString()))
                    {
                        p.Kill();
                    }
                    bool Found = false;
                    LineNum = 11;
                    string processingfile = (Job.IsMuxed) ? Job.MultiSourceDir : 
                        (Job.IsMulti) ? Job.DestMFile : Job.SourcePath + "\\" + Job.SourceFile;
                    
                    if (Job.IsMulti && !Job.IsMuxed)
                    {
                        bool passed = true;
                        int i = 1;
                        List<string> Files = Job.GetCutList().Select(sp => sp.Replace("file ", "").Replace("'", "").Trim()).ToList();
                        string SourceCacheFile = Job.MultiSourceDir + "\\MultiFileCache.cache";
                        LineNum = 12;
                        bool scan = true;
                        if (File.Exists(SourceCacheFile))
                        {
                            LineNum = 13;
                            string CacheData = File.ReadAllText(SourceCacheFile);
                            if (CacheData != "")
                            {
                                LineNum = 14;
                                DateTime cachedate = DateTime.Now;
                                cachedate.AddYears(-1000);
                                DateTime.TryParse(CacheData, out cachedate);
                                LineNum = 15;
                                if (cachedate.Year > 1900)
                                {
                                    DateTime et = DateTime.Now;
                                    TimeSpan ec = et - cachedate;
                                    if (ec.TotalDays < 1)
                                    {
                                        scan = false;
                                    }
                                }
                                LineNum = 16;
                            }
                        }
                        scan = false;

                        if (scan)
                        {
                            LineNum = 17;
                            foreach (var file in Files)
                            {
                                var Task1 = await HasValidVideoStream(file, Job, i, Files.Count);
                                i++;
                                Job.ProbeLock = false;
                                if (Task1)
                                {
                                    (Job.ProbeDate, Job.ProbeLock) = (DateTime.Now.AddYears(-500), false);
                                    Job.Fileinfo = "[Video Decoder ERROR]";// OK]";}
                                    failed++;
                                    LineNum = 18;
                                    ThreadStatsHandler?.Invoke(1, (Job.IsMulti) ? Path.GetFileNameWithoutExtension(Job.DestMFile) : Job.Title);
                                    Thread.Sleep(850);
                                    LineNum = 19;
                                    IsOkay = false;
                                    passed = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (Job.Is4K)
                            {
                                LineNum = 20;
                                int idxx = _4KFiles.IndexOf(Path.GetFileName(Job.DestMFile));
                                if (idxx == -1) _4KFiles.Add(Path.GetFileName(Job.DestMFile));
                            }
                        }
                        scan = true;
                        LineNum = 21;
                        if (passed && scan)
                        {
                            LineNum = 22;
                            if (File.Exists(SourceCacheFile))
                                File.Delete(SourceCacheFile);
                            string cachedate = DateTime.Now.ToString();
                            File.AppendAllLines(SourceCacheFile, new List<string> { cachedate });
                            (Job.Handle, Job.ProbeLock, IsOkay) = ("", false, true);
                        }
                    }
                    else
                    {
                        LineNum = 24;

                        if (Job.IsCST)
                        {
                            IsOkay = true;
                        }
                        else if (System.IO.File.Exists(processingfile) || Job.Handle != "")
                        {
                            LineNum = 25;
                            string LastFile = processingfile;
                            var Task1 = await HasValidVideoStream(processingfile, Job);
                            Job.ProbeLock = false;
                            LineNum = 26;
                            if (Task1)
                            {

                                (Job.ProbeDate, Job.ProbeLock) = (DateTime.Now.AddYears(-500), false);
                                Job.Fileinfo = "[Video Decoder ERROR]";// OK]";}
                                failed++;
                                LineNum = 27;
                                ThreadStatsHandler?.Invoke(1, (Job.IsMulti) ? Path.GetFileNameWithoutExtension(Job.DestMFile) : Job.Title);
                                Thread.Sleep(850);
                                LineNum = 28;

                                IsOkay = false;
                            }
                            else
                            {
                                (Job.Handle, Job.ProbeLock, IsOkay) = ("", false, true);
                            }
                        }
                    }
                    IsOkay = (Job.Handle != "") ? false : IsOkay;
                    if (IsOkay)
                    {
                        LineNum = 29;
                        if (Job.SourceFile != null)
                        {
                            DestFile = (Job.IsTwitchStream) ? DestDirectoryTwitch : (Job.Is4K) ? DestDirectory4K : (Job.Is1080p) ? DestDirectory1080p : DestDirectory720p;
                            if (Job.Is4KAdobe) DestFile = DestDirectoryAdobe4K;
                            if (Job.IsMulti) DestFile = Path.GetDirectoryName(Job.DestMFile);
                            DestFile = DestFile + "\\" + Job.SourceFile;
                        }
                        LineNum = 30;
                        string eee = "overide " + Job.X264Override.ToString();
                        LineNum = 31;
                        if ((!Job.X264Override) && (!Job.IsMulti && !Job.IsMuxed))
                        {
                            LineNum = 33;
                            string filename = Path.GetFileName(mysourcefiles);
                            string pathname = Path.GetDirectoryName(mysourcefiles);
                            LineNum = 34;
                            pathname = (pathname != SourceDirectory) ? SourceDirectory : pathname;
                            string subpath = pathname.Substring(3);
                            processingfile = pathname + "\\" + filename;
                            string DownloadsDir = GetDownloadsFolder();
                            bool IsNotDownloads = !Job.SourcePath.Contains(DownloadsDir);
                            LineNum = 35;
                            if (IsNotDownloads)
                            {
                                if (processingfile.Contains("\\processing\\"))
                                {
                                    processingfile = pathname + "\\" + filename;
                                    if (!Directory.Exists(Path.GetDirectoryName(processingfile)))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(processingfile));
                                    }
                                }
                                else
                                {
                                    if (Job.Is4K)
                                    {
                                        LineNum = 36;
                                        if ((Job.SourcePath != DestDirectory4K) && (Job.SourcePath != DoneDirectoryAdobe4K) && (Job.SourcePath != DoneDirectory4K))
                                        {
                                            LineNum = 37;
                                            processingfile = pathname + "\\processing\\" + filename;
                                            if (!Directory.Exists(pathname + "\\processing\\"))
                                            {
                                                Directory.CreateDirectory(pathname + "\\processing\\");
                                            }
                                        }
                                    }
                                    if (Job.Is1080p)
                                    {
                                        LineNum = 38;
                                        if ((Job.SourcePath != DestDirectory1080p) && (Job.SourcePath != DoneDirectory1080p))
                                        {
                                            processingfile = pathname + "\\processing\\" + filename;
                                            if (!Directory.Exists(pathname + "\\processing\\"))
                                            {
                                                Directory.CreateDirectory(pathname + "\\processing\\");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LineNum = 39;
                                        if ((Job.SourcePath != DestDirectory720p) && (Job.SourcePath != DoneDirectory720p))
                                        {
                                            processingfile = pathname + "\\processing\\" + filename;
                                            if (!Directory.Exists(pathname + "\\processing\\"))
                                            {
                                                Directory.CreateDirectory(pathname + "\\processing\\");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        LineNum = 40;
                        string sourceDir = Job.MultiSourceDir, ext = Job.FileExt;//.LastOrDefault();
                        if (Job.IsMulti) mysourcefiles = Job.DestMFile;
                        if (Job.IsMuxed)
                        {
                            mysourcefiles = Job.MultiSourceDir;
                            Job.FileNoExt = Path.GetFileNameWithoutExtension(mysourcefiles);
                        }
                        string myfile = Path.GetFileNameWithoutExtension(mysourcefiles);
                        bool? isRe = (ThreadStatsHandlerBool?.Invoke(0, myfile));
                        LineNum = 41;
                        bool isOK = isRe.HasValue && isRe.Value;
                        if (!isOK) ThreadStatsHandler?.Invoke(0, myfile);
                        bool ProcessFile = false;
                        LineNum = 41;
                        if (mysourcefiles + Job.FileExt.Replace(".", "") == processingfile)
                        {
                            ProcessFile = true;
                        }
                        else if (MovedIfExists(mysourcefiles, processingfile, "B"))
                        {
                            ProcessFile = true;
                        }
                        LineNum = 42;
                        if (ProcessFile)
                        {
                            string destpath = Path.GetDirectoryName(DestFile);
                        }
                        
                        LineNum = 43;
                        isRe = (ThreadStatsHandlerBool?.Invoke(0, Job.FileNoExt));
                        isOK = isRe.HasValue && isRe.Value;
                        if (Job.IsMulti) Job.SourcePath = Path.GetDirectoryName(Job.DestMFile);
                        LineNum = 44;
                        if ((Job.SourcePath != "") && (isOK))
                        {
                            TimeSpan probedatse = Job.ProbeDate - DateTime.Now;
                            double TotalDays = probedatse.TotalDays;
                            bool founxd = false, InProcess = false;
                            string JobHandle = "", srfilex = (Job.IsMuxed) ? Job.MultiSourceDir: Job.SourcePath + "\\" + Job.FileNoExt + Job.FileExt;
                            if (Job.IsMulti && !Job.IsMuxed) srfilex = Job.DestMFile;
                            LineNum = 45;
                            if (!Job.IsMulti && !Job.IsCST)
                            {
                                List<Process> Procx = Win32Processes.GetProcessesLockingFile(srfilex);
                                if (Procx.Count == 0)
                                {
                                    TotalDays = Procx.Count;
                                }
                                else founxd = true;
                            }
                            LineNum = 46;
                            foreach (var jobpx in ProcessingJobs.Where(cs => cs.FileNoExt == Path.GetFileNameWithoutExtension(Job.FileNoExt)))
                            {
                                JobHandle = jobpx.Handle;
                                break;
                            }
                            LineNum = 47;
                            if (!founxd && (TotalDays < 1) && (Job.IsMulti || System.IO.File.Exists(processingfile) && (JobHandle == "")))
                            {
                                ProcessingTime = DateTime.Now;
                                Job.ProbeDate = DateTime.Now;
                                LineNum = 48;
                                RunConversion(processingfile, DestFile, Job, ThreadStatsHandler).ConfigureAwait(false);
                                LineNum = 49;
                                Thread.Sleep(250);
                                Thread.Sleep(100);
                            }
                            else
                            {
                                LineNum = 50;
                                Job.ProbeDate = DateTime.Now.AddYears(-500);
                                string er = "Failed Processpath " + processingfile + " " + Job.Handle.ToString();
                            }
                        }
                        else Job.ProbeDate = DateTime.Now.AddYears(-500);
                    }
                    else
                    {
                        Job.ProbeDate = DateTime.Now.AddYears(-500);
                    }
                }
                else Job.ProbeDate = DateTime.Now.AddYears(-500);
            }
            catch (Exception ex)
            {
                string[] errorStr = new string[] { "Collection", "was", "modified", "enumeration", "operation", "may", "not", "execute" };
                if (errorStr.All(s => s.Contains(ex.Message)))
                {
                    Job.ProbeDate = DateTime.Now.AddYears(-500);
                    Task.Run(async () => SetupThreadProcessorAsync());
                }
                else ex.LogWrite($"AsyncProbe {LineNum}{MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        public void CheckLFFMPEGLocking(List<Process> Procx, string SourceFile)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                string SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                key?.Close();
                foreach (var proc in Procx.Where(proc => proc.MainModule.ModuleName.Contains("ffmpeg")))
                {
                    bool fnd = false;
                    string myStrQuote = "\"";
                    ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process where name = {myStrQuote}ffmpeg.exe{myStrQuote}");
                    foreach (ManagementObject o in searcher.Get())
                    {
                        string HandleID = o.Properties["Handle"].Value.ToString();
                        if (o["CommandLine"] != null)
                        {
                            string comstr = o["CommandLine"].ToString(), lookupstr = "-f concat -safe 0 -i";
                            if (comstr.ToLower().Contains(lookupstr))
                            {
                                var index = comstr.ToLower().IndexOf(lookupstr);
                                var index2 = comstr.ToLower().IndexOf("\\sourcefiles.txt");
                                if (index != -1 && index2 != -1)
                                {
                                    string lookup = comstr.Substring(index, index2 - index + lookupstr.Length).Trim();
                                    if (lookup != "")
                                    {
                                        foreach (var p in ProcessingJobs.Where(pr => pr.MultiSourceDir == lookup).Where(p => p.IsMulti))
                                        {
                                            if (p.Is4K) Stats_Handler.count4k++;
                                            if (p.Is1080p) Stats_Handler.count1080p++;
                                            if (p.Is1080p || p.Is4K) break;
                                        }
                                    }
                                }
                            }
                            if (comstr.ToLower().Contains(SourceFile.ToLower()))
                            {
                                if (comstr.Contains(SourceDirectory1080p)) Stats_Handler.count1080p++;
                                if (comstr.Contains(SourceDirectory4K)) Stats_Handler.count4k++;
                                if (comstr.Contains(SourceDirectory720p))
                                {
                                    Stats_Handler.count720p++;
                                }
                            }
                            if (!comstr.Contains("-c copy -f null output.mkv"))
                            {
                                if (comstr.Contains(Path.GetFileName(SourceFile)))
                                {
                                    fnd = true;
                                    foreach (var jobp in ProcessingJobs.Where(cs => cs.FileNoExt == Path.GetFileNameWithoutExtension(SourceFile)))
                                    {
                                        jobp.ConversionStarted = false;
                                        jobp.ProbeStarted = true;
                                        break;
                                    }
                                    break;
                                }
                            }
                            if (true)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        List<string> lstTime = new List<string>();
        public async Task ThinProcessingListAsync()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => ThinProcessingListAsync());
                    return;
                }

                DateTime NowTime = DateTime.Now;
                TimeSpan EventSpan2 = NowTime - Start2;
                TimeSpan EventSpan3 = NowTime - Start3;
                if (ProcessingJobs.Count > 0)
                {
                    if (Math.Abs(EventSpan2.TotalMinutes) > 5)
                    {
                        for (int ixy = ProcessingJobs.Count - 1; ixy >= 0; ixy--)
                        {
                            JobListDetails job = ProcessingJobs[ixy];
                            if (job.Handle != "")
                            {
                                Process[] psa = Process.GetProcessesByName("ffmpeg");
                                List<string> ProcessIDs = new List<string>();
                                foreach (var pid in psa)
                                {
                                    ProcessIDs.Add(pid.Id.ToString());
                                }
                                if (!job.Handle.ContainsAny(ProcessIDs))
                                {
                                    double Total = Math.Abs((job.JobDate - Start2).TotalMinutes);
                                    if (Total > 10)
                                    {
                                        job.Handle = "";
                                    }
                                }
                            }
                            if ((job.Handle == "") && (job.IsSkipped || job.Processed))
                            {
                                double Total = Math.Abs((job.JobDate - Start2).TotalMinutes);
                                if (Total > 15)
                                {
                                    removejob(ixy);
                                    (Start2, Start3) = (DateTime.Now, DateTime.Now);
                                    break;
                                }
                            }
                            else
                            {
                                if ((job.Handle == "") && (!job.InProcess) && (job.Processed))
                                {
                                    double Total = Math.Abs((job.JobDate - Start2).TotalMinutes);
                                    if (Total > 30)
                                    {
                                        removejob(ixy);
                                        (Start2, Start3) = (DateTime.Now, DateTime.Now);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if ((Math.Abs(EventSpan2.TotalMinutes) > 12000000) && (backupserver != "127.0.0.1") && (un != "") && (up != ""))
                    {
                        if (CheckServer(backupserver))
                        {
                            var progress = new Progress<long>(percent => { PercentUpdate(percent); });
                            if (backupCompleted != "")
                            {
                                List<string> CompleedFiles = Directory.EnumerateFiles(DoneDirectory1080p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupCompleted}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                    //if (job)
                                    double TotalSecs = 0;
                                    List<string> Files = new List<string>();
                                    if (source.EndsWith(".src") || source.EndsWith(".edt") || source.EndsWith(".cst"))
                                    {
                                        string srcf = File.ReadAllText(source);// Thin Processing List
                                        List<string> Commands = srcf.Split('|').ToList();
                                        if (Commands.Count > 3)
                                        {
                                            Commands.RemoveAt(0);
                                        }
                                        Commands.RemoveAt(0);
                                        Files.Clear();
                                        string sourceDir = Commands.FirstOrDefault(), ext = Commands.LastOrDefault();
                                        Files = Directory.EnumerateFiles(sourceDir, "*" + ext, SearchOption.AllDirectories).ToList();
                                        ffmpegbridge FileIndexer = new ffmpegbridge();
                                        FileIndexer.ReadDuration(Files);
                                        while (!FileIndexer.Finished)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        TotalSecs = FileIndexer.GetDuration().TotalSeconds;
                                        FileIndexer = null;
                                    }
                                    else
                                    {
                                        ffmpegbridge FileIndexer = new ffmpegbridge();
                                        FileIndexer.ReadDuration(source);
                                        while (!FileIndexer.Finished)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        TotalSecs = FileIndexer.GetDuration().TotalSeconds;
                                        FileIndexer = null;
                                    }

                                    ffmpegbridge FileIndexer1 = new ffmpegbridge();
                                    FileIndexer1.ReadDuration(Destination);
                                    while (!FileIndexer1.Finished)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    double DTotalSecs = FileIndexer1.GetDuration().TotalSeconds;
                                    FileIndexer1 = null;
                                    if (TotalSecs == DTotalSecs)
                                    {
                                        if (source.EndsWith(".src") || source.EndsWith(".edt") || source.EndsWith(".cst"))
                                        {
                                            string srcf = File.ReadAllText(source); // Thin Processing List
                                            List<string> Commands = srcf.Split('|').ToList();
                                            string ks = Commands.FirstOrDefault();
                                            if (Commands.Count > 3)
                                            {
                                                Commands.RemoveAt(0);
                                            }
                                            Commands.RemoveAt(0);
                                            Files.Clear();
                                            string sourceDir = Commands.FirstOrDefault(), ext = Commands.LastOrDefault();
                                            Files = Directory.EnumerateFiles(sourceDir, "*" + ext, SearchOption.AllDirectories).ToList();
                                            ffmpegbridge FileIndexer2 = new ffmpegbridge();
                                            FileIndexer2.ReadDuration(Files);
                                            while (!FileIndexer2.Finished)
                                            {
                                                Thread.Sleep(100);
                                            }
                                            TotalSecs = FileIndexer2.GetDuration().TotalSeconds;
                                            FileIndexer2 = null;
                                        }
                                    }
                                }
                            }
                            if (backupDone1080p != "")
                            {
                                List<string> CompleedFiles = Directory.EnumerateFiles(DestDirectory1080p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupDone1080p}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                }
                            }
                            if (backupDone != "")
                            {
                                List<string> CompleedFiles = Directory.EnumerateFiles(DestDirectory1080p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupDone}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                }
                            }
                        }
                    }
                }
                foreach (var Jobentry in ProcessingJobs.Where(jobentry => jobentry.Handle != ""))
                {
                    string filename = Jobentry.FileNoExt;
                    Process[] psa = Process.GetProcessesByName("ffmpeg");
                    List<string> ProcessIDs = new List<string>();
                    foreach (var pid in psa)
                    {
                        ProcessIDs.Add(pid.Id.ToString());
                    }
                    if (!Jobentry.Handle.ContainsAny(ProcessIDs))
                    {
                        if (!Jobentry.ProbeLock)
                        {
                            for (var i = 0; i < ProcessingJobs.Count() - 1; i++)
                            {
                                if (ProcessingJobs[i].FileNoExt == filename)
                                {
                                    //  ProcessingJobs[i].Handle = "";
                                    //   ProcessingJobs[i].Progress = 100;
                                    //   ProcessingJobs[i].InProcess = false;
                                    //   ProcessingJobs[i].Processed = true;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private bool CheckServer(string host)
        {
            try
            {
                Ping myPing = new Ping();
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void PercentUpdate(long percent)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        public void DoTimerSetup()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => DoTimerSetup());
                    return;
                }
                FileQueChecker.Interval = (int)new TimeSpan(0, 0, 5).TotalMilliseconds;
                FileQueChecker.Enabled = true;
                FileQueChecker.Stop();
                FileQueChecker.Start();
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void SpeedUpdate(string bitrate, string bitratespeed, string fps, string frames, string frames1080p)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (MainWindowX.Visibility == Visibility.Visible)
                    {
                        lblBitrate.AutoSizeLabel(bitrate + " " + bitratespeed);
                        lblSpeed.AutoSizeLabel(fps + "x");
                        lblFrames.AutoSizeLabel(frames);
                        lblCurrentFrames.AutoSizeLabel(frames1080p);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void totalTIme(string A)
        {
            try
            {
                if (lblTotalTime.Content != A)
                {
                    lstTime.Add(A);
                    //lblTotalTime.AutoSizeLabel(A);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void percents(string A, double B)
        {
            try
            {
                this.SetValue("Progressbar1", B);
                lblPercent.AutoSizeLabel(A);
                ResizeWindow();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void TotalDuration(string A, string B)
        {
            if (lblTotalTime.Content != A)
            {
                lstTime.Add("*" + A);
                lblTotalTime.AutoSizeLabel(A);
            }
            lblDuration.AutoSizeLabel(B);
            ResizeWindow();

        }

        private void EtaUpdate(string UpdateMessage)
        {
            lblEta.AutoSizeLabel(UpdateMessage);
        }

        public void Loadsettings()
        {
            int LineNum = 0;
            try
            {
                SystemSetup = true;
                List<string> SavedDirectories = new();
                string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null), SetReg = key.GetValueBool("SetAutoLoad", false);
                key?.Close();
                LineNum = 1;
                if (SetReg)
                {
                    key = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run".OpenSubKey(Registry.LocalMachine);
                    if (key != null)
                    {
                        LineNum = 2;
                        string AppPath = GetExePath() + $"\\VideoGUI.exe";
                        if (!key.GetValueNames().ToList().Contains("VideoGUI"))
                        {
                            LineNum = 3;
                            //key.SetValue("VideoGUI", AppPath);
                        }
                        else
                        {
                            LineNum = 4;
                            string VG = (string)key.GetValue("VideoGUI");
                            if ((VG != AppPath) || (AppPath == ""))
                            {
                                LineNum = 5;
                                //key.SetValue("VideoGUI", AppPath);
                            }
                            LineNum = 6;
                        }
                    }
                    key?.Close();
                }
                LineNum = 7;
                key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                LoadedKey = (key != null);
                List<string> Profiles = new();
                List<string> LogFiles = new List<string>();
                string searchpath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                LineNum = 8;
                this.Dispatcher.Invoke(() =>
                {
                    LogFiles = Directory.EnumerateFiles(searchpath, "*-log.log", SearchOption.AllDirectories).ToList<string>();
                });
                foreach (string logfile in LogFiles)
                {
                    if (logfile.Contains("."))
                    {
                        LineNum = 9;
                        string s = Path.GetFileNameWithoutExtension(logfile);
                        string date = s.Substring(0, s.Length - 4);

                        LineNum = 10;
                        DateTime logDate = DateTime.ParseExact(date, "dd_MM_yyyy", CultureInfo.InvariantCulture);
                        TimeSpan LogDateSpan = logDate - DateTime.Now;
                        LineNum = 11;
                        if (LogDateSpan.TotalDays > 5)
                        {
                            System.IO.File.Delete(logfile);
                        }
                        LineNum = 12;
                    }
                }
                LineNum = 13;
                this.Dispatcher.Invoke(() => { InitTitleLength = MainWindowX.Title.Length; });
                txtDestPath = key.GetValueStr("DestDirectory", defaultdrive);
                txtDonepath = key.GetValueStr("CompDirectory", defaultdrive);
                txtErrorPath = key.GetValueStr("ErrorDirectory", defaultdrive);
                backupserver = key.GetValueStr("backupserver", "127.0.0.1");
                backupDone1080p = key.GetValueStr("1080pDoneBackupPath", "");
                backupDone = key.GetValueStr("DoneBackupPath", "");
                backupCompleted = key.GetValueStr("unc_completedpath", "");
                backupun = key.GetValueStr("backupsettings", "");
                defaultprogramlocation = key.GetValueStr("defaultprogramlocation", "c:\\videogui");
                LineNum = 14;
                if (backupun != "")
                {
                    byte[] details = _EncryptPassword(Encoding.ASCII.GetBytes(backupun));
                    string data = Encoding.ASCII.GetString(details);
                    if (data.Contains("|"))
                    {
                        List<string> dts = data.Split('|').ToList<string>();
                        if (dts.Count >= 2)
                        {
                            un = dts.FirstOrDefault();
                            up = dts.LastOrDefault();
                        }
                    }
                }

                LineNum = 15;
                if (key.RegistryValueExists("BitRateSettings"))
                {
                    string SelProfile = key.GetValueStr("SelectedProfile", string.Empty);
                    Profiles.AddRange(key.GetValueStrs("BitRateSettings"));
                    LineNum = 16;
                    if (SelProfile != "")
                    {
                        foreach (string profstr in Profiles)
                        {
                            if (profstr.Split("|").ToList().First() == SelProfile)
                            {
                                List<string> settingslist = profstr.Split("|").ToList();
                                if (settingslist.Count > 11)
                                {
                                    MinBitRate = settingslist[1];
                                    MaxBitRate = settingslist[2];
                                    BitRateBuffer = settingslist[3];
                                    VideoWidth = settingslist[4];
                                    VideoHeight = settingslist[5];
                                    ArModulas = settingslist[6];
                                    ResizeEnable = settingslist[7].ToBool();
                                    ArRoundEnable = settingslist[8].ToBool();
                                    ArScalingEnabled = settingslist[9].ToBool();
                                    VSyncEnable = settingslist[10].ToBool();
                                }
                            }
                        }
                    }
                    else
                    {
                        string result = "";
                        result = $"{result}default|";
                        result = $"{result}675K|";
                        result = $"{result}1150K|";
                        result = $"{result}1200K|";
                        result = $"{result}720|";
                        result = $"{result}|";
                        result = $"{result}16|";
                        result = $"{result}true|";
                        result = $"{result}true|";
                        result = $"{result}true|";
                        result = $"{result}true|";
                        List<string> settingslist = result.Split("|").ToList();
                        if (settingslist.Count == 11)
                        {
                            MinBitRate = settingslist[1];
                            MaxBitRate = settingslist[2];
                            BitRateBuffer = settingslist[3];
                            VideoWidth = settingslist[4];
                            VideoHeight = settingslist[5];
                            ArModulas = settingslist[6];
                            ResizeEnable = settingslist[7].ToBool();
                            ArRoundEnable = settingslist[8].ToBool();
                            ArScalingEnabled = settingslist[9].ToBool();
                            VSyncEnable = settingslist[10].ToBool();
                        }
                        Profiles.Add(result);
                        LineNum = 18;
                        string profname = result.Split("|").ToList().First();
                        key?.SetValue("BitRateSettings", Profiles.ToArray(), RegistryValueKind.MultiString);
                        LineNum = 19;
                        if (profname != "")
                        {
                            key?.SetValue("SelectedProfile", profname);
                        }
                        LineNum = 20;
                    }
                }
                SavedDirectories.Add("\\\\10.10.1.90\\tv.shows\\Docos");
                SavedDirectories.Add("\\\\10.10.1.90\\tv.shows\\ActionShows");
                SavedDirectories.Add("\\\\10.10.1.90\\tv.shows\\FoodShows");
                SavedDirectories.Add("\\\\10.10.1.90\\tv.shows\\processed");
                SavedDirectories.Add("\\\\10.10.1.90\\processed");
                SavedDirectories.Clear();
                LineNum = 21;
                if (key.GetValueNames().Contains("ComparitorList"))
                {
                    string[] arr = key.GetValueStrs("ComparitorList");
                    SavedDirectories = arr?.ToList<string>();
                    this.AddItems("CmbScanDirectory", "Default");
                    foreach (var scandir in SavedDirectories.Where(scandir => scandir != "Default"))
                    {
                        this.AddItems("CmbScanDirectory", scandir);
                    }
                }
                int Index = key.GetValueInt("ComparitorListIndex");
                if (Index <= this.GetCount("CmbScanDirectory"))
                {
                    this.SetSelectedIndex("CmbScanDirectory", Index);
                }
                LineNum = 22;
                this.SetChecked("GPUEncode", key.GetValueBool("GPUEncode", true));
                this.SetChecked("Fisheye", key.GetValueBool("FishEyeRemoval", false));
                this.SetChecked("ChkAutoAAC", key.GetValueBool("AutoAAC"));
                this.SetChecked("ChkResize1080p", key.GetValueBool("resize1080p"));
                this.SetChecked("ChkChangeOutputname", key.GetValueBool("ChangeFileName"));
                this.SetChecked("ChkReEncode", key.GetValueBool("reencodefile"));
                this.SetChecked("X265Output", key.GetValueBool("X265", true));
                this.SetChecked("ChkAudioConversion", key.GetValueBool("AudioConversionAC3", true));
                this.SetSelectedIndex("cmbaudiomode", key.GetValueInt("Audiomode", 0));
                this.SetSelectedIndex("cmbH64Target", key.GetValueInt("h264Target", -1));
                this.SetChecked("ChkResize1080shorts", key.GetValueBool("Do1080pShorts", false));
                this.SetChecked("ChkDropFormat", key.GetValueBool("DropFormat", true));
                LineNum = 24;
                SystemSetup = false;
                key.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadSettings LineNum {LineNum} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        private bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "1.1.1.1";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void SaveComporitorList()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                if (LoadedKey)
                {
                    key.SetValue("ComparitorList", ComparitorList, RegistryValueKind.MultiString);
                }
                key.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void FileQueChecker_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => FileQueChecker_Tick(sender, e));
                    return;
                }
                Task.Run(async () => ThinProcessingListAsync());
                while (Monitor.IsEntered(ProcessingJobslocker))
                {
                    Thread.Sleep(250);
                }
                string ext = "";
                ProcessingCancellationTokenSource.Cancel();
                FileQueChecker.Stop();
                FileQueChecker.Interval = (int)new TimeSpan(0, 0, 5).TotalMilliseconds;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                string SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);

                key?.Close();
                SourceList.Clear();
                if (SourceDirectory720p != string.Empty)
                {
                    string DownloadsDir = GetDownloadsFolder();

                    string[] SourceDirs = { DownloadsDir, SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe };
                    foreach (string SourceDir in SourceDirs)
                    {
                        List<string> SourceList = Directory.EnumerateFiles(SourceDir, "*.*", SearchOption.AllDirectories).
                             Where(s => s.ToLower().EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                        if (SourceDir == SourceDirectory4KAdobe)
                        {
                            for (int i = SourceList.Count - 1; i >= 0; i--)
                            {
                                if (SourceList[i].Contains(@"FullLengths\"))
                                {
                                    SourceList.RemoveAt(i);
                                }
                            }
                        }

                        foreach (string filename in SourceList)
                        {
                            string SourceDirectoryID = "";
                            string myfilename = Path.GetFileNameWithoutExtension(filename);
                            bool IsScript = false;
                            if (filename.ContainsAny(new List<string> { ".src", ".cst", ".edt" }))
                            {
                                IsScript = true;
                                string fn = File.ReadAllText(filename);  // FIleQue Tick
                                List<string> Commands = fn.Split('|').ToList();
                                if (Commands.Count > 3) Commands.RemoveAt(0);
                                myfilename = Path.GetFileNameWithoutExtension(Commands.FirstOrDefault());
                            }
                            if (ProcessingJobs.Count(job => job.Title == myfilename) == 0)
                            {
                                bool Is1080p = SourceDir.Contains("1080p");
                                bool Is4K = SourceDir.EndsWith("4K");

                                bool IsAdobe = SourceDir.ToUpper().EndsWith("4KADOBE");
                                string dDir = (Is4K) ? DestDirectory4K : (Is1080p) ? DestDirectory1080p : DestDirectory720p;
                                if (IsAdobe) dDir = DestDirectoryAdobe4K;
                                bool AddFileOK = false;
                                string pfile = Path.Combine(dDir, Path.GetFileName(filename));
                                if (File.Exists(pfile))
                                {
                                    ffmpegbridge FileIndexer = new ffmpegbridge();
                                    FileIndexer.ReadDuration(pfile);
                                    while (!FileIndexer.Finished)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    int ptime = FileIndexer.GetDuration().TotalSeconds.ToInt();
                                    FileIndexer = null;
                                    if (ptime == 0)
                                    {
                                        File.Delete(pfile);
                                        AddFileOK = true;
                                    }
                                    else
                                    {
                                        ffmpegbridge FileIndexer2 = new ffmpegbridge();
                                        FileIndexer.ReadDuration(filename);
                                        while (!FileIndexer2.Finished)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        int ptime2 = FileIndexer2.GetDuration().TotalSeconds.ToInt();
                                        FileIndexer = null;
                                        ffmpegbridge FileIndexer3 = new ffmpegbridge();
                                        FileIndexer.ReadDuration(pfile);
                                        while (!FileIndexer3.Finished)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        int ptime3 = FileIndexer3.GetDuration().TotalSeconds.ToInt();
                                        FileIndexer = null;
                                        if (Math.Abs(ptime2 - ptime3) > 30)
                                        {
                                            AddFileOK = true;
                                            File.Delete(pfile);
                                        }
                                        else
                                        {
                                            string doneDir = (Is4K) ? DoneDirectory4K : (Is1080p) ? DoneDirectory1080p : DoneDirectory720p;
                                            if (IsAdobe) doneDir = DoneDirectoryAdobe4K;
                                            string moveto = Path.Combine(doneDir, Path.GetFileName(filename));
                                            MoveIfExists(filename, moveto);
                                        }
                                    }


                                }
                                else AddFileOK = true;
                                if (AddFileOK) AddIfVaid(filename, SourceDir);
                            }
                            else if (!IsScript)
                            {
                                string sourcename = "";
                                bool IsFinished = false; ;
                                foreach (var cjob in ProcessingJobs.Where(cjob => (cjob.Complete) && (cjob.Title == myfilename) && (cjob.IsMulti)))
                                {
                                    sourcename = cjob.DestMFile;
                                    IsFinished = cjob.Complete;
                                }
                                if ((IsFinished) && (sourcename != ""))
                                {
                                    List<Process> Processes = Win32Processes.GetProcessesLockingFile(sourcename);
                                    foreach (var _ in Processes.Where(process => process.ProcessName.Contains("ffmpeg")).Select(process => new { }))
                                    {
                                        if (AddIfVaid(filename, SourceDir)) break;
                                    }
                                }
                            }
                        }
                        SourceList.Clear();
                    }
                    ProcessingJobs.OrderBy(q => q.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                ProcessingCancellationTokenSource = new CancellationTokenSource();
                if (ProcessingJobs.Count <= totaltasks + total1080ptasks) FileQueChecker.Interval = (int)new TimeSpan(0, 1, 0).TotalMilliseconds;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Interval = (int)new TimeSpan(0, 1, 0).TotalMilliseconds;
                    FileQueChecker.Start();
                    
                });
            }
        }
        private async Task RunConversion(string SourceFile, string DestFile, JobListDetails job, _StatsHandler stats_handle)
        {
            int LineNum = 0;
            try
            {
                int MSIZE = 0;
                bool _isCopy = false;
                _StatsHandler _stats_handle = stats_handle;
                bool overrider = false, chkresized = false;
                bool _GPUEncode = this.IsChecked("GPUEncode"), _X265Output = !this.IsChecked("X265Output");
                int _cmbaudiomodeSelectedIndex = this.SelectedIndex("cmbaudiomode");
                string myStrQuote = "\"";
                (chkresized, overrider, job.InProcess) = (ResizeEnable, job.X264Override, true);
                Video = CheckForGraphicsCard();
                lbAccelHW.AutoSizeLabel(Video);
                LineNum = 1;
                ffmpeg.VideoCodec Encoder = ffmpeg.VideoCodec.h264;
                int H264Target = this.SelectedIndex("cmbH64Target");
                HardwareAccelerator HardwareAcceleration = HardwareAccelerator.cuda;// "qsv";
                LineNum = 2;
                if (Video.Contains("AMD") || Video.Contains("Radeon")) HardwareAcceleration = HardwareAccelerator.vulkan;
                if (_GPUEncode)
                {
                    if ((!overrider) && (_X265Output))
                    {
                        LineNum = 3;
                        if (Video.Contains("NVIDIA")) Encoder = ffmpeg.VideoCodec.hevc_nvenc;
                        else if (Video.Contains("AMD") || (Video.Contains("Radeon"))) Encoder = ffmpeg.VideoCodec.hevc_amf;
                    }
                    else
                    {
                        LineNum = 4;
                        if (H264Target == 2)
                        {
                            if (Video.Contains("NVIDIA")) Encoder = ffmpeg.VideoCodec.h264_nvenc;
                            else if ((Video.Contains("AMD")) || (Video.Contains("Radeon"))) Encoder = ffmpeg.VideoCodec.h264_amf;
                        }
                        else if ((H264Target == 0) || (job.Mpeg4ASP))
                        {
                            if (!job.Mpeg4AVC) (Encoder, _GPUEncode) = (ffmpeg.VideoCodec.libxvid, false);
                        }
                        else if ((H264Target == 1) || (job.Mpeg4AVC))
                        {
                            if (!job.Mpeg4ASP) (Encoder, _GPUEncode) = (ffmpeg.VideoCodec.mpeg4, false);
                        }
                        LineNum = 5;
                    }
                }
                else
                {
                    LineNum = 6;
                    if (!_X265Output)
                    {  // ASP // AVC // H264
                        Encoder = (H264Target == 0) ? ffmpeg.VideoCodec.libxvid :
                            ((H264Target == 1) ? ffmpeg.VideoCodec.mpeg4 : ffmpeg.VideoCodec.h264);
                        if (H264Target != 2)
                        {
                            if (job.Mpeg4ASP) Encoder = ffmpeg.VideoCodec.libxvid;
                            if (job.Mpeg4AVC) Encoder = ffmpeg.VideoCodec.mpeg4;
                        }
                    }
                    else Encoder = ffmpeg.VideoCodec.libx265;
                    LineNum = 7;
                }
                string AppPath = GetExePath();
                double totalseconds = 0;
                LineNum = 8;
                //SourceFile = SourceFile.Contains(" ") ? myStrQuote + SourceFile + myStrQuote : SourceFile;
                try
                {
                    if (job.IsMulti && !job.IsMuxed)
                    {
                        LineNum = 9;
                        List<string> Files = new List<string>();
                        string filename = "";
                        foreach (string sp in job.GetCutList())
                        {
                            filename = sp.Replace("file '", "").Replace("'", "");
                            Files.Add(filename);
                        }
                        LineNum = 10;
                        ffmpegbridge FileIndexer = new ffmpegbridge();
                        FileIndexer.ReadDuration(Files);

                        while (!FileIndexer.Finished)
                        {
                            Thread.Sleep(100);
                        }
                        LineNum = 11;
                        List<(string, double)> FileInfos = new List<(string, double)>();
                        FileInfos.AddRange(FileIndexer.FileInfoList);
                        LineNum = 12;
                        totalseconds = FileIndexer.GetDuration().TotalSeconds;
                        job.TotalSeconds = totalseconds;
                        LineNum = 13;
                        // Add Get List of files and durations 
                        FileIndexer = null;

                        job.TotalSeconds = totalseconds;
                        bool fnx = false;
                        LineNum = 14;
                        string srcdir = job.MultiSourceDir.Split('\\').ToList().LastOrDefault();

                        foreach (var _ in SourceFileInfos.Where(imt => imt.SourceDirectory == srcdir).Select(imt => new { }))
                        {
                            fnx = true;
                            break;
                        }
                        LineNum = 15;
                        if (!fnx)
                        {
                            TimeSpan StartPos = TimeSpan.Zero;
                            SourceFileCache sfc = new SourceFileCache(job.MultiSourceDir, false);
                            sfc.SourceDirectory = srcdir;
                            SourceFileInfos.Add(sfc);
                            foreach (var file in FileInfos)
                            {
                                TimeSpan tspan = TimeSpan.FromSeconds(file.Item2);
                                SourceFileInfo TFI = new SourceFileInfo(file.Item1, StartPos, tspan);
                                StartPos += tspan;
                                sfc.SourceFiles.Add(TFI);
                            }
                            FileInfos = null;

                        }
                        LineNum = 16;
                    }
                    else
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromSeconds(15));
                        while (!cts.IsCancellationRequested)
                        {
                            List<Process> Processes = Win32Processes.GetProcessesLockingFile(SourceFile);
                            if (Processes.Count == 0) cts.Cancel();

                            Thread.Sleep(100);
                        }
                        LineNum = 18;
                        ffmpegbridge FileIndexer = new ffmpegbridge();
                        FileIndexer.ReadDuration(SourceFile);
                        while (!FileIndexer.Finished)
                        {
                            Thread.Sleep(100);
                        }
                        LineNum = 19;
                        totalseconds = FileIndexer.GetDuration().TotalSeconds;
                        FileIndexer = null;
                        LineNum = 20;
                    }
                    LineNum = 21;
                }
                catch (Exception ex1)
                {
                    if (job.IsMulti && !job.IsMuxed)
                    {
                        LineNum = 22;
                        List<string> Cuts = job.GetCutList();
                        List<string> Files = Cuts.Select(sp => sp.Replace("file ", "").Replace("'", "").Trim()).ToList();
                        string dstfn = "", srcfile, destfn = Path.GetFileNameWithoutExtension(DestFile);
                        foreach (string pr in Files)
                        {
                            LineNum = 23;
                            dstfn = ErrorDirectory + "\\" + destfn + "_" + Path.GetFileName(pr);
                            MoveIfExists(pr, dstfn);
                            LineNum = 24;
                        }
                        LineNum = 25;
                        dstfn = ErrorDirectory + "\\" + Path.GetFileName(job.MultiFile);
                        MoveIfExists(job.MultiFile, dstfn);
                        LineNum = 26;
                    }
                    else
                    {
                        LineNum = 27;
                        SourceFile = SourceFile.Contains(" ") ? myStrQuote + SourceFile + myStrQuote : SourceFile;
                        MoveIfExists(SourceFile, ErrorDirectory + "\\" + Path.GetFileName(SourceFile));
                        LineNum = 28;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        LineNum = 29;
                        job.Fileinfo = "$[ffmpeg open ERROR-{SourceFile}]";// OK]";}
                        ThreadStatsHandler?.Invoke(1, (job.IsMulti) ? Path.GetFileNameWithoutExtension(job.DestMFile) : job.Title);
                        LineNum = 30;
                        LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 7 " + ex1.Message);
                        (job.InProcess, job.Processed, job.IsSkipped) = (false, true, true);
                        failed++;
                    });
                }
                double TotalSecs = 0;
                IAudioStream audioStream = null;
                IVideoStream videoStream = null;
                LineNum = 31;
                if (job.Title is null)
                {
                    job.Title = Path.GetFileName(job.DestMFile);
                }
                TimeSpan SeekFrom = TimeSpan.Zero, SeekTo = TimeSpan.Zero;
                if (job.IsMulti || job.IsTwitchOut)
                {
                    LineNum = 32;
                    // **** DO THINING HERE
                    if ((job.IsCST || job.IsCET) && (job.PosMode is string mode) && (mode.ToLower() == "time")) // CET MOD   
                    {
                        LineNum = 33;
                        SeekFrom = job.StartPos.FromStrToTimeSpan();
                        SeekTo = job.EndPos.FromStrToTimeSpan();
                        _isCopy = true;
                        LineNum = 34;
                    }
                    else if (job.PosMode is not null)
                    {
                        LineNum = 35;
                        if ((job.PosMode.ToLower() == "time"))
                        {
                            LineNum = 36;
                            SeekFrom = job.StartPos.FromStrToTimeSpan();
                            SeekTo = job.EndPos.FromStrToTimeSpan();
                        }
                        LineNum = 37;
                    }
                    if (job.IsCST || job.IsCET) // CET MOD
                    {
                        LineNum = 38;
                        job.TotalSeconds = SeekTo.TotalSeconds - SeekFrom.TotalSeconds;
                        if (job.TotalSeconds > 0)
                        {
                            LineNum = 39;
                            totalseconds = job.TotalSeconds;
                        }
                    }
                    LineNum = 40;
                    TimeSpan NewSeekFrom = TimeSpan.Zero, NewSeekTo = TimeSpan.Zero;
                    if ((job.IsMulti) && (SeekTo != TimeSpan.Zero))
                    {
                        LineNum = 41;
                        NewSeekTo = SeekTo;
                        NewSeekFrom = SeekFrom;
                        string MSRC = job.MultiSourceDir.Split('\\').ToList().LastOrDefault();
                        foreach (var SFI in SourceFileInfos.Where(SFI => SFI.SourceDirectory == MSRC))
                        {
                            LineNum = 42;
                            bool skip = false;
                            List<string> flist = new List<string>();
                            double TotalDuration = 0;
                            foreach (SourceFileInfo fss in SFI.SourceFiles)
                            {
                                TotalDuration += fss.Duration.TotalSeconds;
                            }
                            if (TotalDuration < SeekFrom.TotalSeconds)
                            {
                                job.Complete = true;
                                continue;
                            }
                            foreach (SourceFileInfo fss in SFI.SourceFiles)
                            {
                                if (SeekFrom > (fss.Start + fss.Duration))
                                {
                                    continue;// not in this segment.
                                }
                                else
                                {
                                    if (flist.Count == 0)
                                    {
                                        NewSeekFrom = SeekFrom - fss.Start;
                                        ///NewSeekTo = SeekTo - fss.Start;
                                    }
                                    flist.Add(fss.FileName);
                                    if ((SeekTo + SeekFrom) < fss.Start + fss.Duration)
                                    {
                                        break;
                                    }
                                }
                            }
                            LineNum = 43;

                            if (flist.Count > 0)
                            {
                                SeekFrom = NewSeekFrom;
                                SeekTo = NewSeekTo;
                                job.ClearComplexList();
                                foreach (string se in flist)
                                {
                                    job.AddToComplexList($"file '{se}'");
                                }
                            }
                        }
                    }
                    LineNum = 44;
                    List<string> Files = new List<string>();
                    string filename = "";
                    LineNum = 45;
                    foreach (string sp in job.GetCutList())
                    {
                        filename = sp.Replace("file '", "").Replace("'", "");
                        Files.Add(filename);
                        string newstr = filename.Replace(myStrQuote, "");
                        double fs = new System.IO.FileInfo(newstr).Length / 1048576.00;
                        MSIZE = MSIZE + (int)Math.Round(fs);
                        LineNum = 46;
                    }
                    LineNum = 47;
                    ffmpegbridge bridge = new ffmpegbridge();
                    bridge.ReadDurations(Files);
                    while (!bridge.Finished)
                    {
                        Thread.Sleep(100);
                    }
                    LineNum = 48;
                    TotalSecs = bridge.GetDuration().TotalSeconds;
                    TimeSpan Dur = TimeSpan.Zero;
                    (videoStream, audioStream, Dur) = bridge.ReadMediaFile(Files[0]);
                    bridge = null;
                    LineNum = 49;
                }
                else
                {
                    LineNum = 50;
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(15));
                    while (!cts.IsCancellationRequested)
                    {
                        List<Process> Processes = Win32Processes.GetProcessesLockingFile(SourceFile);
                        if (Processes.Count == 0) cts.Cancel();

                        Thread.Sleep(100);
                    }

                    ffmpegbridge bridge = new ffmpegbridge();
                    TimeSpan Dur = TimeSpan.Zero;
                    (videoStream, audioStream, Dur) = bridge.ReadMediaFile(SourceFile);
                    if (audioStream.Channels == 0)
                    {
                        (videoStream, audioStream, Dur) = bridge.ReadMediaFile(SourceFile);
                    }
                    bridge = null;
                    LineNum = 51;
                    TotalSecs = Dur.TotalSeconds;
                    LineNum = 52;
                }
                LineNum = 53;
                ffmpeg.VideoCodec DecoderCodec = ffmpeg.VideoCodec.h264;
                double aspectratio = -1;
                LineNum = 54;
                if ((audioStream != null) && (videoStream != null))
                {
                    job.Is5K = (videoStream.Width > 4000) ? true : false;
                    LineNum = 55;
                    string codec = videoStream.Codec.ToString(), resize = string.Empty;
                    if (codec == ffmpeg.VideoCodec.h264.ToString()) DecoderCodec = ffmpeg.VideoCodec.h264;
                    if (codec == ffmpeg.VideoCodec.hevc.ToString()) DecoderCodec = ffmpeg.VideoCodec.hevc;
                    if (codec == ffmpeg.VideoCodec.mpeg4.ToString()) DecoderCodec = ffmpeg.VideoCodec.mpeg4;
                    if (codec == ffmpeg.VideoCodec.mpeg2video.ToString()) DecoderCodec = ffmpeg.VideoCodec.mpeg2video;
                    if (codec == ffmpeg.VideoCodec.msmpeg4v3.ToString()) DecoderCodec = ffmpeg.VideoCodec.msmpeg4v3;
                    double videowidth = videoStream.Width, videoheight = videoStream.Height;
                    if (this.IsChecked("ChkAutoAAC"))
                    {
                        LineNum = 56;
                        if (!audioStream.Codec.ToLower().Contains("vorbis") &&
                           (!audioStream.Codec.ToLower().Contains("ac-3")) &&
                           (!audioStream.Codec.ToLower().Contains("mp-3")))
                        {
                            audioStream.AAC_VbrMode(5, job.Is48K);
                        }
                        else
                        {
                            if (job.IsAc3_2Channel || job.IsAc3_2Channel)
                            {
                                audioStream.AAC_VbrMode(5, job.Is48K);
                            }
                            else
                            {
                                if (!job.IsMulti) audioStream.CopyStream();
                            }
                        }
                    }
                    else
                    {
                        if (job.IsAc3_2Channel || job.IsAc3_2Channel)
                        {
                            audioStream.AAC_VbrMode(5, job.Is48K);
                        }
                        else
                        {
                            if (_cmbaudiomodeSelectedIndex == -1)
                            {
                                if (!job.IsMulti) audioStream.CopyStream();
                            }
                            else
                            {
                                if (!job.IsMulti)
                                {
                                    audioStream = _cmbaudiomodeSelectedIndex == 0 ? audioStream.CopyStream() :
                                   (_cmbaudiomodeSelectedIndex == 1) ? audioStream.SetMp3(1024 * 128) :
                                   (_cmbaudiomodeSelectedIndex == 2) ? audioStream.AAC_VbrMode(5) : audioStream;
                                }
                            }
                        }
                    }
                    LineNum = 57;
                    if (chkresized) // svideowidth is the scalewidth, videowidth is the source width
                    {
                        LineNum = 58;
                        float svideowidth = VideoWidth.ToFloat();
                        aspectratio = videowidth / videoheight;
                        if (videowidth < svideowidth) videowidth = 0;/// dont resize..
                    }
                    LineNum = 59;
                    frames = videoStream.Framerate;
                    string videoinfo = $"{videoStream.Width}x{videoStream.Height}";
                    string newstr = SourceFile.Replace(myStrQuote, "");
                    double fs = 0;
                    LineNum = 60;
                    int filesize = 0;
                    if (!job.IsMulti)
                    {
                        LineNum = 61;
                        fs = new System.IO.FileInfo(newstr).Length / 1048576.00;
                        filesize = (int)Math.Round(fs);
                    }
                    LineNum = 63;
                    var dfile = (job.IsTwitchStream) ? DestFile : Path.GetFileName(DestFile);
                    var ConverterProgressDelegate = ConverterProgressEventHandler.AddNewProgressEventHandler(dfile);
                    var conversion = FFmpegCli.Converters.New(ConverterProgressDelegate);
                    bool Isinter = false, IsResize1080shorts = this.IsChecked("ChkResize1080shorts");
                    if (job.IsEdt)
                    {
                        LineNum = 64;
                        if (videowidth > 1080)
                        {
                            videoStream = videoStream.SetSize(720, -8, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                        }
                        if (videoStream.Framerate > 25)
                        {
                            videoStream = videoStream.SetFPS(25.0f);
                        }
                    }
                    else if (job.IsCST)
                    {
                        videoStream.CopyStream();
                        audioStream.CopyStream();
                        _isCopy = true;
                    }
                    else if (job.IsTwitchOut)
                    {
                        audioStream.CopyStream();
                        Encoder = ffmpeg.VideoCodec.h264_amf;
                        videoStream = videoStream.SetFPS(25.0f);
                    }
                    else if (job.IsTwitchActive)
                    {
                        if (videowidth > 1080)
                        {
                            videoStream = videoStream = videoStream.SetSize(1920, -8, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                        }
                        Encoder = ffmpeg.VideoCodec.h264_amf;
                    }
                    else if (job.IsTwitchStream && !job.twitchschedule.HasValue)
                    {
                        if (videowidth > 1080)
                        {
                            videoStream = videoStream = videoStream.SetSize(1920, -8, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                        }
                    }
                    else if ((IsResize1080shorts) || (job.FileNoExt.ToLower().Contains("shorts")))
                    {
                        if (!job.Is720P)
                        {
                            LineNum = 65;
                            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            int CropHeight = key.GetValueInt("CropHeight", 1080);
                            int CropWidth = key.GetValueInt("CropWidth", 608);
                            int CropLeft = key.GetValueInt("CropLeft", 0);
                            int CropTop = key.GetValueInt("CropTop", 0);
                            key?.Close();
                            LineNum = 66;
                            audioStream.AAC_VbrMode(5, job.Is48K);
                            if (videowidth > 1080)
                            {
                                videoStream = videoStream.SetSize(1920, 1080, -1, -1, Scaling.lanczos, CropWidth, CropHeight, CropLeft, CropTop, "9/16");
                            }
                            else if (videowidth == 1080)
                            {
                                videoStream.Crop(CropWidth, CropHeight, CropLeft, CropTop, "9/16");
                            }

                            if (videoStream.Framerate > 25)
                            {
                                videoStream = videoStream.SetFPS(25.0f);
                            }
                            LineNum = 67;

                        }
                    }
                    else if (job.Is4K || job.Is5K)
                    {
                        if (UseFisheyeRemoval)
                        {
                            videoStream.SetFishEyeRemoval(true, LRF, LLC, RRF, RLC);
                        }
                        else if (job.IsMulti)
                        {
                            if (job.DestMFile.Contains("(shorts)") || job.IsShorts)
                            {
                                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                int CropHeight = key.GetValueInt("CropHeight", 1080);
                                int CropWidth = key.GetValueInt("CropWidth", 608);
                                int CropLeft = key.GetValueInt("CropLeft", 0);
                                int CropTop = key.GetValueInt("CropTop", 0);
                                key?.Close();
                                LineNum = 70;
                                if (videowidth > 1080)
                                {
                                    videoStream = videoStream.SetSize(1920, 1080, -1, -1, Scaling.lanczos, CropWidth, CropHeight, CropLeft, CropTop, "9/16");
                                }
                                LineNum = 71;
                            }
                            else if (job.IsCST)
                            {
                                videoStream.CopyStream();
                                _isCopy = true;
                            }
                            else if (job.Is5K && !job.IsEdt)
                            {
                                videoStream = videoStream.SetSize(3840, 2160, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                            }
                        }
                    }
                    else
                    {
                        LineNum = 74;
                        if (!job.Is1080p)
                        {
                            LineNum = 75;
                            videoStream = videoStream.SetSize(VideoWidth.ToInt(), -1, aspectratio,
                                ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                        }
                        else
                        {
                            if (job.IsInterlaced)
                            {
                                videoStream.SetInterlaced();
                                Isinter = true;
                            }
                            /*if ((videoStream.Width > 1080) && (ChkResize1080p.IsChecked.Value)) // is 4k ?
                            {
                                videoStream = videoStream.SetSize(1080, -1, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                            } Handled in MediaTools */
                        }
                    }
                    LineNum = 76;
                    string currentpath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    bool mpeg2 = videoStream.Codec.ToString() != "mpeg2video";
                    if (!_GPUEncode)
                    {
                        LineNum = 77;
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        if (Encoder == ffmpeg.VideoCodec.libxvid)
                        {
                            LineNum = 78;
                            int minQ = key.GetValueInt("minq", 3), maxQ = key.GetValueInt("maxq", 13);
                            videoStream.Mpeg4ASP(minQ, maxQ);// ASP Set the video stream params here. -qmin 3 - qmax 5 - vtag = XVID -aspect 4:3
                        }
                        LineNum = 79;
                        if (Encoder == ffmpeg.VideoCodec.mpeg4)
                        {
                            LineNum = 80;
                            int qscale = key.GetValueInt("qscale", 15);
                            string vTag = key.GetValueStr("vTag", "XVID");
                            videoStream.Mpeg4AVC(qscale, vTag); // AVC / X264 -crf 18 , -preset slow -pix_fmt yuv420p
                        }
                        key?.Close();
                    }
                    LineNum = 80;
                    if ((DecoderCodec == ffmpeg.VideoCodec.hevc) && (HardwareAcceleration == HardwareAccelerator.qsv) && (Encoder == ffmpeg.VideoCodec.h264_nvenc))
                    {
                        (_GPUEncode, HardwareAcceleration) = (true, HardwareAccelerator.software);
                    }
                    LineNum = 81;
                    int MinRate = 0, MaxRate = 0, RateBuffer = 0;
                    if (MinBitRate != "")
                    {
                        int.TryParse(MinBitRate.Substring(0, MinBitRate.Length - 1), out MinRate);
                    }
                    if (MaxBitRate != "")
                    {
                        int.TryParse(MaxBitRate.Substring(0, MaxBitRate.Length - 1), out MaxRate);
                    }
                    if (BitRateBuffer != "")
                    {
                        int.TryParse(BitRateBuffer.Substring(0, BitRateBuffer.Length - 1), out RateBuffer);
                    }
                    decimal samplesize = 1;
                    LineNum = 82;
                    if (job.Is1080p && job.IsInterlaced) samplesize = 16;
                    if ((job.Is1080p && !job.IsInterlaced) || (job.Is4K && !job.IsInterlaced))
                    {
                        samplesize = (!job.Is4K) ? 6.0M : 23.5M;// was 6.5M : 30M
                        if (job.IsMulti)
                        {
                            if (job.DestMFile.Contains("(shorts)") || job.IsShorts) samplesize = 2.2M;

                        }
                        if (!job.IsTwitchStream)
                        {
                            DestFile = DestFile.Replace(".mkv", ".mp4");

                        }
                        else
                        {
                            samplesize = 5.4M;
                            audioStream.CopyStream();
                        }
                    }
                    LineNum = 84;
                    if (job.IsEdt) samplesize = 0.8M;
                    if (job.IsTwitchOut)
                    {
                        samplesize = 5.2M;
                    }

                    string MMFile = (job.IsMulti) ? Path.GetFileName(job.DestMFile) : "";
                    string _MinBitRate = (MinRate > 0) ? Math.Round((decimal)MinRate * samplesize).ToString() + "K" : MinBitRate;
                    string _MaxBitRate = (MaxRate > 0) ? Math.Round((decimal)MaxRate * samplesize).ToString() + "K" : MaxBitRate;
                    string _BitRateBuffer = (RateBuffer > 0) ? Math.Round((decimal)RateBuffer * samplesize).ToString() + "K" : BitRateBuffer;
                    LineNum = 85;

                    //string myStrQuote = "\"";
                    /*string ExeName = "ffmpeg.exe";
                    ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process where name = {myStrQuote}{ExeName}{myStrQuote}");
                    foreach (ManagementObject o in searcher.Get())
                    {
                        if (o["CommandLine"] != null)
                        {
                            string comstr = o["CommandLine"].ToString();
                            if (comstr.Contains("safe"))
                            {
                                x4kcnt++;
                            }
                        }
                    }*/

                    LockedDeviceID = 0;
                    conversion.AddStream(videoStream).AddStream(audioStream)
                                                     .SetOutput(DestFile, job.IsTwitchActive)
                                                     .SetSourceIndex(job.SourceFileIndex)
                                                     .SetTotalTime(totalseconds)
                                                     .SetSeek(SeekFrom).SetOutputTime(SeekTo)
                                                     .SetMultiModeFile(MMFile)
                                                     .SetOverlay(@"c:\videogui\logo1.png", job.IsShorts)
                                                     .SetConcat(job.IsMulti, job.GetCutList())
                                                     .SetMuxing(job.IsMuxed, job.MuxData)
                                                     .UseHardwareAcceleration(_GPUEncode ? HardwareAcceleration : HardwareAccelerator.software, DecoderCodec, Encoder, LockedDeviceID)
                                                     .SetVSync(VSyncEnable ? VsyncParams.vfr : VsyncParams.auto)
                                                     .SetVideoBitrate(_MinBitRate, _MaxBitRate, _BitRateBuffer, Encoder, job.IsComplex, job.ComplexMode, _isCopy);
                    LineNum = 86;
                    conversion.OnConverteringData += new ConverterOnDataEventHandler(OnConvertingEvent);
                    conversion.OnConverterProgress += new ConverterOnProgressEventHandler(OnProgressEvent);
                    conversion.OnConverterStopped += new ConverterOnStoppedEventHandler(OnFinished);
                    conversion.OnConverterOnSeek += new ConverterOnSeekEventHandler(OnSeek);
                    conversion.OnConverterStarted += new ConverterOnStartedEventHandler(OnStart);
                    /*
                     * 
                     * 
                     * 
                         string newstr = SourceFile.Replace(myStrQuote, "");
                    double fs = new System.IO.FileInfo(newstr).Length / 1048576.00;
                    int filesize = (int)Math.Round(fs);
                    */

                    LineNum = 87;
                    if (job.IsMulti)
                    {
                        filesize = MSIZE;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        LineNum = 88;
                        job.Fileinfo = $"[{videoinfo}][{filesize}M>]";
                        //lstBoxJobs.MinHeight = MainWindowX.Height - 220.63;
                        //lstBoxJobs.Height = lstBoxJobs.MinHeight;
                        (job.VideoInfo, job.TotalSeconds, job.Progress) = (videoinfo, TotalSecs, 0); ;
                    });
                    LineNum = 89;
                    string SourceFileNoExt = Path.GetFileNameWithoutExtension(job.Title);
                    ThreadStartTimeSet?.Invoke(SourceFileNoExt, DateTime.Now);
                    LineNum = 90;
                    try
                    {
                        LineNum = 91;
                        string sql = "";
                        int id = 0;
                        if (!job.IsTwitchStream)
                        {
                            id = GetIdFromProcessingLog(SourceFile, DestFile);
                            LineNum = 92;
                            if (id != -1)
                            {
                                sql = $"delete from ProcessingLog where ID = {id}";
                                sql.ToUpper().RunExecuteScalar(connectionString, -1);
                            }
                            LineNum = 93;
                            id = InsertIntoProcessingLog(SourceFile, DestFile);

                        }
                        LineNum = 94;
                        conversion.Start().ConfigureAwait(false);
                        LineNum = 95;
                    }
                    catch (Exception ex)
                    {
                        if (job.IsMulti)
                        {
                            LineNum = 96;
                            job.ProbeDate = DateTime.Now.AddYears(-500);
                            List<string> Cuts = job.GetCutList();
                            List<string> Files = Cuts.Select(sp => sp.Replace("file ", "").Replace("'", "").Trim()).ToList();
                            string dstfn = "", srcfile, destfn = Path.GetFileNameWithoutExtension(DestFile);
                            LineNum = 97;
                            if (!job.KeepSource)
                            {
                                LineNum = 99;
                                foreach (string pr in Files)
                                {
                                    LineNum = 100;
                                    if (!Path.GetFileName(pr).Contains($"{destfn}_"))
                                    {
                                        dstfn = ErrorDirectory + "\\" + destfn + "_" + Path.GetFileName(pr);
                                    }
                                    else destfn = ErrorDirectory + "\\" + Path.GetFileName(pr);
                                    LineNum = 101;
                                    MoveIfExists(pr, dstfn);
                                }
                                LineNum = 102;
                                dstfn = ErrorDirectory + "\\" + Path.GetFileName(job.MultiFile);
                                LineNum = 103;
                                MoveIfExists(job.MultiFile, dstfn);
                                LineNum = 104;
                            }
                        }
                        else MoveIfExists(SourceFile, ErrorDirectory + "\\" + Path.GetFileName(DestFile));
                        LineNum = 105;
                        this.Dispatcher.Invoke(() =>
                        {
                            LineNum = 106;
                            job.Fileinfo = "[" + videoinfo + "][ERROR]";// OK]";}
                            stats_handle?.Invoke(1, Path.GetFileNameWithoutExtension(DestFile));
                            LineNum = 107;
                            ex.LogWrite($"Int RunConversion LineNum {LineNum}" + MethodBase.GetCurrentMethod().Name.ToString() + " 7 " + ex.Message);
                            (job.InProcess, job.Processed, job.IsSkipped) = (false, true, true);
                            failed++;
                        });
                        ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                    }
                    finally
                    {
                        this.Dispatcher.Invoke(() => { currentjob++; });
                    }
                }
                else
                {
                    job.ProbeDate = DateTime.Now.AddYears(-500);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"RunConversion LineNum {LineNum} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void OnSeek(object sender, string filename, string seekinfo, int processid)
        {
            try
            {
                lock (thissLock)
                {
                    bool found = false;
                    foreach (var jo in ProcessingJobs.Where(jo => !jo.Complete && jo.FileNoExt == Path.GetFileNameWithoutExtension(filename)))
                    {
                        if (jo.Fileinfo.Contains("|"))
                        {
                            if (seekinfo != "")
                            {
                                List<string> FileInfos = jo.Fileinfo.Split('|').ToList();
                                jo.Fileinfo = FileInfos.FirstOrDefault() + "|File Seek @ " + seekinfo + "]";
                            }
                            else
                            {
                                List<string> FileInfos = jo.Fileinfo.Split('|').ToList();
                                jo.Fileinfo = FileInfos.FirstOrDefault();
                            }
                        }
                        else
                        {
                            jo.Fileinfo = jo.Fileinfo.Replace("]", "");
                            jo.Fileinfo += "|File Seek @ " + seekinfo + "]";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        /* unsafe private void RunCutProcessor(string SourceDir, string destfilename, string defaultext, TimeSpan Start, TimeSpan End)
         {
             try
             {
                 /*string rootpath = GetExePath();
                  string AvCodecPath  = Directory.EnumerateDirectories(rootpath, "avcodec*.dll", SearchOption.AllDirectories).ToList().FirstOrDefault();
                  AvCodecPath = Path.GetDirectoryName(AvCodecPath);

                  DynamicallyLoadedBindings.LibrariesPath = AvCodecPath;
                  DynamicallyLoadedBindings.Initialize();

                  List<string> Sourcefiles = Directory.EnumerateFiles(SourceDir, "*."+defaultext).ToList();

                  string SourceFile = Sourcefiles.FirstOrDefault();
                  if (SourceFile != null)
                  {
                      var pFormatContext = vectors.avformat_alloc_context();
                      var ret = vectors.avformat_open_input(&pFormatContext, SourceFile, null, null);
                      if (ret !=0)
                      {
                          throw new InvalidOperationException("Cant open file"); 
                      }
                      pFormatContext.

                  }



             }
             catch (Exception ex)
             {
                 ex.LogWrite(MethodBase.GetCurrentMethod().Name);
             }
         }
        */
        /*      unsafe private List<StreamInfo> ExtractStreams(FFmpeg.AutoGen.Abstractions.AVFormatContext* inputContext)
              {
                  try
                  {
                      /*FFmpeg.AutoGen.Abstractions.stre

                      List<StreamInfo> streams = new List<StreamInfo>();
                      if (inputContext->streams == null)
                      {
                          return null;
                      }
                      for (var i =0;i< inputContext->nb_streams;i++)
                      {
                          var s = inputContext->streams[i];
                          var codecContext = vectors.avcodec_alloc_context3(null);
                          FFmpeg.AutoGen.Abstractions.AVCodecParameters* parameters = null;


                          vectors.avcodec_parameters_to_context(codecContext, parameters);
                          parameters->



                          codecContext->properties = parameters->`    codec->properties;
                          codecContext->codec = s->codec->codec;
                          codecContext->qmin = s->codec->qmin;
                          codecContext->qmax = s->codec->qmax;
                          codecContext->coded_width = s->codec->coded_height;
                          codecContext->coded_height = s->codec->coded_width;


                          var stream = new StreamInfo();
                          {
                              StreamId = s->id,
                              StreamIndex = s->index,
                              Metadata = FFDictionary.ToDictionary(s->metadata),
                              CodecType = codecContext->codec_type,
                              CodecTypeName = ffmpeg.av_get_media_type_string(codecContext->codec_type),
                              Codec = codecContext->codec_id,
                              CodecName = ffmpeg.avcodec_get_name(codecContext->codec_id),
                              CodecProfile = ffmpeg.avcodec_profile_name(codecContext->codec_id, codecContext->profile),
                              ReferenceFrameCount = codecContext->refs,
                              CodecTag = codecContext->codec_tag,
                              PixelFormat = codecContext->pix_fmt,
                              FieldOrder = codecContext->field_order,
                              IsInterlaced = codecContext->field_order != AVFieldOrder.AV_FIELD_PROGRESSIVE &&
                                                codecContext->field_order != AVFieldOrder.AV_FIELD_UNKNOWN,
                              ColorRange = codecContext->color_range,
                              PixelWidth = codecContext->width,
                              PixelHeight = codecContext->height,
                              HasClosedCaptions = (codecContext->properties & ffmpeg.FF_CODEC_PROPERTY_CLOSED_CAPTIONS) != 0,
                              IsLossless = (codecContext->properties & ffmpeg.FF_CODEC_PROPERTY_LOSSLESS) != 0,
                              BitRate = bitsPerSample > 0 ?
                                              bitsPerSample * codecContext->channels * codecContext->sample_rate :
                                              codecContext->bit_rate,
                              MaxBitRate = codecContext->rc_max_rate,
                              InfoFrameCount = s->codec_info_nb_frames,
                              TimeBase = s->time_base,
                              SampleFormat = codecContext->sample_fmt,
                              SampleRate = codecContext->sample_rate,
                              DisplayAspectRatio = dar,
                              SampleAspectRatio = sar,
                              Disposition = s->disposition,
                              StartTime = s->start_time.ToTimeSpan(s->time_base),
                              Duration = s->duration.ToTimeSpan(s->time_base),
                              FPS = s->avg_frame_rate.ToDouble(),
                              TBR = s->r_frame_rate.ToDouble(),
                              TBN = 1d / s->time_base.ToDouble(),
                              TBC = 1d / s->codec->time_base.ToDouble()
                          };

                          // Extract valid hardware configurations
                          stream.HardwareDevices = HardwareAccelerator.GetCompatibleDevices(stream.Codec);
                          stream.HardwareDecoders = GetHardwareDecoders(stream.Codec);

                          // TODO: I chose not to include Side data but I could easily do so
                          // https://ffmpeg.org/doxygen/3.2/dump_8c_source.html
                          // See function: dump_sidedata
                          ffmpeg.avcodec_free_context(&codecContext);

                          result.Add(stream);
                      }

                      return streams;
                      return null;
                  }
                  catch (Exception ex)
                  {
                      ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                      return null;
                  }
              }
        */
        private void OnStart(object sender, string filename, int processid)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => OnStart(sender, filename, processid));
                    return;
                }
                int ID = -1;
                foreach (var jo in ProcessingJobs.Where(jo => !jo.Complete && jo.FileNoExt == Path.GetFileNameWithoutExtension(filename)))
                {
                    (jo.Handle, jo.InProcess, jo.ProbeLock, ID) = (processid.ToString(), true, false, jo.DeletionFileHandle);
                    break;
                }
                if (ID != -1)
                {
                    for (int i = 0; i < ComplexProcessingJobList.Count; i++)
                    {
                        if (ComplexProcessingJobList[i].Id.ToInt() == ID)
                        {
                            ComplexProcessingJobList[i].IsLocked = true;
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private async Task<int> GetTotalsAsync(string filename)
        {
            try
            {
                int res = 0;
                ffmpegbridge FileIndexer = new ffmpegbridge();
                FileIndexer.ReadDuration(filename);
                while (!FileIndexer.Finished)
                {
                    Thread.Sleep(100);
                }
                res = FileIndexer.GetDuration().TotalSeconds.ToInt();
                FileIndexer = null;
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        private void OnFinished(object Sender, string filename, int processid, int ExitCode, List<string> errordetails)
        {
            lock (thisfLock)
            {
                string fn1 = Path.GetFileName(filename);
                int _7p = _720PFiles.IndexOf(fn1);
                int _1080p = _1080PFiles.IndexOf(fn1);
                int _4K = _4KFiles.IndexOf(fn1);
                if (_7p != -1) _720PFiles.RemoveAt(_7p);
                if (_1080p != -1) _1080PFiles.RemoveAt(_1080p);
                if (_4K != -1) _4KFiles.RemoveAt(_4K);
                foreach (var cpt in ProgressInfoDisplay)
                {
                    if (cpt.DestName == fn1)
                        cpt.ShowOnExit();
                }
            }


            int LinePos = 0;
            try
            {
                if (errordetails != null)
                {
                    if (ExitCode != 0)
                    {
                        errordetails.WriteLog(filename);
                    }
                }
                lock (thisfLock)
                {
                    bool found = false, IsMSJ = false, IsNVM = false, IsCreateShorts = false;
                    foreach (var jo in ProcessingJobs.Where(jo => !jo.Complete && jo.FileNoExt == Path.GetFileNameWithoutExtension(filename)))
                    {
                        found = true;
                        IsNVM = jo.IsNVM;
                        IsMSJ |= jo.IsMSJ;
                        IsCreateShorts = jo.IsCreateShorts;
                        (jo.Processed, jo.Handle, jo.Progress, jo.InProcess, jo.Complete) = (true, "", 100, false, true);
                        string fm = filename;
                        LinePos = 1;
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromSeconds(15));
                        while (!cts.IsCancellationRequested)
                        {
                            List<Process> Processes = Win32Processes.GetProcessesLockingFile(filename.Replace("\"", "").Trim());
                            if (Processes.Count == 0) cts.Cancel();

                            Thread.Sleep(100);
                        }
                        var tff = new System.IO.FileInfo(filename.Replace("\"", "").Trim());
                        double filesize2 = tff.Length / 1048576.00;

                        int fs2 = (jo.IsTwitchStream && jo.twitchschedule.HasValue) ? 0 : (int)Math.Round(filesize2);
                        LinePos = 2;
                        if (jo.SourceFile is null)
                        {
                            jo.SourceFile = Path.GetFileName(jo.DestMFile);
                        }
                        string srcp = Path.Combine(jo.SourcePath, jo.SourceFile);
                        if (!File.Exists(srcp))
                        {
                            srcp = Path.Combine(jo.SourcePath + "\\processing", jo.SourceFile);
                        }
                        string fn = jo.IsMulti ? jo.DestMFile : srcp, QtrStr = "\"";
                        string mdir = jo.DestMFile;
                        string sourcedir = (jo.Is4K) ? DestDirectory4K : (jo.Is1080p) ? DestDirectory1080p : DestDirectory720p;
                        string compdir = (jo.Is4K) ? DoneDirectory4K : (jo.Is1080p) ? DoneDirectory1080p : DoneDirectory720p;
                        if (jo.Is4KAdobe) sourcedir = DestDirectoryAdobe4K;
                        string destnFile = Path.Combine((ExitCode == 0) ? compdir : ErrorDirectory, jo.SourceFile);
                        fn = (fn is null) ? Path.Combine(sourcedir, jo.SourceFile) : fn;
                        if (jo.IsMulti) fn = jo.DestMFile;

                        if (jo.IsMuxed)
                        {
                            string md = Path.GetDirectoryName(jo.MultiSourceDir);
                            string fd = md.Split('\\').ToList().LastOrDefault();
                            string np = md.Replace(fd, "Filtered");
                            fn = Path.Combine(np,Path.GetFileNameWithoutExtension(jo.MultiSourceDir))+".mp4";
                        }
                        LinePos = 3;
                        var cts2 = new CancellationTokenSource();
                        cts2.CancelAfter(TimeSpan.FromSeconds(15));
                        while (!cts2.IsCancellationRequested)
                        {
                            List<Process> Processes = Win32Processes.GetProcessesLockingFile(fn);
                            if (Processes.Count == 0) cts2.Cancel();
                            Thread.Sleep(100);
                        }
                        var tff1 = new System.IO.FileInfo(fn);
                        double filesize = tff1.Length / 1048576.00;
                        int fs = 0, MSIZE = 0;
                        string myQuoteStr = "\"";
                        if (jo.IsMulti)
                        {
                            foreach (string sp in jo.GetCutList())
                            {
                                filename = sp.Replace("file '", "").Replace("'", "");
                                string newstr = filename.Replace(myQuoteStr, "");
                                double f1s = new System.IO.FileInfo(newstr).Length / 1048576.00;
                                MSIZE = MSIZE + (int)Math.Round(f1s);
                            }
                            fs = MSIZE;
                        }
                        else
                        {
                            fs = (int)Math.Round(filesize);
                        }
                        LinePos = 4;
                        if (jo.IsMulti)
                        {
                            if (!IsNVM)
                            {
                                string newfileext = (jo.IsEdt) ? ".dedt" : ".dsr";
                                string fnx = Path.GetFileNameWithoutExtension(jo.MultiFile);
                                string fnxs = fnx + newfileext;
                                string dmo = Path.GetDirectoryName(jo.MultiFile);
                                string xt = Path.GetExtension(jo.MultiFile);
                                string dsrfile = Path.Combine(dmo, fnx + newfileext);
                                MoveIfExists(jo.MultiFile, dsrfile);
                            }
                            else
                            {
                                string sql = "insert into AutoInsertHistory(srcdir, destfname ,StartPos, Duration , b720p, " +
                                    "bShorts , bCreateShorts, bEncodeTrim ,bCutTrim, bMonitoredSource ,bPersistentJob , " +
                                    "BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID, ISMUXED,MUXDATA)" +
                                    " select srcdir,destfname ,StartPos, Duration , b720p, bShorts , bCreateShorts, bEncodeTrim ,bCutTrim," +
                                    $" bMonitoredSource ,bPersistentJob , BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID,ISMUXED,MUXDATA from AutoInsert where id " +
                                    $"= {jo.DeletionFileHandle} RETURNING ID;";

                                int idxx = sql.RunExecuteScalar(connectionString, -1);
                                // Update DeletionDate
                                if (idxx != -1)
                                {
                                    DateOnly DTS = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                                    ModifyHistory(idxx, DTS);
                                }
                                DeleteRecord(jo.DeletionFileHandle, true);
                                DeleteFromAutoInsertTable(jo.DeletionFileHandle);
                                InsertIntoDeletionTable(Path.GetFileName(jo.DestMFile));
                                if (IsMSJ)
                                {
                                    DeleteIfExists(jo.SourceFile);
                                }
                                if (IsCreateShorts)
                                {
                                    ShortsProcessors.Add(new ShortsProcessor(mdir.Replace("(shorts)", "(shorts_logo)"), DoOnNewShort, DoOnShortsDone));
                                }
                            }
                        }
                        else
                        {
                            if (jo.Is4KAdobe)
                            {
                                string df = Path.Combine(DestDirectoryAdobe4K, jo.SourceFile);
                                if (File.Exists(df))
                                {
                                    var FileIndexer = new ffmpegbridge();
                                    FileIndexer.ReadDuration(df);
                                    while (!FileIndexer.Finished)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    var _TotalSeconds = (double)FileIndexer.GetDuration().TotalSeconds;
                                    FileIndexer = null;
                                    if (_TotalSeconds == 0)
                                    {
                                        ExitCode = -1;
                                    }
                                }
                            }

                            if ((!jo.IsTwitchOut) && (File.Exists(filename) || ExitCode != 0) && (!jo.IsMuxed))
                            {
                                string PathDestnfile = Path.GetDirectoryName(destnFile);
                                if (!Directory.Exists(PathDestnfile))
                                {
                                    Directory.CreateDirectory(PathDestnfile);
                                }
                                MovedIfExists(fn, destnFile);
                            }
                        }


                        string fps = this.GetContent("lblFrames");
                        string[] comps = {"lblBitrate","lblDuration", "lblEta", "lblFrames",
                                 "lblPercent", "lblSpeed", "lblTotalTime" ,"lblCurrentFrames"};
                        this.ClearContents(comps);
                        this.IncreaseProgressValue("Progressbar2");
                        LinePos = 5;
                        if (!jo.IsTwitchActive)
                        {
                            if (fs2 > 0)
                            {
                                jo.Fileinfo = $"[{jo.VideoInfo}][{fs}M>{fs2}M]";
                            }
                        }
                        else
                        {
                            jo.Fileinfo = $"[{jo.VideoInfo}][Twitch Stream]";
                            jo.IsTwitchStream = false;
                        }
                        string newdest = (jo.IsMulti) ? jo.DestMFile : filename;
                        Double Totals = jo.TotalSeconds;
                        if (jo.IsCST || jo.IsCET) // CET MOD
                        {
                            TimeSpan EndTime = TimeSpan.Parse(jo.EndPos);
                            TimeSpan StartTime = TimeSpan.Parse(jo.StartPos);
                            if (EndTime != TimeSpan.Zero)
                            {
                                Totals = EndTime.TotalSeconds - StartTime.TotalSeconds;
                            }

                        }

                        
                        if (!jo.IsShorts|| jo.IsMuxed)
                        {
                            DoAsyncFinish(jo.FileNoExt, newdest, Totals, fs, fs2, fps, filename, jo.IsComplex || jo.IsCST, jo.IsTwitchStream, jo.IsMuxed).ConfigureAwait(false);
                        }
                    }
                    if (!found)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void DoOnShortsDone(int shortnum, string shortname)
        {
            Task.Run(() => { DoAsyncShortsDone(shortnum, shortname); });
        }


        public Task ProcessShortsFile(string source, string image, string orginalfile)
        {
            try
            {


                return Task.CompletedTask;
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }

        public Task DoAsyncShortsDone(int shortnum, string shortname)
        {
            try
            {
                for (int i = 0; i < ProcessingJobs.Count - 1; i++)
                {
                    if (ProcessingJobs[i].SourceFile == shortname)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProcessingJobs[i].VideoInfo = $"{shortnum} Shorts Created";
                        });
                        break;
                    }
                }

                for (int i = 0; i < ShortsProcessors.Count - 1; i++)
                {
                    if (ShortsProcessors[i].SourceFile == shortname)
                    {
                        Thread.Sleep(100);
                        ShortsProcessors[i].Dispose();
                        ShortsProcessors.RemoveAt(i);
                        break;
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }
        public void DoOnNewShort(int shortnum, string shortname)
        {
            Task.Run(() => { DoAsyncOnNewShort(shortnum, shortname); });
        }

        public Task DoAsyncOnNewShort(int shortnum, string shortname)
        {
            try
            {
                for (int i = 0; i < ProcessingJobs.Count - 1; i++)
                {
                    if (ProcessingJobs[i].SourceFile == shortname)
                    {
                        ProcessingJobs[i].VideoInfo = $"Creating Shorts : {shortnum} Created";
                        break;
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }
        public async Task DoAsyncFinish(string sourcefile, string destfile, double TotalSeconds, 
            int fs, int fs2, string fps, string filename, bool _IsComplex, bool isTwitchStream, bool IsMuxed)
        {
            try
            {
                double _TotalSeconds = 0;
                if (!isTwitchStream)
                {
                    var FileIndexer = new ffmpegbridge();
                    FileIndexer.ReadDuration(destfile);
                    while (!FileIndexer.Finished)
                    {
                        Thread.Sleep(100);
                    }

                    _TotalSeconds = (double)FileIndexer.GetDuration().TotalSeconds;
                    FileIndexer = null;
                }
                if ((IsMuxed||isTwitchStream) || (Math.Round(_TotalSeconds) == Math.Truncate(TotalSeconds)) || (_IsComplex) || Math.Round(TotalSeconds * 0.98) < (Math.Round(_TotalSeconds)))
                {
                    Passed++;
                    lblFailpass.AutoSizeLabel(Passed.ToString() + "/" + failed.ToString());
                    string SourceFileNoExt = sourcefile;
                    DateTime Starter = DateTime.Now.AddYears(-100);
                    Starter = (ThreadStartTimeGet != null) ? ThreadStartTimeGet.Invoke(SourceFileNoExt) : Starter;
                    ThreadStatsHandler?.Invoke(1, sourcefile);

                    TimeSpan TimeToProcess = DateTime.Now - Starter;
                    string info = "", total = TimeToProcess.ToCustomTimeString();
                    int totalyears = TotalYears(DateTime.Now, Starter);
                    total = (Math.Abs(totalyears) > 50) ? "" : total;
                    if (ProcessingTimeGlobal == TimeSpan.Zero)
                    {
                        ProcessingTimeGlobal = TimeToProcess; // time taken to process file;
                    }
                    else
                    {
                        ProcessingTimeGlobal += TimeToProcess;
                    }
                    info = $"[{total}@{fps}fps]";
                    bool IsTwitchActive = false, KeepSource = false, Is1080p = false, IsComplex = false, Is4K = false, IsSrc = false, IsMulti = false, IsAdobe = false;
                    List<string> Cuts = new List<string>();
                    string SourceFileIs = "", destmfile = "", Multifile = "", DestMFile = "", Title = "";
                    bool IsNVM = false, IsMonitoredSource = false, bIsMuxed = false ;
                    int ID = -1;
                    for (int jindex = 0; jindex < ProcessingJobs.Count(); jindex++)
                    {
                        if (ProcessingJobs[jindex].FileNoExt == sourcefile)
                        {
                            IsNVM = ProcessingJobs[jindex].IsNVM;
                            ID = ProcessingJobs[jindex].DeletionFileHandle;
                            SourceFileIs = ProcessingJobs[jindex].SourceFile;
                            IsSrc = ProcessingJobs[jindex].IsMulti;
                            IsMonitoredSource = ProcessingJobs[jindex].IsNVM;
                            IsAdobe = ProcessingJobs[jindex].Is4KAdobe;
                            IsTwitchActive = ProcessingJobs[jindex].IsTwitchActive;
                            bIsMuxed = ProcessingJobs[jindex].IsMuxed;
                            destmfile = (IsSrc) ? ProcessingJobs[jindex].DestMFile : ""; ;
                            Multifile = (IsSrc) ? ProcessingJobs[jindex].MultiFile : ""; ;
                            if (IsSrc) Cuts.AddRange(ProcessingJobs[jindex].GetCutList());
                            if (SourceFileIs == "" || SourceFileIs is null || !File.Exists(SourceFileIs))
                            {
                                SourceFileIs = ProcessingJobs[jindex].SourcePath + "\\" +
                                    ProcessingJobs[jindex].SourceFile;
                            }
                            ProcessingJobs[jindex].Processed = true;
                            ProcessingJobs[jindex].InProcess = false;
                            ProcessingJobs[jindex].Handle = "";
                            KeepSource = ProcessingJobs[jindex].KeepSource;
                            Is1080p = ProcessingJobs[jindex].Is1080p;
                            Is4K = ProcessingJobs[jindex].Is4K;
                            IsComplex = ProcessingJobs[jindex].IsComplex;
                            DestMFile = ProcessingJobs[jindex].DestMFile;
                            IsMulti = ProcessingJobs[jindex].IsMulti;
                            Title = ProcessingJobs[jindex].Title;
                            if (IsComplex)
                            {
                                Cuts.AddRange(ProcessingJobs[jindex].GetCutList());
                            }
                            if (!IsTwitchActive)
                            {
                                if (fs2 > 0)
                                {
                                    if (bIsMuxed) 
                                    {
                                        info = "Muxed Ok";
                                        ProcessingJobs[jindex].Fileinfo = $"[{ProcessingJobs[jindex].VideoInfo}][{fs}M]{info}";// OK]"; 
                                    }
                                    else ProcessingJobs[jindex].Fileinfo = $"[{ProcessingJobs[jindex].VideoInfo}][{fs}M>{fs2}M]{info}";// OK]"; 
                                    ProcessingJobs[jindex].Progress = 100;
                                }
                                else
                                    ProcessingJobs[jindex].Progress = 0;
                            }
                            else
                            {
                                ProcessingJobs[jindex].Fileinfo = $"[Twitch Stream Complete {ProcessingJobs[jindex].VideoInfo}][{fs}{DateTime.Now}]{info}";// OK]"; 
                                ProcessingJobs[jindex].Progress = 100;
                            }
                            break;
                        }
                    }
                    if (!isTwitchStream) AddToRecentDocs(DestFile);

                    if (!File.Exists(SourceFileIs) && !IsMuxed)
                    {
                        string SourceDirectory = (Is4K) ? SourceDirectory4K : (Is1080p) ? SourceDirectory1080p : SourceDirectory720p;
                        if (IsAdobe) SourceDirectory = SourceDirectoryAdobe4K;

                        List<string> files = Directory.EnumerateFiles(SourceDirectory, SourceFileIs, SearchOption.AllDirectories).ToList();
                        if (files.Count > 0)
                        {
                            SourceFileIs = files.FirstOrDefault();
                        }
                    }
                    string dDir = (Is4K) ? DoneDirectory4K : (Is1080p) ? DoneDirectory1080p : DoneDirectory720p;
                    if (IsAdobe) dDir = DoneDirectoryAdobe4K;

                    string destdir = dDir + "\\" + Path.GetFileName(SourceFileIs.Replace("\"", ""));
                    if (!IsMuxed)
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromSeconds(15));
                        while (!cts.IsCancellationRequested)
                        {
                            List<Process> Processes = Win32Processes.GetProcessesLockingFile(SourceFileIs);
                            if (Processes.Count == 0) cts.Cancel();

                            Thread.Sleep(100);
                        }
                        if (!IsMulti && (DoesDeletionFileExist(SourceFileIs)))
                        {
                            DeleteFromDeletionTable(SourceFileIs);
                        }
                    }
                     
                    if (IsMuxed)
                    {
                        DeleteFromAutoInsertTable(ID); 
                    }
                    else if (IsSrc)
                    {
                        List<string> Files = Cuts.Select(sp => sp.Replace("file ", "").Replace("'", "").Trim()).ToList();
                        string dstfn = "", srcfile, destfn = Path.GetFileNameWithoutExtension(Title);
                        DeleteFromAutoInsertTable(ID);
                        if (!KeepSource && !IsMuxed)
                        {
                            foreach (string pr in Files)
                            {
                                if (!Path.GetFileName(pr).Contains($"{destfn}_"))
                                {
                                    dstfn = dDir + "\\" + destfn + "_" + Path.GetFileName(pr);
                                }
                                else destfn = dDir + "\\" + Path.GetFileName(pr);
                                MoveIfExists(pr, dstfn);
                            }
                            dstfn = dDir + "\\" + Path.GetFileName(Multifile);
                            MoveIfExists(Multifile, dstfn);
                        }
                    }
                    else if (!IsMuxed)
                    {
                        var t = GetDesinationFromLog(SourceFileIs);
                        bool RunDelete = true;
                        if (t is string DestSqlFile)
                        {
                            if (!File.Exists(DestSqlFile))
                            {
                                RunDelete = false;
                            }
                        }
                        string SQL = $"Delete from ProcessingLog where Source = {SourceFileIs};";
                        SQL.ToUpper().RunExecuteScalar(connectionString, -1);
                        if (!isTwitchStream && !IsMuxed)
                        {
                            if ((IsNVM) && (DoesDeletionFileExist(Path.GetFileName(destdir))))
                            {
                                DeleteFromDeletionTable(Path.GetFileName(destdir));
                                if (RunDelete) DeleteIfExists(SourceFileIs);
                            }
                            else
                            {
                                if (RunDelete) MoveIfExists(SourceFileIs, destdir);
                            }
                        }
                    }
                }
                else
                {
                    ThreadStatsHandler?.Invoke(1, sourcefile);
                    failed++;
                    lblFailpass.AutoSizeLabel(Passed.ToString() + "/" + failed.ToString());
                    string destdir = ErrorDirectory + "\\" + Path.GetFileName(filename.Replace("\"", "")), sc = "";
                    bool IsComplex = false;
                    bool Is4kAdobe = false;
                    string dfile = "";
                    for (int jindex = 0; jindex < ProcessingJobs.Count(); jindex++)
                    {
                        if (ProcessingJobs[jindex].FileNoExt == sourcefile)
                        {
                            Is4kAdobe = ProcessingJobs[jindex].Is4KAdobe;
                            sc = ProcessingJobs[jindex].SourceFile;
                            IsComplex = ProcessingJobs[jindex].IsComplex;
                            dfile = ProcessingJobs[jindex].SourcePath;
                            if (Is4kAdobe)
                            {
                                ProcessingJobs.RemoveAt(jindex);
                            }
                            break;
                        }
                    }
                    if (sc != "" && !Is4kAdobe)
                    {
                        MoveIfExists(sc, destdir);
                    }
                    string mismatch = $"Dest {Math.Round(_TotalSeconds)} Source {Math.Truncate(TotalSeconds)}";
                    if (Is4kAdobe && !IsMuxed)
                    {
                        string df = Directory.EnumerateFiles(dfile, $"{sourcefile}.mp4", SearchOption.AllDirectories).ToList().FirstOrDefault();
                        if (File.Exists(df))
                        {
                            File.Delete(df);
                        }
                        LogWrite($"file re-que for {Path.GetFileNameWithoutExtension(sourcefile)} as size mismatch issue.");
                    }
                    else LogWrite($"{sourcefile} Failed Due to size mismatch {mismatch}");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void FilterHistoric()
        {
            try
            {
                var CollectionView = new CollectionViewSource();
                CollectionView.Source = ComplexProcessingJobHistory;
                CollectionView.Filter += new FilterEventHandler(CollectionFilter);
                complexfrm.lstSchedules.ItemsSource = (System.Collections.IEnumerable)CollectionView;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void CollectionFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public int TotalYears(DateTime start, DateTime end)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    int res = 0;
                    Dispatcher.Invoke(() => res = TotalYears(start, end));
                    return res;
                }
                return (end.Year - start.Year - 1) +
                (((end.Month > start.Month) ||
                ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -100;
            }
        }
        public async Task RunGrabUpdate(string URL)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => RunGrabUpdate(URL));
                    return;
                }
                var progress = new ProgressMessageHandler();
                progress.HttpReceiveProgress += Progress_HttpReceiveProgressUpdate;
                var client = HttpClientFactory.Create(progress);
                using (var response = await client.GetStreamAsync(URL))
                {
                    Process thisprocess = Process.GetCurrentProcess();
                    string me = thisprocess.MainModule.FileName;
                    canclose = true;
                    string myfile = Assembly.GetExecutingAssembly().GetName().ToString();
                    string newfile = Path.ChangeExtension(me, ".bak");
                    MoveIfExists(me, newfile);
                    string SourceAssembly = Path.ChangeExtension(me, ".zip");
                    Thread.Sleep(10);
                    using (ZipArchive zipArchive = new ZipArchive(response, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries)
                        {
                            zipEntry.ExtractToFile(Path.ChangeExtension(me, ".exe"));
                        }
                    }
                    var spawn = Process.Start(me);
                    thisprocess.CloseMainWindow();
                    thisprocess.Close();
                    thisprocess.Dispose();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Progress_HttpReceiveProgressUpdate(object? sender, HttpProgressEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Progress_HttpReceiveProgressUpdate(sender, e));
                }
                if ((e.BytesTransferred == e.TotalBytes))
                {
                    string TitleA = Title.Substring(0, InitTitleLength) + " [unzipping VideoGui] ";
                    Title = TitleA;
                    if (e.ProgressPercentage == 100)
                    {
                        TitleA = Title.Substring(0, InitTitleLength);
                        Title = TitleA;
                    }
                }
                else
                {
                    string TitleA = Title.Substring(0, InitTitleLength) + " [downfAg VideoGui " + e.ProgressPercentage.ToString() + " %]";
                    Title = TitleA;
                    ffmpegready = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public enum ShellAddToRecentDocsFlags
        {
            Pidl = 0x001,
            Path = 0x002,
            PathW = 0x003
        }
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);
        public void AddToRecentDocs(string filename)
        {
            try
            {
                SHAddToRecentDocs(ShellAddToRecentDocsFlags.PathW, filename);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + "  " + ex.Message);
            }
        }
        public bool MovedIfExists(string filename, string newfile, string source = "")
        {
            try
            {
                string myQuoteStr = "\"";
                filename = filename.Contains(myQuoteStr) ? filename.Replace(myQuoteStr, "") : filename;
                newfile = newfile.Contains(myQuoteStr) ? newfile.Replace(myQuoteStr, "") : newfile;
                if (filename != newfile)
                {
                    if (System.IO.File.Exists(filename)) if (System.IO.File.Exists(filename))
                        {
                            DeleteIfExists(newfile);
                            System.IO.File.Move(filename, newfile);
                            return true;
                        }
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + "|" + source);
                return false;
            }
        }
        public void MoveIfExists(string filename, string newfile)
        {
            try
            {
                if (filename != null)
                {
                    string myQuoteStr = "\"";
                    filename = filename.Contains(myQuoteStr) ? filename.Replace(myQuoteStr, "") : filename;
                    newfile = newfile.Contains(myQuoteStr) ? newfile.Replace(myQuoteStr, "") : newfile;
                    if (System.IO.File.Exists(filename))
                    {
                        DeleteIfExists(newfile);

                        System.IO.File.Move(filename, newfile);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" | {filename} , {newfile}");
            }
        }
        public void DeleteIfExists(string filename)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public string CheckForGraphicsCard()
        {
            try
            {
                String Card = string.Empty;
                ManagementObjectSearcher searcher =
                 new("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                    PropertyData description = mo.Properties["Description"];
                    if (currentBitsPerPixel != null && description != null)
                    {
                        if ((description.Value.ToString().Contains("NVIDIA")) || ((description.Value.ToString().Contains("Radeon")) && (description.Value.ToString().Contains("AMD"))))
                        {
                            Card = description.Value.ToString();
                        }
                    }
                }
                return Card;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        private void OnConvertingEvent(object sender, string Data, string filename, int processid)
        {
            try
            {
                string _currentfile = Path.GetFileNameWithoutExtension(filename);
                this.Dispatcher.Invoke(() =>
                {
                    bool Is1080p = false;
                    if (filename != "")
                    {
                        foreach (var job in ProcessingJobs.Where(job => !job.Complete && job.FileNoExt == _currentfile))
                        {
                            (job.Handle, job.InProcess, job.Processed) = (processid.ToString(), true, false);
                            Is1080p = job.Is1080p && job.IsInterlaced;
                        }
                        ThreadStatsHandler?.Invoke(0, _currentfile);
                    }
                    if ((Data.ToString().Contains("bitrate")) && (Data.ToString().Contains("speed")))
                    {
                        string data = Data.ToString();
                        string framecp = data.Substring(0, data.IndexOf("fps"));
                        string currentframe = framecp.Substring(data.IndexOf("=")).Replace("=", "").Trim();
                        string bitrate = data.Substring(data.IndexOf("bitrate"), 18);
                        string Speed = data.Substring(data.IndexOf("speed"), 12);
                        Speed = Speed.Replace("speed=", "").Trim();
                        float Spd = Speed.Replace("x", "").ToFloat();
                        string totalb = bitrate.Substring(bitrate.IndexOf("=") + 1).Trim();
                        string framess = data.Substring(data.IndexOf("fps=") + 4, 6).Trim();
                        framess = framess.Replace("-", "").Replace("q", "").Replace("=", "").Trim();
                        totalb = totalb.Replace("kbit", string.Empty).Trim();

                        double framecalc = 0;
                        if (framess != "N/A")
                        {
                            framecalc = framess.ToDouble();
                        }
                        if (data != string.Empty)
                        {
                            ThreadUpdateSpeed?.Invoke(_currentfile, Spd, totalb.ToFloat(), Convert.ToInt32(Math.Floor(framecalc)), currentframe);
                            int index2 = 1;
                            foreach (JobListDetails jobb in ProcessingJobs)
                            {
                                if (jobb.FileNoExt == _currentfile) break;
                                index2++;
                            }
                            fileprogress = $"[{index2}/{ProcessingJobs.Count}] " + _currentfile + " " + Progressbar1.Value.ToString() + " %";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnProgressEvent(object sender, string filename, int Percent, TimeSpan Duration, TimeSpan Total, int processid)
        {
            try
            {
                string _currentfile = Path.GetFileNameWithoutExtension(filename);

                ThreadUpdateProgress?.Invoke(_currentfile, Percent, Duration, Total);
                Thread.Sleep(10);
                DateTime LastProgressEvent = DateTime.Now.AddYears(-1500);
                if (CanUpdate)
                {
                    for (int i = 0; i < ProcessingJobs.Count; i++)
                    {
                        if (ProcessingJobs[i].IsMulti)
                        {
                            ProcessingJobs[i].FileNoExt = Path.GetFileNameWithoutExtension(ProcessingJobs[i].DestMFile);
                        }
                    }
                    foreach (var job in ProcessingJobs.Where(job => !job.Complete && job.FileNoExt == _currentfile))
                    {
                        LastProgressEvent = job.LastProgressEvent;
                        if ((job.Fileinfo is not null) && ((job.Fileinfo == "") || (job.Fileinfo.Contains("Probing"))))
                        {
                            string sourcefile = job.SourcePath + "\\" + job.SourceFile;
                            if (job.IsMulti)
                            {
                                job.SourcePath = Path.GetDirectoryName(job.DestMFile);
                            }
                            if (!File.Exists(sourcefile))
                            {
                                sourcefile = Directory.EnumerateFiles(job.SourcePath, "*.*", SearchOption.AllDirectories).Where(filenames => filenames.Contains(job.Title)).FirstOrDefault();
                                if (sourcefile == "")
                                {
                                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                    string _SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                                    string _SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p", string.Empty);
                                    string _SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);

                                    string _SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);
                                    key?.Close();
                                    string regstr = (job.Is4K) ? _SourceDirectory4K : (job.Is1080p) ? _SourceDirectory1080p : _SourceDirectory720p;
                                    if (job.Is4KAdobe) regstr = _SourceDirectory4KAdobe;
                                    sourcefile = Directory.EnumerateFiles(regstr, "*.*", SearchOption.AllDirectories).Where(filenames => filenames.Contains(job.Title)).FirstOrDefault();
                                }
                            }
                            var tff1 = new System.IO.FileInfo(sourcefile);
                            if ((tff1 != null) && (tff1.Length != null))
                            {
                                double filesize = tff1.Length / 1048576.00;
                                int fs = (int)Math.Round(filesize);
                                job.Fileinfo = $"[{job.VideoInfo}][{fs}M>]";
                            }
                        }
                        foreach (var jk in ProcessingJobs.Where(jk => !jk.Complete && jk.FileNoExt == _currentfile).Where(JI => !JI.InProcess))
                        {
                            (jk.InProcess, jk.Processed) = (true, false);
                            break;
                        }
                        job.Progress = Percent;
                    }
                }
                DateTime TimeTaken = DateTime.Now, Starter = DateTime.Now.AddYears(-100);
                Starter = (ThreadStartTimeGet != null) ? ThreadStartTimeGet.Invoke(_currentfile) : Starter;
                TimeSpan ConversionTime = TimeTaken - Starter, TotalTimeTaken = TimeSpan.Zero;
                int totalyears = TotalYears(TimeTaken, Starter);
                lblQueInfo.AutoSizeLabel((ProcessingJobs.Count - currentjob).ToString());// + " / " + ProcessingJobs.Count.ToString() + " Files";
                if (Percent <= 1)
                {
                    switch (LastProgressEvent.Year)
                    {
                        case < 1800:
                            LastProgressEvent = DateTime.Now;
                            break;
                        default:
                            {
                                TimeSpan dts = DateTime.Now.Subtract(LastProgressEvent);
                                if (dts.TotalMilliseconds > 300)
                                {
                                    LastProgressEvent = DateTime.Now;
                                }
                                break;
                            }
                    }
                }
                if ((ConversionTime.TotalSeconds > 0) && (Percent > 0))
                {
                    double totaltime = ConversionTime.TotalSeconds / Percent;
                    double timeleft = (100 - Percent);
                    timeleft *= totaltime;
                    ThreadUpdateTime?.Invoke(0, _currentfile, TimeSpan.FromSeconds(timeleft));//     lblEta.Content = t.ToString(@"mm\:ss");
                }
                TotalTimeTaken = (ProcessingTimeGlobal) + (ConversionTime);
                if ((ConversionTime != TimeSpan.Zero) && (TotalTimeTaken != TimeSpan.Zero))
                {
                    TimeSpan RealTime = GetConversionTimes();
                    LblTotalTIMEAll.AutoSizeLabel(RealTime.ToCustomTimeString());
                    //lblTotalTime.AutoSizeLabel(RealTime.ToCustomTimeString());
                    ThreadUpdateTime?.Invoke(1, _currentfile, RealTime); // LblTotalTIME. = span3.ToString(@"mm\:ss");
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public TimeSpan GetConversionTimes()
        {
            try
            {
                TimeSpan Cov = ProcessingTimeGlobal;
                foreach (JobListDetails jl in ProcessingJobs.Where(job => job.InProcess && !job.IsSkipped))
                {
                    DateTime TimeTaken = DateTime.Now, Starter = DateTime.Now;
                    Starter = (ThreadStartTimeGet != null) ? ThreadStartTimeGet.Invoke(jl.FileNoExt) : Starter;
                    TimeSpan ConversionTime = TimeTaken - Starter;
                    Cov += ConversionTime;
                }
                return Cov;
            }
            catch (Exception ex)
            {
                return TimeSpan.Zero;
            }
        }
        public void OnConverionEnded(TimeSpan TimeTaken)
        {

        }

        /*public void _Getupdateffmpeg3()
        {
            try
            {
                GetUpdate("https://ottverse.com/ffmpeg-builds/", new Ottverse());
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }*/
        public void OnAllCoversionsFinished()
        {
            try
            {
                fileprogress = "[idle]";
                trayicon.ToolTipText = fileprogress;
                string icon = "pack://application:,,,/icons/computer.ico";
                trayicon.IconSource = new ImageSourceConverter().ConvertFromString(icon) as ImageSource;
                if (TrayIcon != null)
                {
                    TrayIcon.Stop();
                    TrayIcon.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public string SelectMasterDir(string SelectionText, string SettingsDirName)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                Root = key.GetValueStr("RootSelect", string.Empty);
                if (Root == "") Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                var dlg = new FolderBrowserDialog
                {
                    Title = SelectionText,
                    InitialFolder = Root,
                };
                string InitDir = key.GetValueStr(SettingsDirName, string.Empty);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if ((dlg.SelectedFolder != InitDir) && (LoadedKey)) key.SetValue(SettingsDirName, dlg.SelectedFolder);
                    Root = Path.GetPathRoot(dlg.SelectedFolder);
                    if (LoadedKey) key.SetValue("RootSelect", Root);
                    key?.Close();
                    return dlg.SelectedFolder;
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                return string.Empty;
            }
        }
        public void DownloadUpdateFolder()
        {  // downloads the encypted version.inc
            try
            {
                if (CheckInternet())
                {
                    string updatefolder = "https://drive.google.com/uc?export=download&id=1sarh7L6WnQqE_0DFJmwTIndDsLqxAIMe";
                    WebClient webClient = new WebClient();
                    string AppName = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    byte[] encrptedfile = webClient.DownloadData(new Uri(updatefolder));
                    int[] AccessKey = { 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99 };
                    EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
                    byte[] EncKey = { 00, 32, 67, 32, 00, 99, 77, 11, 44, 56, 78, 63, 52, 63, 95, 76 };
                    byte[] verisionidfile = EMP.RC4(encrptedfile, EncKey);
                    string[] vers = Encoding.ASCII.GetString(verisionidfile).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string versionnum = "", download = "", copyv = "";
                    bool found = false;
                    foreach (string ss in vers)
                    {
                        versionnum = ss.Split("|")[0].Substring(2);
                        string SourceAssembly = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + ".exe"; ;
                        var versionInfo = FileVersionInfo.GetVersionInfo(SourceAssembly);
                        if (ServerFileIsOlder(versionnum, versionInfo))
                        {
                            found = true;
                            download = ss.Split("|")[1];
                            copyv = versionnum;
                        }
                    }
                    if (found)
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Application Update " + copyv + " Available", "Download ? Yes/No", MessageBoxButtons.YesNo);
                        if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            downloadupdate(download);
                        }
                        else ffmpegready = true;
                    }
                }
                else
                {
                    ffmpegready = true;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void downloadupdate(string update)
        {
            try
            {
                string AppName = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string path2 = AppName + "\\videogui.zip";
                DeleteIfExists(path2);
                RunGrabUpdate(update).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " downloadupdate " + ex.Message);
            }
        }
        private void WriteStream(Stream mss, string filename)
        {
            try
            {
                DeleteIfExists(filename);
                mss.Position = 0;
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    mss.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " WebClientDownload_FFMPEG_Completed " + ex.Message);
            }
        }
        private void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Button_ClickAsync(sender, e));
                    return;
                }


                LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " Button_ClickAsync");
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                string SourceDirectory1080p = key.GetValueStr("SourceDirectory1080p", string.Empty);
                string SourceDirectory4k = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4kAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);
                key?.Close();
                string buttonname = (sender is System.Windows.Controls.Button buttonid) ? buttonid.Name : string.Empty;
                if ((SourceDirectory720p == string.Empty) || (buttonname == "btnselect"))
                    SelectMasterDir("Select Source Directory", "SourceDirectory");
                SelectFiles(SourceDirectory720p, SourceDirectory1080p, SourceDirectory4k, SourceDirectory4kAdobe);
            }
            catch (Exception ex)
            {
                ex.LogWrite("_" + MethodBase.GetCurrentMethod().Name + " " + ex.Message);
            }
        }
        public void SelectFiles(string SourceDirectory720p, string SourceDirectory1080p, string SourceDirectory4K, string SourceDirectory4KAdobe)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => SelectFiles(SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe));
                    return;
                }
                List<string> Source = new List<string>();
                string DownloadsDir = GetDownloadsFolder();
                string[] SourceDirs = { DownloadsDir, SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe };
                foreach (string SourceDir in SourceDirs)
                {
                    List<string> SourceList = Directory.EnumerateFiles(SourceDir, "*.*", SearchOption.AllDirectories).
                         Where(s => s.ToLower().EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                    if (SourceDir == SourceDirectory4KAdobe)
                    {
                        for (int i = SourceList.Count - 1; i >= 0; i--)
                        {
                            if (SourceList[i].Contains(@"FullLengths\"))
                            {
                                SourceList.RemoveAt(i);
                            }
                        }
                    }

                    foreach (string filename in SourceList)
                    {
                        string myfilename = Path.GetFileNameWithoutExtension(filename);
                        if (myfilename.ContainsAny(new List<string> { ".src", ".cst", ".edt" }))
                        {
                            string fn = File.ReadAllText(filename); // Select Files
                            List<string> Commands = fn.Split('|').ToList();
                            if (Commands.Count > 3) Commands.RemoveAt(0);
                            myfilename = Path.GetFileNameWithoutExtension(Commands.FirstOrDefault());
                        }
                        if (ProcessingJobs.Count(job => job.Title == myfilename) == 0)
                        {
                            AddIfVaid(filename, SourceDir);
                        }
                    }
                    SourceList.Clear();
                }
                lstBoxJobs.AllowDrop = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void Start()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Start());
                    return;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Stop();
                });

                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory = key.GetValueStr("SourceDirectory", string.Empty);
                DestDirectory720p = key.GetValueStr("DestDirectory720p", string.Empty);
                DestDirectory1080p = key.GetValueStr("DestDirectory1080p", string.Empty);
                DestDirectory4K = key.GetValueStr("DestDirectory4K", string.Empty);
                DestDirectoryAdobe4K = key.GetValueStr("DestDirectoryAdobe4k", string.Empty);
                DestDirectoryTwitch = key.GetValueStr("DestDirectoryTwitch", "");
                key.Close();
                SourceList.AddRange(ProcessingJobs.Select(jobs => jobs.SourcePath));
                Progressbar1.Maximum = 100;
                Progressbar1.Value = 0;
                List<Task> TaskList = new List<Task>();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                FileQueChecker.Start();
            }
            finally
            {
                processing = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Interval = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
                    FileQueChecker.Enabled = true;
                    FileQueChecker.Stop();
                    FileQueChecker.Start();
                });
            }
        }
        public async Task<bool> HasValidVideoStream(string newfile, JobListDetails job, int filenum = -1, int Max = -1)
        {
            bool isfound = false;
            int LineNum = 0;
            try
            {

                var ffprobe = FFmpegCli.Converters.New();
                job.ProbeLock = true;
                LineNum = 1;
                string fileid = ((filenum != -1) && (Max != -1)) ? $" {filenum} of {Max} - {Path.GetFileNameWithoutExtension(newfile)}" : "";
                LineNum = 2;
                ThreadStatsHandlerXtra?.Invoke(job.FileNoExt, $"[Probing File{fileid} For Video Stream]");
                ffprobe.OnProbeData += new ConverterProbDataEventHandler(OnProbeEvents);

                LineNum = 3;
                if (job.Is4K) _4KFiles.Add(Path.GetFileName(newfile));
                if (job.Is1080p) _1080PFiles.Add(Path.GetFileName(newfile));
                if (job.Is720P) _720PFiles.Add(Path.GetFileName(newfile));
                LineNum = 4;


                await ffprobe.ProbeFile(newfile, job.Is1080p);
                LineNum = 5;
                lock (thispLock)
                {
                    LineNum = 6;
                    ThreadStatsHandlerXtra?.Invoke(job.FileNoExt, "");
                    LineNum = 7;
                    if (ffprobe.ProbeResults.ToList().Count > 0)
                    {

                        string LastResult = ffprobe.ProbeResults.Last<string>();
                        LineNum = 8;
                        if (LastResult != null)
                        {
                            LineNum = 9;
                            isfound = LastResult.Contains("video:0kB");
                        }
                        LineNum = 10;
                        bool _IsInterlaced = false;
                        foreach (string data in ffprobe.ProbeResults)
                        {
                            if (data != null)
                            {
                                LineNum = 11;
                                if (data.Contains("missing picture") || data.Contains("Invalid NAL unit size"))
                                {
                                    //isfound = false;
                                    //break;
                                }
                                LineNum = 12;
                                if (data.Contains("Stream") && data.Contains("Video") &&
                                    data.Contains("yuv420p") && data.Contains("top first"))
                                {
                                    job.IsInterlaced = true;
                                }
                                LineNum = 13;
                                if (data.Contains("Stream") && data.Contains("Audio"))
                                {
                                    if (data.Contains("48000")) job.Is48K = true;
                                    if (data.Contains("ac3"))
                                    {
                                        job.IsAc3_2Channel = true;
                                    }
                                    else if (data.Contains("ac3") && data.Contains("5.1(side)"))
                                    {
                                        job.IsAc3_6Channel = true;
                                    }
                                }
                                LineNum = 14;
                            }
                        }
                        return isfound;
                    }
                    return isfound;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"HasValidVideoStream LineNum {LineNum} {MethodBase.GetCurrentMethod().Name}");
                return false;
            }
            finally
            {
                if (isfound)
                {
                    lock (thisfLock)
                    {
                        LineNum = 15;
                        string fn1 = Path.GetFileName(newfile);
                        int _7p = _720PFiles.IndexOf(fn1);
                        int _1080p = _1080PFiles.IndexOf(fn1);
                        int _4K = _4KFiles.IndexOf(fn1);
                        if (_7p != -1) _720PFiles.RemoveAt(_7p);
                        if (_1080p != -1) _1080PFiles.RemoveAt(_1080p);
                        if (_4K != -1) _4KFiles.RemoveAt(_4K);
                        LineNum = 16;
                    }
                }
            }
        }
        private void OnProbeEvents(object sender, int ProbeIDs, string Data)
        {
            try
            {
                string probchar = "";
                switch (ProbeIDs)
                {
                    case 0:
                        {
                            probchar = "//";
                            break;
                        }
                    case 1:
                        {
                            probchar = ".";
                            break;
                        }
                    case 2:
                        {
                            probchar = "-";
                            break;
                        }
                    case 3:
                        {
                            probchar = "|";
                            break;
                        }
                    case 4:
                        {
                            probchar = "\\";
                            break;
                        }
                }
                ThreadStatsHandlerXtra?.Invoke(Data, "[Probing File" + probchar + "]");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public async Task<bool> IsfinishedFileAsync(string newfile)
        {
            int LineNum = 0;
            bool Res = true;
            try
            {
                var ffprobe = FFmpegCli.Converters.New();
                LineNum = 1;
                newfile = !newfile.Contains(" ") ? newfile : $"{"\""}{newfile}{"\""}";
                LineNum = 2;
                ffprobe.OnProbeData += new ConverterProbDataEventHandler(OnProbeEvents);
                await ffprobe.ProbeFile(newfile, false);
                LineNum = 3;
                string ProbeData = "";
                foreach (string ProbeDetails in ffprobe.ProbeResults)
                {
                    if (!ProbeDetails.Contains("muxing") || ProbeDetails.Contains("File ended prematurely") || ProbeDetails.Contains("Duration: N/A"))
                    {
                        return false;
                    }
                }
                LineNum = 4;
                return Res;// (newfilesize.Duration.TotalSeconds >= (orginalfilesize.Duration.TotalSeconds * 0.95));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsfinishedFileAsync LineNo {LineNum} {MethodBase.GetCurrentMethod().Name}");
                return false;
            }
        }
        public byte[] _EncryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 57, 14, 2, 38, 49, 33, 44, 16, 28, 99, 00, 11, 31, 17, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 32, 33, 22, 27, 41, 44, 36, 72, 23, 32, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return encvar;
        }
        public string EncryptPassword(string password)
        {
            byte[] _Password = Encoding.ASCII.GetBytes(password);
            int[] AccessKey = { 30, 11, 32, 57, 14, 2, 38, 49, 33, 44, 16, 28, 99, 00, 11, 31, 17, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 32, 33, 22, 27, 41, 44, 36, 72, 23, 32, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_Password, EncKey);
            return Encoding.ASCII.GetString(encvar);
        }
        public async Task SetupHandlers()
        {
            try
            {
                SystemSetup = true;
                this.Dispatcher.Invoke(() =>
                {
                    lblSpeedStatus.SetLabelWidth();
                    lblCurrentFrameStatus.SetLabelWidth();
                    lblFramesStatus.SetLabelWidth();
                    lblBitrateStatus.SetLabelWidth();
                    lblTotalStatus.SetLabelWidth();
                    lblSpeedStatus.SetLabelWidth();
                    lblDurationStatus.SetLabelWidth();
                    lblETAStatus.SetLabelWidth();
                    lblAccelStatus.SetLabelWidth();
                    lblPercentStatus.SetLabelWidth();
                    lblTotalTimeStatus.SetLabelWidth();
                    lblQueStatus.SetLabelWidth();
                    lblPassFailStatus.SetLabelWidth();
                    lblFrames.Width = this.MeasureString("lblFrames", "www");
                    lblBitrate.Width = this.MeasureString("lblBitrate", "300 Kbit");
                    lblSpeed.Width = this.MeasureString("lblSpeed", "16X");
                    lblEta.Width = this.MeasureString("lblEta", "00:00");
                    lblDuration.Width = this.MeasureString("lblDuration", "00:00");
                    LblTotalTIMEAll.Width = this.MeasureString("LblTotalTIMEAll", "00:00");
                    ChkDropFormat.Click += new RoutedEventHandler(OnChkButton_Click);
                    ChkAutoAAC.Click += new RoutedEventHandler(OnChkButton_Click);
                    //ChkResize1080p.Click += new RoutedEventHandler(OnChkButton_Click);
                    //ChkResize1080shorts.Click += new RoutedEventHandler(OnChkButton_Click);
                    ChkChangeOutputname.Click += new RoutedEventHandler(OnChkButton_Click);
                    ChkReEncode.Click += new RoutedEventHandler(OnChkButton_Click);
                    Title += " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    X265Output.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    Fisheye.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    ChkAudioConversion.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    GPUEncode.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    X265Output.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    cmbH64Target.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    txtMaxShorts.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);


                    SystemSetup = false;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (!SystemSetup)
                {
                    string defaultvalue = string.Empty;
                    if (sender is System.Windows.Controls.TextBox FornTextBox)
                    {
                        string CompName2;
                        (CompName2, defaultvalue) = (FornTextBox.Name, FornTextBox.Text.Trim());
                        if (e.Key == Key.Enter)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnFocusChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SystemSetup)
                {
                    string defaultvalue = string.Empty, selectedindex = "2";
                    bool df = true;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser); ;
                    bool LoadedKey = (key != null);
                    if (sender is System.Windows.Controls.ComboBox _ComboBox)
                    {
                        string CompName;
                        (CompName, selectedindex) = (_ComboBox.Name, _ComboBox.SelectedIndex.ToString());
                        switch (CompName)
                        {

                            case "cmbH64Target":
                                {
                                    if (LoadedKey) key.SetValue("h264Target", selectedindex);
                                    break;
                                }
                        }
                        key.Close();
                    }
                    if (sender is ToggleButton ToggleBox)
                    {
                        string CompName;
                        (CompName, df) = (ToggleBox.Name, ToggleBox.IsChecked.Value);
                        switch (CompName)
                        {
                            case "Fisheye":
                                {
                                    if (LoadedKey) key.SetValue("FishEye", df);
                                    break;
                                }
                            case "GPUEncode":
                                {
                                    if (LoadedKey) key.SetValue("GPUEncode", df);
                                    break;
                                }
                            case "X265Output":
                                {
                                    if (LoadedKey) key.SetValue("X265", df);
                                    break;
                                }

                            case "ChkAudioConversion":
                                {
                                    if (LoadedKey) key.SetValue("AudioConversionAC3", df);
                                    break;
                                }
                        }
                        if (sender is System.Windows.Controls.TextBox FornTextBox)
                        {
                            string CompName2;
                            (CompName2, defaultvalue) = (FornTextBox.Name, FornTextBox.Text.Trim());
                            switch (CompName2)
                            {
                                case "txtscalewidth":
                                    {
                                        if (LoadedKey) key.SetValue("scalewidth", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtscaleheight":
                                    {
                                        if (LoadedKey) key.SetValue("scaleheight", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtscalemodulas":
                                    {
                                        if (LoadedKey) key.SetValue("scalemodulas", defaultvalue.ToInt());
                                        break;
                                    }

                                case "txtDestPath":
                                    {
                                        if (LoadedKey) key.SetValue("SourceDirectory", defaultvalue.ToString());
                                        break;
                                    }

                                case "txtminbitrate":
                                    {
                                        if (LoadedKey) key.SetValue("minbitrate", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtmaxbitrate":
                                    {
                                        if (LoadedKey) key.SetValue("maxbitrate", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtMaxShorts":
                                    {
                                        if (LoadedKey) key.SetValue("MaxShorts", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtbuffersize":
                                    {
                                        if (LoadedKey) key.SetValue("buffersize", defaultvalue.ToString());
                                        break;
                                    }
                                case "txtCompPath":
                                    {
                                        if (LoadedKey) key.SetValue("CompletedScanDir", defaultvalue.ToString());
                                        SaveComporitorList();
                                        break;
                                    }
                            }
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DirectoryThreads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCompairFinished = new CompairFinished(OnFinishCompair);
                DefaultDirectories ddthreaew = new(OnCompairFinished);
                Hide();
                ddthreaew.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnDestPath_Click " + ex.Message);
            }
        }
        public void OnChkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SystemSetup)
                {
                    string CompName = string.Empty;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    bool CompChecked = false, LoadedKey = (key != null);
                    if (sender is System.Windows.Controls.CheckBox FormCheckBox)
                    {
                        (CompName, CompChecked) = (FormCheckBox.Name, FormCheckBox.IsChecked.Value);
                        switch (CompName)
                        {
                            /*case "ChkResize10080shorts":
                                {
                                    if (LoadedKey) key.SetValue("Do1080pShorts", CompChecked);
                                    if (CompChecked)
                                    {
                                        ChkResize1080p.IsChecked = false;
                                    }
                                    break;
                                }*/
                           /* case "ChkResize1080p":
                                {
                                    if (LoadedKey) key.SetValue("resize1080p", CompChecked);
                                    if (CompChecked)
                                    {
                                        ChkResize1080shorts.IsChecked = false;
                                    }
                                    break;
                                }*/
                            case "chk480400fix":
                                {
                                    if (LoadedKey) key.SetValue("fix480to400", CompChecked);
                                    break;
                                }
                            case "chkmovecompleted":
                                {
                                    if (LoadedKey) key.SetValue("movecompleted", CompChecked);
                                    break;
                                }

                            case "chkAutoStart":
                                {
                                    if (LoadedKey) key.SetValue("autostart", CompChecked);
                                    break;
                                }
                            case "chkVSYnc":
                                {
                                    if (LoadedKey) key.SetValue("Vsync", CompChecked);
                                    break;
                                }
                            case "chkAutocrop":
                                {
                                    if (LoadedKey) key.SetValue("Autocrop", CompChecked);
                                    break;
                                }
                            case "ChkDropFormat":
                                {
                                    if (LoadedKey) key.SetValue("DropFormat", CompChecked);
                                    break;
                                }
                            case "ChkAAC":
                                {
                                    if (LoadedKey) key.SetValue("AutoAAC", CompChecked);
                                    break;
                                }
                            case "ChkVideoCopy":
                                {
                                    if (LoadedKey) key.SetValue("VideoCopy", CompChecked);
                                    break;
                                }
                            case "ChkChangeOutputname":
                                {
                                    if (LoadedKey) key.SetValue("ChangeFileName", CompChecked);
                                    break;
                                }


                            case "ChkAutoAAC":
                                {
                                    if (LoadedKey) key.SetValue("AutoAAC", CompChecked);
                                    break;

                                }
                            case "ChkReEncode":
                                {
                                    if (LoadedKey) key.SetValue("reencodefile", CompChecked);
                                    break;
                                }
                        }
                        key.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public string GetDownloadsFolder()
        {
            try
            {
                string Path = "";
                Guid FOLDERID_AppsFolder = new Guid("{374DE290-123F-4565-9164-39C4925E467B}");
                ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);
                if (appsFolder != null)
                {
                    Path = ((Microsoft.WindowsAPICodePack.Shell.FileSystemKnownFolder)appsFolder).Path;
                }
                return Path;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetDownloads Folder {MethodBase.GetCurrentMethod().Name}  {ex.Message}");
                return "";
            }
        }

        public void AudioJoiner_OnClose()
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));
                while (AutoJoinerFrm.IsActive && !cts.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                AutoJoinerFrm = null;
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AudioJoiner_OnClose {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        private void btnRunTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AutoJoinerFrm = new AudioJoiner(AudioJoiner_OnClose);
                Hide();
                Task.Run(() =>
                {
                    KillOrphanProcess("avidemux_cli.exe");
                });

                AutoJoinerFrm.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"RunTest {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        public void Test1()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string BaseDir = key.GetValueStr("BaseFixDir", @"c:\gopro9");
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder";
                folderBrowserDialog.InitialFolder = BaseDir;
                folderBrowserDialog.AllowMultiSelect = false;

                key?.Close();
                var folder = "";
                var selectresult = folderBrowserDialog.ShowDialog();
                if (selectresult == System.Windows.Forms.DialogResult.OK)
                {
                    folder = folderBrowserDialog.SelectedFolder;
                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("BaseFixDir", folder);
                    key?.Close();
                    bool done = false;
                    var files = Directory.EnumerateFiles(folder, "*.mp4", SearchOption.AllDirectories).ToList();
                    foreach (var _file in files)
                    {
                        if (Path.GetFileName(_file).Contains("(shorts)"))
                        {
                            string fna = Path.GetFileName(_file);
                            int mp = fna.IndexOf("(shorts)");
                            if (mp != -1)
                            {
                                string newfile = fna.Substring(mp + 9);
                                newfile = Path.GetDirectoryName(_file) + $"\\{newfile}";
                                if (newfile != "")
                                {
                                    File.Move(_file, newfile);
                                    done = true;
                                }
                            }

                        }
                    }
                    if (done) System.Windows.MessageBox.Show("Done");
                }
                //RunCutProcessor(@"D:\nvme\raw\Metro Dandenong To Flinders Street 150623", "D:\\nvme\\raw\\Metro Dandenong To Flinders Street 150623 test.mp4", "mp4", TimeSpan.Zero, TimeSpan.FromSeconds(100));
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        public void AddImportRecord(string f1, string f2, TimeSpan t1)
        {
            try
            {
                FileRenamer.Add(new FileInfoGoPro(f1, f2, t1));
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }


        public bool CheckImportRecords()
        {
            try
            {
                bool res = false;
                foreach (var f in FileRenamer)
                {
                    if (f.FileName == f.NewFile)
                    {
                        res = true;
                        break;
                    }
                }


                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
                return false;
            }
        }

        public void FileImportClear()
        {
            try
            {
                FileRenamer.Clear();
                ObservableCollectionFilter?.ClearImportTimes();


            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        public void ReOrderFilesInc(string txtsrcdir)
        {
            try
            {
                foreach (var r in FileRenamer)
                {
                    string Newfile = r.NewFile;
                    string OldFile = r.FileName;
                    if (Newfile != OldFile)
                    {
                        string newFile = System.IO.Path.Combine(txtsrcdir, Newfile);
                        string filename = System.IO.Path.Combine(txtsrcdir, OldFile);
                        System.IO.File.Move(filename, newFile);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SetImportFromTime(TimeSpan span)
        {
            try
            {
                ObservableCollectionFilter?.SetFromTimeSpan(span);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SetImportToTime(TimeSpan span)
        {
            try
            {
                ObservableCollectionFilter?.SetToTimeSpan(span);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void ClearImportTimes()
        {
            try
            {
                ObservableCollectionFilter?.ClearImportTimes();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void DoImporter()
        {
            try
            {
                Hide();
                GoProMediaImporter = new MediaImporter(FileImportClear, AddImportRecord, CheckImportRecords,
                    ConnnectLists, ReOrderFilesInc, ClearImportTimes, SetImportFromTime, SetImportToTime);
                GoProMediaImporter.ShowDialog();
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }
        private void btnMediaImporter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoImporter();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        private void btnScriptEditor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoComplexEditor();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        public void ProgressEventOnFinish(string name)
        {
            try
            {
                for (int i = 0; i < ProgressInfoDisplay.Count; i++)
                {

                    if (Path.GetFileNameWithoutExtension(ProgressInfoDisplay[i].Name) == Path.GetFileNameWithoutExtension(name))
                    {
                        if (ProgressInfoDisplay[i].IsActive)
                        {
                            ProgressInfoDisplay[i].Close();
                        }
                        ProgressInfoDisplay[i] = null;
                        ProgressInfoDisplay.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        public IEnumerable<string> ProgressEventConnector(string name)
        {
            try
            {
                foreach (var pp in ConverterProgressEventHandler?.converterLists.Where(pp => pp.DestNameNoExt() == name))
                {
                    return pp.ProgressList;
                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        private void ViewOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstBoxJobs.SelectedItem != null)
                {
                    var currentjob = (JobListDetails)lstBoxJobs.SelectedItem;
                    if (currentjob != null)
                    {
                        bool found = false;
                        string DestFile = Path.GetFileName(currentjob.SourceFile);
                        if (currentjob.SourceFile is null)
                        {
                            DestFile = Path.GetFileNameWithoutExtension(currentjob.DestMFile);
                        }
                        foreach (ConverterProgressInfo v in ProgressInfoDisplay.Where(s => s.GetDestNameNoExt() == DestFile))
                        {
                            v.ShowActivated = true;
                            v.Show();
                            found = true;
                            break;
                        }

                        if (!found)
                        {
                            var fp = new ConverterProgressInfo(DestFile, ProgressEventOnFinish, ProgressEventConnector, ConverterProgressEventHandler.GetCount);
                            if (fp != null)
                                ConverterProgressEventHandler.SetScrollHander(DestFile, fp.ScrollIntoViewHandler);
                            {
                                ProgressInfoDisplay.Add(fp);
                            }
                            fp.ShowActivated = true;
                            fp.Show();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        public void OnShortsCreatorFinish()
        {
            try
            {
                frmShortsCreator = null;
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        private void btnShortsCreator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (frmShortsCreator == null)
                {
                    frmShortsCreator = new ShortsCreator(OnShortsCreatorFinish);
                    Hide();
                    Task.Run(() =>
                    {
                        KillOrphanProcess("avidemux_cli.exe");
                    });


                    frmShortsCreator.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void DoVideoEditForm_Close()
        {
            try
            {
                var cancellationTokenSourcex = new CancellationTokenSource();
                cancellationTokenSourcex.CancelAfter(2500);
                while (frmVCE.IsActive)
                {
                    Thread.Sleep(100);
                    if (cancellationTokenSourcex.IsCancellationRequested)
                    {
                        break;
                    }
                }
                frmVCE = null;
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void btnScraperDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ShowScraper();
            }
            catch(Exception ex)
            {
                ex.LogWrite($"btnScraperDraft_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        private void btnSetupload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*var filess = Directory.EnumerateFiles(@"D:\Filtered\Steamrails Goulburn Valley Explorer 270724", "*.mp4", SearchOption.AllDirectories).ToList();
                var files = Directory.EnumerateFiles(@"D:\shorts\Steamrails Goulburn Valley Explorer 270724", "*.mp4", SearchOption.AllDirectories).ToList();
                foreach(var filet in files)
                {
                    var fp = Path.GetFileName(filet);
                    foreach(var filex in filess)
                    {
                        if (fp == Path.GetFileName(filex))
                        {
                            File.Delete(filet);
                            File.Move(filex, filet);
                            break;
                        }
                    }
                }


                return;*/
                if (selectShortUpload is not null && !selectShortUpload.IsClosed)
                {
                    if (selectShortUpload.IsClosing) selectShortUpload.Close();
                    while (!selectShortUpload.IsClosed)
                    {
                        Thread.Sleep(100);
                    }
                    selectShortUpload.Close();
                    selectShortUpload = null;
                }
                if (selectShortUpload is null)
                {
                    Hide();
                    selectShortUpload = new SelectShortUpload(ModuleCallback, SelectShortUpload_onFinish);
                    selectShortUpload.ShowActivated = true;
                    selectShortUpload.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSetupload_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void SelectShortUpload_onFinish()
        {
            try
            {
                Show();
                if (selectShortUpload is not null && !selectShortUpload.IsClosed)
                {
                    selectShortUpload.Hide();
                    Task.Run(() =>
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(1500);
                        while (selectShortUpload is not null && selectShortUpload.IsClosing && !cts.Token.IsCancellationRequested)
                        {
                            Thread.Sleep(100);
                        }
                        selectShortUpload = null;
                    });
                }
                else
                {
                    if (selectShortUpload is not null && selectShortUpload.IsClosed)
                    {
                        selectShortUpload = null;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSetupload_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnVIdeoEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (frmVCE == null)
                {
                    frmVCE = new VideoCutsEditor(AddRecord, DoVideoEditForm_Close, connectionString);
                    Hide();
                    frmVCE.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }


        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Cmbaudiomode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                bool LoadedKey = (key != null);
                if (LoadedKey) key.SetValue("Audiomode", cmbaudiomode.SelectedIndex);
                if (LoadedKey) key.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ResizeWindow();
                var cts = new CancellationTokenSource();
                for (int i = 0; i < 40; i++)
                {
                    cts.CancelAfter(TimeSpan.FromMilliseconds(250));
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                    ResizeWindow();

                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_FocusableChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeWindow();
            var cts = new CancellationTokenSource();
            for (int i = 0; i < 40; i++)
            {
                cts.CancelAfter(TimeSpan.FromMilliseconds(250));
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                ResizeWindow();

            }
        }


        private int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.id='" + Id.ToString() + "'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }
        private async Task<bool> KillOrphanProcess(string ExeName = "ffmpeg.exe")
        {
            try
            {
                string myStrQuote = "\"";
                ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process where name = {myStrQuote}{ExeName}{myStrQuote}");
                foreach (ManagementObject o in searcher.Get())
                {
                    string HandleID = o.Properties["Handle"].Value.ToString();
                    string ParentProcessId = o.Properties["ParentProcessID"].Value.ToString();
                    string ID = o.Properties["ProcessID"].Value.ToString();
                    if (o["CommandLine"] != null)
                    {
                        string comstr = o["CommandLine"].ToString();
                        if (comstr.ToUpper().Contains("RTMP:")) continue;
                    }
                    if (ParentProcessId != "")
                    {
                        try
                        {
                            var pr = Process.GetProcessById(ParentProcessId.ToInt());
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ContainsAny(new List<string> { "not", "running", "Process", ParentProcessId.ToString() }))
                            {
                                var rpt = Process.GetProcessById(ID.ToInt());
                                rpt.Kill();
                            }
                        }
                    }
                    return true;
                }
                return true;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                key?.SetValue("Width", MainWindowX.ActualWidth);//876
                key?.SetValue("Height", MainWindowX.ActualHeight);//444
                int maxid = -1;
                switch (MainWindowX.WindowState)
                {
                    case WindowState.Normal:
                        {
                            maxid = 0;
                            break;
                        }
                    case WindowState.Maximized:
                        {
                            maxid = 1;
                            break;
                        }
                    case WindowState.Minimized:
                        {
                            maxid = 2;
                            break;
                        }
                }
                key?.SetValue("WindowState", maxid);//444
                key?.Close();

                if (canclose || ShiftActiveWindowClosing)
                {
                    RequestStop();
                    Task.Run(() => { KillOrphanProcess(); });
                    Task.Run(() => { KillOrphanProcess("avidemux_cli"); });

                }
                else
                {
                    trayicon.Visibility = Visibility.Visible;
                    e.Cancel = true;
                    InTray = true;
                    TrayIcon = new DispatcherTimer();
                    TrayIcon.Tick += (ChangeIcon);
                    TrayIcon.Interval = new TimeSpan(0, 0, 23);
                    TrayIcon.Start();
                    Hide();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void ChangeIcon(object sender, EventArgs e)
        {
            try
            {
                trayiconnum++;
                if (trayiconnum > 5) trayiconnum = 0;
                string icon = (trayiconnum == 0) ? "pack://application:,,,/icons/computer.ico" : "pack://application:,,,/icons/computer" + trayiconnum.ToString() + ".ico";
                trayicon.IconSource = new ImageSourceConverter().ConvertFromString(icon) as ImageSource;
                trayicon.ToolTipText = fileprogress;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadUpdateFolder();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                canclose = true;
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowConfig();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void ShowConfig()
        {
            try
            {
                ffmpegready = false;
                ConfigurationSettings cd = new ConfigurationSettings();
                cd.ShowDialog();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                usetorrents = (bool)(!LoadedKey || (string)key.GetValue("UseUWebApi", true).ToString() == "True");
                key.Close();
                ffmpegready = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Btnstart_Click(object sender, RoutedEventArgs e)
        {
            //Parsesource();hide
            try
            {
                //ResizeWindow();
                //                Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Show_Config(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowConfig();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TrayIcon.IsEnabled = false;
                trayicon.IconSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/icons/computer.ico") as ImageSource;
                TrayIcon.Stop();
                TrayIcon.IsEnabled = false;
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public string GetExePath()
        {
            try
            {
                string res = "";
                res = (Debugger.IsAttached) ? defaultprogramlocation : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        private async void BtnErrorPath_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => BtnErrorPath_Copy_Click(sender, e));
                    return;
                }
                string AppPath = GetExePath(), searchdir = SelectMasterDir("Select Parser Directory", "SourceDirectory");
                List<string> PathList = new List<string>();
                PathList = Directory.EnumerateFiles(searchdir, "*.*", SearchOption.AllDirectories).
                         Where(s => s.EndsWithAny(GetDefaultVideoExts())).ToList<string>();
                (Progressbar2.Maximum, Progressbar2.Value) = (ProcessingJobs.Count, 0);
                foreach (string SourceFile in PathList)
                {

                    ffmpegbridge bridge = new ffmpegbridge();
                    TimeSpan Dur = TimeSpan.Zero;
                    (IVideoStream videoStream, IAudioStream audioStream, Dur) = bridge.ReadMediaFile(SourceFile);
                    bridge = null;


                    if ((videoStream.Height == 480) && (videoStream.Width == 720))
                    {
                        string DestFile = Path.GetDirectoryName(SourceFile) + "\\REDO\\" + Path.GetFileName(SourceFile);
                        System.IO.File.Move(SourceFile, DestFile);
                    }
                    Progressbar2.Value++;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Btnrescan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Btnrescan_Click(sender, e));
                    return;
                }
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                string SourceDirectory720p = LoadedKey ? (string)key.GetValue("SourceDirectory720p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory1080p = LoadedKey ? (string)key.GetValue("SourceDirectory1080p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4K = LoadedKey ? (string)key.GetValue("SourceDirectory4k", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4KAdobe = LoadedKey ? (string)key.GetValue("SourceDirectory4kAdobe", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";

                key.Close();
                List<string> templist = (SourceList.Where(ss => System.IO.File.Exists(ss))).ToList();
                SourceList.Clear();
                SourceList.AddRange(templist);
                SelectFiles(SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DownloadUpdateFolder();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        /*private bool ServerFileIsNewer(string clientFileVersion, FileVersionInfo serverFile)
        {
            Version client = new Version(clientFileVersion);
            Version server = new Version(string.Format("{0}.{1}.{2}.{3}", serverFile.FileMajorPart, serverFile.FileMinorPart, serverFile.FileBuildPart, serverFile.FilePrivatePart));
            return server > client;
        }*/
        private bool ServerFileIsOlder(string clientFileVersion, FileVersionInfo serverFile)
        {
            Version client = new Version(clientFileVersion);
            Version server = new Version(string.Format("{0}.{1}.{2}.{3}", serverFile.FileMajorPart, serverFile.FileMinorPart, serverFile.FileBuildPart, serverFile.FilePrivatePart));
            return server < client;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstBoxJobs.SelectedIndex != -1)
                {
                    int x = 0;
                    foreach (var job in ProcessingJobs)
                    {
                        if (x == lstBoxJobs.SelectedIndex)
                        {
                            (job.X264Override, job.Processed, job.Progress, job.Handle, job.Fileinfo) = (true, false, 0, "", ""); ;
                            job.ForegroundColor = System.Windows.Media.Color.FromScRgb(100, 6, 186, 28);// : Color.FromArgb(100, 246, 8, 50);
                            break;
                        }
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CanUpdate = false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MenuItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CanUpdate = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void lstBoxJobs_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            try
            {

                if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, false))
                    e.Effects = System.Windows.DragDropEffects.All;
                else
                    e.Effects = System.Windows.DragDropEffects.None;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }
        private bool AddFiles(string newfile, bool IsMpeg4ASP = false, bool IsMpeg4AVC = false)
        {
            bool JobsAdded = false, IsX265 = false;
            string ext = Path.GetExtension(newfile);
            if ((ext == ".mts") || (ext == ".avi") || (ext == ".mkv") || (ext == ".mp4") ||
                (ext == ".m2ts") || (ext == ".src") || (ext == ".cst") || (ext == ".edt"))
            {
                string SourceFile = Path.GetFileNameWithoutExtension(newfile);
                if (SourceFile.ContainsAny(new List<string> { ".src", ".cst", ".edt" }))
                {
                    string fn = File.ReadAllText(SourceFile);// Add Files
                    List<string> Commands = fn.Split('|').ToList();
                    if (Commands.Count > 3) Commands.RemoveAt(0);
                    SourceFile = Path.GetFileNameWithoutExtension(Commands.FirstOrDefault());
                }
                string SourceDir = Path.GetDirectoryName(newfile);
                if (!ProcessingJobs.Any(job => job.Title == SourceFile))
                {
                    JobsAdded = AddIfVaid(newfile, SourceDir);
                }
            }
            return JobsAdded;
        }

        public bool AddIfVaid(string newfile, string SourceDir)
        {
            try
            {
                bool res = false;
                string DownloadsDir = GetDownloadsFolder();

                bool IsNotDownloads = Path.GetDirectoryName(SourceDir).Contains(DownloadsDir);// """DownloadsDir;
                IsNotDownloads = !IsNotDownloads;

                var newjob = new JobListDetails(newfile, SourceIndex++, SourceDir.Contains("1080p") && IsNotDownloads,
                    SourceDir.ToUpper().EndsWith("4K") && IsNotDownloads, SourceDir.ToUpper().EndsWith("4KADOBE") && IsNotDownloads);
                if (newjob.IsMulti)
                {
                    if (newjob.GetCutList().Count > 0)
                    {
                        ProcessingJobs.Add(newjob);
                        res = true;
                    }
                    else SourceIndex--;
                }
                else
                {
                    ProcessingJobs.Add(newjob);
                    res = true;
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        private void lstBoxJobs_Drop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                bool JobsAdded = false, Shift = (e.KeyStates & DragDropKeyStates.ShiftKey) == (DragDropKeyStates.ShiftKey);
                bool Control = (e.KeyStates & DragDropKeyStates.ControlKey) == (DragDropKeyStates.ControlKey);
                bool IsMpeg4ASP = ((Control) && (!Shift)), IsMpeg4AVC = ((Control) && (Shift));
                foreach (var newfile in (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false))
                {
                    if (Directory.Exists(newfile))
                    {
                        foreach (var _ in Directory.GetFiles(newfile).Where(newfiles => AddFiles(newfiles, IsMpeg4ASP, IsMpeg4AVC)).Select(newfiles => new { }))
                        {
                            JobsAdded = true;
                        }
                    }
                    else if (AddFiles(newfile, IsMpeg4ASP, IsMpeg4AVC)) JobsAdded = true;
                }
                if (JobsAdded)
                {
                    Start();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void RequestStop()
        {
            //_shouldStop = true;
        }
        private void BtnDirThreads_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OnCompairFinished = new Models.delegates.CompairFinished(OnFinishCompair);
                DefaultDirectories ddthreaew = new(OnCompairFinished);
                Hide();
                ddthreaew.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnDestPath_Click " + ex.Message);
            }
        }
        private void MainWindowX_StateChanged(object sender, EventArgs e)
        {
            try
            {
                ResizeWindow();
                var cts = new CancellationTokenSource();
                for (int i = 0; i < 40; i++)
                {
                    cts.CancelAfter(TimeSpan.FromMilliseconds(250));
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                    ResizeWindow();


                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnFileFixer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> ListOfFiles = Directory.EnumerateFiles(@"C:\GoPro9\masters\New folder", "*.txt", SearchOption.AllDirectories).ToList<string>();
                if (ListOfFiles.Count > 0)
                {
                    List<(TimeSpan, TimeSpan, string)> Sorted = new List<(TimeSpan, TimeSpan, string)>();

                    foreach (string FileName in ListOfFiles)
                    {
                        List<string> fileinfo = File.ReadLines(FileName).ToList();
                        string EncodeStr = "", DecodeEnd = "";
                        string Decode = "Time code of last frame     : ";
                        string Encode = "Time code of first frame    : ";
                        foreach (var linetext in fileinfo.Where(linetext => linetext.Contains(Encode)))
                        {
                            EncodeStr = linetext.Substring(Encode.Length).Trim();
                            break;
                        }
                        foreach (var linetext in fileinfo.Where(linetext => linetext.Contains(Decode)))
                        {
                            DecodeEnd = linetext.Substring(Decode.Length).Trim();
                            break;
                        }
                        if (EncodeStr.Length > 0 && DecodeEnd.Length > 0)
                        {
                            TimeSpan Start = EncodeStr.FromStrToTimeSpan();
                            TimeSpan End = DecodeEnd.FromStrToTimeSpan();
                            Sorted.Add((Start, End, FileName));
                        }
                    }
                    Sorted.Sort();
                    int Index = 1;
                    foreach (var fn in Sorted)
                    {
                        string file_n = Path.GetFileName(fn.Item3).Replace(".txt", "");
                        string file_p = Path.GetDirectoryName(fn.Item3);
                        string Times = $"{fn.Item1.ToFFmpeg().Replace(".000", "")}-{fn.Item2.ToFFmpeg().Replace(".000", "")}";
                        Times = Times.Replace(":", "_");
                        string NewFilename = Path.Combine(file_p, $"{Index++} {Times} {file_n}");
                        string OldFileName = Path.Combine(file_p, file_n);
                        if (NewFilename.Length > 0)
                        {
                            MoveIfExists(OldFileName, NewFilename);
                        }
                    }
                    System.Windows.MessageBox.Show("Done");


                }
                //videoResCompare = new(new CompairFinished(OnFinishCompair));
                //Hide();
                //compareform.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MainWindowX_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => MainWindowX_GotFocus(sender, e));
                    return;
                }
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                string SourceDirectory720p = LoadedKey ? (string)key.GetValue("SourceDirectory720p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory1080p = LoadedKey ? (string)key.GetValue("SourceDirectory1080p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4K = LoadedKey ? (string)key.GetValue("SourceDirectory4K", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4KAdobe = LoadedKey ? (string)key.GetValue("SourceDirectory4KAdobe", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";

                key.Close();
                List<string> templist = (SourceList.Where(ss => System.IO.File.Exists(ss))).ToList();
                SourceList.Clear();
                SourceList.AddRange(templist);
                SelectFiles(SourceDirectory720p, SourceDirectory1080p, SourceDirectory4K, SourceDirectory4KAdobe);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnVideoCardDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                videoCardDetailsSelector = new(new Models.delegates.CompairFinished(OnFinishCompair));
                Hide();
                videoCardDetailsSelector.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void lstBoxJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void BtnViewLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => BtnViewLog_Click(sender, e));
                    return;
                }
                if (CurrentLogFile != "")
                {
                    if (File.Exists(CurrentLogFile))
                        _ = Process.Start("notepad.exe", CurrentLogFile);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        List<(long, long, string)> ComplexData = new List<(long, long, string)>();
        public unsafe void ProcessMutliFile(string sourcefile, List<(long, long, string)> CutData)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MainWindowX_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ResizeWindow();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void ResizeWindow()
        {
            try
            {
                if (MainWindowX.IsLoaded)
                {
                    if ((lstBoxJobs.ActualWidth != double.NaN) && (Grid1.ActualWidth != double.NaN) &&
                        (MainWindowX.ActualWidth != double.NaN) && (MainWindowX.ActualWidth != 0) &&
                         (MainWindowX.Height != double.NaN) && (MainWindowX.Width != double.NaN))
                    {
                        Grid1.Width = MainWindowX.ActualWidth - 2;


                        lstboxresize();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MainWindowX_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                ResizeWindow();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void lstBoxJobs_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                lstBoxJobs.AllowDrop = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnBitRateSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectEditProfile selEditProfile = new();
                selEditProfile.ShowDialog();
                selEditProfile = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite();
            }
        }
        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string scandir = (CmbScanDirectory.SelectedIndex == 0) ? "" : CmbScanDirectory.Text;
                OnCompairFinished = new CompairFinished(OnFinishCompair);
                compareform = new SourceDestComp(OnCompairFinished, scandir);
                Hide();
                compareform.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnFinishCompair()
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DeleteSel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (CmbScanDirectory.SelectedItem != null)
                {
                    string selitem = CmbScanDirectory.Text;
                    int selindex = CmbScanDirectory.Items.IndexOf(selitem);
                    if (selindex != -1)
                    {
                        CmbScanDirectory.Items.RemoveAt(selindex);
                        CmbScanDirectory.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void CmbScanDirectory_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> DirList = new();
                int index = CmbScanDirectory.SelectedIndex;
                foreach (string ss in CmbScanDirectory.Items)
                {
                    if (ss != "Default")
                    {
                        DirList.Add(ss);
                    }
                }
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                if (key != null)
                {
                    key.SetValue("ComparitorList", DirList.ToArray(), RegistryValueKind.MultiString);
                    if (index != -1)
                    {
                        key.SetValue("ComparitorListIndex", index);
                    }
                }
                key.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void ResetErrored_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstBoxJobs.SelectedIndex != -1)
                {
                    JobListDetails Job = (JobListDetails)lstBoxJobs.Items[lstBoxJobs.SelectedIndex];
                    string info = Job.Fileinfo.ToString().ToLower();
                    if (System.IO.File.Exists(Job.SourcePath + "\\" + Job.Title))
                    {
                        if (info.Contains("skipped") || info.Contains("error") || info.Contains("locked") || info.Contains("allready"))
                        {
                            (Job.X264Override, Job.Processed, Job.Progress, Job.Handle, Job.Fileinfo) = (true, false, 0, "", "");
                            Job.ForegroundColor = System.Windows.Media.Color.FromScRgb(100, 6, 186, 28);// : Color.FromArgb(100, 246, 8, 50);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void lstboxresize()
        {
            try
            {

                if (MainWindowX.IsLoaded)
                {
                    if ((MainWindowX.Height != 0) && (MainWindowX.Width != 0) &&
                        (MainWindowX.Height != double.NaN) && (MainWindowX.Width != double.NaN))
                    {
                        brdlstbox.Height = MainWindowX.Height - 258;
                        brdlstbox.Width = MainWindowX.Width - 25;
                        lstBoxJobs.MinWidth = brdlstbox.Width - 10;
                        lstBoxJobs.Width = lstBoxJobs.MinWidth;
                        Progressbar1.Width = lstBoxJobs.Width - 120;
                        Progressbar2.Width = Progressbar1.Width;
                      //  statusbar1.Width = MainWindowX.Width - 20;
                       // statusbar2.Width = statusbar1.Width;


                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void MainWindowX_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {


                lstboxresize();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Interval = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
                    FileQueChecker.Enabled = true;
                    FileQueChecker.Stop();
                    FileQueChecker.Start();
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void ScanSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => ScanSource_Click(sender, e));
                    return;
                }
                Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Interval = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
                    FileQueChecker.Enabled = true;
                    FileQueChecker.Stop();
                    FileQueChecker.Start();

                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Settings_Click(sender, e));
                    return;
                }
                SelectEditProfile selEditProfile = new();
                selEditProfile.ShowDialog();
                selEditProfile = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void CmbScanDirectory_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    string newentry = CmbScanDirectory.Text;
                    if (CmbScanDirectory.Items.IndexOf(newentry) == -1)
                    {
                        CmbScanDirectory.Items.Add(newentry);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCompairFinished = new CompairFinished(OnFinishCompair);
                Swm = new(OnCompairFinished);
                Hide();
                Swm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => OpenLogFile_Click(sender, e));
                    return;
                }
                if (CurrentLogFile != "")
                {
                    if (File.Exists(CurrentLogFile))
                        _ = Process.Start("notepad.exe", CurrentLogFile);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => DeleteFile_Click(sender, e));
                    return;
                }
                if (lstBoxJobs.SelectedIndex != -1)
                {
                    int Index = lstBoxJobs.SelectedIndex;
                    if (ProcessingJobs[Index] != null)
                    {
                        if ((!ProcessingJobs[Index].IsSkipped) && (!ProcessingJobs[Index].Processed))
                        {
                            ProcessingJobs[Index].IsSkipped = true;
                            ProcessingJobs[Index].Fileinfo = "[Skipped]";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void TxtAudioMode_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                ShowAudioControlDialog = (e.KeyStates == DragDropKeyStates.AltKey);
                if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, false))
                    e.Effects = System.Windows.DragDropEffects.All;
                else
                    e.Effects = System.Windows.DragDropEffects.None;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnSeletDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofg = new FolderBrowserDialog();
                ofg.Title = "Select Source Directory";
                ofg.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void LogWrite(string logMessage)
        {
            try
            {
                logMessage.WriteLog();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }
    }
}
