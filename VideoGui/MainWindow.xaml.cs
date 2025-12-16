using AsyncAwaitBestPractices;
using CliWrap;
using Coravel.Scheduling.Schedule;
using FirebirdSql.Data.FirebirdClient;
using FolderBrowserEx;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.YouTube.v3.Data;
using MediaInfo.Model;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using MS.WindowsAPICodePack.Internal;
using Nancy;
using Nancy.Diagnostics;
using Nancy.Extensions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using SevenZip;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Formats.Tar;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Versioning;
using System.Security.Cryptography.Pkcs;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xaml;
using System.Xml.Linq;
using VideoGui.ffmpeg;
using VideoGui.ffmpeg.Events;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Text;
using VideoGui.ffmpeg.Streams.Video;
using VideoGui.Models;
using VideoGui.Models.delegates;
using Windows.AI.MachineLearning;
using Windows.Storage;
using WinRT.Interop;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static VideoGui.ffmpeg.Probe.ProbeModel;
using Application = System.Windows.Application;
using File = System.IO.File;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Object = System.Object;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using Window = System.Windows.Window;



namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// :\VideoGui\src\VideoGui\MainWindow.xaml.cs  // ver 280 merge from 279 server ver
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
        public _UpdateSpeed ThreadUpdateSpeed;
        public _UpdateProgress ThreadUpdateProgress;
        public _UpdateTime ThreadUpdateTime;
        public _StatsHandlerDateTimeSetter ThreadStartTimeSet;
        public _StatsHandlerDateTimeGetter ThreadStartTimeGet;
        public _StatsHandlerBool ThreadStatsHandlerBool;
        public _StatsHandlerExtra ThreadStatsHandlerXtra;
        public IsFileInUse OnIsFileInUse;
        public Models.delegates.CompairFinished OnCompairFinished;
        public VideoSizeChecker videoResCompare;
        public VideoCardSelector videoCardDetailsSelector;
        //public ScraperModule scraperModule = null;//, scheduleScraperModule = null;
        public SelectShortUpload selectShortUpload = null;
        public MasterTagSelectForm MasterTagSelectFrm = null;
        public SelectReleaseSchedule SelectReleaseScheduleFrm = null;
        // public ProcessSchedule ScheduleProccessor = null;
        bool AutoClose = false;
        AutoCancel DoAutoCancel = null;
        List<ListScheduleItems> ScheduleListItems = new List<ListScheduleItems>();
        List<ListScheduleItems> ScheduleListedItems = new List<ListScheduleItems>();
        System.Threading.Timer EventTimer = null;
        ObservableCollection<GroupTitleTags> groupTitleTagsList = new ObservableCollection<GroupTitleTags>();
        ObservableCollection<Descriptions> DescriptionsList = new ObservableCollection<Descriptions>();
        ObservableCollection<SelectedTags> selectedTagsList = new ObservableCollection<SelectedTags>();
        ObservableCollection<AvailableTags> availableTagsList = new ObservableCollection<AvailableTags>();
        ObservableCollection<TitleTags> TitleTagsList = new ObservableCollection<TitleTags>();
        ObservableCollection<VideoSchedules> VideoSchedulesList = new ObservableCollection<VideoSchedules>();
        ObservableCollection<VideoSchedule> VideoScheduleList = new ObservableCollection<VideoSchedule>();
        ObservableCollection<AppliedSchedules> AppliedSchedulesList = new ObservableCollection<AppliedSchedules>();
        ObservableCollection<AppliedSchedule> AppliedScheduleList = new ObservableCollection<AppliedSchedule>();
        ObservableCollection<EventDefinition> EventDefinitionsList = new ObservableCollection<EventDefinition>();
        ObservableCollection<ScheduleMapNames> SchedulingNamesList = new ObservableCollection<ScheduleMapNames>();
        ObservableCollection<ScheduleMapItem> SchedulingItemsList = new ObservableCollection<ScheduleMapItem>();
        ObservableCollection<ScheduledActions> YTScheduledActionsList = new ObservableCollection<ScheduledActions>();
        ObservableCollection<Rematched> RematchedList = new ObservableCollection<Rematched>();
        ObservableCollection<ProcessTargets> ProcessTargetsList = new ObservableCollection<ProcessTargets>();
        ObservableCollection<MultiShortsInfo> ShortsDirectoryList = new ObservableCollection<MultiShortsInfo>();
        ObservableCollection<SelectedShortsDirectories> SelectedShortsDirectoriesList = new ObservableCollection<SelectedShortsDirectories>();
        ObservableCollection<SelectedShortsDirectories> SelectedShortsDirectoriesListTest = new ObservableCollection<SelectedShortsDirectories>();
        List<ShortsDirectory> EditableshortsDirectoryList = new List<ShortsDirectory>();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource ProcessingCancellationTokenSource = new CancellationTokenSource();
        CollectionViewSource titletagsViewSource = new CollectionViewSource();
        CollectionViewSource availabletagsViewSource = new CollectionViewSource();


        ObservableCollection<DraftShorts> DraftShortsList = new ObservableCollection<DraftShorts>();
        int TitleTagsSrc = -1;
        double frames, LRF, RRF, LLC, RLC;
        DateTime totaltimeprocessed = DateTime.Now;
        object lookuplocked = new object();
        int SelectedTagId = -1, MaxUploads = -1, TitleId = -1, DescId = -1, SchMaxUploads = 0;
        bool IsUploading = false, SystemSetup = false, InTray = false, ffmpegdone = false, processing = false;
        private System.Object thisLock = new Object();
        private Object thisfLock = new Object();
        private Object thispLock = new Object();
        private Object thissLock = new Object();
        public object statslocker = new(), ProcessingJobslocker = new(), ProbeLock = new();
        public int LockedDeviceID = 0, SourceIndex = 0;
        public object StatsLock = new();
        List<Task> PageDownloadTasks = new();
        public bool canclose = false, ShiftActiveWindowClosing = false;
        int failed = 0, Passed = 0, InitTitleLength = 0, cnt4K = 0, cnt = 0, cnt1440p = 0, currentjob = 0, trayiconnum = 0;
        private string[] FilesInProcess = Array.Empty<string>();
        string DestFile, filename_pegpeg, link, ProbeData, Root, ffmpeg_ver = "", ffmpeg_gitver = "", un = "", up = "";
        string MinBitRate = "", MaxBitRate = "", BitRateBuffer = "", VideoHeight, VideoWidth, ArRounding, ArModulas, Video = "";
        bool ArRoundEnable = true, VSyncEnable, ResizeEnable = true, ArScalingEnabled = true;
        List<string> SourceList = new List<string>();
        StatsHandler Stats_Handler = new StatsHandler();
        DateTime ProcessingTime;
        System.Windows.Forms.Timer FileQueChecker, FormResizerEvent;
        public TimeSpan LastTick = TimeSpan.Zero, UploadWaitTime = TimeSpan.Zero;
        DispatcherTimer TrayIcon;
        ObservableCollection<Titles> TitlesList = new ObservableCollection<Titles>();
        ObservableCollection<Titles> TitlesList2 = new ObservableCollection<Titles>();
        public ConverterProgressInfo frmConverterProgressInfo = null;
        public ObservableCollection<JobListDetails> ProcessingJobs = new ObservableCollection<JobListDetails>();
        public ObservableCollection<SourceFileCache> SourceFileInfos = new ObservableCollection<SourceFileCache>();
        public CollectionViewSource HistoricCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource CurrentCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource ImportCollectionViewSource = new CollectionViewSource();

        public ObservableCollection<ComplexJobList> ComplexProcessingJobList = new ObservableCollection<ComplexJobList>();
        public ObservableCollection<ComplexJobHistory> ComplexProcessingJobHistory = new ObservableCollection<ComplexJobHistory>();
        public ObservableCollection<FileInfoGoPro> FileRenamer = new ObservableCollection<FileInfoGoPro>();
        public ObservableCollection<PlanningQue> PlanningQueList = new ObservableCollection<PlanningQue>();
        public ObservableCollection<PlanningCuts> PlanningCutsList = new ObservableCollection<PlanningCuts>();
        public ObservableCollection<EditableScheduleEvent> EditableScheduleEventsList = new ObservableCollection<EditableScheduleEvent>();
        public ObservableCollection<ShortsProcessor> ShortsProcessors = new ObservableCollection<ShortsProcessor>();
        VideoCutsEditor frmVCE = null;
        public List<string> ComparitorList = new();
        string DestDirectory720p = string.Empty, DestDirectoryAdobe4K = string.Empty, DestDirectory4K = string.Empty, DestDirectory1440p = string.Empty, backupun = "", DestDirectoryTwitch = "";
        string fileprogress = "", DoneDirectory720p = string.Empty, DoneDirectory1440p = string.Empty, DoneDirectory4K = string.Empty, DoneDirectoryAdobe4K = string.Empty;
        string ErrorDirectory = string.Empty, SourceDirectory4K = string.Empty, SourceDirectoryAdobe4K = string.Empty, SourceDirectory1440p = string.Empty, SourceDirectory720p = string.Empty;
        TimeSpan ProcessingTimeGlobal;
        DateTime StartTime = DateTime.Now, Start2 = DateTime.Now, Start3 = DateTime.Now;
        bool ffmpegready = false, usetorrents = true, CanUpdate = true, ShowAudioControlDialog = true, UseFisheyeRemoval = false;
        int totaltasks = 4, total1440ptasks = 2, total4kTasks = 3, CurrentDbId = -1;
        string backupserver = "", backupDone1440p = "", backupDone = "", backupCompleted = "", connectionString = "";
        string txtDestPath = "", txtDonepath = "", txtErrorPath = "", CurrentLogFile = "", defaultprogramlocation = "";
        CancellationTokenSource ffmpeg_download_cancellationToken, app_download_cancellationToken;
        private readonly SynchronizationContext _syncContext;
        List<Task> mytasklist = new List<Task>();
        List<string> _720PFiles = new(), _1440pFiles = new(), _4KFiles = new();
        List<ConverterProgressInfo> ProgressInfoDisplay = new List<ConverterProgressInfo>();
        string TitleStr = "", DescStr = "";
        //ComplexSchedular complexfrm = null;
        private ConverterProgress ConverterProgressEventHandler = new ConverterProgress();
        private ObservableCollectionFilters ObservableCollectionFilter;
        public DirectoryTitleDescEditor directoryTitleDescEditor = null;
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







        public void ShowScraper(Nullable<DateTime> startdate = null, Nullable<DateTime> enddate = null,
            List<ListScheduleItems> _listSchedules = null, int SchMaxUploads = 100, int _eventid = 0)
        {
            try
            {

                WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                string gUrl = webAddressBuilder.Dashboard().Address;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                key?.Close();
                int MaxShorts = MaxUploads.ToInt(80);
                int MaxPerSlot = uploadsnumber.ToInt(100);
                string TargetUrl = webAddressBuilder.AddFilterByDraftShorts().GetHTML();
                OldgUrl = gUrl;
                OldTarget = TargetUrl;
                var __scraperModule = new ScraperModule(InvokerHandler<object>, FinishScraper, gUrl, TargetUrl, 0);
                Hide();
                __scraperModule.ShowActivated = true;
                __scraperModule.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ShowScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        string OldTarget = "", OldgUrl = "";
        private TResult InvokerHandler<TResult>(object ThisForm, object tld)
        {
            try
            {
                switch (ThisForm)
                {

                    case ProcessDraftTargets processDraftTargets:
                        {
                            return processDraftTargets_Handler<TResult>(tld, processDraftTargets);
                        }
                    case MultiShortsUploader frmMultiShortsUploader:
                        {
                            return formObjectHandler_MultiShortsUploader<TResult>(tld, frmMultiShortsUploader);
                        }
                    case TitleSelectFrm frmTitleSelect:
                        {
                            return formObjectHandler_TitleSelect<TResult>(tld, frmTitleSelect);
                        }
                    case ScraperModule scraperModule:
                        {
                            return scraperModule_Handler<TResult>(tld, scraperModule);
                        }
                    case SelectShortUpload selectShortUpload:
                        {
                            return selectShortUpload_Handler<TResult>(tld, selectShortUpload);
                        }
                    case MasterTagSelectForm frmMasterTagSelectForm:
                        {
                            return formObjectHandler_MasterTagSelect<TResult>(tld, frmMasterTagSelectForm);
                        }
                    case DescSelectFrm frmDescSelectFrm:
                        {
                            return formObjectHandler_DescSelect<TResult>(tld, frmDescSelectFrm);
                        }
                    case ScheduleEventCreator scheduleEventCreatorFrm:
                        {
                            return formObjectHandler_scheduleEventCreator<TResult>(tld, scheduleEventCreatorFrm);
                        }
                    case DirectoryTitleDescEditor directoryTitleDescEditor:
                        {
                            return formObjectHandler_DirectoryTitleDescEditor<TResult>(tld, directoryTitleDescEditor);
                        }
                    case SelectReleaseSchedule selectReleaseSchedule:
                        {
                            return formObjectHandler_SelectReleaseSchedule<TResult>(tld, selectReleaseSchedule);
                        }
                    case SchedulingSelectEditor schedulingSelectEditor:
                        {
                            return formObjectHandler_SchedulingSelectEditor<TResult>(tld, schedulingSelectEditor);
                        }
                    case ScheduleActioner scheduleActioner:
                        {
                            return formObjectHandler_ScheduleActioner<TResult>(tld, scheduleActioner);
                        }
                    case ActionScheduleSelector actionScheduleSelector:
                        {
                            return formObjectHandler_ActionScheduleSelector<TResult>(tld, actionScheduleSelector);
                        }
                    case ManualScheduler manualScheduler:
                        {
                            return formObjectHandler_ManualScheduler<TResult>(tld, manualScheduler);
                        }
                    case ComplexSchedular complexSchedular:
                        {
                            return formObjectHandler_ComplexSchedular<TResult>(tld, complexSchedular);
                        }
                    case MediaImporter goProMediaImporter:
                        {
                            return formObjectHandler_GoProMediaImporter<TResult>(tld, goProMediaImporter);
                        }

                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InvokerHandler {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return default(TResult);
            }
        }

        private TResult processDraftTargets_Handler<TResult>(object tld, ProcessDraftTargets processDraftTargets)
        {
            try
            {
                if (tld is CustomParams_SetIndex cpsii)
                {
                    ShortsDirectoryIndex = cpsii.index;
                }
                else if (tld is CustomParams_GetConnectionString CGCS)
                {
                    CGCS.ConnectionString = connectionString;
                    return (TResult)Convert.ChangeType(connectionString, typeof(TResult));
                }
                else if (tld is CustomParams_Initialize cpInit)
                {
                    processDraftTargets.msuTargets.ItemsSource = ProcessTargetsList;
                }
                return default(TResult);
                //return (TResult)Convert.ChangeType(true, typeof(TResult));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"processDraftTargets_Handler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        public void InsertIntoMultiShortsInfo(int NumberofShorts,
            int LinkedId, DateTime LastTimeUploaded, bool _IsActive = false)
        {
            try
            {
                string sqlA = "";
                int NewId = -1;
                DateTime dtr = LastTimeUploaded.Date;
                TimeSpan dtt = LastTimeUploaded.TimeOfDay;
                sqlA = "SELECT * FROM MULTISHORTSINFO WHERE ISSHORTSACTIVE = 1;";
                int activecnt = connectionString.ExecuteScalar(sqlA).ToInt(-1);
                bool IsActive = activecnt == -1;
                if (dtr.Year > 2000)
                {
                    sqlA = "INSERT INTO MULTISHORTSINFO (ISSHORTSACTIVE,NUMBEROFSHORTS," +
                        " LINKEDSHORTSDIRECTORYID,LASTUPLOADEDDATE,LASTUPLOADEDTIME)" +
                        "VALUES (@ISACTIVE,@NUMBEROFSHORTS,@LINKEDID,@DT,@DTT) RETURNING ID;";
                    NewId = connectionString.ExecuteScalar(sqlA, [("@NUMBEROFSHORTS", NumberofShorts),
                     ("@ISACTIVE",IsActive),("@LINKEDID", LinkedId), ("@DT", dtr), ("@DTT", dtt)]).ToInt(-1);
                }
                else
                {
                    sqlA = "INSERT INTO MULTISHORTSINFO (ISSHORTSACTIVE,NUMBEROFSHORTS," +
                     " LINKEDSHORTSDIRECTORYID)" +
                     "VALUES (@ISACTIVE,@NUMBEROFSHORTS,@LINKEDID) RETURNING ID;";
                    NewId = connectionString.ExecuteScalar(sqlA, [("@NUMBEROFSHORTS", NumberofShorts),
                    ("@ISACTIVE", IsActive),("@LINKEDID", LinkedId)]).ToInt(-1);
                }
                if (NewId != -1)
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    sqlA = "SELECT " +
                  "M.ID, M.ISSHORTSACTIVE, M.NUMBEROFSHORTS, " +
                  "M.LASTUPLOADEDDATE, M.LASTUPLOADEDTIME, M.LINKEDSHORTSDIRECTORYID, " +
                  "COALESCE(S2.ID, S1.ID) as SHORTSDIRECTORY_ID, " +
                  "COALESCE(S2.DIRECTORYNAME, S1.DIRECTORYNAME) as DIRECTORYNAME, " +
                  "COALESCE(S2.TITLEID, S1.TITLEID) as TITLEID, " +
                  "COALESCE(S2.DESCID, S1.DESCID) as DESCID, " +
                  "COALESCE(" +
                  "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S2.TITLEID), " +
                  "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S1.TITLEID)" +
                  ") AS LINKEDTITLEIDS, " + "COALESCE(" +
                  "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S2.DESCID), " +
                  "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S1.DESCID)" +
                  ") AS LINKEDDESCIDS " + "FROM MULTISHORTSINFO M " +
                  "LEFT JOIN (" + "REMATCHED R " +
                  "INNER JOIN SHORTSDIRECTORY S2 ON R.OLDID = S2.ID" +
                  ") ON M.LINKEDSHORTSDIRECTORYID = R.NEWID " +
                  "LEFT JOIN SHORTSDIRECTORY S1 ON M.LINKEDSHORTSDIRECTORYID = S1.ID " +
                 "WHERE COALESCE(S2.ID, S1.ID) IS NOT NULL AND M.ID = @ID";
                    connectionString.ExecuteReader(sqlA, [("@ID", NewId)], cts, (FbDataReader r) =>
                    {
                        SelectedShortsDirectoriesList.Add(new(r));
                        cts.Cancel();
                    });
                    if (true)
                    {

                    }


                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertIntoMultiShorts {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public (DateTime, bool) CheckUploads(int LinkedId)
        {
            try
            {
                bool processed = false;
                string filen = "";
                CancellationTokenSource cts = new CancellationTokenSource();
                DateTime LastTimeUploaded = DateTime.Now.Date.AddYears(-100);
                string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 500;";
                connectionString.ExecuteReader(SQLB, cts, (FbDataReader r) =>
                {
                    filen = (r["UPLOADFILE"] is string f) ? f : "";
                    var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                    TimeSpan dtr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                    if (filen.Contains("_") && !processed)
                    {
                        string Idx = filen.Split('_').LastOrDefault();
                        if (Idx != "" && Idx == LinkedId.ToString())
                        {
                            LastTimeUploaded = dt.AtTime(dtr);
                            processed = true;
                            cts.Cancel();
                        }
                    }
                });
                return (LastTimeUploaded, processed);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"rds {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return (DateTime.Now.Date.AddYears(-100), false);
            }
        }

        private TResult formObjectHandler_MultiShortsUploader<TResult>(object tld, MultiShortsUploader frmMultiShortsUploader)
        {
            try
            {
                if (tld is CustomParams_MoveOrphanFiles cpMOF)
                {
                    string ActiveDir = "";
                    foreach (var t in SelectedShortsDirectoriesList.
                        Where(t => t.IsShortActive && t.NumberOfShorts > 0))
                    {
                        ActiveDir = t.DirectoryName;
                        break;
                    }
                    if (ActiveDir != "")
                    {
                        int cnt = 0;
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        string shortsdir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                        key?.Close();
                        string MoveDir = Path.Combine(shortsdir, ActiveDir);
                        string MDir = Directory.EnumerateDirectories(MoveDir).FirstOrDefault();
                        MoveDir = MDir;
                        string SearchDir = "";
                        var inactiveDirs = SelectedShortsDirectoriesList
                         .Where(t => !t.IsShortActive)
                         .Select(t => Path.Combine(shortsdir, t.DirectoryName))
                         .Where(Directory.Exists);

                        foreach (var searchDir in inactiveDirs)
                        {

                            if (Directory.Exists(MoveDir))
                            {
                                string ssearch = searchDir.Replace(shortsdir, "").Replace("\\", "").ToLower();

                                int LinkedId = SelectedShortsDirectoriesList.
                                        Where(t => t.DirectoryName.ToLower() == ssearch).
                                        FirstOrDefault(new SelectedShortsDirectories()).Id;
                                if (LinkedId != -1)
                                {
                                    var filesToMove = Directory.EnumerateFiles(searchDir, "*.mp4", SearchOption.AllDirectories).Take(4);
                                    foreach (var file in filesToMove)
                                    {
                                        string newfile = file;
                                        if (!file.Contains("_"))
                                        {
                                            newfile = file.Remove(file.LastIndexOf(".")) + "_" + LinkedId.ToString() + ".mp4";
                                        }
                                        File.Move(file, Path.Combine(MoveDir, Path.GetFileName(newfile)));
                                        cnt++;
                                    }
                                }
                            }
                        }
                        for (int i = SelectedShortsDirectoriesList.Count - 1; i >= 0; i--)
                        {
                            var item = SelectedShortsDirectoriesList[i];
                            if (!item.IsShortActive)
                            {
                                string dir = Path.Combine(shortsdir, item.DirectoryName);
                                if (!Directory.EnumerateFiles(dir, "*.mp4", SearchOption.AllDirectories).Any())
                                {
                                    string sql = "DELETE FROM MULTISHORTSINFO WHERE ID = @ID;";
                                    connectionString.ExecuteScalar(sql, [("@ID", item.Id)]);
                                    SelectedShortsDirectoriesList.RemoveAt(i);
                                }
                            }
                        }
                    }


                    return default(TResult);
                }
                if (tld is CustomParams_ScheduleShorts cpsss)
                {
                    var _manualScheduler = new ManualScheduler(InvokerHandler<object>,
                        ManualSchedulerFinish);
                    _manualScheduler.ShowActivated = true;
                    _manualScheduler.IsMultiForm = true;
                    _manualScheduler.ShowMultiForm += (sender) =>
                    {
                        frmMultiShortsUploader.Show();
                    };

                    frmMultiShortsUploader.Hide();
                    _manualScheduler.Show();
                }
                if (tld is CustomParams_RemoveSchedule cpRS)
                {
                    for (int i = SelectedShortsDirectoriesList.Count - 1; i >= 0; i--)
                    {
                        if (SelectedShortsDirectoriesList[i].Id == cpRS.id)
                        {
                            string sql = "DELETE FROM MULTISHORTSINFO WHERE ID = @ID;";
                            connectionString.ExecuteScalar(sql, [("@ID", cpRS.id)]);
                            SelectedShortsDirectoriesList.RemoveAt(i);
                            break;
                        }
                    }
                    return default(TResult);
                }

                if (tld is CustomParams_RemoveMulitShortsInfoById cpRMSI)
                {
                    //frmMultiShortsUploader.msuSchedules.ItemsSource = null;
                    for (int i = frmMultiShortsUploader.msuSchedules.Items.Count - 1; i >= 0; i--)
                    {
                        if (SelectedShortsDirectoriesList[i].NumberOfShorts == 0)
                        {
                            string sql = "DELETE FROM MULTISHORTSINFO WHERE" +
                                " LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                            connectionString.ExecuteScalar(sql, [("@LINKEDID",
                                SelectedShortsDirectoriesList[i].LinkedShortsDirectoryId)]);
                            SelectedShortsDirectoriesList.RemoveAt(i);
                        }
                    }
                    return default(TResult);
                }
                if (tld is CustomParams_SetActive tsa)
                {
                    foreach (var item in SelectedShortsDirectoriesList)
                    {
                        item.IsActive = (item.LinkedShortsDirectoryId == tsa.index) ? true : false;
                        string sql = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE = @ISSHORTSACTIVE "
                            + "WHERE LINKEDSHORTSDIRECTORYID = @LINKEDSHORTSDIRECTORYID;";
                        connectionString.ExecuteScalar(sql, [("@ISSHORTSACTIVE", item.IsActive),
                            ("@LINKEDSHORTSDIRECTORYID", item.LinkedShortsDirectoryId)]);
                        if (item.IsActive)
                        {
                            string sqla = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 10;";
                            CancellationTokenSource cts = new();
                            DateTime Opt = new DateTime(1900, 1, 1);
                            connectionString?.ExecuteReader(sqla.ToUpper(), cts, (FbDataReader r) =>
                            {
                                var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : Opt;// new DateTime(1900, 1, 1);
                                var tt = (r["UPLOAD_TIME"] is TimeSpan t) ? t : TimeSpan.Zero;// new DateTime(0, 0, 0);
                                var DateA = dt.AtTime(tt);
                                string UploadFile = (r["UPLOADFILE"] is string f) ? f : "";
                                if (UploadFile.Contains("_") && dt.Year > 1900)
                                {
                                    int Idx = UploadFile.Split('_').LastOrDefault().ToInt(-1);
                                    if (Idx != -1 && item.LinkedShortsDirectoryId == Idx)
                                    {
                                        item.LastUploadedDateFile = DateA;
                                        cts.Cancel();
                                    }
                                }
                            });
                        }
                        else
                        {
                            item.LastUploadedDateFile = new DateTime(1900, 1, 1);
                        }
                    }
                }
                if (tld is CustomParams_SetIndex cpsii)
                {
                    ShortsDirectoryIndex = cpsii.index;
                }
                if (tld is CustomParams_RematchedLookup cpRL)
                {
                    foreach (var item in RematchedList.Where(s => s.OldId == cpRL.oldId))
                    {
                        return (TResult)Convert.ChangeType(item.NewId, typeof(TResult));
                    }
                }

                if (tld is CustomParams_LookUpId pclki)
                {
                    string dir = pclki.DirectoryName.ToUpper();
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Directory.ToUpper() == dir))
                    {
                        return (TResult)Convert.ChangeType(item.Id, typeof(TResult));
                    }
                    return (TResult)Convert.ChangeType(-1, typeof(TResult));
                }
                else if (tld is CustomParams_GetDirectory cgfd)
                {
                    int index = -1;
                    bool found = false;
                    string DirName = (tld as CustomParams_GetDirectory).DirectoryName;
                    string HostDir = DirName;
                    DirName = (DirName.Contains("\\")) ? DirName.Split('\\').LastOrDefault() : DirName;
                    foreach (var item in EditableshortsDirectoryList.Where(
                        s => s.Directory.ToUpper() == DirName.ToUpper()))
                    {
                        index = item.Id;
                        found = true;
                        break;
                    }
                    if (!found)
                    {
                        index = InsertUpdateShorts(DirName);
                        string indexStr = $"_{index}.mp4";
                        var files = Directory.EnumerateFiles(HostDir, "*.mp4", SearchOption.AllDirectories).ToList();
                        foreach (var _file in files.Where(s => !s.Contains("_")))
                        {
                            string fx = _file.ToLower().Replace(".mp4", $"{indexStr}");
                            if (fx.EndsWith(".mp4"))
                            {
                                File.Move(_file, fx);
                            }
                        }
                    }

                    return (TResult)Convert.ChangeType(index, typeof(TResult));
                }
                else if (tld is CustomParams_DescUpdate pcdu)
                {
                    string DirName = (tld as CustomParams_DescUpdate).DirectoryName;
                    string Desc = (tld as CustomParams_DescUpdate).Description;
                    var result = DescUpdater(frmMultiShortsUploader, DirName, Desc, false);
                    return (TResult)Convert.ChangeType(result, typeof(TResult));
                }
                else if (tld is CustomParams_UpdateDescById CPUDI)
                {
                    foreach (var item in SelectedShortsDirectoriesList.Where(s => s.Id == CPUDI.Id))
                    {
                        item.TitleId = CPUDI.Desc;
                    }
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == CPUDI.LinkedId))
                    {
                        item.DescId = CPUDI.Desc;
                    }
                    string LinkedDesc = "", sql = "UPDATE SHORTSDIRECTORY SET DESCID=@DESCID WHERE ID=@ID;";
                    connectionString.ExecuteScalar(sql, [("@DESCID", CPUDI.Desc), ("@ID", CPUDI.Id)]);
                    sql = GetShortsDirectorySql(CPUDI.LinkedId);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionString.ExecuteReader(sql, cts, (FbDataReader r) =>
                    {
                        if (LinkedDesc == "")
                        {
                            LinkedDesc = (r["LINKEDDESCIDS"] is string lt) ? lt : "";
                        }
                        cts.Cancel();
                    });
                    foreach (var item in SelectedShortsDirectoriesList.Where(s => s.Id == CPUDI.Id))
                    {
                        item.LinkedDescId = LinkedDesc;
                        break;
                    }
                }
                else if (tld is CustomParams_UpdateTitleById CPUTI)
                {
                    foreach (var item in SelectedShortsDirectoriesList.Where(s => s.Id == CPUTI.Id))
                    {
                        item.TitleId = CPUTI.Title;
                    }
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == CPUTI.LinkedId))
                    {
                        item.TitleId = CPUTI.Title;
                    }
                    string LinkedTitle = "", sql = "UPDATE SHORTSDIRECTORY SET TITLEID=@TITLEID WHERE ID=@ID;";
                    connectionString.ExecuteScalar(sql, [("@TITLEID", CPUTI.Title), ("@ID", CPUTI.Id)]);
                    sql = GetShortsDirectorySql(CPUTI.LinkedId);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionString.ExecuteReader(sql, cts, (FbDataReader r) =>
                    {
                        if (LinkedTitle == "")
                        {
                            LinkedTitle = (r["LINKEDTITLEIDS"] is string lt) ? lt : "";
                        }
                        cts.Cancel();
                    });
                    foreach (var item in SelectedShortsDirectoriesList.Where(s => s.Id == CPUTI.Id))
                    {
                        item.LinkedTitleId = LinkedTitle;
                        break;
                    }
                }
                else if (tld is CustomParams_GetConnectionString)
                {
                    return (TResult)Convert.ChangeType(connectionString, typeof(TResult));
                }
                else if (tld is CustomParams_RemoveSelectedDirectory CPRSD)
                {
                    string s = "DELETE FROM MULTISHORTSINFO WHERE ID = @ID;";
                    connectionString.ExecuteScalar(s, [("@ID", CPRSD.Id)]);
                    SelectedShortsDirectoriesList.Remove(SelectedShortsDirectoriesList.Where(s => s.Id == CPRSD.Id).FirstOrDefault());
                }
                else if (tld is CustomParams_AddDirectory CPAD)
                {
                    string sql = "";
                    int LinkedId = InsertUpdateShorts(CPAD.DirectoryName);
                    bool found = false, processed = false;
                    string uploaddir = CPAD.DirectoryName;
                    DateTime LastTimeUploaded = DateTime.Now.Date.AddYears(-100);
                    (LastTimeUploaded, processed) = CheckUploads(LinkedId);

                    found = SelectedShortsDirectoriesList.Any(item => item.DirectoryName == uploaddir);
                    if (!found && uploaddir != "")
                    {
                        InsertUpdateMultiShorts(LinkedId, uploaddir);
                    }
                    foreach (var ssd in ShortsDirectoryList)
                    {
                        ssd.IsActive = !SelectedShortsDirectoriesList.Any(ssdk => ssdk.DirectoryName.ToLower() == ssd.DirectoryName.ToLower());
                    }
                }
                else if (tld is CustomParams_MoveDirectory CPMD)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string shortsdir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                    key?.Close();
                    string oldpathdir = Path.Combine(shortsdir, CPMD.DirectoryName);
                    if (Path.Exists(oldpathdir))
                    {
                        string newpathdir = Path.Combine(shortsdir, "done", CPMD.DirectoryName);
                        string ShortsPath = Path.Combine(shortsdir, "done");
                        List<string> FileToMove = Directory.EnumerateFiles(oldpathdir, "*.*", SearchOption.AllDirectories).ToList();
                        foreach (string file in FileToMove)
                        {
                            string nw = file.Replace(shortsdir, ShortsPath + "\\");
                            string NewPath = Path.GetDirectoryName(nw);
                            if (!Directory.Exists(NewPath))
                            {
                                Directory.CreateDirectory(NewPath);
                            }
                            File.Move(file, nw);
                        }
                        Directory.Delete(oldpathdir, true);
                        for (int i = 0; i < ShortsDirectoryList.Count; i++)
                        {
                            if (ShortsDirectoryList[i].DirectoryName == CPMD.DirectoryName)
                            {
                                ShortsDirectoryList.RemoveAt(i);
                                break;
                            }
                        }
                        foreach (var ssd in ShortsDirectoryList)
                        {
                            ssd.IsActive = !SelectedShortsDirectoriesList.Any(ssdk => ssdk.DirectoryName.ToLower() == ssd.DirectoryName.ToLower());
                        }
                    }
                }
                else if (tld is CustomParams_Initialize CPRE)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string shortsdir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                    string uploaddir = FindUploadPath();
                    key?.Close();
                    CancellationTokenSource cts = new();
                    string filen = "", shortdirectoryId = "";
                    DateTime LastUploaded = DateTime.Now.Date.AddYears(-100);
                    string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 1;";
                    connectionString.ExecuteReader(SQLB, cts, (FbDataReader r) =>
                    {
                        filen = (r["UPLOADFILE"] is string f) ? f : "";
                        var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                        TimeSpan dtr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                        LastUploaded = dt.AtTime(dtr);
                        cts.Cancel();
                    });
                    frmMultiShortsUploader.lblUploaded.Content = $"Last uploaded : {filen.Replace("_", "-")} @ {LastUploaded}";
                    if (Path.Exists(shortsdir))
                    {
                        ShortsDirectoryList.Clear();
                        List<string> DirectoryList = Directory.EnumerateDirectories(shortsdir).ToList();
                        foreach (var dir in DirectoryList.Where(dir => !dir.ToLower().EndsWith("done")))
                        {
                            var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                               .Where(f => f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                               f.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase));
                            if (files.Any())
                            {
                                bool IsLinked = false;
                                ShortsDirectoryList.Add(new MultiShortsInfo(dir, files.Count(), IsActive));
                            }

                        }
                    }
                    bool found = false;
                    int LinkedId = -1;




                    if (uploaddir != "" && Path.Exists(uploaddir) &&
                        Directory.EnumerateFiles(uploaddir, "*.mp4", SearchOption.AllDirectories).Count() > 0)
                    {
                        string SearchDir = uploaddir.Substring(shortsdir.Length).ToLower();

                        foreach (var fnd in EditableshortsDirectoryList.Where(fnd => fnd.Directory.ToLower() == SearchDir))
                        {
                            LinkedId = fnd.Id;
                            break;
                        }
                        bool processed = false;
                        DateTime LastTimeUploaded = DateTime.Now.Date.AddYears(-100);
                        (LastTimeUploaded, processed) = CheckUploads(LinkedId);
                        foreach (var _ in SelectedShortsDirectoriesList.Where(item => item.DirectoryName == uploaddir).Select(item => new { }))
                        {
                            found = true;
                            break;
                        }
                        if (!found && uploaddir != "")
                        {
                            key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            string shorts_dir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                            key?.Close();
                            SearchDir = Path.Combine(shorts_dir, uploaddir);
                            int NumberofShorts = Directory.EnumerateFiles(SearchDir, "*.mp4", SearchOption.AllDirectories).Count();
                            string sqlA = "SELECT ID FROM MULTISHORTSINFO WHERE" +
                                " LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                            int id = connectionString.ExecuteScalar(sqlA, [("@LINKEDID", LinkedId)]).ToInt(-1);
                            if (id == -1)
                            {
                                InsertIntoMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded, true);
                            }
                            else
                            {
                                UpdateMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded, uploaddir);
                            }
                        }

                    }
                    else
                    {
                        if (true)
                        {

                        }
                    }
                    CancellationTokenSource ctsx = new();
                    filen = "";
                    shortdirectoryId = "";
                    LastUploaded = DateTime.Now.Date.AddYears(-100);
                    SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 1;";
                    connectionString.ExecuteReader(SQLB, ctsx, (FbDataReader r) =>
                    {
                        filen = (r["UPLOADFILE"] is string f) ? f : "";
                        var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                        TimeSpan dtr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                        if (filen.Contains("_"))
                        {
                            shortdirectoryId = filen.Split('_').LastOrDefault();
                            LastUploaded = dt.AtTime(dtr);
                            ctsx.Cancel();
                        }
                    });

                    if (shortdirectoryId.NotNullOrEmpty() && shortdirectoryId.ToInt(-1) != -1)
                    {
                        foreach (var item in SelectedShortsDirectoriesList.Where(s => s.IsActive).Where(item => item.LinkedShortsDirectoryId == shortdirectoryId.ToInt()))
                        {
                            item.LastUploadedDateFile = LastUploaded;
                            break;
                        }
                    }

                    foreach (var ssd in ShortsDirectoryList)
                    {
                        ssd.IsActive = !SelectedShortsDirectoriesList.Any(ssdk => ssdk.DirectoryName.ToLower() == ssd.DirectoryName.ToLower());
                    }

                    frmMultiShortsUploader.msuShorts.ItemsSource = ShortsDirectoryList;
                    frmMultiShortsUploader.msuSchedules.ItemsSource = SelectedShortsDirectoriesList;
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_MultiShortsUploader {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        public string FindUploadPath()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shortsdir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                string uploaddir = key.GetValueStr("UploadPath", "");
                key?.Close();
                if (!Path.Exists(uploaddir))
                {
                    foreach (var dir in from dir in Directory.EnumerateDirectories(shortsdir).ToList()
                                        let p = dir.Split('\\').LastOrDefault()
                                        where p == uploaddir.Split('\\').LastOrDefault()
                                        select dir)
                    {
                        return dir;
                    }
                }
                return uploaddir;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FindUploadPath {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return "";
            }
        }

        private TResult formObjectHandler_GoProMediaImporter<TResult>(object tld, MediaImporter goProMediaImporter)
        {
            try
            {
                if (tld is CustomParams_ImportRecord cpIR)
                {
                    FileRenamer.Add(new FileInfoGoPro(cpIR.f1, cpIR.f2, cpIR.t1));
                }
                if (tld is CustomParams_ReOrderFiles cpref)
                {
                    foreach (var r in FileRenamer)
                    {
                        string Newfile = r.NewFile;
                        string OldFile = r.FileName;
                        if (Newfile != OldFile)
                        {
                            string newFile = System.IO.Path.Combine(cpref.filename, Newfile);
                            string filename = System.IO.Path.Combine(cpref.filename, OldFile);
                            System.IO.File.Move(filename, newFile);
                        }
                    }
                }
                if (tld is CustomParams_ClearCheck cpcc)
                {
                    if (cpcc.mode == ClearModes.ClearImports)
                    {
                        ObservableCollectionFilter?.ClearImportTimes();
                    }
                    else if (cpcc.mode == ClearModes.ClearTimes)
                    {
                        FileRenamer.Clear();
                        ObservableCollectionFilter?.ClearImportTimes();
                    }
                    else if (cpcc.mode == ClearModes.CheckImports)
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
                        if (cpcc.files is not null && cpcc.files.Count > 0)
                        {
                            res = true;
                            bool SortNeeded = false;
                            foreach (var f in FileRenamer)
                            {
                                
                                if (!cpcc.files.Contains(f.NewFile))
                                {
                                    res = false;
                                    SortNeeded = true;
                                    break;
                                }
                            }
                           
                            if (res)
                            {
                                List<(string, TimeSpan)> FileData = new();
                                foreach(var r in cpcc.files)
                                {
                                    foreach(var r1 in FileRenamer.Where(s=>s.FileName == r))
                                    {
                                        FileData.Add(new(r, r1.TimeData));
                                    }
                                }
                                if (FileRenamer.Count == FileData.Count)
                                {
                                    for (int i = 0; i < FileRenamer.Count - 1; i++)
                                    {
                                        if (FileRenamer[i].TimeData != FileData[i].Item2)
                                        {
                                            res = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }

                        return (TResult)Convert.ChangeType(!res, typeof(TResult));
                    }
                }
                if (tld is CustomParams_SetTimeSpan cts)
                {
                    if (cts.mode == TimeSpanMode.ToTime)
                    {
                        ObservableCollectionFilter?.SetToTimeSpan(cts.thistime);
                    }
                    else if (cts.mode == TimeSpanMode.FromTime)
                    {
                        ObservableCollectionFilter?.SetFromTimeSpan(cts.thistime);
                    }
                }
                if (tld is CustomParams_GetConnectionString CGCS)
                {
                    CGCS.ConnectionString = connectionString;
                    return (TResult)Convert.ChangeType(connectionString, typeof(TResult));
                }
                else if (tld is CustomParams_Initialize cpInit)
                {
                    if (ObservableCollectionFilter is not null)
                    {
                        FileRenamer.Clear();
                        ObservableCollectionFilter.ImportCollectionViewSource.Source = FileRenamer;
                        ObservableCollectionFilter.ImportCollectionViewSource.View.Refresh();
                        goProMediaImporter.msuSchedules.ItemsSource = ObservableCollectionFilter.ImportCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                }
                else if (tld is CustomParams_DataSelect cds)
                {
                    if (ObservableCollectionFilter is not null)
                    {
                        FileRenamer.Clear();
                        ObservableCollectionFilter.ImportCollectionViewSource.Source = FileRenamer;
                        ObservableCollectionFilter.ImportCollectionViewSource.View.Refresh();
                        goProMediaImporter.msuSchedules.ItemsSource = ObservableCollectionFilter.ImportCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_GoProMediaImporter {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        private TResult formObjectHandler_ComplexSchedular<TResult>(object tld, ComplexSchedular complexSchedular)
        {
            try
            {
                if (tld is CustomParams_DataSelect cds && complexSchedular.IsLoaded)
                {
                    if (cds.Id == 0)
                    {
                        ObservableCollectionFilter.CurrentCollectionViewSource.Source = ComplexProcessingJobList;
                        complexSchedular.msuComplexSchedules.ItemsSource = ObservableCollectionFilter.CurrentCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                    else if (cds.Id == 1)
                    {
                        ObservableCollectionFilter.HistoricCollectionViewSource.Source = ComplexProcessingJobHistory;
                        complexSchedular.msuComplexSchedules.ItemsSource = ObservableCollectionFilter.HistoricCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                }
                else if (tld is CustomParams_GetConnectionString CGCS)
                {
                    CGCS.ConnectionString = connectionString;
                    return (TResult)Convert.ChangeType(connectionString, typeof(TResult));
                }
                else if (tld is CustomParams_Initialize cpInit)
                {
                    if (cpInit.Id == 0)
                    {
                        ObservableCollectionFilter.CurrentCollectionViewSource.Source = ComplexProcessingJobList;
                        complexSchedular.msuComplexSchedules.ItemsSource = ObservableCollectionFilter.CurrentCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                    else if (cpInit.Id == 1)
                    {
                        ObservableCollectionFilter.HistoricCollectionViewSource.Source = ComplexProcessingJobHistory;
                        complexSchedular.msuComplexSchedules.ItemsSource = ObservableCollectionFilter.HistoricCollectionViewSource.View;
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_ComplexSchedular {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        public Nullable<DateTime> LoadDate(string name)
        {
            try
            {
                string sql = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                int idx = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (idx == -1) return null;
                sql = "SELECT SETTINGDATE FROM SETTINGS WHERE SETTINGNAME = @P0";
                var obj = connectionString.ExecuteScalar(sql, [("@P0", name)]);
                if (obj is DateTime dt)
                {
                    return dt;
                }
                else return null;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadDate {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return null;
            }
        }

        public Nullable<TimeSpan> LoadTime(string name)
        {
            try
            {
                string sql = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                int idx = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (idx == -1) return null;
                sql = "SELECT SETTINGTIME FROM SETTINGS WHERE SETTINGNAME = @P0";
                var obj = connectionString.ExecuteScalar(sql, [("@P0", name)]);
                if (obj is TimeSpan ts)
                {
                    return ts;
                }
                else return null;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadTime {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return null;
            }
        }

        public void SaveTime(TimeSpan startdate, string name, bool IsDateTime = false)
        {
            try
            {
                string sql = "SELECT * FROM SETTINGS WHERE SETTINGNAME = @P0";
                int id = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (id != -1)
                {
                    sql = "UPDATE SETTINGS SET SETTINGTIME = @P1 WHERE ID = @P2";
                    connectionString.ExecuteScalar(sql, [("@P1", startdate), ("@P2", id)]);
                }
                else
                {
                    sql = "INSERT INTO SETTINGS (SETTINGTIME,SETTINGNAME) VALUES (@P0,@P1)";
                    connectionString.ExecuteScalar(sql, [("@P0", startdate), ("@P1", name)]);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SaveTime {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public string LoadString(string name)
        {
            try
            {
                string sql = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                int idx = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (idx == -1) return null;
                sql = "SELECT SETTING FROM SETTINGS WHERE SETTINGNAME = @P0";
                var obj = connectionString.ExecuteScalar(sql, [("@P0", name)]);
                if (obj is string strx)
                {
                    return strx;
                }
                else return null;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadString {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return null;
            }
        }
        public void SaveString(string str, string name)
        {
            try
            {
                string sql = "SELECT * FROM SETTINGS WHERE SETTINGNAME = @P0";
                int id = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (id != -1)
                {
                    sql = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @P2";
                    connectionString.ExecuteScalar(sql, [("@P1", str), ("@P2", id)]);
                }
                else
                {
                    sql = "INSERT INTO SETTINGS (SETTING,SETTINGNAME) VALUES (@P0,@P1)";
                    connectionString.ExecuteScalar(sql, [("@P0", str), ("@P1", name)]);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SaveString {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void SaveDates(DateTime startdate, string name, bool IsDateTime = false)
        {
            try
            {
                string sql = "SELECT * FROM SETTINGS WHERE SETTINGNAME = @P0";
                int id = connectionString.ExecuteScalar(sql, [("@P0", name)]).ToInt(-1);
                if (id != -1)
                {
                    sql = "UPDATE SETTINGS SET SETTINGDATE = @P1 WHERE ID = @P2";
                    connectionString.ExecuteScalar(sql, [("@P1", startdate.Date), ("@P2", id)]);
                }
                else
                {
                    sql = "INSERT INTO SETTINGS (SETTINGDATE,SETTINGNAME) VALUES (@P0,@P1)";
                    connectionString.ExecuteScalar(sql, [("@P0", startdate.Date), ("@P1", name)]);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SavedDates {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private TResult formObjectHandler_ManualScheduler<TResult>(object tld, ManualScheduler manualScheduler)
        {
            try
            {
                if (tld is CustomParams_Initialize cpInit)
                {
                    Nullable<DateTime> dt = LoadDate("ScheduleDate");
                    Nullable<TimeSpan> st = LoadTime("ScheduleTimeStart");
                    Nullable<TimeSpan> et = LoadTime("ScheduleTimeEnd");
                    var s = LoadString("maxr");

                    if (et.HasValue && st.HasValue)
                    {
                        TimeSpan ts = new TimeSpan(0, 0, 0);
                        if (st.Value == ts && et.Value == ts)
                        {
                            st = new TimeSpan(11, 0, 0);
                            et = new TimeSpan(20, 0, 0);
                            SaveTime(st.Value, "ScheduleTimeStart");
                            SaveTime(et.Value, "ScheduleTimeEnd");
                        }
                    }
                    var test = LoadString("TestMode");
                    bool IsTest = (test == "True") ? true : false;
                    string pp = (s is string str) ? str : "";
                    DateTime r = new DateTime(1, 1, 1);
                    if (dt.HasValue && st.HasValue && et.HasValue && pp != "")
                    {
                        if (dt.Value.Year < 1900)
                        {
                            int yrdiff = DateTime.Now.Year - dt.Value.Year;
                            dt = dt.Value.AddYears(yrdiff);
                        }
                        manualScheduler.txtMaxSchedules.Text = pp;
                        manualScheduler.ReleaseDate.Value = dt.Value.Date;
                        manualScheduler.ReleaseTimeStart.Value = r.AtTime(st);
                        manualScheduler.ReleaseTimeEnd.Value = r.AtTime(et);
                        manualScheduler.chkSchedule.IsChecked = IsTest;
                    }
                }
                else if (tld is CustomParams_SaveSchedule cpSaveSchedule)
                {
                    var dt = cpSaveSchedule.ScheduleDate.Date.Date;
                    var st = cpSaveSchedule.ScheduleTimeStart;
                    var et = cpSaveSchedule.ScheduleTimeEnd;
                    SaveDates(dt, "ScheduleDate");
                    SaveTime(st, "ScheduleTimeStart");
                    SaveTime(et, "ScheduleTimeEnd");
                    SaveString(cpSaveSchedule.TestMode.ToString(), "TestMode");
                    SaveString(cpSaveSchedule.max.ToString(), "maxr");
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_ManualScheduler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }
        private TResult formObjectHandler_ActionScheduleSelector<TResult>(object tld, ActionScheduleSelector actionScheduleSelector)
        {
            try
            {
                if (tld is CustomParams_Initialize cpInit)
                {
                    ObservableCollectionFilter.SetFilterData(ObservableCollectionFilter.ActionsScheduleCollectionViewSource,
                        ActionScheduleFilterType.TargetDate, DateTime.Now, false);
                    ObservableCollectionFilter.SetFilterData(ObservableCollectionFilter.ActionsScheduleCollectionViewSource,
                    ActionScheduleFilterType.IsCompleted, false, true);

                    actionScheduleSelector.lstItems.ItemsSource = ObservableCollectionFilter.ActionsScheduleCollectionViewSource.View;
                }
                else if (tld is CustomParams_Delete cpDel && cpDel.Id != -1)
                {
                    int index = YTScheduledActionsList.Where(s => s.Id == cpDel.Id).FirstOrDefault()?.Id ?? -1;
                    if (index != -1)
                    {
                        YTScheduledActionsList.RemoveAt(index);
                        string sql = "DELETE FROM YTACTIONS WHERE ID = @ID";
                        connectionString.ExecuteScalar(sql, [("@ID", cpDel.Id)]);
                    }

                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_ActionScheduleSelector {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        private TResult formObjectHandler_ScheduleActioner<TResult>(object tld, ScheduleActioner scheduleActioner)
        {
            try
            {
                if (tld is CustomParams_Initialize cpInit)
                {
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int idx = connectionString.ExecuteScalar(sqla, [("@P0", "ACTIONSCHEDULEID")]).ToInt(-1);
                    if ((cpInit.Id != -1) || (cpInit.Id == -1 && idx != -1))// grab data
                    {
                        int Index = -1;
                        if (cpInit.Id != -1) Index = cpInit.Id;
                        if (cpInit.Id == -1 && idx != -1) Index = cpInit.Id;
                        foreach (var item in YTScheduledActionsList.Where(s => s.Id == cpInit.Id))
                        {
                            scheduleActioner.txtActionName.Text = (scheduleActioner.IsCopy) ? "" : item.ActionName;
                            scheduleActioner.txtMaxSchedules.Text = item.Max.ToString();
                            scheduleActioner.txtSchName.Text = item.ScheduleName;
                            scheduleActioner.ReleaseDate.Value = (item.ActionSchedule.HasValue) ?
                                new DateTime(item.ActionSchedule.Value.Year,
                                item.ActionSchedule.Value.Month, item.ActionSchedule.Value.Day) : null;
                            scheduleActioner.ReleaseTimeStart.Value = (item.ActionScheduleStart.HasValue) ?
                                new DateTime(1, 1, 1, item.ActionScheduleStart.Value.Hours,
                                item.ActionScheduleStart.Value.Minutes,
                                item.ActionScheduleStart.Value.Seconds) : null;//new DateTime(item.ActionScheduleStart.Value.Year, item.ActionScheduleStart.Value.Month, item.ActionScheduleStart.Value.Day) : null;
                            scheduleActioner.ReleaseTimeEnd.Value = (item.ActionScheduleEnd.HasValue) ?
                                new DateTime(1, 1, 1, item.ActionScheduleEnd.Value.Hours,
                                item.ActionScheduleEnd.Value.Minutes,
                                item.ActionScheduleEnd.Value.Seconds) : null;//new DateTime(item.ActionScheduleStart.Value.Year, item.ActionScheduleStart.Value.Month, item.ActionScheduleStart.Value.Day) : null;


                            scheduleActioner.AppliedDate.Value = (item.AppliedAction.HasValue) ? item.AppliedAction : null;
                            scheduleActioner.AppliedTime.Value = (item.AppliedAction.HasValue) ? item.AppliedAction : null;
                            break;
                        }
                    }
                    else
                    {
                        scheduleActioner.txtActionName.Text = "";
                        scheduleActioner.txtMaxSchedules.Text = "";
                        scheduleActioner.txtSchName.Text = "";
                        scheduleActioner.ReleaseDate.Value = DateTime.Now.Date;
                        scheduleActioner.ReleaseTimeStart.Value = null;
                        scheduleActioner.ReleaseTimeEnd.Value = null;
                        scheduleActioner.AppliedDate.Value = DateTime.Now.Date;
                        scheduleActioner.AppliedTime.Value = null;
                    }
                }
                else if (tld is CustomParams_UpdateAction cpUpdateAction)
                {
                    bool found = false;
                    foreach (var item in YTScheduledActionsList.Where(s => s.ActionName == cpUpdateAction.ActionName))
                    {
                        found = true;
                        break;
                    }
                    if (!found)
                    {
                        /*"SCHEDULENAMEID,ACTIONNAMEID,SCHEDULENAME, ACTIONNAME, MAX,VIDEOTYPE 0, ISACTIONED 0
                          SCHEDULED_DATE DATE, SCHEDULED_TIME TIME,ACTION_DATE DATE, ACTION_TIME TIME); */
                        var ActionDate = cpUpdateAction.ActionDate.Value.Date;
                        var ActionTime = cpUpdateAction.ActionDate.Value.TimeOfDay;
                        var ScheduleDate = cpUpdateAction.ScheduleDate.Value;
                        var ScheduleTimeStart = cpUpdateAction.ScheduleTimeStart.Value;
                        var ScheduleTimeEnd = cpUpdateAction.ScheduleTimeEnd.Value;
                        int SheduleNameId = -1;
                        foreach (var item in SchedulingNamesList.Where(item => item is ScheduleMapNames scc && scc.Name == cpUpdateAction.ScheduleName))
                        {
                            SheduleNameId = item.Id;
                            break;
                        }
                        string sqla = "INSERT INTO YTACTIONS(SCHEDULENAMEID,SCHEDULENAME,ACTIONNAME,MAXSCHEDULES," +//4
                            "VIDEOTYPE,SCHEDULED_DATE,SCHEDULED_TIME_START,SCHEDULED_TIME_END,ACTION_DATE," +//9
                            "ACTION_TIME,ISACTIONED) VALUES(@SCHEDULENAMEID,@SCHEDULENAME,@ACTIONNAME," +
                            "@MAX,0,@SCHEDULED_DATE,@SCHEDULED_TIME_START,@SCHEDULED_TIME_END,@ACTION_DATE," +
                            "@ACTION_TIME,0) RETURNING ID;";
                        int idx = connectionString.ExecuteScalar(sqla,
                            [("@SCHEDULENAMEID", SheduleNameId),("@SCHEDULENAME", cpUpdateAction.ScheduleName),
                            ("@ACTIONNAME", cpUpdateAction.ActionName),("@MAX", cpUpdateAction.Max),("@ACTION_TIME", ActionTime),
                            ("@SCHEDULED_DATE", ScheduleDate),("@SCHEDULED_TIME_START", ScheduleTimeStart),
                            ("@SCHEDULED_TIME_END", ScheduleTimeEnd),("@ACTION_DATE", ActionDate)]).ToInt(-1);
                        if (idx != -1)
                        {
                            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                            sqla = "SELECT * FROM YTACTIONS WHERE ID = @ID;";
                            connectionString.ExecuteReader(sqla, [("@ID", idx)], cts, (FbDataReader r) =>
                            {
                                YTScheduledActionsList.Add(new ScheduledActions(r));
                                cts.Cancel();
                            });
                        }
                    }
                    else
                    {
                        List<(string, object)> Params = new List<(string, object)>();
                        string usql = "update YTACTIONS set ";
                        string wsql = "where ID = @ID and ACTIONNAME = @ACTIONNAME;";
                        Params.Add(("@ACTIONNAME", cpUpdateAction.ActionName));
                        Params.Add(("@ID", cpUpdateAction.id));
                        foreach (var item in YTScheduledActionsList.Where(s => s.ActionName == cpUpdateAction.ActionName))
                        {
                            bool update = false, cpsd = cpUpdateAction.ScheduleDate.HasValue,
                                iaa = item.AppliedAction.HasValue,
                                cpad = cpUpdateAction.ActionDate.HasValue, ias = item.ActionSchedule.HasValue;

                            update = cpUpdateAction.ScheduleDate.HasValue && (item.ActionSchedule == null || item.ActionSchedule.Value != cpUpdateAction.ScheduleDate.Value);
                            if (update)
                            {
                                usql += "SCHEDULED_DATE = @SCHEDULED_DATE, ";
                                Params.Add(("@SCHEDULED_DATE", cpUpdateAction.ScheduleDate.Value));
                            }

                            if (cpUpdateAction.ScheduleTimeStart.HasValue && (item.ActionScheduleStart == null || item.ActionScheduleStart.Value != cpUpdateAction.ScheduleTimeStart.Value))
                            {
                                usql += "SCHEDULED_TIME_START = @SCHEDULED_TIME_START, ";
                                Params.Add(("@SCHEDULED_TIME_START", cpUpdateAction.ScheduleTimeStart.Value));
                            }

                            if (cpUpdateAction.ScheduleTimeEnd.HasValue && (item.ActionScheduleEnd == null || item.ActionScheduleEnd.Value != cpUpdateAction.ScheduleTimeEnd.Value))
                            {
                                usql += "SCHEDULED_TIME_END = @SCHEDULED_TIME_END, ";
                                Params.Add(("@SCHEDULED_TIME_END", cpUpdateAction.ScheduleTimeEnd.Value));
                            }
                            /*if ((cpsd && !ias) || (cpsd && ias && item.ActionSchedule.Value != cpUpdateAction.ScheduleDate.Value.Date))
                            {
                                var ScheduleDate = cpUpdateAction.ScheduleDate.Value.Date;
                                var ScheduleTime = cpUpdateAction.ScheduleDate.Value.TimeOfDay;
                                usql += "SCHEDULED_DATE = @SCHEDULED_DATE, SCHEDULED_TIME = @SCHEDULED_TIME, ";
                                Params.Add(("SCHEDULED_DATE", ScheduleDate));
                                Params.Add(("SCHEDULED_TIME", ScheduleTime));
                            }*/
                            if ((cpad && !iaa) || (cpad && iaa && item.AppliedAction.Value != cpUpdateAction.ActionDate.Value))
                            {
                                var ActionDate = cpUpdateAction.ActionDate.Value.Date;
                                var ActionTime = cpUpdateAction.ActionDate.Value.TimeOfDay;
                                usql += "ACTION_DATE = @ACTION_DATE, ACTION_TIME = @ACTION_TIME, ";
                                Params.Add(("@ACTION_DATE", ActionDate));
                                Params.Add(("@ACTION_TIME", ActionTime));
                            }
                            if (cpUpdateAction.Max != item.Max)
                            {
                                usql += "MAXSCHEDULES = @MAX, ";
                                Params.Add(("@MAX", cpUpdateAction.Max));
                            }
                            if (cpUpdateAction.ScheduleName != item.ScheduleName)
                            {
                                usql += "SCHEDULENAME = @SCHEDULENAME, SCHEDULENAMEID = @SCHEDULENAMEID, ";
                                int SheduleNameId = -1;
                                foreach (var items in SchedulingNamesList.Where(item => item is ScheduleMapNames scc && scc.Name == cpUpdateAction.ScheduleName))
                                {
                                    SheduleNameId = items.Id;
                                    break;
                                }
                                Params.Add(("@SCHEDULENAME", (SheduleNameId != -1) ? cpUpdateAction.ScheduleName : ""));
                                Params.Add(("@SCHEDULENAMEID", SheduleNameId));
                            }
                            if (usql != "update YTACTIONS set ")
                            {
                                usql = usql.Substring(0, usql.Length - 2);
                                usql += " " + wsql;
                                connectionString.ExecuteScalar(usql, Params);
                                ScheduledActions itemx = new ScheduledActions();
                                connectionString.ExecuteReader("select * from YTACTIONS where id = @ID", [("@ID", cpUpdateAction.id)], (FbDataReader r) =>
                                {
                                    itemx = new ScheduledActions(r);
                                });
                                if (itemx is not null && itemx.ActionName == cpUpdateAction.ActionName && itemx.Id == cpUpdateAction.id)
                                {
                                    foreach (var itemd in YTScheduledActionsList.Where(s => s.ActionName == cpUpdateAction.ActionName && s.Id == itemx.Id))
                                    {
                                        itemd.IsActioned = (itemd.IsActioned != itemx.IsActioned) ? itemx.IsActioned : itemd.IsActioned;
                                        itemd.ActionSchedule = (itemd.ActionSchedule != itemx.ActionSchedule) ? itemx.ActionSchedule : itemd.ActionSchedule;
                                        itemd.ActionScheduleStart = (itemd.ActionScheduleStart != itemx.ActionScheduleStart) ? itemx.ActionScheduleStart : itemd.ActionScheduleStart;
                                        itemd.ActionScheduleEnd = (itemd.ActionScheduleEnd != itemx.ActionScheduleEnd) ? itemx.ActionScheduleEnd : itemd.ActionScheduleEnd;
                                        itemd.AppliedAction = (itemd.AppliedAction != itemx.AppliedAction) ? itemx.AppliedAction : itemd.AppliedAction;
                                        itemd.Max = (itemd.Max != itemx.Max) ? itemx.Max : itemd.Max;
                                        itemd.ScheduleName = (itemd.ScheduleName != itemx.ScheduleName) ? itemx.ScheduleName : itemd.ScheduleName;
                                        itemd.ScheduleNameId = (itemd.ScheduleNameId != itemx.ScheduleNameId) ? itemx.ScheduleNameId : itemd.ScheduleNameId;
                                        itemd.CompletedScheduledDate = (itemd.CompletedScheduledDate != itemx.CompletedScheduledDate) ? itemx.CompletedScheduledDate : itemd.CompletedScheduledDate;
                                        itemd.VideoActionType = (itemd.VideoActionType != itemx.VideoActionType) ? itemx.VideoActionType : itemd.VideoActionType;
                                        //actionScheduleSelector.lstItems.ItemsSource = 

                                        ObservableCollectionFilter.ActionsScheduleCollectionViewSource.View.Refresh();
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_ScheduleActioner {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        private TResult formObjectHandler_SelectReleaseSchedule<TResult>(object tld, SelectReleaseSchedule selectReleaseSchedule)
        {
            try
            {
                if (tld is CustomParams_Initialize cpInit)
                {
                    selectReleaseSchedule.lstMainSchedules.ItemsSource = SchedulingNamesList;
                }
                else if (tld is CustomParams_Save cpSave)
                {
                    if (cpSave.id != -1)
                    {
                        string sql = "update SCHEDULINGNAMES set " +
                            "NAME = @inputValue where ID = @inputValue2";
                        connectionString.ExecuteNonQuery(sql, [("@inputValue", cpSave.Name), ("@inputValue2", cpSave.id)]);
                        SchedulingListUpdate(cpSave.id, false, cpSave.Name);
                    }
                    else
                    {
                        AddSchedulingName(cpSave.Name);
                    }
                }
                else if (tld is CustomParams_Delete cpd)
                {
                    string sql = "DELETE FROM SCHEDULES WHERE ID = @P0";
                    connectionString.ExecuteScalar(sql, [("@P0", cpd.Id)]);
                    foreach (var item in VideoSchedulesList.Where(s => s.Id == cpd.Id))
                    {
                        VideoSchedulesList.Remove(item);
                        break;
                    }
                }
                else if (tld is CustomParams_Select spSelect)
                {
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int idx = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                    if (idx == -1)
                    {
                        sqla = "INSERT INTO SETTINGS(SETTINGNAME,SETTING) VALUES(@P1,@P2) RETURNING ID;";
                        idx = connectionString.ExecuteScalar(sqla, [("@P1", "CURRENTSCHEDULINGID"), ("@P2", spSelect.Id)]).ToInt(-1);
                    }
                    else
                    {
                        sqla = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @ID";
                        connectionString.ExecuteScalar(sqla, [("@ID", idx), ("@P1", spSelect.Id)]).ToInt(-1);
                    }

                }
                else if (tld is CustomParams_Finish cpFinish)
                {
                    bool found = false;
                    int id = -1;
                    foreach (var item in SchedulingNamesList.Where(s => s.Name == cpFinish.name))
                    {
                        found = true;
                        id = item.Id;
                        break;
                    }
                    if (found)
                    {
                        string sqla1 = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                        int idx1 = connectionString.ExecuteScalar(sqla1, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                        if (idx1 == -1)
                        {
                            sqla1 = "INSERT INTO SETTINGS(SETTINGNAME,SETTING) VALUES(@P1,@P2) RETURNING ID;";
                            idx1 = connectionString.ExecuteScalar(sqla1, [("@P1", "CURRENTSCHEDULINGID"), ("@P2", id)]).ToInt(-1);
                        }
                        else
                        {
                            sqla1 = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @ID";
                            connectionString.ExecuteScalar(sqla1, [("@ID", id), ("@P1", id)]).ToInt(-1);
                        }
                    }
                    else
                    {
                        string sqlxx = "INSERT INTO SCHEDULINGNAMES(NAME) VALUES(@NAMES) REURTING ID;";
                        int idxp = connectionString.ExecuteScalar(sqlxx, [("@NAMES", cpFinish.name)]).ToInt(-1);
                        if (idxp != -1)
                        {
                            SchedulingNamesList.Add(new ScheduleMapNames(cpFinish.name, idxp));
                            string sqla2 = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                            int idx2 = connectionString.ExecuteScalar(sqla2, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                            if (idx2 == -1)
                            {
                                sqla2 = "INSERT INTO SETTINGS(SETTINGNAME,SETTING) VALUES(@P1,@P2) RETURNING ID:";
                                idx2 = connectionString.ExecuteScalar(sqla2, [("@P1", "CURRENTSCHEDULINGID"), ("@P2", id)]).ToInt(-1);
                            }
                            else
                            {
                                sqla2 = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @ID";
                                connectionString.ExecuteScalar(sqla2, [("@ID", id), ("@P1", id)]).ToInt(-1);
                            }
                        }
                    }
                }
                return (TResult)Convert.ChangeType(true, typeof(TResult));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_SelectReleaseSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        private void AddSchedulingName(string name)
        {
            try
            {
                string sql = "insert into SCHEDULINGNAMES(NAME) values(@inputValue) returning ID;";
                int idx = connectionString.ExecuteScalar(sql, [("@inputValue", name)]).ToInt(-1);
                if (idx != -1)
                {
                    SchedulingNamesList.Add(new ScheduleMapNames(name, idx));
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int id = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                    if (id == -1)
                    {
                        sqla = "INSERT INTO SETTINGS(SETTINGNAME,SETTING) VALUES(@P1,@P2) RETURNING ID:";
                        id = connectionString.ExecuteScalar(sqla, [("@P1", "CURRENTSCHEDULINGID"), ("@P2", idx)]).ToInt(-1);
                    }
                    else
                    {
                        sqla = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @P2";
                        sqla.ExecuteScalar(sqla, [("@P1", idx), ("@P2", id)]);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddSchedulingName {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void SchedulingListUpdate(int id, bool remove = true, string name = "")
        {
            try
            {
                for (int i = SchedulingNamesList.Count - 1; i >= 0; i--)
                {
                    if (SchedulingNamesList[i].Id == id)
                    {
                        if (remove)
                        {
                            SchedulingNamesList.RemoveAt(i);
                            string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                            int idx = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                            if (idx == -1)
                            {
                                sqla = "INSERT INTO SETTINGS(SETTINGNAME,SETTING) VALUES(@P1,@P2) RETURNING ID:";
                                idx = connectionString.ExecuteScalar(sqla, [("@P1", "CURRENTSCHEDULINGID"), ("@P2", -1)]).ToInt(-1);
                            }
                            else
                            {
                                sqla = "UPDATE SETTINGS SET SETTING = @P1 WHERE ID = @P2";
                                sqla.ExecuteScalar(sqla, [("@P1", -1), ("@P2", idx)]);
                            }
                            //ColectionFilter.SetSchedulingTag(-1);
                        }
                        else
                        {
                            SchedulingNamesList[i].Name = name;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"RemoveFromSchedulingList {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public ObservableCollection<ScheduleMapItem> GetScheduleList(int idx, SchedulingSelectEditor schedulingSelectEditor)
        {
            try
            {
                schedulingSelectEditor.TitleId = idx;
                return new ObservableCollection<ScheduleMapItem>(SchedulingItemsList.Where(s => s.ScheduleId == idx).OrderBy(s => s.Start).ToList());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetScheduleList {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }
        private TResult formObjectHandler_SchedulingSelectEditor<TResult>(object tld, SchedulingSelectEditor schedulingSelectEditor)
        {
            try
            {
                if (tld is CustomParams_Initialize cpi)
                {
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int idx = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                    if (idx != -1)
                    {
                        foreach (var cp in SchedulingNamesList.Where(cp => cp.Id == idx && idx != -1))
                        {
                            schedulingSelectEditor.txtTitle.Text = cp.Name;
                            break;
                        }

                        schedulingSelectEditor.lstTitles.ItemsSource = GetScheduleList(idx, schedulingSelectEditor);

                    }
                    else
                    {
                        schedulingSelectEditor.lstTitles.ItemsSource = null;
                        schedulingSelectEditor.TitleId = -1;
                    }
                }
                else if (tld is CustomParams_RemoveTimeSpans cp_RMTSE)
                {
                    RemoveTimeSpansFromSchedulingItemsList(cp_RMTSE.id, cp_RMTSE.RemoveAll);
                    schedulingSelectEditor.lstTitles.ItemsSource = SchedulingItemsList.Where(s => s.ScheduleId == schedulingSelectEditor.TitleId).ToList();
                }
                else if (tld is CustomParams_EditTimeSpans cp_ETSE)
                {
                    if (cp_ETSE.id != -1)
                    {
                        EditTimeSpansFromSchedulingItemsList(cp_ETSE.id, cp_ETSE.Start, cp_ETSE.End, cp_ETSE.Gap);
                        schedulingSelectEditor.lstTitles.ItemsSource = GetScheduleList(schedulingSelectEditor.TitleId, schedulingSelectEditor);
                    }
                    else
                    {
                        var Id = schedulingSelectEditor.TitleId;
                        AddTimeSpansToSchedulingItemsList(cp_ETSE.Start, cp_ETSE.End, cp_ETSE.Gap, Id);
                        schedulingSelectEditor.lstTitles.ItemsSource = GetScheduleList(schedulingSelectEditor.TitleId, schedulingSelectEditor);
                    }
                }
                else if (tld is CustomParams_Refresh cR)
                {
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int idx = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                    if (idx != -1)
                    {
                        foreach (var cp in SchedulingNamesList.Where(cp => cp.Id == idx))
                        {
                            schedulingSelectEditor.txtTitle.Text = cp.Name;
                            schedulingSelectEditor.TitleId = idx;
                            break;
                        }
                        schedulingSelectEditor.lstTitles.ItemsSource = null;
                        schedulingSelectEditor.lstTitles.ItemsSource = GetScheduleList(idx, schedulingSelectEditor);
                    }
                    else
                    {
                        schedulingSelectEditor.lstTitles.ItemsSource = null;
                        schedulingSelectEditor.TitleId = -1;
                    }
                }
                else if (tld is CustomParams_AddTimeSpanEntries cp_ATSE)
                {
                    var Id = schedulingSelectEditor.TitleId;
                    AddTimeSpansToSchedulingItemsList(cp_ATSE.Start, cp_ATSE.End, cp_ATSE.Gap, Id);
                    schedulingSelectEditor.lstTitles.ItemsSource = GetScheduleList(schedulingSelectEditor.TitleId, schedulingSelectEditor);
                }
                else if (tld is CustomParams_Update cpu)
                {
                    string sql = "UPDATE SCHEDULES SET NAME = @P1 WHERE ID = @P0";
                    connectionString.ExecuteScalar(sql, [("@P0", cpu.id), ("@P1", cpu.DirectoryName)]);
                    foreach (var item in VideoSchedulesList.Where(s => s.Id == cpu.id))
                    {
                        item.Name = cpu.DirectoryName;
                        break;
                    }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_SchedulingSelectEditor {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        public void AddTimeSpansToSchedulingItemsList(TimeSpan Start, TimeSpan End, int Gap, int SchId)
        {
            try
            {
                bool found = false;
                foreach (var _ in SchedulingItemsList.Where(times => (Start > times.Start && Start < times.End) || (End > times.Start && End < times.End)).Select(times => new { }))
                {
                    found = true;
                    break;
                }

                string sql = "";
                //int idx1 = connectionString.ExecuteScalar(sql, [("@inputValue", Start), ("@SCHEDULEID", SchId)]).ToInt(-1);
                if (!found)
                {
                    int idx = -1;
                    string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                    int SchedulingId = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);

                    string sqlx = "INSERT INTO SCHEDULINGITEMS(SSTART,SEND,Gap,SCHEDULINGID) VALUES(@START,@END,@GAP,@SCHEDULINGID) RETURNING ID;";
                    idx = connectionString.ExecuteScalar(sqlx, [("@START", Start), ("@END", End), ("@GAP", Gap), ("@SCHEDULINGID", SchedulingId)]).ToInt(-1);
                    if (idx != -1)
                    {
                        connectionString.ExecuteReader($"SELECT * FROM SCHEDULINGITEMS WHERE ID = {idx}", (FbDataReader r) =>
                        {
                            SchedulingItemsList.Add(new ScheduleMapItem(r));
                        }); ;
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Time Span Already Exists");
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddTimeSpansToSchedulingItemsList {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void EditTimeSpansFromSchedulingItemsList(int id, TimeSpan start, TimeSpan end, int gap)
        {
            try
            {
                string sql = "UPDATE SCHEDULINGITEMS SET SSTART = @inputValue1, " +
                    "SEND = @inputValue2, GAP = @inputValue3 WHERE ID = @inputValue4";
                connectionString.ExecuteNonQuery(sql, [("@inputValue1", start), ("@inputValue2", end), ("@inputValue3", gap), ("@inputValue4", id)]);
                foreach (var item in SchedulingItemsList.Where(item => item.Id == id))
                {
                    item.Start = start;
                    item.End = end;
                    item.Gap = gap;
                    break;
                }
                //ColectionFilter.SchedulingItemsView.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"EditTimeSpansFromSchedulingItemsList {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void RemoveTimeSpansFromSchedulingItemsList(int id, bool All = false)
        {
            try
            {
                string sql = $"DELETE FROM SCHEDULINGITEMS WHERE {(All ? "SCHEDULINGID" : "ID")} = @inputValue";
                connectionString.ExecuteNonQuery(sql, [("@inputValue", id)]);
                foreach (var item in SchedulingItemsList.Where(item => item.Id == id))
                {
                    SchedulingItemsList.Remove(item);
                    break;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"RemoveTimeSpansFromSchedulingItemsList {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public string GetShortsDirectorySql(int index = -1)
        {
            try
            {
                return "SELECT S.ID, S.DIRECTORYNAME, S.TITLEID, S.DESCID, " +
                       "(SELECT LIST(TAGID, ',') FROM TITLETAGS " +
                       " WHERE GROUPID = S.TITLEID) AS LINKEDTITLEIDS, " +
                       " (SELECT LIST(ID,',') FROM DESCRIPTIONS " +
                       "WHERE TITLETAGID = S.DESCID) AS LINKEDDESCIDS " +
                       "FROM SHORTSDIRECTORY S" +
                (index != -1 ? $" WHERE S.ID = {index} " : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} GetShortsDirectorySql {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }
        private TResult formObjectHandler_DirectoryTitleDescEditor<TResult>(object tld, DirectoryTitleDescEditor directoryTitleDescEditor)
        {
            try
            {
                if (tld is CustomParams_Select cps)
                {
                    ShortsDirectoryIndex = cps.Id;
                }
                else if (tld is CustomParams_Initialize cpi)
                {
                    directoryTitleDescEditor.msuDirectoryList.ItemsSource = EditableshortsDirectoryList;
                }
                else if (tld is CustomParams_Update cpu)
                {
                    ShortsDirectoryIndex = cpu.id;
                    if (cpu.updatetype == UpdateType.Title)
                    {

                        string sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TID WHERE ID = @ID;";
                        connectionString.ExecuteScalar(sql, [("@ID", ShortsDirectoryIndex), ("@TID", cpu.id)]);
                        TitleId = cpu.id;
                        sql = GetShortsDirectorySql(ShortsDirectoryIndex);
                        string LinkedTitleIds = "";
                        CancellationTokenSource cts = new CancellationTokenSource();
                        connectionString.ExecuteReader(sql, cts, (FbDataReader r) =>
                        {
                            LinkedTitleIds = (r["LINKEDTITLEIDS"] is string lkd) ? lkd : "";
                            cts.Cancel();
                        });
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                        {
                            item.TitleId = cpu.id;
                            item.LinkedTitleIds = LinkedTitleIds;
                            break;
                        }

                    }

                    else if (cpu.updatetype == UpdateType.Description)
                    {
                        string sql = "UPDATE SHORTSDIRECTORY SET DESCID = @DID WHERE ID = @ID;";
                        connectionString.ExecuteScalar(sql, [("@ID", ShortsDirectoryIndex), ("@DID", cpu.id)]);
                        DescId = cpu.id;
                        string LinkedDescIds = "";
                        sql = GetShortsDirectorySql(ShortsDirectoryIndex);
                        CancellationTokenSource cts = new CancellationTokenSource();
                        connectionString.ExecuteReader(sql, cts, (FbDataReader r) =>
                        {
                            LinkedDescIds = (r["LINKEDDESCIDS"] is string lkd) ? lkd : "";
                            cts.Cancel();
                        });
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                        {
                            item.DescId = cpu.id;
                            item.LinkedDescIds = LinkedDescIds;
                            break;
                        }
                    }
                }
                else if (tld is CustomParams_DescSelect cds)
                {
                    DescId = cds.UploadsReleaseInfo.Id;
                    directoryTitleDescEditor.DoDescSelectCreate(DescId, cds.UploadsReleaseInfo.Id);
                    string sql = "SELECT DESCID FROM SHORTSDIRECTORY WHERE ID = @ID;";
                    int id = connectionString.ExecuteScalar(sql, [("@ID", cds.UploadsReleaseInfo.Id)]).ToInt(-1);
                    if (id != -1) cds.UploadsReleaseInfo.DescId = DescId;
                }
                else if (tld is CustomParams_TitleSelect cts)
                {
                    TitleId = cts.UploadsReleaseInfo.TitleId;
                    directoryTitleDescEditor.DoTitleSelectCreate(TitleId,
                        cts.UploadsReleaseInfo.Id);
                    string sql = "SELECT TITLEID FROM SHORTSDIRECTORY WHERE ID = @ID;";
                    int id = connectionString.ExecuteScalar(sql, [("@ID", cts.UploadsReleaseInfo.Id)]).ToInt(-1);
                    if (id != -1) cts.UploadsReleaseInfo.TitleId = TitleId;
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_DirectoryTitleDescEditor {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        public Nullable<int> EventId = null;

        private TResult formObjectHandler_scheduleEventCreator<TResult>(object tld, ScheduleEventCreator scheduleEventCreatorFrm)
        {
            try
            {
                if (tld is CustomParams_Initialize cpi)
                {
                    string sql = "SELECT ES.EVENTID,SP.SOURCE,SP.MAXDAILY,SP.MAXEVENT,SP.NAME,ESD.START,ESD.END,ESD.STARTTIME,ESD.ENDTIME," +
                          "SD.START,SD.END,SD.STARTTIME,SD.ENDTIME FROM " +
                          "EVENTSCHEDULES ES  " +
                          "INNER JOIN EVENTSCHEDULEDATE ESD ON ESD.EVENTID = ES.EVENTID " +
                          "INNER JOIN SCHEDULEDATE SD ON SD.EVENTID = ES.EVENTID " +
                          "INNER JOIN SCHEDULES SP ON SP.ID = ES.SCHEDULEID " +
                          "INNER JOIN SCHEDULEDPOOL SP ON SP.ID = ES.SCHEDULEID " +
                         $"WHERE SP.ISSCHEDULE = 1;";
                    connectionString.ExecuteReader(sql, (FbDataReader r) =>
                    {
                        var Edit = new EditableScheduleEvent(r);
                        if (Edit.Name != "")
                        {
                            EditableScheduleEventsList.Add(Edit);
                        }
                    });
                    scheduleEventCreatorFrm.lstSchedules.ItemsSource = EditableScheduleEventsList;
                    sql = "SELECT SETTING FROM SETTINGS WHERE NAME = @P0";
                    int res = connectionString.ExecuteScalar(sql, [("@P0", "EVENTID")]).ToInt(-1);
                    if (res != -1)
                    {
                        bool found = true;
                        foreach (var item in EditableScheduleEventsList.Where(idx => idx.EventId == res).Select(idx => idx))
                        {
                            found = true;
                            scheduleEventCreatorFrm.lstSchedules.SelectedItem = item;
                            scheduleEventCreatorFrm.txtEventName.Text = item.Name;
                            scheduleEventCreatorFrm.EventStart.Value = item.EventStart;
                            scheduleEventCreatorFrm.EventEnd.Value = item.EventEnd;
                            scheduleEventCreatorFrm.ScheduleStart.Value = item.ScheduleStart;
                            scheduleEventCreatorFrm.ScheduleEnd.Value = item.ScheduleEnd;
                            scheduleEventCreatorFrm.txtMax.Text = item.MaxDaily.ToString();
                            scheduleEventCreatorFrm.txtMaxEvent.Text = item.MaxEvent.ToString();
                            //scheduleEventCreatorFrm.cbxVideoType.SelectedIndex = item.Source;
                            // scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                            break;
                        }
                        if (!found)
                        {
                            //scheduleEventCreatorFrm.btnEventCheck.IsChecked = false;
                            scheduleEventCreatorFrm.lstSchedules.SelectedItem = null;
                            scheduleEventCreatorFrm.txtEventName.Text = "";
                            bool Executed = false;
                            Nullable<DateOnly> StartDate = null;
                            Nullable<DateOnly> EndDate = null;
                            Nullable<TimeOnly> StartTime = null;
                            Nullable<TimeOnly> EndTime = null;
                            string Name = "";
                            sql = "SELECT ESD.START,ESD.END,ESD.STARTTIME,ESD.ENDTIME,SP.NAME FROM EVENTSCHEDULES ES " +
                                "INNER JOIN EVENTSCHEDULEDATE ESD ON ESD.EVENTID = ES.EVENTID " +
                                "INNER JOIN SCHEDULES SP ON SP.ID = ES.SCHEDULEID WHERE ES.EVENTID = @EVENTID AND WHERE SP.ISSCHEDULE = 1;";
                            connectionString.ExecuteReader(sql, (FbDataReader r) =>
                            {
                                if (!Executed)
                                {
                                    StartDate = (r[0] is DateOnly sdd) ? sdd : null;
                                    EndDate = (r[1] is DateOnly sed) ? sed : null;
                                    StartTime = (r[2] is TimeOnly sst) ? sst : null;
                                    EndTime = (r[3] is TimeOnly set) ? set : null;
                                    Name = (r[4] is string nm ? nm : "");
                                    Executed = true;
                                }
                            });
                            if (StartDate is not null && StartTime is not null)
                            {
                                //scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                                scheduleEventCreatorFrm.EventStart.Value = StartDate.Value.ToDateTime(StartTime.Value);
                            }
                            else
                            {
                                //scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                                scheduleEventCreatorFrm.EventStart.Value = null;
                            }
                            if (EndDate is not null && EndTime is not null)
                            {
                                //scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                                scheduleEventCreatorFrm.EventEnd.Value = EndDate.Value.ToDateTime(EndTime.Value);
                            }
                            else
                            {
                                //scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                                scheduleEventCreatorFrm.EventEnd.Value = null;
                            }
                            if (Name != "")
                            {
                                //scheduleEventCreatorFrm.btnEventCheck.IsChecked = true;
                                scheduleEventCreatorFrm.txtEventName.Text = Name;
                            }
                            if (scheduleEventCreatorFrm.btnEventCheck.IsChecked.Value == true)
                            {
                                sql = "SELECT SD.START,SD.END,SD.STARTTIME,SD.ENDTIME,SP.SOURCE,SP.MAXDAILY,SP.MAXEVENT,SP.NAME FROM " +
                                      "EVENTSCHEDULES ES " +
                                      "INNER JOIN SCHEDULEDATE SD ON SD.EVENTID = ES.EVENTID " +
                                      "INNER JOIN SCHEDULES SP ON SP.ID = ES.SCHEDULEID " +
                                     $"WHERE SP.ISSCHEDULE = 1;";
                                Executed = false;
                                int src = -1, maxd = -1, maxe = -1;

                                connectionString.ExecuteReader(sql, (FbDataReader r) =>
                                {
                                    if (!Executed)
                                    {
                                        StartDate = (r[0] is DateOnly sdd) ? sdd : null;
                                        EndDate = (r[1] is DateOnly sed) ? sed : null;
                                        StartTime = (r[2] is TimeOnly sst) ? sst : null;
                                        EndTime = (r[3] is TimeOnly set) ? set : null;
                                        src = (r[4] is int ssrc ? ssrc : -1);
                                        maxd = (r[5] is int smaxd ? smaxd : -1);
                                        maxe = (r[6] is int smaxe ? smaxe : -1);
                                        Executed = true;
                                    }
                                });
                                scheduleEventCreatorFrm.cbxVideoType.SelectedIndex = src;
                                scheduleEventCreatorFrm.txtMax.Text = (maxd != -1) ? maxd.ToString() : "";
                                scheduleEventCreatorFrm.txtMaxEvent.Text = (maxe != -1) ? maxe.ToString() : "";
                                if (StartDate is not null && StartTime is not null)
                                {
                                    scheduleEventCreatorFrm.ScheduleStart.Value = StartDate.Value.ToDateTime(StartTime.Value);
                                }
                                else
                                {
                                    scheduleEventCreatorFrm.ScheduleStart.Value = null;
                                }
                                if (EndDate is not null && EndTime is not null)
                                {
                                    scheduleEventCreatorFrm.ScheduleEnd.Value = EndDate.Value.ToDateTime(EndTime.Value);
                                }
                                else
                                {
                                    scheduleEventCreatorFrm.ScheduleEnd.Value = null;
                                }
                            }
                            else
                            {
                                scheduleEventCreatorFrm.ScheduleStart.Value = null;
                                scheduleEventCreatorFrm.ScheduleEnd.Value = null;
                                scheduleEventCreatorFrm.txtMax.Text = "";
                                scheduleEventCreatorFrm.txtMaxEvent.Text = "";
                                scheduleEventCreatorFrm.cbxVideoType.SelectedIndex = -1;
                                scheduleEventCreatorFrm.cbxVideoType.Text = "";
                            }
                        }
                    }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_scheduleEventCreator {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);

            }
        }

        private TResult formObjectHandler_DescSelect<TResult>(object tld, DescSelectFrm frmDescSelectFrm, bool IsShort = false)
        {
            try
            {
                switch (tld)
                {
                    case CustomParams_DescUpdate:
                        {
                            string DirName = (tld as CustomParams_DescUpdate).DirectoryName;
                            string Desc = (tld as CustomParams_DescUpdate).Description;
                            var r = DescUpdater(frmDescSelectFrm, DirName, Desc, false);
                            return (TResult)Convert.ChangeType(r, typeof(TResult));
                            break;
                        }
                    case CustomParams_Update:
                        {
                            if (tld is CustomParams_Update p && p is not null)
                            {
                                if (p.id != -1) // always exists.
                                {
                                    string field = (p.updatetype == UpdateType.Title) ? "TITLEID" : "DESCID";
                                    var sql = $"UPDATE SHORTSDIRECTORY SET {field} = @P1 WHERE ID = @P0";
                                    connectionString.ExecuteScalar(sql, [("@P0", p.id),
                                        ("@P1", (p.updatetype == UpdateType.Title) ? TitleId : DescId)]);
                                }


                                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                string rootfolder = FindUploadPath();
                                key?.Close();
                                string ThisDir = rootfolder.Split(@"\").ToList().LastOrDefault();
                                if (ThisDir != "")
                                {
                                    string sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                                    int res = connectionString.ExecuteScalar(sql, [("@P0", ThisDir)]).ToInt(-1);
                                    if (res != -1)
                                    {
                                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                                        connectionString.ExecuteScalar(sql, [("@P0", res), ("@P1", p.id)]);
                                    }
                                    sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                                    res = connectionString.ExecuteScalar(sql, [("@P0", ThisDir)]).ToInt(-1);
                                    if (res != -1)
                                    {
                                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                                        res = connectionString.ExecuteScalar(sql, [("@P0", res), ("@P1", p.id)]).ToInt(-1);
                                    }
                                    string linkeddescids = "";
                                    string sqla = GetUploadReleaseBuilderSql(res);
                                    CancellationTokenSource cts = new CancellationTokenSource();
                                    connectionString.ExecuteReader(sqla, cts, (FbDataReader r) =>
                                    {
                                        linkeddescids = (r["LINKEDDESCIDS"] is string ldid ? ldid : "");
                                        cts.Cancel();
                                    });

                                    selectShortUpload.UpdateDescId(p.id, linkeddescids);
                                }
                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Initialize cpInit:
                        {
                            frmDescSelectFrm.lstAllDescriptions.ItemsSource = DescriptionsList;
                            frmDescSelectFrm.chkIsShortVideo.IsChecked = IsShort;
                            frmDescSelectFrm.IsShortVideo = IsShort;
                            bool Found = false;
                            if (ShortsDirectoryIndex != frmDescSelectFrm.Id)
                            {
                                ShortsDirectoryIndex = frmDescSelectFrm.Id;
                            }
                            foreach (var item in DescriptionsList.Where(s => s.IsShortVideo == IsShort && s.Id == ShortsDirectoryIndex))
                            {
                                frmDescSelectFrm.txtDesc.Text = item.Description;
                                frmDescSelectFrm.Desc = item.Description;
                                frmDescSelectFrm.txtDescName.Text = item.Name;
                                frmDescSelectFrm.chkIsShortVideo.IsChecked = item.IsShortVideo;
                                frmDescSelectFrm.IsShortVideo = item.IsShortVideo;
                                frmDescSelectFrm.TitleTagId = item.TitleTagId;
                                Found = true;
                                break;
                            }
                            if (!Found)
                            {
                                foreach (var itemr in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                                {
                                    frmDescSelectFrm.txtDesc.Text = itemr.Directory +
                                           Environment.NewLine + Environment.NewLine
                                           + "Follow me @ twitch.tv/justinstrainclips"
                                           + Environment.NewLine + Environment.NewLine +
                                          "Support Me On Patreon - https://www.patreon.com/join/JustinsTrainJourneys";

                                    frmDescSelectFrm.txtDescName.Text = itemr.Directory;
                                    frmDescSelectFrm.TitleTagId = ShortsDirectoryIndex;
                                    Found = true;
                                    int idx = -1;
                                    frmDescSelectFrm.IsDescChanged = false;
                                    string sql = $"INSERT INTO DESCRIPTIONS(DESCRIPTION,TITLETAGID,NAME,ISSHORTVIDEO, ISTAG)" +
                                     $" VALUES(@DESC,@TITLETAG,@NAME,@IsShortVideo,@IsTag) RETURNING ID;";
                                    idx = connectionString.ExecuteScalar(sql, [("@DESC", itemr.Directory),
                                        ("@TITLETAG", ShortsDirectoryIndex), ("@NAME", itemr.Directory),
                                        ("@IsShortVideo", frmDescSelectFrm.IsShortVideo), ("@IsTag", false)]).ToInt(-1);
                                    if (idx != -1)
                                    {
                                        sql = $"SELECT * FROM DESCRIPTIONS WHERE ID = {idx}";
                                        CancellationTokenSource cts = new CancellationTokenSource();
                                        connectionString.ExecuteReader(sql, cts, (dr) =>
                                        {
                                            DescriptionsList.Add(new Descriptions(dr));
                                            cts.Cancel();
                                        });
                                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                                        connectionString.ExecuteScalar(sql, [("@P0", ShortsDirectoryIndex), ("@P1", idx)]);
                                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                                        {
                                            item.DescId = idx;
                                        }
                                        frmDescSelectFrm.Id = idx;
                                    }
                                    break;
                                }
                            }
                            return default(TResult);
                            break;
                        }
                    case null:
                        {
                            frmDescSelectFrm.lstAllDescriptions.ItemsSource = DescriptionsList;
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Remove cpRemove:
                        {
                            string sql = $"SELECT * FROM DESCRIPTIONS WHERE ID = {cpRemove.id} RETURNING ID;";
                            int idx = connectionString.ExecuteScalar(sql).ToInt(-1);
                            if (idx != -1)
                            {
                                for (int i = 0; i < DescriptionsList.Count; i--)
                                {
                                    if (DescriptionsList[i].Id == cpRemove.id)
                                    {
                                        DescriptionsList.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_AddDescription cpAdd:
                        {
                            int idx = -1;
                            string sql = $"SELECT ID FROM DESCRIPTIONS WHERE TITLETAGID = @TID";
                            idx = connectionString.ExecuteScalar(sql, [("@TID", cpAdd.Id)]).ToInt(-1);
                            if (idx == -1)
                            {
                                sql = $"INSERT INTO DESCRIPTIONS(DESCRIPTION,TITLETAGID,NAME,ISSHORTVIDEO, ISTAG)" +
                                    $" VALUES(@DESC,@TITLETAG,@NAME,@IsShortVideo,@IsTag) RETURNING ID;";
                                idx = connectionString.ExecuteScalar(sql, [("@DESC", cpAdd.Description),
                                    ("@TITLETAG", cpAdd.Id), ("@NAME", cpAdd.Name),
                                    ("@IsShortVideo", frmDescSelectFrm.IsShortVideo),
                                    ("@IsTag", false)]).ToInt(-1);
                                if (idx != -1)
                                {
                                    frmDescSelectFrm.IsDescChanged = true;
                                    frmDescSelectFrm.LinkedId = idx;
                                    DescriptionsList.Add(new Descriptions(idx, cpAdd.Description,
                                        frmDescSelectFrm.IsShortVideo, "", cpAdd.Name));
                                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                                    {
                                        item.DescId = idx;
                                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                                        connectionString.ExecuteScalar(sql, [("@P0", ShortsDirectoryIndex), ("@P1", idx)]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                sql = $"UPDATE DESCRIPTIONS SET DESCRIPTION=@DESC, NAME=@NAME WHERE " +
                                    "TITLETAGID = @TITLETAGID AND ISTAG = @ISTAG RETURNING ID;";
                                idx = connectionString.ExecuteScalar(sql, [("@DESC", cpAdd.Description),
                                    ("@NAME", cpAdd.Name), ("@TITLETAGID", cpAdd.Id), ("@ISTAG", false)]).ToInt(-1);
                                foreach (var item in DescriptionsList.Where(s => s.TitleTagId == cpAdd.Id))
                                {
                                    item.Description = cpAdd.Description;
                                    item.Name = cpAdd.Name;
                                    break;
                                }
                            }
                            return default(TResult);
                            break;
                        }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_DescSelect - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return default(TResult);
            }
        }

        private IdhasUpdated DescUpdater(object frmDescSelectFrm, string DirName, string Desc, bool IsShortVideo = false)
        {
            try
            {
                string sql = "";
                DirName = (DirName.Contains("\\")) ? DirName.Split('\\').LastOrDefault() : DirName;
                bool found = false;
                int DirId = -1;
                foreach (var Item in DescriptionsList.Where(
                    t => t.Name.ToUpper() == DirName.ToUpper()))
                {
                    if (Item.Description != Desc)
                    {
                        Item.Description = Desc;
                        DirId = Item.Id;
                        found = true;
                        sql = "UPDATE DESCRIPTIONS SET DESCRIPTION = @P1 WHERE ID = @P0";
                        connectionString.ExecuteScalar(sql, [("@P0", DirId), ("@P1", Desc)]);
                    }
                    break;
                }

                if (found)
                {
                    bool Updated = false;
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Directory.ToUpper() == DirName.ToUpper()))
                    {
                        if (item.DescId != DirId)
                        {
                            Updated = true;
                            item.DescId = DirId;
                            ShortsDirectoryIndex = item.Id;
                            sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                            connectionString.ExecuteScalar(sql, [("@P0", item.Id),
                                            ("@P1", DirId)]);
                            if (frmDescSelectFrm is DescSelectFrm frm)
                            {
                                frm.Id = DirId;
                            }
                        }
                        break;
                    }
                    if (Updated)
                    {
                        sql = "UPDATE DESCRIPTIONS SET NAME = @P1, " +
                        "DESCRIPTION = @P2 WHERE ID = @P0";
                        connectionString.ExecuteScalar(sql, [("@P0", DirId),
                                    ("@P1", DirName), ("@P2", Desc)]);
                    }
                    return new IdhasUpdated(ShortsDirectoryIndex, Updated);
                }
                else
                {
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Directory.ToUpper() == DirName.ToUpper()))
                    {
                        ShortsDirectoryIndex = item.Id;
                        break;
                    }

                    foreach (var Item in DescriptionsList.Where(
                    t => t.Name is not null && t.Name.ToUpper() == DirName.ToUpper()))
                    {
                        found = true;
                    }

                    if (!found)
                    {
                        sql = $"INSERT INTO DESCRIPTIONS(DESCRIPTION,TITLETAGID,NAME,ISSHORTVIDEO, ISTAG)" +
                             $" VALUES(@DESC,@TITLETAG,@NAME,@IsShortVideo,@IsTag) RETURNING ID;";
                        int idx = connectionString.ExecuteScalar(sql, [("@DESC", Desc),
                                        ("@TITLETAG", ShortsDirectoryIndex), ("@NAME", DirName),
                                        ("@IsShortVideo", IsShortVideo), ("@IsTag", false)]).ToInt(-1);
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Directory.ToUpper() == DirName.ToUpper()))
                        {
                            item.DescId = idx;
                            break;
                        }

                        if (frmDescSelectFrm is DescSelectFrm frm)
                        {
                            frm.Id = idx;
                        }

                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P1 WHERE ID = @P0";
                        connectionString.ExecuteScalar(sql, [("@P0", ShortsDirectoryIndex),
                                    ("@P1", idx)]);
                        return new IdhasUpdated(ShortsDirectoryIndex, true);
                    }
                    return new IdhasUpdated(ShortsDirectoryIndex, false);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DescUpdater {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return new IdhasUpdated(-1, false);
            }
        }

        private TResult formObjectHandler_MasterTagSelect<TResult>(object tld, MasterTagSelectForm frmMasterTagSelectForm)
        {
            try
            {
                string sql = "";
                switch (tld)
                {
                    case CustomParams_Get cpGet:
                        {
                            int id = cpGet.Id;
                            if (frmMasterTagSelectForm.IsTitleTag)
                            {
                                foreach (var item in TitleTagsList.Where(idx => idx.GroupId == frmMasterTagSelectForm.ParentId))
                                {
                                    sql = "DELETE FROM TITLETAGS WHERE ID = @ID";
                                    connectionString.ExecuteScalar(sql, [("@ID", item.Id)]);
                                    for (int i = TitleTagsList.Count - 1; i >= 0; i--)
                                    {
                                        if (TitleTagsList[i].Id == item.Id)
                                        {
                                            TitleTagsList.RemoveAt(i);
                                        }
                                    }
                                }
                                ObservableCollection<TitleTags> TitleTagsList1 = new();
                                foreach (var tags in TitleTagsList.Where(avi => avi.GroupId == cpGet.Id))
                                {
                                    int idx = -1;
                                    sql = "INSERT INTO TITLETAGS(TAGID, GROUPID) VALUES(@TAGID, @GROUPID) RETURNING ID;";
                                    idx = connectionString.ExecuteScalar(sql, [("@TAGID", tags.TagId),
                                        ("@GROUPID", frmMasterTagSelectForm.ParentId)]).ToInt(-1);
                                }
                                sql = $"SELECT * FROM TITLETAGS T INNER JOIN AVAILABLETAGS S ON T.TAGID = S.ID WHERE GROUPID = {frmMasterTagSelectForm.ParentId}";
                                connectionString.ExecuteReader(sql, (FbDataReader r) =>
                                {
                                    TitleTagsList.Add(new TitleTags(r));
                                });
                                frmMasterTagSelectForm.TagSetChanged = true;
                                break;

                            }
                            else
                            {
                                sql = "DELETE FROM SELECTEDTAGS WHERE GROUPTAGID = @TAGID";
                                connectionString.ExecuteScalar(sql, [("@TAGID", frmMasterTagSelectForm.ParentId)]);
                                for (int i = selectedTagsList.Count - 1; i >= 0; i--)
                                {
                                    if (selectedTagsList[i].GroupTagId == frmMasterTagSelectForm.ParentId)
                                    {
                                        selectedTagsList.RemoveAt(i);
                                    }
                                }
                                ObservableCollection<SelectedTags> selectedTagsList1 = new();
                                foreach (var tags in selectedTagsList.Where(avi => avi.GroupTagId == id))
                                {
                                    int idxx = -1;
                                    sql = "INSERT INTO SELECTEDTAGS(SELECTEDTAG, GROUPTAGID) VALUES(@SELECTEDTAG, @GRP) RETURNING ID;";
                                    idxx = connectionString.ExecuteScalar(sql, [("@SELECTEDTAG", tags.SelectedTagId),
                                        ("@GRP", frmMasterTagSelectForm.ParentId)]).ToInt(-1);
                                    if (idxx != -1)
                                    {
                                        selectedTagsList1.Add(new SelectedTags(idxx, tags.SelectedTagId, frmMasterTagSelectForm.ParentId, tags.Description));
                                    }
                                }
                                foreach (var x in selectedTagsList1)
                                {
                                    selectedTagsList.Add(x);
                                }
                                frmMasterTagSelectForm.TagSetChanged = true;

                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Select cpSelect:
                        {
                            int id = cpSelect.Id;
                            string TagList = "";
                            if (frmMasterTagSelectForm.IsTitleTag)
                            {
                                foreach (var item in TitleTagsList.Where(idx => idx.GroupId == id))
                                {
                                    if (TagList == "")
                                    {
                                        TagList = $"#{item.Description}";
                                    }
                                    else
                                    {
                                        TagList += $" #{item.Description}";
                                    }
                                }
                            }
                            else
                            {
                                foreach (var itemx in selectedTagsList.Where(idxx => idxx.GroupTagId == id))
                                {
                                    if (TagList == "")
                                    {
                                        TagList = $"#{itemx.Description}";
                                    }
                                    else
                                    {
                                        TagList += $" #{itemx.Description}";
                                    }
                                }
                            }
                            TagList = TagList.Trim();

                            frmMasterTagSelectForm.txtTags.Text = TagList;

                            return default(TResult);
                            break;
                        }
                    case CustomParams_Initialize:
                        {
                            TitlesList2.Clear();
                            sql = "SELECT DISTINCT TF.ID,TF.DESCRIPTION FROM TITLES TF LEFT JOIN TITLETAGS TT ON " +
                                "TF.ID = TT.GROUPID LEFT JOIN SELECTEDTAGS ST ON TF.ID = ST.GROUPTAGID";
                            connectionString.ExecuteReader(sql, OnReadTitlesTags2);

                            frmMasterTagSelectForm.lstDescriptions.ItemsSource =
                                TitlesList2.Where(idx => idx.Id != frmMasterTagSelectForm.ParentId);//.Where(ind => ind.IsTag = frmMasterTagSelectForm.IsTitleTag);
                            return default(TResult);
                            break;
                        }
                    case null:
                        {
                            frmMasterTagSelectForm.lstDescriptions.ItemsSource = TitlesList.Where(ind => ind.IsTag);
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Add cpAdd:
                        {
                            int idx = -1;
                            string Sql = $"INSERT INTO DESCRIPTIONS(DESCRIPTION,ISTAG) VALUES(@Name,@istag) RETURNG ID;";
                            idx = connectionString.ExecuteScalar(Sql, [("@Name", cpAdd.Name), ("@istag", true)]).ToInt(-1);
                            if (idx != -1)
                            {
                                DescriptionsList.Add(new Descriptions(idx, cpAdd.Name, false, "", cpAdd.Name, true));
                            }
                            return default(TResult);
                            break;
                        }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_MasterTagSelect {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }
        private void OnReadTitlesTags2(FbDataReader reader)
        {
            try
            {
                TitlesList2.Add(new Titles(reader));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnReadTitlesTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        int ShortsDirectoryIndex = -1;
        private TResult formObjectHandler_TitleSelect<TResult>(object tld, TitleSelectFrm frmTitleSelect)
        {
            try
            {
                switch (tld)
                {
                    case CustomParams_LookUpTitleId:
                        {
                            string Dir = (tld as CustomParams_LookUpTitleId).DirectoryName.ToUpper();
                            foreach (var item in EditableshortsDirectoryList.Where(s => s.Directory.ToUpper() == Dir))
                            {
                                return (TResult)Convert.ChangeType(item.Id, typeof(TResult));
                            }
                            return (TResult)Convert.ChangeType(-1, typeof(TResult));
                            break;
                        }
                    case CustomParams_SetFilterId:
                        {
                            TitleTagsSrc = (tld as CustomParams_SetFilterId).FilterId;
                            ShortsDirectoryIndex = TitleTagsSrc;
                            break;
                        }
                    case CustomParams_InsertTags:
                        {
                            bool Update = false;
                            CustomParams_InsertTags cpInsertTags = (CustomParams_InsertTags)tld;
                            foreach (var (item, found) in from item in cpInsertTags.TagIds
                                                          let found = false
                                                          select (item, found))
                            {
                                bool f = false;
                                foreach (var st in TitleTagsList.Where(idx => idx.GroupId == cpInsertTags.GroupId))
                                {
                                    if (st.TagId == item)
                                    {
                                        f = true;
                                        break;
                                    }
                                }

                                if (!f)
                                {
                                    string sqla = $"INSERT INTO TITLETAGS(GROUPID,TAGID) VALUES({cpInsertTags.GroupId},{item}) RETURNING ID;";
                                    int id = connectionString.ExecuteScalar(sqla).ToInt(-1);
                                    if (id != -1)
                                    {
                                        connectionString.ExecuteReader($"SELECT * FROM TITLETAGS T INNER JOIN AVAILABLETAGS S " +
                                            "ON T.TAGID = S.ID WHERE T.ID = {id}", (FbDataReader r) =>
                                            {
                                                TitleTagsList.Add(new TitleTags(r));
                                            });
                                    }


                                    Update = true;
                                }
                            }

                            if (Update)
                            {
                                string BaseStr1 = frmTitleSelect.BaseTitle + " ";

                                foreach (var item in TitleTagsList.Where(s => s.GroupId == cpInsertTags.GroupId))
                                {
                                    BaseStr1 += (!BaseStr1.Contains($"#{item.Description}")) ? $"#{item.Description} " : "";
                                }
                                BaseStr1 = BaseStr1.Trim();
                                frmTitleSelect.txtTitle.Text = BaseStr1.Trim();
                                frmTitleSelect.lblTitleLength.Content = BaseStr1.Trim().Length;

                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Update:
                        {
                            if (tld is CustomParams_Update p && p is not null)
                            {
                                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                string rootfolder = FindUploadPath();// key.GetValueStr("UploadPath", @"D:\shorts\");
                                key?.Close();
                                string ThisDir = rootfolder.Split(@"\").ToList().LastOrDefault();
                                if (ThisDir != "")
                                {
                                    int res = -1;
                                    string sql = "select * from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                                    res = connectionString.ExecuteScalar(sql, [("@P0", ThisDir)]).ToInt(-1);
                                    if (res != -1)
                                    {
                                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @P1 WHERE ID = @P0";
                                        res = connectionString.ExecuteScalar(sql, [("@P0", res), ("@P1", p.id)]).ToInt(-1);
                                    }
                                    string linkedtitleids = "";
                                    CancellationTokenSource cts = new CancellationTokenSource();
                                    connectionString.ExecuteReader(GetUploadReleaseBuilderSql(res), cts, (FbDataReader r) =>
                                    {
                                        linkedtitleids = (r["LINKEDTITLEIDS"] is string ldid ? ldid : "");
                                        cts.Cancel();
                                    });
                                    selectShortUpload.UpdateTitleId(p.id, linkedtitleids);
                                }
                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Initialize:
                        {
                            string _title = "", BaseTitle = "", xx = "", part = "";

                            if (ShortsDirectoryIndex == -1)
                            {
                                var itempx = EditableshortsDirectoryList.LastOrDefault();
                                if (itempx is not null)
                                {
                                    ShortsDirectoryIndex = itempx.Id;
                                    break;
                                }
                            }
                            int index = -1;// UploadReleasesBuilderIndex;
                            //frmTitleSelect.lstTitles.ItemsSource = EditableshortsDirectoryList;

                            //90 IDX TID = 150 DID = 58
                            foreach (var item in EditableshortsDirectoryList.Where(item => item.Id == ShortsDirectoryIndex))
                            {
                                BaseTitle = item.Directory;
                                var _tid = item.TitleId;
                                if (item.TitleId == -1)
                                {
                                    foreach (var t in TitlesList.Where(i => i.GroupId == ShortsDirectoryIndex && !i.IsTag))
                                    {
                                        BaseTitle = t.Description;
                                        index = t.Id;
                                        item.TitleId = t.Id;
                                        string SQLa = "UPDATE SHORTSDIRECTORY SET TITLEID = @TID WHERE ID = @ID;";
                                        connectionString.ExecuteScalar(SQLa, [("@ID", ShortsDirectoryIndex),
                                            ("@TID", t.Id)]);
                                        frmTitleSelect.SetTitleTag(t.Id);
                                        return default(TResult);
                                        break;
                                    }
                                }
                                else
                                {
                                    bool fnd = false;
                                    foreach (var t in TitlesList.Where(i => i.GroupId == ShortsDirectoryIndex && !i.IsTag))
                                    {
                                        BaseTitle = t.Description;
                                        index = t.Id;
                                        TitleId = t.Id;
                                        frmTitleSelect.SetTitleTag(t.Id);
                                        fnd = true;
                                        break;
                                    }
                                    if (!fnd)
                                    {
                                        foreach (var t in TitlesList.Where(i => i.Description == BaseTitle && !i.IsTag))
                                        {
                                            if (t.GroupId == -1)
                                            {
                                                string sql = "";
                                                t.GroupId = ShortsDirectoryIndex;
                                                if (t.Id != _tid)
                                                {
                                                    item.TitleId = t.Id;
                                                    index = t.Id;
                                                    sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TID WHERE ID = @ID;";
                                                    connectionString.ExecuteScalar(sql, [("@ID",
                                                        ShortsDirectoryIndex), ("@TID", t.Id)]);
                                                }
                                                else index = t.Id;
                                                sql = "UPDATE TITLES SET GROUPID = @GRPID WHERE ID =@ID";
                                                connectionString.ExecuteScalar(sql, [("@GRPID", ShortsDirectoryIndex), ("@ID", t.Id)]);

                                            }
                                            frmTitleSelect.SetTitleTag(t.Id);
                                            fnd = true;
                                            break;
                                        }
                                    }
                                }

                                frmTitleSelect.BaseTitle = $"{BaseTitle}";
                                frmTitleSelect.txtBaseTitle.Content = $"{BaseTitle}";
                                frmTitleSelect.txtTitle.Text = $"{BaseTitle}".ToPascalCase();
                                break;
                            }

                            if (index == -1)
                            {
                                int id = -1;
                                string BaseStrX = frmTitleSelect.BaseTitle.ToPascalCase();
                                if (BaseStrX == "")
                                {
                                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                    string rootfolder = FindUploadPath();
                                    key?.Close();
                                    BaseStrX = rootfolder.Split(@"\").ToList().LastOrDefault();
                                }
                                if (ShortsDirectoryIndex == -1)
                                {
                                    ShortsDirectoryIndex = EditableshortsDirectoryList.Where
                                        (item => item.Directory == BaseStrX).FirstOrDefault(new ShortsDirectory(-1, "")).Id;
                                }
                                TitleId = InsertUpdateTitle(BaseStrX, ShortsDirectoryIndex);
                                DescId = InsertUpdateDescription(BaseStrX, ShortsDirectoryIndex);

                                BaseTitle = BaseStrX;
                                frmTitleSelect.BaseTitle = $"{BaseTitle}".ToPascalCase();
                                frmTitleSelect.txtBaseTitle.Content = $"{BaseTitle}";
                                frmTitleSelect.txtTitle.Text = $"{BaseTitle}".ToPascalCase();
                                frmTitleSelect.SetTitleTag(TitleId);
                                string SQL = "UPDATE SHORTSDIRECTORY SET TITLEID = @TID WHERE ID = @ID;";
                                connectionString.ExecuteScalar(SQL, [("@ID", ShortsDirectoryIndex), ("@TID", id)]);
                            }
                            else TitleId = index;

                            frmTitleSelect.TitleId = TitleId;
                            string BaseStr = frmTitleSelect.BaseTitle + " ";
                            ///TitleTagsSrc = ShortsDirectoryIndex;
                            int Tid = EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex).FirstOrDefault().TitleId;
                            foreach (var item in TitleTagsList.Where(s => s.GroupId == Tid))
                            {
                                if (!BaseStr.Contains($"#{item.Description}"))
                                {
                                    BaseStr += $"#{item.Description} ";
                                }
                            }

                            /* string sql = "SELECT T.GROUPID, D.DESCRIPTION, "+
                                "LIST(T.TAG, '|#') AS TAGS, LIST(T.TAGID, ',') AS IDS FROM TITLETAGS T" +
                                "INNER JOIN TITLES D ON T.GROUPID = D.GROUPID "+
                               $"WHERE T.GROUPID != {index} AND D.ISTAG = 0 GROUP BY D.NAME,T.GROUPID;";
                            connectionString.ExecuteReader(sql, (FbDataReader r) =>
                            {
                                groupTitleTagsList.Add(new GroupTitleTags(r));
                            })*/
                            if (groupTitleTagsList.Count != 0)
                            {
                                frmTitleSelect.lstTitles.Items.Clear();
                                frmTitleSelect.lstTitles.ItemsSource = null;
                                frmTitleSelect.lstTitles.ItemsSource = groupTitleTagsList;
                            }
                            BaseStr = BaseStr.Trim().ToPascalCase();
                            frmTitleSelect.txtTitle.Text = BaseStr.Trim();
                            frmTitleSelect.lblTitleLength.Content = BaseStr.Trim().Length;

                            if (TitlesList.Where(s => s.Id == TitleId).Count() > 0)
                            {

                                TitleTagsSrc = TitlesList.Where(s => s.Id == TitleId).FirstOrDefault().Id;
                                titletagsViewSource.SortDescriptions.Add(new SortDescription("Description", ListSortDirection.Ascending));
                                titletagsViewSource.Source = TitleTagsList;
                                titletagsViewSource.Filter += (object sender, FilterEventArgs e) =>
                                {
                                    if (e.Item is TitleTags titleTag)
                                    {
                                        e.Accepted = titleTag.GroupId == TitleTagsSrc;
                                    }
                                };
                                availabletagsViewSource.Source = availableTagsList;
                                availabletagsViewSource.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
                                availabletagsViewSource.Filter += (object sender, FilterEventArgs e) =>
                                    {
                                        if (e.Item is AvailableTags availTag)
                                        {
                                            bool fndx = false;
                                            foreach (var item in titletagsViewSource.View)
                                            {
                                                if (item is TitleTags titleTag && titleTag.Description == availTag.Tag)
                                                {
                                                    fndx = true;
                                                    break;
                                                }
                                            }
                                            e.Accepted = !fndx;
                                        }
                                    };
                                frmTitleSelect.TagAvailable.ItemsSource = availabletagsViewSource.View;
                                frmTitleSelect.TagsGrp.ItemsSource = titletagsViewSource.View;
                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Refresh:
                        {
                            int titleid = frmTitleSelect.TitleId;
                            string BaseStr = frmTitleSelect.BaseTitle + " ";
                            foreach (var item in TitleTagsList.Where(s => s.GroupId == titleid).ToList())
                            {
                                BaseStr += (!BaseStr.Contains($"#{item.Description}")) ? $"#{item.Description} " : "";
                            }
                            frmTitleSelect.txtTitle.Text = BaseStr.Trim();
                            frmTitleSelect.lblTitleLength.Content = BaseStr.Trim().Length;
                            RefreshView();
                            return default(TResult);
                            break;
                        }
                    case CustomParams_InsertWithId cpInsert:
                        {
                            TagUpdate(dataUpdatType.Insert, cpInsert.id, cpInsert.Groupid, frmTitleSelect);
                            int titleid = frmTitleSelect.TitleId;
                            string x = OnGetAllTags(frmTitleSelect.GetTitleTag());
                            cpInsert.TitleLength = x.Length;
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Remove cpRemove:
                        {
                            TagUpdate(dataUpdatType.Remove, cpRemove.id, -1, frmTitleSelect, cpRemove.Name);
                            string x = OnGetAllTags(frmTitleSelect.GetTitleTag());
                            cpRemove.TitleLength = x.Length;
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Add cpAdd:
                        {
                            AddAvailableTag(cpAdd.data_string, frmTitleSelect);
                            return default(TResult);
                            break;
                        }
                    case CustomParams_EditName cp_Update:
                        {
                            bool found = false;
                            foreach (var item in TitlesList.Where(ik => ik.Id == cp_Update.id && !ik.IsTag))
                            {
                                item.Description = cp_Update.name;
                                found = true;
                            }
                            if (found)
                            {
                                string sql = "UPDATE TITLES SET DESCRIPTION = @name WHERE ID = @id;";
                                connectionString.ExecuteScalar(sql, [("@name", cp_Update.name), ("@id", cp_Update.id)]);
                            }
                            return default(TResult);
                            break;
                        }
                    case CustomParams_Get cpGet:
                        {
                            int id = cpGet.Id;
                            if (MasterTagSelectFrm is null)
                            {
                                frmTitleSelect.Hide();
                                MasterTagSelectFrm = new MasterTagSelectForm(frmTitleSelect.IsShorts,
                                    () => { frmTitleSelect.Show(); DoMasterTagClose(); }, InvokerHandler<object>, true, frmTitleSelect.TitleId);
                                MasterTagSelectFrm.ParentId = frmTitleSelect.TitleId;
                                MasterTagSelectFrm.Show();
                            }
                            return default(TResult);
                            break;
                        }
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"formObjectHandler_TitleSelect - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return default(TResult);
            }
        }
        private void OnReadTitlesTags(FbDataReader reader)
        {
            try
            {
                TitleTagsList.Add(new TitleTags(reader));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnReadTitlesTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void AddAvailableTag(string tagdescription, object ThisForm)
        {
            try
            {
                if (ThisForm is TitleSelectFrm)
                {
                    bool found = false;
                    foreach (var _ in availableTagsList.Where(tg => tg.Tag == tagdescription).Select(tg => new { }))
                    {
                        found = true;
                        break;
                    }

                    if (!found)
                    {
                        int idx = -1;
                        string Sql = $"insert into AVAILABLETAGS(TAG) VALUES(@tagdescription) RETURNING ID;";
                        idx = connectionString.ExecuteScalar(Sql, [("@tagdescription", tagdescription)]).ToInt(-1);
                        if (idx != -1)
                        {
                            availableTagsList.Add(new AvailableTags(tagdescription, idx));
                            RefreshView();
                            (ThisForm as TitleSelectFrm).txtNewTag.Text = "";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddAvailableTag {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void DoMasterTagClose()
        {
            try
            {
                if (MasterTagSelectFrm is not null)
                {
                    if (MasterTagSelectFrm.TagSetChanged)
                    {
                        /*if (ReleaseBuilder is not null)
                        {
                            string BaseStr = ReleaseBuilder.DoTitleSelectFrm.BaseTitle + " ";
                            foreach (var item in ColectionFilter.TitleTagSelectorView.View)
                            {
                                if (!BaseStr.Contains($"#{(item as TitleTags).Description}"))
                                {
                                    BaseStr += $"#{(item as TitleTags).Description} ";
                                }
                            }
                            BaseStr = BaseStr.Trim();
                            ReleaseBuilder.DoTitleSelectFrm.txtTitle.Text = BaseStr.Trim();
                            ReleaseBuilder.DoTitleSelectFrm.lblTitleLength.Content = BaseStr.Trim().Length;
                            ColectionFilter.TitleTagSelectorView.View.Refresh();
                            ColectionFilter.TagSelectorView.View.Refresh();
                            ColectionFilter.TagAvailableView.View.Refresh();
                            ColectionFilter.TitleTagAvailableView.View.Refresh();
                        }*/
                    }
                    if (MasterTagSelectFrm.IsClosing)
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(1500);
                        if (true && !cts.IsCancellationRequested)
                        {
                            Thread.Sleep(100);
                            System.Windows.Forms.Application.DoEvents();
                        }
                        MasterTagSelectFrm = null;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ObjectHandler - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public void TagUpdate(dataUpdatType dt, int id, int GroupId, object ThisForm, string Desc = "")
        {
            try
            {
                string Sql = "";
                bool found = false;
                if (ThisForm is TitleSelectFrm frmTitleSelect)
                {
                    if ((dt == dataUpdatType.Insert))
                    {
                        int idx = -1;
                        //TAGID INTEGER, TITLEID INTEGER)
                        string TableName = (ThisForm is TitleSelectFrm) ? "TITLETAGS" : "SELECTEDTAGS";
                        string TagID = ThisForm is TitleSelectFrm ? "TAGID" : "SELECTEDTAG";
                        string TitleID = ThisForm is TitleSelectFrm ? "GROUPID" : "GROUPTAGID";
                        Sql = $"SELECT ID FROM {TableName} WHERE {TagID} = @TAGID AND {TitleID} = @GROUPID";
                        idx = connectionString.ExecuteScalar(Sql, [("@TAGID", id), ("@GROUPID", GroupId)]).ToInt(-1);
                        if (idx == -1)
                        {
                            Sql = $"INSERT INTO {TableName}({TagID}, {TitleID}) VALUES(@TAGID, @GROUPID) RETURNING ID";
                            idx = connectionString.ExecuteScalar(Sql, [("@TAGID", id), ("@GROUPID", GroupId)]).ToInt(-1);

                            if (idx != -1)
                            {
                                //T.TAGID = S.ID
                                string SQL = $"Select * FROM {TableName} T INNER JOIN AVAILABLETAGS S ON S.ID = T.TAGID WHERE T.ID = {idx}";
                                connectionString.ExecuteReader(SQL, (FbDataReader r) =>
                                {
                                    TitleTagsList.Add(new TitleTags(r));
                                });
                                RefreshView();
                            }
                            else
                            {
                                RefreshView();
                            }
                        }
                    }
                    if ((dt == dataUpdatType.Add))
                    {
                        int idx = -1;
                        Sql = $"INSERT INTO TITLES(DESCRIPTION) VALUES(@description) RETURNING ID";
                        idx = connectionString.ExecuteScalar(Sql, [("@description", Desc)]).ToInt(-1);
                        SelectedTagId = (idx != -1) ? idx : SelectedTagId;
                        frmTitleSelect.SetTitleTag(SelectedTagId);
                        UpdateTitleTagDesc(SelectedTagId, ThisForm);
                    }
                    else if ((dt == dataUpdatType.Remove))
                    {
                        string TableName = (ThisForm is TitleSelectFrm) ? "TITLETAGS" : "SELECTEDTAGS";
                        Sql = $"DELETE FROM {TableName} WHERE ID = {id}";
                        connectionString.ExecuteScalar(Sql);
                        SelectedTagId = (SelectedTagId == id) ? -1 : SelectedTagId;
                        for (int i = 0; i < TitleTagsList.Count; i++)
                        {
                            if (TitleTagsList[i].Id == id)
                            {
                                TitleTagsList.RemoveAt(i);
                                break;
                            }
                        }
                        RefreshView();

                    }
                    else if ((dt == dataUpdatType.Edit))
                    {
                        Sql = $"UPDATE TITLES SET DESCRIPTION = @description WHERE ID = @id";
                        int idx = connectionString.ExecuteScalar(Sql, [("@description", Desc), ("@id", id)]).ToInt(-1);
                        frmTitleSelect.SetTitleTag(SelectedTagId);
                        UpdateTitleTagDesc(SelectedTagId, ThisForm);
                    }
                    else if ((dt == dataUpdatType.Change))
                    {
                        SelectedTagId = id;
                        frmTitleSelect.SetTitleTag(SelectedTagId);
                        UpdateTitleTagDesc(SelectedTagId, ThisForm);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + " " + ex.Message);
            }
        }

        private void RefreshView()
        {
            int Tid = EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex).FirstOrDefault().TitleId;
            TitleTagsSrc = TitlesList.Where(s => s.Id == Tid).FirstOrDefault().Id;
            titletagsViewSource.View.Refresh();
            availabletagsViewSource.View.Refresh();
        }

        private int InsertUpdateTitle(string Description, int GroupId = -1)
        {
            try
            {
                string sql = "";
                int LinkedId = -1, id = TitlesList.Where(t => t.Description.ToLower() == Description.ToLower()).FirstOrDefault(new Titles(-1)).Id;
                if (id == -1)
                {
                    sql = "SELECT * FROM TITLES WHERE DESCRIPTION = @P0 AND ISTAG = @P1 AND GROUPID = @P2";
                    sql = "INSERT INTO TITLES(DESCRIPTION,ISTAG,GROUPID) " +
                        "VALUES(@P0,@P1,@P2) RETURNING ID;";
                    int _GroupId = (GroupId == -1) ? LinkedId : GroupId;
                    id = connectionString.ExecuteScalar(sql, [("@P0", Description),
                                ("@P1", false), ("@P2", _GroupId)]).ToInt(-1);
                    if (id != -1)
                    {
                        TitleId = id;
                        sql = "SELECT * FROM TITLES WHERE ID = @ID";
                        CancellationTokenSource cts = new CancellationTokenSource(2000);
                        connectionString.ExecuteReader(sql, [("@ID", id)], cts, (FbDataReader r) =>
                        {
                            TitlesList.Add(new Titles(r));
                            cts.Cancel();
                        });
                    }
                    else
                    {
                        sql = "SELECT ID FROM TITLES WHERE DESCRIPTION = @P0 AND ISTAG = @P1 AND GROUPID = @P2";
                        id = connectionString.ExecuteScalar(sql, [("@P0", Description),
                                ("@P1", false), ("@P2", -1)]).ToInt(-1);

                        if (id != -1)
                        {
                            sql = "UPDATE TITLES SET GROUPID = @P0 WHERE ID = @P1";
                            connectionString.ExecuteScalar(sql, [("@P0", GroupId), ("@P1", id)]);
                            foreach (var p in TitlesList.Where(p => p.Id == id))
                            {
                                p.GroupId = GroupId;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    TitleId = id;

                    int groupid = TitlesList.Where(t => t.Description.ToLower() == Description.ToLower()).FirstOrDefault(new Titles(-1)).GroupId;
                    if (groupid == -1)
                    {
                        sql = "UPDATE TITLES SET GROUPID = @P0 WHERE ID = @P1";
                        connectionString.ExecuteScalar(sql, [("@P0", GroupId), ("@P1", id)]);
                        foreach (var p in TitlesList.Where(p => p.Id == id))
                        {
                            p.GroupId = GroupId;
                            break;
                        }
                    }
                }

                return id;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertUpdateTitle - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return -1;
            }
        }

        public void UpdateTitleTagDesc(int id, object ThisForm)
        {
            try
            {
                int TitleTag = -1;
                if (ThisForm is TitleSelectFrm frmTitleSelect)
                {
                    int TitleId = frmTitleSelect.GetTitleTag();
                    var TagDescriptions = OnGetAllTags(TitleTag);
                    int idx = -1;
                    foreach (var p in TitlesList.Where(p => p.Id == TitleId))
                    {
                        idx = TitlesList.IndexOf(p);
                        break;
                    }
                    if (idx == -1)
                    {
                        TitlesList[idx].VisualDescription = TitlesList[idx].Description + TagDescriptions;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + " " + ex.Message);
            }
        }

        private string GetDefaultDescription()
        {
            try
            {
                return Environment.NewLine + Environment.NewLine
                         + "Follow me @ twitch.tv/justinstrainclips" +
                         Environment.NewLine + Environment.NewLine +
                         "Support Me On Patreon - https://www.patreon.com/join/JustinsTrainJourneys";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetDefaultDescription - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }

        private int InsertUpdateDescription(string descr, int LinkedId)
        {
            try
            {
                if (descr == "") return -1;
                string sql = "";
                string desc = GetDefaultDescription();
                int id = DescriptionsList.Where(t => t.TitleTagId == LinkedId &&
                   t.Name.ToUpper() == descr.ToUpper() && t.IsShortVideo).
                   FirstOrDefault(new Descriptions(-1)).Id;
                if (id == -1)
                {
                    sql = "INSERT INTO DESCRIPTIONS(DESCRIPTION,TITLETAGID,NAME,ISSHORTVIDEO, ISTAG) " +
                            "VALUES(@DESC,@TITLETAG,@NAME,@ISSHORTVIDEO,@ISTAG) RETURNING ID;";
                    id = connectionString.ExecuteScalar(sql, [("@DESC", descr+desc),
                          ("@TITLETAG", LinkedId), ("@NAME", descr.ToUpper()),
                          ("@ISSHORTVIDEO", true), ("@ISTAG", false)]).ToInt(-1);
                    if (id != -1)
                    {
                        DescId = id;
                        sql = "SELECT * FROM DESCRIPTIONS WHERE ID = @ID";
                        CancellationTokenSource cts = new CancellationTokenSource(2000);
                        connectionString.ExecuteReader(sql, [("@ID", id)], cts, (FbDataReader r) =>
                        {
                            DescriptionsList.Add(new(r));
                            cts.Cancel();
                        });
                    }
                }
                else DescId = id;
                return id;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertUpdateDescription - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return -1;
            }
        }

        private int InsertUpdateShorts(string DirectoryName, int TitleId = -1, int DescId = -1)
        {
            try
            {
                string sql = "";
                int LinkedId = -1;//, DescId = -1, TitleId = -1;
                LinkedId = EditableshortsDirectoryList.Where(fnd => fnd.Directory.ToLower() ==
                DirectoryName.ToLower()).FirstOrDefault(new ShortsDirectory(-1)).Id;
                if (LinkedId == -1)
                {
                    sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME,TITLEID,DESCID) " +
                        "VALUES(@P0,@P1,@P2) RETURNING ID;";
                    LinkedId = connectionString.ExecuteScalar(sql, [("@P0", DirectoryName.ToUpper()),
                            ("@P1", TitleId), ("@P2", DescId)]).ToInt(-1);
                    if (LinkedId != -1)
                    {
                        sql = "SELECT * FROM SHORTSDIRECTORY WHERE ID = @ID";
                        CancellationTokenSource cts = new CancellationTokenSource(2000);
                        connectionString.ExecuteReader(sql, [("@ID", LinkedId)], cts, (FbDataReader r) =>
                        {
                            EditableshortsDirectoryList.Add(new ShortsDirectory(r));
                            cts.Cancel();
                        });
                    }
                    if (TitleId == -1) TitleId = InsertUpdateTitle(DirectoryName.ToUpper());
                    if (DescId == -1) DescId = InsertUpdateDescription(DirectoryName.ToUpper(), LinkedId);
                    sql = "UPDATE SHORTSDIRECTORY SET TITLEID=@TITLEID,DESCID=@DESCID WHERE ID=@ID;";
                    connectionString.ExecuteScalar(sql, [("@ID", LinkedId),
                            ("@TITLEID", TitleId), ("@DESCID", DescId)]);
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id != LinkedId))
                    {
                        item.TitleId = TitleId;
                        item.DescId = DescId;
                    }
                }
                return LinkedId;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertUpdateShorts - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return -1;
            }
        }

        private void InsertUpdateMultiShorts(int LinkedId, string DirectoryName)
        {
            try
            {
                DateTime LastTimeUploaded = DateTime.Now.Date.AddYears(-100);
                (LastTimeUploaded, _) = CheckUploads(LinkedId);
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shorts_dir1 = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                key?.Close();
                string SearchDir1 = Path.Combine(shorts_dir1, DirectoryName);
                int NumberofShorts = Directory.EnumerateFiles(SearchDir1, "*.mp4", SearchOption.AllDirectories).Count();
                string sql = "SELECT * FROM MULTISHORTSINFO WHERE ISSHORTSACTIVE = 1;";
                bool IsShortsActive = connectionString.ExecuteScalar(sql).ToInt(-1) != -1;

                sql = "SELECT ID FROM MULTISHORTSINFO WHERE LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                int id = connectionString.ExecuteScalar(sql, [("@LINKEDID", LinkedId)]).ToInt(-1);
                if (id == -1)
                {
                    InsertIntoMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded);
                }
                else
                {
                    UpdateMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded, DirectoryName);
                }
                if (!IsShortsActive)
                {
                    sql = "UPDATE MULTISHORTSINFO SET LASTUPLOADEDDATE=@DT,LASTUPLOADEDTIME=@DTT,ISSHORTSACTIVE=0 WHERE ID != @ID;";
                    connectionString.ExecuteScalar(sql, [("@ID", LinkedId), ("@DT", null), ("@DTT", null)]);
                    sql = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE=1 WHERE ID = @ID;";
                    connectionString.ExecuteScalar(sql, [("@ID", LinkedId)]);
                    foreach (var item in SelectedShortsDirectoriesList)
                    {
                        item.IsShortActive = (item.Id == LinkedId);
                        item.LastUploadedDateFile = (item.Id == LinkedId) ? item.LastUploadedDateFile : DateTime.Now.Date.AddYears(-100);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertUpdateMultiShorts - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private TResult selectShortUpload_Handler<TResult>(object tld, SelectShortUpload selectShortUpload)
        {
            try
            {
                if (tld is CustomParams_GetShortsDirectoryById pcGSD)
                {
                    int TitleId = -1, DescId = -1, idx = pcGSD.Id;
                    foreach (var it in RematchedList.Where(s => s.OldId == idx))
                    {
                        idx = it.NewId;
                        break;
                    }

                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == idx))
                    {
                        TitleId = item.TitleId;
                        DescId = item.DescId;
                        break;
                    }
                    string Title = (TitleId == -1) ? "" : TitlesList.Where(s => s.Id == TitleId).
                        FirstOrDefault(new Titles(-1)).Description;
                    string BaseStr = " ";
                    if (TitleId != -1)
                    {
                        int TagCnt = 0;
                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == TitleId))
                        {
                            if (!BaseStr.Contains($"#{item2.Description}"))
                            {
                                BaseStr += $"#{item2.Description} ";
                                TagCnt++;
                            }
                        }
                        if (TagCnt <= 2)
                        {
                            BaseStr = " ";
                            List<string> Tags = new List<string> {
                               "#TRAINS", "#TRAVEL", "#SHORTS", "#RAILFANS",
                               "#RAILWAY", "#RAIL"};

                            if (BaseStr.Contains("VLINE"))
                            {
                                Tags.Insert(0, "#VLINE");
                            }
                            while (Tags.Count > 0 && BaseStr.Length < 100)
                            {
                                string tg = BaseStr + " " + Tags[0];
                                if (tg.Length < 100)
                                {
                                    BaseStr = tg;
                                    Tags.RemoveAt(0);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        Title += BaseStr;
                    }



                    string Desc = (DescId == -1) ? "" : DescriptionsList.Where(s => s.Id == DescId).
                        FirstOrDefault(new Descriptions(-1)).Description;

                    var r = new TurlpeDualString(Title.Trim(), Desc, idx);
                    return (TResult)Convert.ChangeType(r, typeof(TResult));
                }

                if (tld is CustomParams_AddDirectory cpAPD)
                {
                    string Title = "", Desc = "";
                    int TitleId = -1, DescId = -1;
                    int idx = InsertUpdateShorts(cpAPD.DirectoryName);

                    if (idx != -1)
                    {
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == idx))
                        {
                            TitleId = item.TitleId;
                            DescId = item.DescId;
                            break;
                        }
                    }
                    Title = (TitleId == -1) ? "" : TitlesList.Where(s => s.Id == TitleId).FirstOrDefault(new Titles(-1)).Description;
                    Desc = (DescId == -1) ? "" : DescriptionsList.Where(s => s.Id == DescId).FirstOrDefault(new Descriptions(-1)).Description;
                    string BaseStr = " ";
                    if (TitleId != -1)
                    {
                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == TitleId))
                        {
                            if (!BaseStr.Contains($"#{item2.Description}"))
                            {
                                BaseStr += $"#{item2.Description} ";
                            }
                        }
                        Title += BaseStr;
                    }
                    var rr = new TurlpeDualString(Title.Trim(), Desc, idx);
                    return (TResult)Convert.ChangeType(rr, typeof(TResult));
                }
                if (tld is CustomParams_RematchedUpdate rfu)
                {
                    string sql = "SELECT ID FROM SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                    var sid = connectionString.ExecuteScalar(sql, [("@P0", rfu.directory)]).ToInt(-1);
                    int OldId = -1, NewId = -1;
                    foreach (var item in RematchedList.Where(r => r.OldId == rfu.newid))
                    {
                        OldId = item.OldId;
                        NewId = item.NewId;
                        break;
                    }
                    if (OldId == -1)
                    {
                        CancellationTokenSource cts = new CancellationTokenSource(2000);
                        sql = "SELECT NEWID,OLDID FROM REMATCHED WHERE OLDID = @P0";
                        connectionString.ExecuteReader(sql, [("@P0", rfu.newid)], cts, (r) =>
                        {
                            OldId = (r["OLDID"] is int oldid) ? oldid : -1;
                            NewId = (r["NEWID"] is int newid) ? newid : -1;
                            cts.Cancel();
                        });
                    }

                    if ((OldId == -1 && NewId == -1))
                    {
                        sql = "INSERT INTO REMATCHED(NEWID,OLDID) VALUES (@P0,@P1) returning ID";
                        int idx = connectionString.ExecuteScalar(sql,
                            [("@P0", rfu.newid), ("@P1", sid)]).ToInt(-1);
                        RematchedList.Add(new Rematched(idx, rfu.newid, sid, ""));
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                    else
                    {
                        if (NewId != sid)
                        {
                            sql = "UPDATE REMATCHED SET NEWID = @P0 WHERE OLDID = @P1";
                            connectionString.ExecuteNonQuery(sql,
                                [("@P0", sid), ("@P1", OldId)]);
                            foreach (var r in RematchedList.Where(s => s.OldId == OldId))
                            {
                                r.NewId = sid;
                                break;
                            }
                            return (TResult)Convert.ChangeType(true, typeof(TResult));
                        }
                        return (TResult)Convert.ChangeType(true, typeof(TResult));
                    }
                }
                else if (tld is CustomParams_GetDescIdByDirectory CGGG)
                {
                    string Dir = CGGG.DirectoryName.ToUpper();
                    foreach (var item in EditableshortsDirectoryList.Where(
                        s => s.Directory.ToUpper() == Dir))
                    {
                        return (TResult)Convert.ChangeType(item.DescId, typeof(TResult));
                    }
                }

                else if (tld is CustomParams_DescUpdate CPDE)
                {
                    var ss = DescUpdater(selectShortUpload, CPDE.DirectoryName, CPDE.Description, false);
                    return (TResult)Convert.ChangeType(ss, typeof(TResult));
                }
                else if (tld is CustomParams_GetCurrentDescId CGCD)
                {
                    int DescId = -1;
                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == ShortsDirectoryIndex))
                    {
                        DescId = item.DescId;
                        break;
                    }
                    return (TResult)Convert.ChangeType(DescId, typeof(TResult));
                }
                else if (tld is CustomParams_LookUpTitleId CPTI)
                {
                    int id = InsertUpdateShorts(CPTI.DirectoryName.ToUpper());
                    if (id != -1)
                    {
                        int TitleId = -1;
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == id))
                        {
                            TitleId = item.TitleId;
                            break;
                        }
                        return (TResult)Convert.ChangeType(TitleId, typeof(TResult));
                    }
                }
                else if (tld is CustomParams_LookUpId CPSTI)
                {
                    //int id = InsertUpdateShorts(CPSTI.DirectoryName.ToUpper());
                    int TitleId = -1;
                    foreach (var item in EditableshortsDirectoryList.Where(
                        s => s.Directory.ToUpper() == CPSTI.DirectoryName.ToUpper()))
                    {
                        TitleId = item.Id;
                        break;
                    }
                    return (TResult)Convert.ChangeType(TitleId, typeof(TResult));

                }
                if (tld is CustomParams_InsertIntoShortsDirectory CPISD)
                {
                    int _TitleId = InsertUpdateTitle(CPISD.DirectoryName, ShortsDirectoryIndex);
                    int _DescId = InsertUpdateDescription(CPISD.DirectoryName, ShortsDirectoryIndex);
                    if (ShortsDirectoryIndex == -1)
                    {
                        int IDX = InsertUpdateShorts(CPISD.DirectoryName, _TitleId, _DescId);
                        ShortsDirectoryIndex = IDX;
                    }

                    string sql = "update MULTISHORTS set ISSHORTSACTIVE = @ACTIVE where LINKEDSHORTSDIRECTORYID = @ID;";
                    connectionString.ExecuteNonQuery(sql, [("@ACTIVE", 1), ("@ID", ShortsDirectoryIndex)]);
                    sql = "update MULTISHORTS set ISSHORTSACTIVE = @ACTIVE where LINKEDSHORTSDIRECTORYID != @ID;";
                    connectionString.ExecuteNonQuery(sql, [("@ACTIVE", 0), ("@ID", ShortsDirectoryIndex)]);
                    string linkedtitleids = "", linkeddescids = "";
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionString.ExecuteReader(GetUploadReleaseBuilderSql(ShortsDirectoryIndex), (FbDataReader r) =>
                    {
                        linkedtitleids = (r["LINKEDTITLEIDS"] is string ldid1 ? ldid1 : "");
                        linkeddescids = (r["LINKEDDESCIDS"] is string lditt ? lditt : "");
                        cts.Cancel();
                    });
                    selectShortUpload.UpdateTitleId(ShortsDirectoryIndex, linkedtitleids);
                    selectShortUpload.UpdateDescId(ShortsDirectoryIndex, linkeddescids);
                    InsertUpdateMultiShorts(ShortsDirectoryIndex, CPISD.DirectoryName);
                    return (TResult)Convert.ChangeType(ShortsDirectoryIndex, typeof(TResult));
                }
                else if (tld is CustomParams_UpdateMultishortsByDir CPAD)
                {
                    string sql = "";
                    int LinkedId = InsertUpdateShorts(CPAD.DirectoryName);
                    bool found = SelectedShortsDirectoriesList.Any(item => item.DirectoryName == CPAD.DirectoryName);
                    if (!found && CPAD.DirectoryName != "")
                    {
                        InsertUpdateMultiShorts(LinkedId, CPAD.DirectoryName);
                    }
                    return default(TResult);
                }
                if (tld is CustomParams_InsertMultiShortsInfo cpsi)
                {
                    InsertIntoMultiShortsInfo(cpsi.numberofShorts, cpsi.linkedId, cpsi.lastTimeUploaded, cpsi.IsActive);
                }
                else if (tld is CustomParams_UpdateMultiShortsInfo cpup)
                {
                    UpdateMultiShortsInfo(cpup.numberofShorts, cpup.linkedId, cpup.lastTimeUploaded, cpup.uploaddir);
                }
                else if (tld is CustomParams_GetConnectionString CGCS)
                {
                    CGCS.ConnectionString = GetConectionString();
                    return (TResult)Convert.ChangeType(CGCS.ConnectionString, typeof(TResult));
                }
                else if (tld is CustomParams_Select SPS)
                {
                    ShortsDirectoryIndex = SPS.Id;
                    if (!EditableshortsDirectoryList.Any(s => s.Id == SPS.Id))
                    {
                        string sql = "SELECT * FROM SHORTSDIRECTORY WHERE ID = @ID";
                        CancellationTokenSource cts = new CancellationTokenSource();
                        connectionString.ExecuteReader(sql, [("@ID", SPS.Id)], cts, (FbDataReader r) =>
                        {
                            EditableshortsDirectoryList.Add(new ShortsDirectory(r));
                            cts.Cancel();
                        });
                        TitleTagsSrc = SPS.Id;


                    }
                }
                else if (tld is CustomParams_UpdateTitleById SPU)
                {
                    foreach (var p in EditableshortsDirectoryList.Where(s => s.Id == SPU.Id))
                    {
                        p.TitleId = SPU.Title;
                        break;
                    }
                    if (!TitlesList.Any(s => s.Id == SPU.Title))
                    {
                        string sql = "SELECT * FROM TITLES WHERE ID = @ID";
                        CancellationTokenSource cts = new CancellationTokenSource();
                        connectionString.ExecuteReader(sql, [("@ID", SPU.Title)], cts, (FbDataReader r) =>
                        {
                            TitlesList.Add(new Titles(r));
                            cts.Cancel();
                        });
                    }

                }
                else if (tld is CustomParams_Get)
                {
                    if (ShortsDirectoryIndex == -1)
                    {
                        var r = EditableshortsDirectoryList.LastOrDefault();
                        if (r is not null)
                        {
                            return (TResult)Convert.ChangeType(r.Id, typeof(TResult));
                        }
                    }
                    return default(TResult);
                }

                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"selectShortUpload_Handler - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return default(TResult);
            }
        }

        private void UpdateMultiShortsInfo(int NumberofShorts, int LinkedId, DateTime LastTimeUploaded, string uploaddir)
        {
            try
            {
                string updir = uploaddir.Split('\\').LastOrDefault();
                string sql = "";
                if (LastTimeUploaded.Date.Year > 2000)
                {
                    sql = "UPDATE MULTISHORTSINFO SET NUMBEROFSHORTS = @NUMBEROFSHORTS," +
                        " LASTUPLOADEDDATE = @DT, LASTUPLOADEDTIME = @DTT WHERE" +
                        " LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                    connectionString.ExecuteScalar(sql, [("@NUMBEROFSHORTS", NumberofShorts),
                                    ("@LINKEDID", LinkedId), ("@DT", LastTimeUploaded.Date),   ("@DTT", LastTimeUploaded.TimeOfDay)]);
                    foreach (var ip in SelectedShortsDirectoriesList.Where(item => item.DirectoryName == uploaddir))
                    {
                        ip.LastUploadedDateFile = LastTimeUploaded;
                        ip.NumberOfShorts = NumberofShorts;
                        break;
                    }
                }
                else
                {
                    sql = "UPDATE MULTISHORTSINFO SET NUMBEROFSHORTS = @NUMBEROFSHORTS WHERE" +
                        " LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                    connectionString.ExecuteScalar(sql, [("@NUMBEROFSHORTS", NumberofShorts),
                                    ("@LINKEDID", LinkedId)]);
                    foreach (var ip in SelectedShortsDirectoriesList.Where(item => item.DirectoryName == updir))
                    {
                        ip.NumberOfShorts = NumberofShorts;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + " " + ex.Message);
            }
        }

        public string OnGetAllTags(int Id)
        {
            try
            {
                string res = "";
                if (Id != -1)
                {
                    foreach (var tag in TitleTagsList.Where(s => s.GroupId == Id))
                    {
                        res += $"|{tag.Description}";

                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnGetAllTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }

        int _LastPTLinked = -1, _LastTitleId = -1, _LastDescId = -1;
        string _LastPTTitle = "";
        string _LastPTDesc = "";

        private TResult scraperModule_Handler<TResult>(object tld, ScraperModule scraperModule)
        {
            try
            {
                if (tld is CustomParams_DisplayTargets cpDTT)
                {
                    var _DisplayT = new ProcessDraftTargets(InvokerHandler<object>,
                        OnTargetsClose, cpDTT.ShowAction);
                    _DisplayT.Show();
                }
                if (tld is CustomParams_ProcessTargets cpPTT)
                {
                    int LinkedId = cpPTT.LinkedId;
                    if (cpPTT.LinkedId == -1)
                    {
                        foreach (var drshort in DraftShortsList.Where(s => s.VideoId == cpPTT.VideoId))
                        {
                            string filen = drshort.FileName;
                            if (filen.Contains("_"))
                            {
                                LinkedId = filen.Split('_')[1].ToInt(-1);
                            }
                            break;
                        }
                        if (LinkedId == -1)
                        {
                            ProcessTargetsList.Add(new ProcessTargets(ProcessTargetsList.Count + 1, cpPTT.VideoId, -1, "", -1, "", -1));
                        }

                        return default(TResult);
                    }
                    else
                    {
                        bool found = false;
                        if (ProcessTargetsList.Where(s => s.LinkedId == LinkedId).Count() == 0)
                        {
                            string Title = cpPTT.Title;
                            string Desc = cpPTT.Description;
                            if (_LastPTLinked != LinkedId)
                            {
                                foreach (var editable in EditableshortsDirectoryList.Where(s => s.Id == LinkedId))
                                {
                                    int TitleId = editable.TitleId;
                                    int DescId = editable.DescId;
                                    _LastTitleId = TitleId;
                                    _LastDescId = DescId;
                                    if (TitleId != -1)
                                    {
                                        Title = TitlesList.Where(s => s.Id == TitleId).FirstOrDefault(new Titles(-1)).Description;
                                        string BaseStr = " ";
                                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == TitleId))
                                        {
                                            if (!BaseStr.Contains($"#{item2.Description}"))
                                            {
                                                BaseStr += $"#{item2.Description} ";
                                            }
                                        }
                                        Title += BaseStr;
                                    }
                                    if (DescId != -1)
                                    {
                                        Desc = DescriptionsList.Where(s => s.Id == DescId).FirstOrDefault(new Descriptions(-1)).Description;
                                    }
                                    _LastPTLinked = LinkedId;
                                    break;
                                }
                            }
                            else
                            {
                                Title = _LastPTTitle;
                                Desc = _LastPTDesc;
                            }
                            if (!ProcessTargetsList.Any(s => s.LinkedId == LinkedId))
                            {

                                ProcessTargetsList.Add(new ProcessTargets(ProcessTargetsList.Count + 1, "",
                                    LinkedId, Title, _LastTitleId,
                                    Desc, _LastDescId));
                            }
                            found = true;
                        }
                    }
                }
                else if (tld is CustomParams_GetShortsDirectoryById pcGSD)
                {
                    int TitleId = -1, DescId = -1, idx = pcGSD.Id;
                    foreach (var it in RematchedList.Where(s => s.OldId == idx))
                    {
                        idx = it.NewId;
                        break;
                    }

                    foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == idx))
                    {
                        TitleId = item.TitleId;
                        DescId = item.DescId;
                        break;
                    }
                    string Title = (TitleId == -1) ? "" : TitlesList.Where(s => s.Id == TitleId).
                        FirstOrDefault(new Titles(-1)).Description;
                    string Desc = (DescId == -1) ? "" : DescriptionsList.Where(s => s.Id == DescId).
                        FirstOrDefault(new Descriptions(-1)).Description;
                    string BaseStr = " ";
                    if (TitleId != -1)
                    {
                        int tl = Title.Length;
                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == TitleId))
                        {
                            if (tl + $"#{item2.Description} ".Length < 100)
                            {
                                if (!BaseStr.Contains($"#{item2.Description}"))
                                {
                                    BaseStr += $"#{item2.Description} ";
                                }
                            }
                        }
                        Title += BaseStr;
                    }
                    var r = new TurlpeDualString(Title.Trim(), Desc, idx);
                    return (TResult)Convert.ChangeType(r, typeof(TResult));
                }
                if (tld is CustomParams_AddDirectory cpAPD)
                {
                    string Title = "", Desc = "";
                    int TitleId = -1, DescId = -1;
                    int idx = InsertUpdateShorts(cpAPD.DirectoryName);
                    if (idx != -1)
                    {
                        foreach (var item in EditableshortsDirectoryList.Where(s => s.Id == idx))
                        {
                            TitleId = item.TitleId;
                            DescId = item.DescId;
                            break;
                        }
                    }
                    Title = (TitleId == -1) ? "" : TitlesList.Where(s => s.Id == TitleId).FirstOrDefault(new Titles(-1)).Description;
                    Desc = (DescId == -1) ? "" : DescriptionsList.Where(s => s.Id == DescId).FirstOrDefault(new Descriptions(-1)).Description;
                    string BaseStr = " ";
                    if (TitleId != -1)
                    {
                        int tl2 = Title.Length;
                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == TitleId))
                        {
                            if (tl2 + $"#{item2.Description} ".Length < 100)
                            {
                                if (!BaseStr.Contains($"#{item2.Description}"))
                                {
                                    BaseStr += $"#{item2.Description} ";
                                }
                            }
                        }
                        Title += BaseStr;
                    }

                    var rr = new TurlpeDualString(Title.Trim(), Desc, idx);
                    return (TResult)Convert.ChangeType(rr, typeof(TResult));
                }
                if (tld is CustomParams_GetUploadsRecCnt cpGURC)
                {
                    string sql = "select count(Id) from UPLOADSRECORD WHERE UPLOAD_DATE = @P0 AND UPLOADTYPE = 0";
                    if (cpGURC.IsLast24Hours)
                    {
                        sql = "SELECT Count(Id) FROM UPLOADSRECORD WHERE UPLOAD_DATE = CURRENT_DATE AND " +
                        "UPLOAD_TIME >= CURRENT_TIME - 1 AND UPLOADTYPE = 0 OR UPLOAD_DATE = CURRENT_DATE - 1 " +
                        "AND UPLOAD_TIME >= CURRENT_TIME - 1 AND UPLOADTYPE = 0 AND UPLOADSRECORD.UPLOADFILE NOT LIKE '%mp4'";
                    }
                    var k = connectionString.ExecuteScalar(sql, [("@p0", DateTime.Now.Date)]).ToInt(-1);
                    return (TResult)Convert.ChangeType(k, typeof(TResult));
                }
                if (tld is CustomParams_UpdateUploadsRecords CPUUR)
                {
                    foreach (var file in CPUUR.DirectoryName)
                    {
                        string fname = Path.GetFileNameWithoutExtension(file.ToUpper());
                        string sql = "SELECT ID FROM UPLOADSRECORD WHERE UPLOADFILE = @P0 AND UPLOADTYPE = 0";
                        int id = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", fname)]).ToInt(-1);
                        if (id == -1)
                        {
                            sql = "INSERT INTO UPLOADSRECORD(UPLOADFILE, UPLOAD_DATE, UPLOAD_TIME,UPLOADTYPE,DIRECTORYNAME)" +
                                " VALUES (@P0,@P1,@P2,@P3,@P4) RETURNING ID";
                            id = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", fname),
                                      ("@P1", DateTime.Now.Date), ("@P2", DateTime.Now.TimeOfDay), ("@P3", 0),
                                      ("@P4", CPUUR.ParentDirectory)]).ToInt(-1);
                        }
                    }
                }
                if (tld is CustomParams_UpdateStats CPUSS)
                {
                    string sgl = "SELECT ID FROM SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                    int sid = connectionString.ExecuteScalar(sgl.ToUpper(),
                        [("@P0", CPUSS.DirectoryName.ToUpper())]).ToInt(-1);
                    if (sid != -1)
                    {
                        sgl = "UPDATE MULTISHORTSINFO SET LASTUPLOADEDDATE = @P1, LASTUPLOADEDTIME = @P2 " +
                              "DIRECTORYNAME = @P0" +
                            "WHERE LINKEDSHORTSDIRECTORYID = @iD";
                        connectionString.ExecuteNonQuery(sgl.ToUpper(),
                            [("@P1", DateTime.Now.Date), ("@P2", DateTime.Now.TimeOfDay),
                                ("@iD", sid)]);
                    }
                    int res = -1;
                    string fname = Path.GetFileNameWithoutExtension(CPUSS.DirectoryName.ToUpper());
                    string sql = "select ID from UPLOADSRECORD WHERE UPLOADFILE = @P0 AND UPLOADTYPE = 0";
                    res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", fname)]).ToInt(-1);
                    if (res == -1)
                    {
                        sql = "INSERT INTO UPLOADSRECORD(UPLOADFILE, UPLOAD_DATE, UPLOAD_TIME,UPLOADTYPE,DIRECTORYNAME) " +
                            "VALUES (@P0,@P1,@P2,@P3,@P4) RETURNING ID";
                        res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", fname), ("@P1", DateTime.Now.Date),
                                ("@P2", DateTime.Now.TimeOfDay), ("@P3", 0), ("@P4", CPUSS.DirectoryName.ToUpper())]).ToInt(-1);
                    }
                    else
                    {
                        sql = "UPDATE UPLOADSRECORD SET UPLOAD_DATE = @P1, UPLOAD_TIME = @P2 WHERE ID = @P0";
                        connectionString.ExecuteNonQuery(sql.ToUpper(), [("@P0", res), ("@P1", DateTime.Now.Date),
                                ("@P2", DateTime.Now.TimeOfDay)]);
                    }
                }
                if (tld is CustomParams_SetTimeSpans STT)
                {
                    return (TResult)Convert.ChangeType(EventTypes.ShortsSchedule, typeof(TResult));
                }

                if (tld is CustomParams_SelectById csi)
                {
                    string filename = "", vid = "";
                    foreach (var item in DraftShortsList.Where(s => s.VideoId == csi.VideoId))
                    {
                        filename = item.FileName;
                        vid = item.VideoId;

                        for (int i = 0; i < scraperModule.lstMain.Items.Count; i++)
                        {
                            object id = scraperModule.lstMain.Items[i];
                            if (id is string s && s.StartsWith(vid))
                            {
                                string o = scraperModule.lstMain.Items[i] as string;
                                o = o + $" {filename}";
                                scraperModule.lstMain.Items[i] = o;
                                break;
                            }
                        }
                        return (TResult)Convert.ChangeType(item.FileName, typeof(TResult));
                    }
                    return (TResult)Convert.ChangeType("", typeof(TResult));
                }
                if (tld is CustomParams_GetDesc cgd)
                {
                    foreach (var item in DescriptionsList.Where(s => s.TitleTagId == cgd.id))
                    {
                        cgd.name = item.Description;
                        break;
                    }
                    if ((!cgd.name.Contains("https://www.patreon.com/join/JustinsTrainJourneys"))
                            && (!cgd.name.Contains("www.patreon.com")))
                    {
                        cgd.name += Environment.NewLine + Environment.NewLine +
                             "Support Me On Patreon - https://www.patreon.com/join/JustinsTrainJourneys";
                    }
                    return (TResult)Convert.ChangeType(cgd.name, typeof(TResult));
                }
                else if (tld is CustomParams_GetTitle cgt)
                {
                    foreach (var item in EditableshortsDirectoryList.Where(item => item.Id == cgt.id))
                    {
                        string BaseTitle = item.Directory;
                        if (item.TitleId != -1)
                        {
                            foreach (var t in TitlesList.Where(i => i.GroupId == cgt.id && !i.IsTag))
                            {
                                BaseTitle = t.Description;
                                break;
                            }
                        }
                        string BaseStr = "";
                        int TagCnt = 0;
                        foreach (var item2 in TitleTagsList.Where(s => s.GroupId == cgt.id))
                        {
                            if (!BaseStr.Contains($"#{item2.Description}"))
                            {
                                BaseStr += $"#{item2.Description} ";
                                TagCnt++;
                            }
                        }
                        BaseStr = BaseStr.Trim();
                        if (TagCnt <= 2)
                        {
                            BaseStr = "";
                            List<string> Tags = new List<string> {
                               "#TRAINS", "#TRAVEL", "#SHORTS", "#RAILFANS",
                               "#RAILWAY", "#RAIL"};
                            if (BaseStr.Contains("VLINE"))
                            {
                                Tags.Insert(0, "#VLINE");
                            }
                            while (Tags.Count > 0 && BaseStr.Length < 100)
                            {
                                string tg = BaseStr + " " + Tags[0];
                                if (tg.Length < 100)
                                {
                                    BaseStr = tg;
                                    Tags.RemoveAt(0);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        cgt.name = BaseTitle.ToPascalCase() + " " + BaseStr;
                        return (TResult)Convert.ChangeType(cgt.name, typeof(TResult));
                    }
                }
                else if (tld is CustomParams_AddVideoInfo avi)
                {
                    int id = -1;
                    string sql = $"SELECT ID FROM {avi.TableName} WHERE FILENAME = @FILENAME";
                    id = connectionString.ExecuteScalar(sql, [("@FILENAME", avi.filename)]).ToInt(-1);
                    if (id == -1)
                    {
                        sql = $"INSERT INTO {avi.TableName} (VIDEOID,FILENAME) VALUES (@VIDEOID,@FILENAME)";
                        connectionString.ExecuteScalar(sql, [("@VIDEOID", avi.id), ("@FILENAME", avi.filename)]);
                    }
                    else
                    {
                        sql = $"UPDATE {avi.TableName} SET VIDEOID = @VIDEOID WHERE ID = @ID AND FILENAME = @FILENAME";
                        connectionString.ExecuteScalar(sql, [("@ID", id), ("@VIDEOID", avi.id), ("@FILENAME", avi.filename)]);
                    }
                    return default(TResult);
                }
                else if (tld is CustomParams_InsertIntoTable cit)
                {
                    int id = -1;
                    bool found = false;
                    foreach (var item in DraftShortsList.Where(s => s.FileName == cit.filename && s.VideoId == cit.id))
                    {
                        id = item.Id;
                        found = true;
                        break;
                    }
                    string sql = "SELECT ID FROM DRAFTSHORTS WHERE FILENAME = @FILENAME AND VIDEOID = @VIDEOID";
                    if (!found)
                    {
                        id = connectionString.ExecuteScalar(sql, [("@FILENAME", cit.filename), ("@VIDEOID", cit.id)]).ToInt(-1);
                    }
                    if (id == -1)
                    {
                        sql = "INSERT INTO DRAFTSHORTS (VIDEOID,FILENAME) VALUES (@VIDEOID,@FILENAME) RETURNING ID";
                        int idxx = connectionString.ExecuteScalar(sql, [("@VIDEOID", cit.id), ("@FILENAME", cit.filename)]).ToInt(-1);
                        if (idxx != -1)
                        {
                            sql = "SELECT * FROM DRAFTSHORTS WHERE ID = @ID";
                            CancellationTokenSource cts = new CancellationTokenSource(2000);
                            connectionString.ExecuteReader(sql, [("@ID", idxx)], (FbDataReader r) =>
                            {
                                DraftShortsList.Add(new DraftShorts(r));
                                cts.Cancel();
                            });

                        }
                        else
                        {
                            sql = "UPDATE DRAFTSHORTS SET VIDEOID = @VIDEOID WHERE ID = @ID AND FILENAME = @FILENAME";
                            connectionString.ExecuteScalar(sql, [("@ID", id), ("@VIDEOID", cit.id), ("@FILENAME", cit.filename)]);
                            foreach (var draft in DraftShortsList.Where(s => s.Id == id))
                            {
                                draft.VideoId = cit.id;
                                break;
                            }
                        }
                    }
                    return default(TResult);
                }


                if (tld is CustomParams_Wait)
                {
                    UploadWaitTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5));
                    return default(TResult);
                }

                if (tld is CustomParams_GetConnectionString CGCS)
                {
                    CGCS.ConnectionString = GetConectionString();
                    return (TResult)Convert.ChangeType(CGCS.ConnectionString, typeof(TResult));
                }
                return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"scraperModule_Handler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return default(TResult);
            }
        }

        private void OnTargetsClose(object ThisForm)
        {
            try
            {
                if (ThisForm is ProcessDraftTargets pdt)
                {
                    pdt.ShowParent();
                    pdt = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnTargetsClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void FinishScraper(object thisform, int id)
        {
            try
            {
                if (thisform is ScraperModule scraperModulex)
                {
                    bool IsTimeOut = (scraperModulex is ScraperModule sls) ? sls.TimedOutClose : false;
                    scraperModulex = null;
                    if (IsTimeOut)
                    {
                        var scraperModules = new ScraperModule(InvokerHandler<object>, FinishScraper, OldgUrl, OldTarget, 0);
                        Hide();
                        scraperModules.ShowActivated = true;
                        scraperModules.Show();
                    }
                    Show();
                }
                else
                {
                    Show();
                }
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
                /*
                 * bool res = false;
                if (ObservableCollectionFilter is not null)
                {
                    ObservableCollectionFilter.HistoricCollectionViewSource.Source = ComplexProcessingJobHistory;
                    if (complexfrm is not null && complexfrm.IsLoaded)
                    {
                        complexfrm.lstSchedules.ItemsSource = ObservableCollectionFilter.HistoricCollectionViewSource.View;
                        res = true;
                    }
                }*/
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
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




        int AutoCloseUd = -1;
        private void DoAutoCancelClose()
        {
            try
            {
                if ((DoAutoCancel is not null && DoAutoCancel.IsCloseAction))
                {
                    Close();
                    return;
                }
                DoAutoCancel = null;

                Show();
                if (AutoCloseUd != -1)
                {
                    connectionString.ExecuteNonQuery($"DELETE FROM EVENTS WHERE ID = {AutoCloseUd};");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoAutoCancelClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void DoFullSchedule(EventDefinition eventdef)
        {
            try
            {
                //Get ScheduleID from EventID in ScheduleList
                //Get ReleaseBuilder from ReleaseBuilderList on ScheduleID
                //Build Upload List
                //Schedule from Maxupload number
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoFullSchedule {MethodBase.GetCurrentMethod().Name} {this} {eventdef} {ex.Message}");
            }
        }




        public string GetEncryptedString(byte[] encriptedString)
        {
            try
            {
                return DecryptPassword(encriptedString);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
            return "";
        }
        public int GetFileCount(string Folder)
        {
            try
            {
                int res = 0;
                List<string> files = Directory.EnumerateFiles(Folder, "*.mp4", SearchOption.AllDirectories).ToList();
                res += files.Where(filename => filename.Contains("_")).Count();
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite();
                return -1;
            }
        }
        private void uploadonfinish(object sender, int id)
        {
            try
            {
                if (sender is ScraperModule sm)
                {
                    WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                    string gUrl = webAddressBuilder.Dashboard().Address;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string rootfolder = FindUploadPath();
                    key?.Close();
                    int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    if (sm != null && !sm.KilledUploads)
                    {
                        List<string> filesdone = new List<string>();
                        bool Exc = sm.Exceeded;
                        filesdone.AddRange(sm.ScheduledOk);
                        int Uploaded = sm.TotalScheduled;
                        int shortsleft = GetFileCount(rootfolder);
                        if (!Exc && shortsleft > 0 && Uploaded < MaxUploads)
                        {
                            var xxscraperModule = new ScraperModule(InvokerHandler<object>, uploadonfinish, gUrl, MaxUploads, 15);
                            xxscraperModule.ShowActivated = true;
                            xxscraperModule.ScheduledOk.AddRange(filesdone);
                            Hide();
                            var rr = GetEncryptedString(new int[] { 187, 54, 76, 68, 254, 212, 193, 85, 230,
                            88, 9, 166, 209, 171, 74, 122, 47, 247, 153, 225, 226 }.Select(i => (byte)i).ToArray());
                            Process[] webView2Processes = Process.GetProcessesByName(rr);
                            foreach (Process process in webView2Processes)
                            {
                                process.Kill();
                            }
                            xxscraperModule.Show();
                            return;
                        }
                    }

                    Show();
                    if (id != -1)
                    {
                        string sql = $"DELETE FROM EVENTS WHERE ID = {id};";
                        connectionString.ExecuteNonQuery(sql);
                    }
                    IsUploading = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"uploadonfinish {MethodBase.GetCurrentMethod().Name} {this} {ex.Message} {ex.StackTrace}");
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
                var x = GetEncryptedString(new int[] { 146, 58, 77, 67, 246 }.Select(i => (byte)i).ToArray());
                if (AppPath.ToLower().Contains(x))
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
                string sql = "select Destination from ProcessingLog where Source = @P0";
                object result = connectionString.ExecuteScalar(sql.ToUpper(), [("p0", Source)]);
                if (result is string idxx)
                {
                    return idxx;
                }
                return "";
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
                string sql = "insert into ProcessingLog(Source,Destination) values(@P0,@p1) returning Id";
                int res = connectionString.ExecuteScalar(sql.ToUpper(), [("p0", Source), ("p1", Destination)]).ToInt(-1);
                return res;
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
                string sql = "Select Id From ProcessingLog where Source = @P0 and Destination = @p1";
                int res = connectionString.ExecuteScalar(sql.ToUpper(), [("p0", Source), ("p1", Destination)]).ToInt(-1);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        public int ModifyRecordInDb(int idx, string SourceDir, string DestinationFName, TimeSpan StartPos, TimeSpan Duration,
                                     bool Is720p, bool IsShorts, int IsCreateShorts, bool IsEncodeTrim, bool IsCutTrim, bool IsMonitoredSource,
                                     bool IsPersisentJob, bool ismuxed, string muxdata)
        {
            try
            {
                string sql = "update AutoInsert set srcdir=@p1, StartPos=@p2, Duration=@p3, b720p=@p4, bShorts=@p5, bCreateShorts=@p6," +
                           "bEncodeTrim=@p7, bCutTrim=@p8, bMonitoredSource=@p90, bPersistentJob=@p10, RunId=@p11, IsMuxed = @p12, MuxData = @p13) " +
                           "where Id = @p99 returning Id";
                int res = connectionString.ExecuteScalar(sql, [("p1", SourceDir), ("p2", StartPos.TotalMilliseconds), ("p3", Duration.TotalMilliseconds),
                    ("p4", Is720p), ("p5", IsShorts), ("p6", IsCreateShorts), ("p7", IsEncodeTrim), ("p8", IsCutTrim), ("p9", IsMonitoredSource),
                    ("p10", IsPersisentJob), ("p11", CurrentDbId), ("p12", ismuxed), ("p13", muxdata), ("p99", idx)]).ToInt(-1);
                return res;
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
                string sql = "update AutoInsertHistory set DELETIONDATE=@p0 where Id = @p1 returning Id";
                int res = connectionString.ExecuteScalar(sql.ToUpper(), [("p0", DELETIONDATE), ("p1", idx)]).ToInt(-1);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }

        public int InsertRecordIntoDb(string SourceDir, string DestinationFName, TimeSpan StartPos, TimeSpan Duration,
                                     bool Is720p, bool IsShorts, int IsCreateShorts, bool IsEncodeTrim, bool IsCutTrim, bool IsMonitoredSource,
                                     bool IsPersisentJob, Nullable<DateTime> TwitchSchedule = null,
                                     string RTMP = "", bool IsTwitchStream = false, bool IsMuxed = false, string MuxData = "")
        {
            try
            {
                int res = -1;
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

                res = connectionString.ExecuteScalar(sql, [("p0", SourceDir),
                    ("p1", DestinationFName), ("p2", StartPos.TotalMilliseconds),
                    ("p3", Duration.TotalMilliseconds), ("p4", Is720p),
                    ("p5", IsShorts), ("p6", IsCreateShorts), ("p7", IsEncodeTrim),
                    ("p8", IsCutTrim), ("p9", IsMonitoredSource), ("p10", IsPersisentJob),
                    ("p11", CurrentDbId), ("p12", _TwichDate), ("p13", _TwichTime),
                    ("p14", RTMP), ("p15", IsTwitchStream), ("p16", IsMuxed), ("p17", MuxData)]).ToInt(-1);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return -1;
            }
        }
        public void CreateDBInvoker()
        {
            try
            {
                string exepath = GetExePath() + GetEncryptedString(new int[] { 170, 9, 70, 82, 244, 200,
                    233, 70, 251, 51, 11, 165, 214 }.Select(i => (byte)i).ToArray());
                connectionString = GetConectionString();

                if (!File.Exists(exepath))
                {
                    FbConnection.CreateDatabase(connectionString);
                }
                string Id = "id integer generated by default as identity primary key";
                string sqlstring = $"create table AutoInsert({Id},srcdir varchar(500), destfname varchar(500),StartPos BIGINT,Duration BIGINT,b720p SMALLINT," +
                    "bShorts SMALLINT,bEncodeTrim SMALLINT,bCutTrim SMALLINT,bMonitoredSource SMALLINT,bPersistentJob SMALLINT, RUNID INTEGER," +
                     "ISMUXED SMALLINT, MUXDATA VARCHAR(256)); ";

                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("AutoInsert", "ISMUXED", "SMALLINT", 0);
                connectionString.AddFieldToTable("AutoInsert", "MUXDATA", "VARCHAR(256)", "");
                sqlstring = $"create table AutoInsertHistory({Id},srcdir varchar(500), destfname varchar(500),StartPos BIGINT,Duration BIGINT,b720p SMALLINT," +
                    "bShorts SMALLINT,bEncodeTrim SMALLINT,bCutTrim SMALLINT,bMonitoredSource SMALLINT,bPersistentJob SMALLINT, RUNID INTEGER," +
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
                    var tablename = connectionString.ExecuteScalar(sqlstring);
                    if (tablename is null)
                    {
                        sqlstring = $"ALTER TABLE AUTOINSERT ADD {Fields[i]} {FieldType[i]};";
                        connectionString.ExecuteScalar(sqlstring);
                    }

                    sqlstring = $"SELECT RDB$FIELD_NAME AS FIELD_NAME FROM RDB$RELATION_FIELDS WHERE RDB$RELATION_NAME='AUTOINSERTHISTORY'AND RDB$FIELD_NAME = '{Fields[i]}';";
                    var xtablename = connectionString.ExecuteScalar(sqlstring).ToString();
                    if (xtablename is null)
                    {
                        sqlstring = $"ALTER TABLE AUTOINSERTHISTORY ADD {Fields[i]} {FieldType[i]};";
                        connectionString.ExecuteScalar(sqlstring);
                    }
                }

                sqlstring = $"create table MonitoredDeletion({Id},DestinationFile varchar(500))";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table UPLOADSRECORD({Id},UPLOADFILE varchar(256), UPLOAD_DATE DATE, UPLOAD_TIME TIME,UPLOADTYPE INTEGER)";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("UPLOADSRECORD", "UPLOADTYPE", "INTEGER", 0);
                connectionString.AddFieldToTable("UPLOADSRECORD", "DIRECTORYNAME", "VARCHAR(255)");
                sqlstring = $"create table PLANINGQUES({Id},SOURCE varchar(500),SourceDir varchar(500))";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table SETTINGS({Id},SETTINGNAME varchar(250),SETTING varchar(250))";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table PLANINGCUTS({Id}, PLANNINGQUEID INTEGER,STARTA BIGINT  ,ENDA BIGINT , " +
                    " CUTNO SMALLINT , FILENAME VARCHAR(500))";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table DRAFTSHORTS({Id}, VIDEOID VARCHAR(255), FILENAME VARCHAR(500))";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table ProcessingLog({Id},Source varchar(500),Destination varchar(500),InProcess SmallInt);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"create table SHORTSDIRECTORY({Id},DIRECTORYNAME varchar(500), TITLEID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SHORTSDIRECTORY", "TITLEID", "INTEGER", -1);
                connectionString.AddFieldToTable("SHORTSDIRECTORY", "DESCID", "INTEGER", -1);
                sqlstring = $"create table AutoEdits({Id},Source varchar(500),Destination varchar(500),Threshhold varchar(255)," +
                    "Segment varchar(255),Target varchar(255));".ToUpper();
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"delete from ProcessingLog where InProcess = @PUD;";
                connectionString.ExecuteNonQuery(sqlstring, [("@PUD", 1)]);
                sqlstring = $"CREATE TABLE AVAILABLETAGS({Id},TAG VARCHAR(500));";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SELECTEDTAGS({Id},SELECTEDTAG INTEGER, GROUPTAGID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE TITLETAGS({Id},TAGID INTEGER, GROUPID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE TITLES({Id},DESCRIPTION VARCHAR(500), TITLETAGID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("TITLES", "GROUPID", "INTEGER", -1);
                connectionString.AddFieldToTable("TITLES", "ISTAG", "INTEGER", -1);
                sqlstring = $"CREATE TABLE DESCRIPTIONS({Id},NAME VARCHAR(250),DESCRIPTION VARCHAR(2500),TITLETAGID INTEGER, ISSHORTVIDEO SMALLINT, ISTAG SMALLINT);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULEDPOOL({Id},NAME VARCHAR(250));";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULEDPOOLS({Id},POOLID INTEGER, DIRECTORY VARCHAR(512));";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULEUPLOADS({Id},POOLID INTEGER, DAYS SMALLINT, UPLOADTIME TIME, MAXA SMALLINT);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE EVENTSCHEDULES({Id},EVENTID INTEGER, SCHEDULEID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE EVENTSCHEDULEDATE({Id},EVENTID INTEGER, STARTA DATE, ENDA DATE, STARTTIME TIME, ENDTIME TIME);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULEDATE({Id},EVENTID INTEGER, STARTA DATE, ENDA DATE, STARTTIME TIME, ENDTIME TIME);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULES({Id},NAME VARCHAR(250), ISSCHEDULE SMALLINT, SCHEDULEID INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SCHEDULES", "SCHEDULEID", "INTEGER", -1);
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SCHEDULES", "SOURCE", "INTEGER", -1);
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SCHEDULES", "MAXDAILY", "INTEGER", -1);
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SCHEDULES", "MAXEVENT", "INTEGER", -1);
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("SETTINGS", "SETTINGDATE", "DATE");
                connectionString.AddFieldToTable("SETTINGS", "SETTINGTIME", "TIME");
                sqlstring = $"CREATE TABLE VIDEOSCHEDULE({Id},SCHEDULEID INTEGER, SCHEDULETIME TIME,DAYS INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE APPLIEDSCHEDULE({Id},SCHEDULEID INTEGER, STARTHOUR SMALLINT,ENDHOUR SMALLINT,GAP SMALLINT,DAYS INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCRAPEINFOTABLE({Id},EVENTID INTEGER, SCRAPETYPE INTEGER, SCRAPECOMMAND VARCHAR(255), TABLEDESTINATION VARCHAR(255));";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("DESCRIPTIONS", "ISTAG", "SMALLINT", 0);
                connectionString.CreateTableIfNotExists($"CREATE TABLE RUNNINGID({Id}, ACTIVE SMALLINT)");
                sqlstring = $"INSERT INTO RUNNINGID(ACTIVE) VALUES(0) RETURNING ID;";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULINGITEMS({Id},SCHEDULINGID INTEGER,SSTART TIME,SEND TIME,GAP INTEGER);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE SCHEDULINGNAMES({Id},NAME VARCHAR(250));";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE REMATCHED({Id},OLDID INTEGER,NEWID INTEGER, DIRECTORY VARCHAR(500));";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE MULTISHORTSINFO({Id},ISSHORTSACTIVE SMALLINT,NUMBEROFSHORTS INTEGER, " +
                    "LINKEDSHORTSDIRECTORYID INTEGER, LASTUPLOADEDDATE DATE, LASTUPLOADEDTIME TIME);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE CONVERTERSETTINGS({Id},ISDEFAULT SMALLINT,MINBITRATE VARCHAR(255)" +
                    ",MAXBITRATE VARCHAR(255),BITRATEBUFFER VARCHAR(255),VIDEOWIDTH VARCHAR(255)," +
                    "VIDEOHEIGHT VARCHAR(255),ARMODULAS VARCHAR(255), RESIZEENABLE SMALLINT," +
                    "ARROUNDENABLE SMALLINT,ARSCALINGENABLED SMALLINT,VSYNCEABLE SMALLINT);";
                connectionString.CreateTableIfNotExists(sqlstring);
                sqlstring = $"CREATE TABLE YTACTIONS({Id}, SCHEDULENAMEID INTEGER, SCHEDULENAME VARCHAR(255)," +
                             "ACTIONNAME VARCHAR(255), MAXSCHEDULES INTEGER, VIDEOTYPE INTEGER, SCHEDULED_DATE DATE, SCHEDULED_TIME_START TIME, SCHEDULED_TIME_END TIME," +
                             "ACTION_DATE DATE, ACTION_TIME TIME, COMPLETED_DATE DATE, COMPLETED_TIME TIME, ISACTIONED SMALLINT);";
                connectionString.CreateTableIfNotExists(sqlstring);
                connectionString.AddFieldToTable("YTACTIONS", "SCHEDULED_TIME_START", "TIME", null);
                connectionString.AddFieldToTable("YTACTIONS", "SCHEDULED_TIME_END", "TIME", null);
                int idx = connectionString.ExecuteScalar(sqlstring).ToInt(-1);
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
                string sql = "DELETE FROM AUTOINSERT WHERE ID = @FILENAME;";
                int idx = connectionString.ExecuteScalar(sql, [("@FILENAME", filename)]).ToInt(-1);
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
                    string sql = $"delete fromm MonitoredDeletion where DestinationFile = @FILENAME;";
                    int idx = connectionString.ExecuteScalar(sql, [("@FILENAME", filename)]).ToInt(-1);
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
                    string sql = "insert into MonitoredDeletion(DestinationFile) values(@FILENAME);";
                    int idx = connectionString.ExecuteScalar(sql.ToUpper(), [("@FILENAME", filename)]).ToInt(-1);
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
                string sqlstring = "select Id from MonitoredDeletion where DestinationFile = @FILENAME";
                int idx = connectionString.ExecuteScalar(sqlstring, [("@FILENAME", filename)]).ToInt(-1);
                return (idx != -1) ? true : false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public void CloseDialogComplexEditor(object sender, int e)
        {

            try
            {
                Show();
                if (sender is ComplexSchedular cf)
                {
                    cf = null;
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
                connectionString.ExecuteReader("SELECT * FROM AUTOINSERT", (FbDataReader r) =>
                {
                    ComplexProcessingJobList.Add(new ComplexJobList(r));
                    var InMemoryJob = new JobListDetails(r, ProcessingJobs.Count + 1);
                    if (InMemoryJob.IsMulti && InMemoryJob.GetCutList().Count > 0)
                    {

                        if (InMemoryJob.GetCutList().Count > 0)
                        {
                            ProcessingJobs.Add(InMemoryJob);
                        }
                    }
                });

                string uploadId = "";
                DateOnly dts = new();
                TimeOnly tts = new();
                connectionString.ExecuteReader("SELECT * FROM REMATCHED", (FbDataReader r) =>
                {
                    RematchedList.Add(new Rematched(r));
                });
                string SQL = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 1;";
                int Id = -1;
                DateTime newDate = DateTime.Now.Date.AddYears(-100);
                connectionString.ExecuteReader(SQL, (FbDataReader r) =>
                {
                    uploadId = (r["UPLOADFILE"] is string f) ? f : "";
                    var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                    TimeSpan dtr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                    dts = DateOnly.FromDateTime(dt);
                    tts = TimeOnly.FromTimeSpan(dtr);
                    newDate = dt.Date.AtTime(dtr);
                });
                /*if (uploadId.Contains("_"))
                {
                    Id = uploadId.Split('_')[1].ToInt(-1);
                    if (Id != -1)
                    {
                        foreach (var item in RematchedList.Where(s => s.OldId == Id))
                        {
                            Id = item.NewId;
                            break;
                        }
                        SQL = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE=1,LASTUPLOADEDDATE=@LID,LASTUPLOADEDTIME=@LIT" +
                            " WHERE LINKEDSHORTSDIRECTORYID=@LINKEDID";
                        connectionString.ExecuteScalar(SQL, [("@LID", dts), ("@LIT", tts), ("@ID", Id), ("@LINKEDID", Id)]);
                        SQL = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE=@ISS WHERE LINKEDSHORTSDIRECTORYID!=@LINKEDID";
                        connectionString.ExecuteScalar(SQL, [("@LINKEDID", Id), ("@ISS", 0)]);
                    }
                }*/
                connectionString.ExecuteReader("SELECT * FROM AUTOINSERTHISTORY Order By DELETIONDATE desc", (FbDataReader r) =>
                {
                    ComplexProcessingJobHistory.Add(new ComplexJobHistory(r));
                });
                connectionString.ExecuteReader("SELECT * FROM PLANINGQUES", (FbDataReader r) =>
                {
                    PlanningQueList.Add(new PlanningQue(r));
                });
                connectionString.ExecuteReader("SELECT * FROM PLANINGCUTS", (FbDataReader r) =>
                {
                    PlanningCutsList.Add(new PlanningCuts(r));
                });





                connectionString.ExecuteReader("SELECT * FROM DRAFTSHORTS", (FbDataReader r) =>
                {
                    DraftShortsList.Add(new DraftShorts(r));
                });



                //EVENTSDEFINITIONS
                // connectionString.ExecuteNonQuery("DELETE FROM UPLOADSRECORD WHERE UPLOAD_DATE <> CAST(GETDATE() AS DATE);");
                connectionString.ExecuteReader("SELECT * FROM AVAILABLETAGS", (FbDataReader r) =>
                {
                    availableTagsList.Add(new AvailableTags(r));
                });
                connectionString.ExecuteReader("SELECT * FROM SELECTEDTAGS S INNER JOIN AVAILABLETAGS T ON T.ID = S.SELECTEDTAG", (FbDataReader r) =>
                {
                    selectedTagsList.Add(new SelectedTags(r));
                });
                connectionString.ExecuteReader("SELECT * FROM TITLETAGS T INNER JOIN AVAILABLETAGS S ON T.TAGID = S.ID", (FbDataReader r) =>
                {
                    TitleTagsList.Add(new TitleTags(r));
                });
                connectionString.ExecuteReader("SELECT * FROM TITLES", (FbDataReader r) =>
                {
                    TitlesList.Add(new Titles(r, OnGetAllTags));
                });
                connectionString.ExecuteReader("SELECT * FROM DESCRIPTIONS", (FbDataReader r) =>
                {
                    DescriptionsList.Add(new Descriptions(r));
                });
                connectionString.ExecuteReader("SELECT * FROM SCHEDULINGNAMES", (FbDataReader r) =>
                {
                    SchedulingNamesList.Add(new ScheduleMapNames(r));
                });
                connectionString.ExecuteReader("SELECT * FROM SCHEDULINGITEMS", (FbDataReader r) =>
                {
                    SchedulingItemsList.Add(new ScheduleMapItem(r));
                });


                //EditableshortsDirectoryList
                connectionString.ExecuteReader(GetUploadReleaseBuilderSql(), (FbDataReader r) =>
                {
                    EditableshortsDirectoryList.Add(new ShortsDirectory(r));
                });

                connectionString.ExecuteReader("SELECT * FROM YTACTIONS", (FbDataReader r) =>
                    {
                        YTScheduledActionsList.Add(new ScheduledActions(r));
                    });
                ObservableCollectionFilter.ActionsScheduleCollectionViewSource.Source = YTScheduledActionsList;
                /*
                  sqlstring = $"CREATE TABLE MULTISHORTSINFO({Id},ISSHORTSACTIVE SHORT,NUMBEROFSHORTS INTEGER, "+
                    "LINKEDSHORTSDIRECTORYID INTEGER, LASTUPLOADEDDATE DATE, LASTUPLOADEDTIME TIME);";
                 */
                SelectedShortsDirectoriesList.Clear();
                string sql = "SELECT " +
                  "M.ID, M.ISSHORTSACTIVE, M.NUMBEROFSHORTS, " +
                  "M.LASTUPLOADEDDATE, M.LASTUPLOADEDTIME, M.LINKEDSHORTSDIRECTORYID, " +
                  "COALESCE(S2.ID, S1.ID) as SHORTSDIRECTORY_ID, " +
                  "COALESCE(S2.DIRECTORYNAME, S1.DIRECTORYNAME) as DIRECTORYNAME, " +
                  "COALESCE(S2.TITLEID, S1.TITLEID) as TITLEID, " +
                  "COALESCE(S2.DESCID, S1.DESCID) as DESCID, " +
                  "COALESCE(" +
                  "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S2.TITLEID), " +
                  "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S1.TITLEID)" +
                  ") AS LINKEDTITLEIDS, " + "COALESCE(" +
                  "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S2.DESCID), " +
                  "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S1.DESCID)" +
                  ") AS LINKEDDESCIDS " + "FROM MULTISHORTSINFO M " +
                  "LEFT JOIN (" + "REMATCHED R " +
                  "INNER JOIN SHORTSDIRECTORY S2 ON R.OLDID = S2.ID" +
                  ") ON M.LINKEDSHORTSDIRECTORYID = R.NEWID " +
                  "LEFT JOIN SHORTSDIRECTORY S1 ON M.LINKEDSHORTSDIRECTORYID = S1.ID " +
                 "WHERE COALESCE(S2.ID, S1.ID) IS NOT NULL";
                connectionString.ExecuteReader(sql, (FbDataReader r) =>
                {
                    SelectedShortsDirectoriesList.Add(new SelectedShortsDirectories(r));
                });
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string BaseDirectory = key?.GetValueStr("shortsdirectory", @"d:\shorts");
                key?.Close();

                foreach (var t in SelectedShortsDirectoriesList)
                {
                    bool fnd = EditableshortsDirectoryList.
                        Any(s => s.Directory.ToLower() == t.DirectoryName.ToLower()) != null;

                    int Idx = InsertUpdateShorts(t.DirectoryName);
                    t.LinkedShortsDirectoryId = Idx;
                    sql = "UPDATE MULTISHORTSINFO SET LINKEDSHORTSDIRECTORYID = @ID WHERE ID = @ID2;";
                    connectionString.ExecuteScalar(sql, [("@ID", Idx), ("@ID2", t.Id)]);
                    string newpath = Path.Combine(BaseDirectory, t.DirectoryName);

                }
                FilterActiveShortsDirlectoryList();
                bool _fnd = false;
                int _i = 0;
                foreach (var d in DescriptionsList.Where(s => !s.IsTag))
                {
                    foreach (var t in DescriptionsList.Where(s => !s.IsTag && s.Id != d.Id))
                    {
                        if (d.Name.ToLower() == t.Name.ToLower())
                        {
                            sql = "DELETE FROM DESCRIPTIONS WHERE ID = @ID;";
                            connectionString.ExecuteScalar(sql, [("@ID", t.Id)]);
                            _i++;
                        }
                    }
                }
                foreach (var shorts in EditableshortsDirectoryList)
                {
                    int Idx = shorts.Id;
                    int cnt = DescriptionsList.Where(s => s.TitleTagId == Idx).Count();
                    if (cnt == 0)
                    {
                        int descid = InsertUpdateDescription(shorts.Directory, Idx);
                        if (descid != -1)
                        {
                            string s = "UPDATE SHORTSDIRECTORY SET DESCID = @DESCID WHERE ID = @ID;";
                            connectionString.ExecuteScalar(s, [("@DESCID", descid), ("@ID", Idx)]);
                            shorts.DescId = descid;
                        }
                    }
                    else if (cnt == 1)
                    {
                        int did = DescriptionsList.Where(s => s.TitleTagId == Idx).First().Id;
                        if (did != shorts.DescId)
                        {
                            string s = "UPDATE SHORTSDIRECTORY SET DESCID = @DESCID WHERE ID = @ID;";
                            connectionString.ExecuteScalar(s, [("@DESCID", did), ("@ID", Idx)]);
                            shorts.DescId = did;
                        }
                    }
                }
                foreach (var item in EditableshortsDirectoryList)
                {
                    if (item.DescId == 1)
                    {

                    }


                }





            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void FilterActiveShortsDirlectoryList()
        {
            try
            {
                int id = -1;
                foreach (var t in SelectedShortsDirectoriesList.Where(t => t.IsShortActive && t.NumberOfShorts == 0))
                {
                    id = t.Id.ToInt(-1);
                }
                string sql = "DELETE FROM MULTISHORTSINFO WHERE ID = @ID;";
                if (id != -1)
                {
                    foreach (var t in SelectedShortsDirectoriesList.Where(t => t.IsShortActive && t.NumberOfShorts == 0))
                    {
                        if (id != t.Id.ToInt(-1))
                        {
                            SelectedShortsDirectoriesList.Remove(t);
                            connectionString.ExecuteNonQuery(sql, [("@ID", id)]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public string GetUploadReleaseBuilderSql(int index = -1)
        {
            try
            {
                // SHORTSDIRECTORY
                return "SELECT S.ID, S.DIRECTORYNAME, S.TITLEID, S.DESCID, " +
                       "(SELECT LIST(TAGID, ',') FROM TITLETAGS " +
                       " WHERE GROUPID = S.TITLEID) AS LINKEDTITLEIDS, " +
                       " (SELECT LIST(ID,',') FROM DESCRIPTIONS " +
                       "WHERE TITLETAGID = S.DESCID) AS LINKEDDESCIDS " +
                       "FROM SHORTSDIRECTORY S" +
                (index != -1 ? $" WHERE U.ID = {index} " : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetUploadReleaseBuilderSql {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }

        public void AddRecord(bool IsElapsed, bool Is720P, bool IsShorts, int IsCreateShorts,
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
                    jp.IsDownloads = false;
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
                        if (IsCreateShorts != -1) ScriptType = 4;
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
                        JobListDetails InMemoryJob = new JobListDetails(false, Title, SourceIndex++,
                            iddx, ScriptFile,
                            ScriptType, false, true,
                            IsPersistantSource, IsAdobe, IsShorts,
                            IsCreateShorts, "", ismuxed, muxdata);
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
                            IsTrimEncode, IsCutEncode, IsDeleteMonitored, IsPersistantSource, idx, ismuxed, muxdata, false);
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
                        if (IsCreateShorts != -1) ScriptType = 4;
                        if (IsCutEncode) ScriptType = 1;
                        if (IsTrimEncode) ScriptType = 3;
                        if (IsTwitchStream) ScriptType = 5;
                        string CutFrames = ((textstart != "") && (textduration != "")) ? $"|{textstart}|{Final.ToFFmpeg()}|time" : "";
                        string ScriptFile = $"true|{destFilename}|{sourcedirectory}|*.mp4{CutFrames}";
                        JobListDetails InMemoryJob = new JobListDetails(false, Title, SourceIndex++,
                            idx, ScriptFile, ScriptType,
                            false, true,
                            IsPersistantSource, IsAdobe, IsShorts, IsCreateShorts, "", ismuxed, muxdata);
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

                string exepath = GetExePath() + GetEncryptedString(new int[] { 170, 9, 70, 82, 244, 200,
                    233, 70, 251, 51, 11, 165, 214 }.Select(i => (byte)i).ToArray());
                var clt = GetEncryptedString(new int[] { 170, 57, 77, 85, 253, 206, 203, 93, 230, 51, 9, 173, 216 }.Select(i => (byte)i).ToArray());
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
                    ClientLibrary = GetExePath() + clt
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

        public bool IsClosed = false, IsClosing = false;
        public MainWindow(OnFinish DoOnFinish, bool IsReloaded = false, object formsList = null)
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
                ObservableCollectionFilter = new ObservableCollectionFilters();
                MainWindowX.KeyDown += Window_KeyDown_EventHandler;
                Loadsettings();
                string clientSecret = "", p = "";
                connectionString = GetConectionString();
                CreateDBInvoker();
                connectionString.ExecuteReader("SELECT * FROM SETTINGS WHERE SETTINGNAME = 'CLIENT_SECRET';", (FbDataReader r) =>
                {
                    clientSecret = (r["SETTINGBLOB"] is byte[] res) ? Encoding.ASCII.GetString(CryptData(res)) : "";
                });

                if (clientSecret == "")
                {
                    var lines = File.ReadAllText(GetExePath() + "\\client_secrets.json");
                    byte[] details = CryptData(Encoding.ASCII.GetBytes(lines));
                    string sql = $"INSERT INTO SETTINGS (SETTINGNAME, SETTINGBLOB) VALUES (@FIELD, @DATA) RETURNING ID;";
                    int id = connectionString.ExecuteScalar(sql, [("@FIELD", "CLIENT_SECRET"), ("@DATA", details)]).ToInt(-1);
                }






                var x = GetEncryptedString(new int[] { 151, 41, 70, 82, 244, 202,
                    219, 75, 205, 126, 1, 168, 154, 153, 87, 125 }.Select(i => (byte)i).ToArray());

                this.httpClientFactory = httpClientFactory;
                KillOrphanProcess().SafeFireAndForget();
                KillOrphanProcess(x).SafeFireAndForget();
                var ffm = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                var ffp = GetEncryptedString(new int[] { 170, 57, 73, 70, 227, 200,
                    204, 86, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                var e = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192,
                    142, 92, 224, 61, 11, 167, 196, 142, 64,
                    122, 60, 190, 181, 229, 163, 210, 204, 44, 84, 56, 244, 241,
                    35, 228, 106, 174, 61, 254 }.Select(i => (byte)i).ToArray());
                var isok = SanityCheck().GetAwaiter().GetResult();
                if (!isok)
                {
                    string err = e;
                    err.WriteLog();
                    string AppPath = GetExePath();
                    //DeleteIfExists(AppPath + ffp);
                    //DeleteIfExists(AppPath + ffm);
                }
                lstBoxJobs.ItemsSource = ProcessingJobs;
                SetupHandlers().ConfigureAwait(false);
                //Task.Run(() => Loadsettings());
                ParseAndDeleteLogs().ConfigureAwait(false);
                CheckForGraphicsCard();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                totaltasks = key.GetValueInt("maxthreads", 10);
                total1440ptasks = key.GetValueInt("max1440pthreads", 1);
                total4kTasks = key.GetValueInt("max4Kthreads", 3);// 1 for laptop. 2 for desktop
                UseFisheyeRemoval = key.GetValueBool("Fisheye", false);
                LRF = key.GetValueFloat("FisheyeRemoval_LRF", 0.5f);
                LLC = key.GetValueFloat("FisheyeRemoval_LLC", 0.5f);
                RRF = key.GetValueFloat("FisheyeRemoval_RRF", -0.335f);
                RLC = key.GetValueFloat("FisheyeRemoval_RLC", 0.097f);

                key?.Close();
                FileQueChecker = new System.Windows.Forms.Timer();
                FileQueChecker.Tick += new EventHandler(FileQueChecker_Tick);
                FileQueChecker.Interval = (int)new TimeSpan(0, 0, 1).TotalMilliseconds;
                Task.Run(async () => SetupThreadProcessorAsync());
                Thread.Sleep(100);
                SetupTicker();
                Task.Run(async () => { await ClearLogs(); });

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



        public async Task ClearLogs()
        {
            try
            {
                // 01_02_2022
                string exePath = GetExePath();
                var files = Directory.EnumerateFiles(exePath, "*.log").ToList();//ForEach(f =>
                foreach (string fr in files)
                {
                    string f = Path.GetFileNameWithoutExtension(fr);
                    if (f.Contains("_") && f.Length > 10)
                    {
                        //dd_mm_yyyy
                        int day = f.Substring(0, 2).ToInt(-1);
                        int month = f.Substring(3, 2).ToInt(-1);
                        int year = f.Substring(6, 4).ToInt(-1);
                        if (day == -1 || month == -1 || year == -1) continue;
                        DateTime dt = new DateTime(year, month, day);
                        if (DateTime.Now.Date.Subtract(dt).Days > 0)
                        {
                            File.Delete(fr);
                        }
                    }
                }
                ;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ClearLogs {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public byte[] CryptData(byte[] _password)
        {
            int[] AccessKey = { 32, 16, 22, 157, 214, 12, 138, 249, 133, 244, 116, 28, 99, 00, 111, 131, 17, 174, 21,
                88, 99, 33, 44, 166, 88, 99, 100, 11, 232, 157, 74, 1, 28, 39, 33, 244, 166, 88, 99, 100,
                14, 132, 157, 74, 123, 28, 49, 233, 144, 166, 188, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 122, 244, 162, 232, 133, 222, 127, 141, 244, 136, 172, 223, 132, 233, 125, 126 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return encvar;
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
                var ffm = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                var ffp = GetEncryptedString(new int[] { 170, 57, 73, 70, 227, 200, 204, 86, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                await Cli.Wrap(appPath + ffp).
                     WithArguments("-version").WithWorkingDirectory(appPath).
                      WithStandardErrorPipe(PipeTarget.ToStringBuilder(StdErr)).
                      WithStandardOutputPipe(PipeTarget.ToStringBuilder(StdOut)).
                      ExecuteAsync().ConfigureAwait(false);
                StdErr.Clear();
                StdOut.Clear();
                var v = GetEncryptedString(new int[] { 219, 41, 74, 68, 226, 206, 193, 93 }.Select(i => (byte)i).ToArray());
                StringBuilder StdErr2 = new StringBuilder();
                StringBuilder StdOut2 = new StringBuilder();
                await Cli.Wrap(appPath + ffm).
                    WithArguments(v).WithWorkingDirectory(appPath).
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
                        var s = GetEncryptedString(new int[] { 165, 26, 99, 115, 210, 243,
                            142, 25, 178, 91, 63, 142, 249, 220, 120, 113, 55, 173, 206, 201,
                            134, 206, 205, 105, 23, 91, 223, 250, 59, 243, 113, 171, 63, 252, 103 }.Select(i => (byte)i).ToArray());
                        lstboxresize();
                        key.Close();
                        var vp = GetEncryptedString(new int[] { 160, 54, 75, 83, 254, 247, 220, 92, 241, 120, 30, 178, 219, 142 }.Select(i => (byte)i).ToArray());
                        var dd = GetEncryptedString(new int[] { 178, 58, 92, 85, 227, 206, 222, 71, 251, 114, 3 }.Select(i => (byte)i).ToArray());
                        if (selectedcard != "")
                        {
                            Video = selectedcard;
                            lbAccelHW.AutoSizeLabel(Video);
                            ManagementObjectSearcher searcher = new(s);
                            foreach (ManagementObject mo in searcher.Get())
                            {
                                PropertyData description = mo.Properties[dd];
                                PropertyData VideoProcessor = mo.Properties[vp];
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
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory720p = key.GetValueStr("SourceDirectory720p", string.Empty);
                string SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);
                key?.Close();
                SelectFiles(SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe);
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
                        foreach (var v in ProcessingJobs.Where(v => v.FileNoExt == record))
                        {
                            Thread.Sleep(100);
                            v.Fileinfo = filename;
                            string pr = record + "|" + v.Fileinfo;
                            found = true;
                            break;
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
        public void UpdateSpeeds(string filename, float framess, float totalb, int framecalc, string frames1440p)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    lock (statslocker)
                    {
                        Stats_Handler.UpdateSpeed(filename, framess, totalb, framecalc, frames1440p);
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
            return new string[] { ".avi", ".mkv", ".mp4", ".m2ts" };
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
                });
                LineNum = 1;
                string TwitchStreamKey = string.Empty;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                DestDirectory720p = key.GetValueStr("DestDirectory720p");
                DestDirectory720p.CreatePathIfNotExists();
                DestDirectory1440p = key.GetValueStr("DestDirectory1440p");
                DestDirectory1440p.CreatePathIfNotExists();
                DestDirectoryTwitch = key.GetValueStr("DestDirectoryTwitch");
                DestDirectoryTwitch.CreatePathIfNotExists();
                DestDirectory4K = key.GetValueStr("DestDirectory4k");
                DestDirectory4K.CreatePathIfNotExists();
                TwitchStreamKey = key.GetValueStr("TwitchStreamKey", "live_1061414984_Vu5NrETzHYqB1f4bZO12dxaCOfUkxf");
                DestDirectoryAdobe4K = key.GetValueStr("DestDirectoryAdobe4k");
                DestDirectoryAdobe4K.CreatePathIfNotExists();
                DoneDirectory720p = key.GetValueStr("CompDirectory720p");
                DoneDirectory720p.CreatePathIfNotExists();
                DoneDirectory1440p = key.GetValueStr("CompDirectory1440p");
                DoneDirectory1440p.CreatePathIfNotExists();
                DoneDirectory4K = key.GetValueStr("CompDirectory4K");
                DoneDirectory4K.CreatePathIfNotExists();
                DoneDirectoryAdobe4K = key.GetValueStr("CompDirectory4KAdobe");
                DoneDirectory4K.CreatePathIfNotExists();
                SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p");
                SourceDirectory1440p.CreatePathIfNotExists();
                SourceDirectory720p = key.GetValueStr("SourceDirectory720p");
                SourceDirectory720p.CreatePathIfNotExists();
                SourceDirectory4K = key.GetValueStr("SourceDirectory4K");
                bool ChkAutoAAC_IsChecked = key.GetValueBool("ChkAutoAAC", true);
                bool ChkReEncode_IsChecked = key.GetValueBool("reencodefile", true);
                SourceDirectoryAdobe4K = key.GetValueStr("SourceDirectory4KAdobe");
                SourceDirectoryAdobe4K.CreatePathIfNotExists();
                ErrorDirectory = key.GetValueStr("ErrorDirectory");
                ErrorDirectory.CreatePathIfNotExists();
                totaltasks = key.GetValueInt("maxthreads", 10);
                total1440ptasks = key.GetValueInt("max1440pthreads", 1);
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
                    if (!this.IsVisible && !this.InTray)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            trayicon.Visibility = Visibility.Visible;
                            this.InTray = true;
                        });
                    }
                    int xcnt1440p = 0, xcnt4K = 0, xcnt = 0;
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
                                (ProcessingJobs[xp].Is1440p || ProcessingJobs[xp].IsTwitchOut) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt1440p++;
                            }
                            if ((ProcessingJobs[xp] is not null) && (ProcessingJobs[xp].Handle == "") &&
                                (ProcessingJobs[xp].Is720P) && (!ProcessingJobs[xp].InProcess) && (!ProcessingJobs[xp].IsSkipped))
                            {
                                xcnt++;
                            }
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            trayicon.ToolTipText = "Idle";
                        });
                    }

                    if (xcnt == 0 && xcnt1440p == 0 && xcnt4K == 0 && ProcessingJobs.Count == 0)
                    {
                        Thread.Sleep(500);
                        continue;
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
                    if (xcnt1440p > 0)
                    {
                        if (ProcessingJobs.Where(s => (s.Is1440p || s.IsTwitchOut) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(total1440ptasks).Count() > 0)
                        {
                            NewProcessingListTemp.AddRange(ProcessingJobs.Where(s => (s.Is1440p || s.IsTwitchOut) && (!s.Fileinfo.IsNullStr().Contains("Allready Processed")) && (s.Handle == "" || s.Handle is null) && (!s.Processed) && (!s.IsSkipped)).Take(total1440ptasks));
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
                            var x = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                            Process[] psa = Process.GetProcessesByName(x);
                            List<string> ProcessIDs = (psa.Select(pid => pid.Id.ToString())).ToList();
                            if (Job.Handle.ContainsAny(ProcessIDs))
                            {
                                Thread.Sleep(250);
                                continue;
                            }
                            LineNum = 7;
                            var r = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                            while (Process.GetProcessesByName(r).Count() > 0)
                            {
                                //CountFFMPEGs(NewProcessingList);
                                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                int t720p = key2.GetValueInt("maxthreads", 5);
                                int t1440p = key2.GetValueInt("max1440pthreads", 2);
                                totaltasks = key2.GetValueInt("maxthreads", 10);
                                total1440ptasks = key2.GetValueInt("max1440pthreads", 1);
                                total4kTasks = key2.GetValueInt("max4Kthreads", 3);// 1 for laptop. 2 for desktop
                                key2?.Close();
                                t720p = totaltasks;
                                totaltasks = (_4KFiles.Count > 0) ? 4 : t720p;
                                total1440ptasks = (_4KFiles.Count > 0) ? 1 : t1440p;
                                LineNum = 8;
                                foreach (var j in ProcessingJobs.Where(j => _4KFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _4KFiles.Remove(j.SourceFile);
                                }
                                foreach (var j in ProcessingJobs.Where(j => _1440pFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _1440pFiles.Remove(j.SourceFile);
                                }
                                foreach (var j in ProcessingJobs.Where(j => _720PFiles.Contains(j.SourceFile)).Where(j => j.Complete))
                                {
                                    _720PFiles.Remove(j.SourceFile);
                                }
                                //(Stats_Handler.count720p, Stats_Handler.count1440p, Stats_Handler.count4k)
                                if (((Job.Is1440p || Job.IsTwitchOut) && (_1440pFiles.Count >= total1440ptasks)) ||
                                    ((Job.Is4K || Job.IsMuxed) && (_4KFiles.Count >= total4kTasks)) ||
                                    ((!Job.Is720P) && (_720PFiles.Count >= totaltasks)))
                                {
                                    Thread.Sleep(200);
                                    continue;
                                }
                                if ((Job.Is1440p || Job.IsTwitchOut) && (_1440pFiles.Count < total1440ptasks))
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
                                    connectionString.ExecuteScalar(sql.ToUpper(), [("p0", SourceDir)]);
                                    break;
                                }
                                else continue;
                            }
                            var tt = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                            if ((!Job.IsTwitchStream && Job.twitchschedule.HasValue) && (Process.GetProcessesByName(tt).Count() == 0) && (NewProcessingList.Count > 1))
                            {
                                while (true && NewProcessingList.Count > 1)
                                {
                                    if (((Job.Is1440p || Job.IsTwitchOut) && (_1440pFiles.Count >= total1440ptasks)) ||
                                       ((Job.Is4K || Job.IsMuxed) && (_4KFiles.Count >= total4kTasks)) ||
                                     ((!Job.Is720P) && (_720PFiles.Count >= totaltasks)))
                                    {
                                        Thread.Sleep(100);
                                        continue;
                                    }
                                    if ((Job.Is1440p || Job.IsTwitchOut) && (_1440pFiles.Count < total1440ptasks))
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

                            LineNum = 13;
                            string DestFile = "", SourceDirectory = Job.SourcePath, sep = Job.X264Override ? "\\x264\\" : "\\";
                            DestFile = (Job.IsTwitchStream && !Job.twitchschedule.HasValue) ? DestDirectoryTwitch : (Job.Is4KAdobe) ? DestDirectoryAdobe4K : (Job.Is4K) ? DestDirectory4K : (Job.Is1440p) ? DestDirectory1440p : DestDirectory720p;
                            if (Job.IsMuxed)
                            {
                                string dfile = Path.GetDirectoryName(zprocessingfile);
                                string ff = dfile.Split('\\').ToList().LastOrDefault();
                                string newd = dfile.Replace(ff, "Filtered");
                                DestFile = Path.Combine(newd, Path.GetFileNameWithoutExtension(zprocessingfile) + ".mp4");
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



                            if (ChkReEncode_IsChecked && (File.Exists(DestFile)))
                            {
                                bool doSwap = false;
                                doSwap = ((ChkAutoAAC_IsChecked) && (!DestFile.EndsWith("[AAC].mkv"))) || doSwap;
                                doSwap = ((!ChkAutoAAC_IsChecked) && (!DestFile.EndsWith("[NEW].mkv"))) || doSwap;
                                if (doSwap)
                                {
                                    DestFile = ChkAutoAAC_IsChecked ? DestFile.Replace(".mkv", "[AAC].mkv") : DestFile.Replace(".mkv", "[NEW].mkv");
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
                                string dDir = (Job.Is4K) ? DoneDirectory4K : (Job.Is1440p) ? DoneDirectory1440p : DoneDirectory720p;
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
                    var ffm = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                    Process[] ps1 = Process.GetProcessesByName(ffm);
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

                        if (System.IO.File.Exists(processingfile) || Job.Handle != "")
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
                            DestFile = (Job.IsTwitchStream) ? DestDirectoryTwitch : (Job.Is4K) ? DestDirectory4K : (Job.Is1440p) ? DestDirectory1440p : DestDirectory720p;
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
                                    if (Job.Is1440p)
                                    {
                                        LineNum = 38;
                                        if ((Job.SourcePath != DestDirectory1440p) && (Job.SourcePath != DoneDirectory1440p))
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
                            string JobHandle = "", srfilex = (Job.IsMuxed) ? Job.MultiSourceDir : Job.SourcePath + "\\" + Job.FileNoExt + Job.FileExt;
                            if (Job.IsMulti && !Job.IsMuxed) srfilex = Job.DestMFile;
                            LineNum = 45;
                            if (!Job.IsMulti)
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
                string SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                key?.Close();
                var ff1 = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 86, 234, 120 }.Select(i => (byte)i).ToArray());
                var ff = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                foreach (var proc in Procx.Where(proc => proc.MainModule.ModuleName.Contains(ff)))
                {
                    bool fnd = false;
                    string myStrQuote = "\"";
                    var fft = GetEncryptedString(new int[] { 219, 57, 15, 85, 254, 201, 205, 82,
                        230, 61, 64, 178, 213, 154, 74, 56, 105, 190, 209, 255 }.Select(i => (byte)i).ToArray());
                    var cl = GetEncryptedString(new int[] { 181, 48, 66, 91, 240, 201, 202, 127, 251, 115, 8 }.Select(i => (byte)i).ToArray());
                    var h = GetEncryptedString(new int[] { 190, 62, 65, 82, 253, 194 }.Select(i => (byte)i).ToArray());
                    var x = GetEncryptedString(new int[] { 165, 26, 99, 115, 210, 243, 142, 25, 178, 91, 63, 142, 249, 220, 120, 113, 55, 173,
                        206, 201, 128, 213, 198, 111, 29, 107, 195, 180, 56, 233, 123, 181, 54, 185, 123, 229, 249, 81 }.Select(i => (byte)i).ToArray());
                    ManagementObjectSearcher searcher = new(x + $" = {myStrQuote}{ff1}{myStrQuote}");
                    foreach (ManagementObject o in searcher.Get())
                    {
                        string HandleID = o.Properties[h].Value.ToString();
                        if (o["CommandLine"] != null)
                        {
                            string comstr = o[cl].ToString(), lookupstr = fft;
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
                                            if (p.Is1440p) Stats_Handler.count1440p++;
                                            if (p.Is1440p || p.Is4K) break;
                                        }
                                    }
                                }
                            }
                            if (comstr.ToLower().Contains(SourceFile.ToLower()))
                            {
                                if (comstr.Contains(SourceDirectory1440p)) Stats_Handler.count1440p++;
                                if (comstr.Contains(SourceDirectory4K)) Stats_Handler.count4k++;
                                if (comstr.Contains(SourceDirectory720p))
                                {
                                    Stats_Handler.count720p++;
                                }
                            }
                            var r = GetEncryptedString(new int[] { 219, 60, 15, 85, 254, 215, 215, 19, 191,
                                123, 77, 175, 193, 144, 67, 56, 54, 235, 136, 230,
                                165, 211, 135, 97, 19, 110 }.Select(i => (byte)i).ToArray());
                            if (!comstr.Contains(r))
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
                                var t = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                                Process[] psa = Process.GetProcessesByName(t);
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
                                List<string> CompleedFiles = Directory.EnumerateFiles(DoneDirectory1440p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupCompleted}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                    //if (job)
                                    double TotalSecs = 0;
                                    List<string> Files = new List<string>();
                                    ffmpegbridge FileIndexer = new ffmpegbridge();
                                    FileIndexer.ReadDuration(source);
                                    while (!FileIndexer.Finished)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    TotalSecs = FileIndexer.GetDuration().TotalSeconds;
                                    FileIndexer = null;


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

                                    }
                                }
                            }
                            if (backupDone1440p != "")
                            {
                                List<string> CompleedFiles = Directory.EnumerateFiles(DestDirectory1440p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupDone1440p}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                }
                            }
                            if (backupDone != "")
                            {
                                List<string> CompleedFiles = Directory.EnumerateFiles(DestDirectory1440p, "*.*", SearchOption.AllDirectories).ToList<string>();
                                foreach (string source in CompleedFiles)
                                {
                                    string Destination = $"////{backupserver}{backupDone}//" + Path.GetFileName(source);
                                    await Extensions.CopyFiles(backupserver, source, Destination, un, up, progress);
                                }
                            }
                        }
                    }
                }
                var ffm = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray());
                foreach (var Jobentry in ProcessingJobs.Where(jobentry => jobentry.Handle != ""))
                {
                    string filename = Jobentry.FileNoExt;
                    Process[] psa = Process.GetProcessesByName(ffm);
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
        private void SpeedUpdate(string bitrate, string bitratespeed, string fps, string frames, string frames1440p)
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
                        lblCurrentFrames.AutoSizeLabel(frames1440p);
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
                        var f = GetEncryptedString(new int[] { 160, 54, 75, 83, 254, 224, 251, 122, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                        string AppPath = GetExePath() + f;
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
                key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                List<string> Profiles = new();
                List<string> LogFiles = new List<string>();
                string searchpath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                this.Dispatcher.Invoke(() =>
                {
                    LogFiles = Directory.EnumerateFiles(searchpath, "*-log.log", SearchOption.AllDirectories).ToList<string>();
                });
                foreach (string logfile in LogFiles)
                {
                    if (logfile.Contains("."))
                    {
                        string s = Path.GetFileNameWithoutExtension(logfile);
                        string date = s.Substring(0, s.Length - 4);
                        DateTime logDate = DateTime.ParseExact(date, "dd_MM_yyyy", CultureInfo.InvariantCulture);
                        TimeSpan LogDateSpan = logDate - DateTime.Now;
                        if (LogDateSpan.TotalDays > 5)
                        {
                            System.IO.File.Delete(logfile);
                        }
                    }
                }
                LineNum = 13;
                this.Dispatcher.Invoke(() => { InitTitleLength = MainWindowX.Title.Length; });
                var values = key.GetValueNames();
                if (values is not null)
                {

                }



                txtDestPath = key.GetValueStr("DestDirectory", defaultdrive);
                txtDestPath.CreatePathIfNotExists();
                txtDonepath = key.GetValueStr("CompDirectory", defaultdrive);
                txtDonepath.CreatePathIfNotExists();
                txtErrorPath = key.GetValueStr("ErrorDirectory", defaultdrive);
                txtErrorPath.CreatePathIfNotExists();
                backupserver = key.GetValueStr("backupserver", "127.0.0.1");
                backupDone1440p = key.GetValueStr("1440pDoneBackupPath", "");
                backupDone1440p.CreatePathIfNotExists();
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

                if (!LoadConverterSettingsFromDB() && key.RegistryValueExists("BitRateSettings"))
                {
                    string SelProfile = key.GetValueStr("SelectedProfile", string.Empty);
                    Profiles.AddRange(key.GetValueStrs("BitRateSettings"));
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
                                    // Insert

                                    string SQL = "INSERT INTO CONVERTERSETTINGS (" +
                                        "ISDEFAULT,MINBITRATE,MAXBITRATE,BITRATEBUFFER," +
                                        "VIDEOWIDTH,VIDEOHEIGHT,ARMODULAS,RESIZEENABLE," +
                                        "ARROUNDENABLE,ARSCALINGENABLED,VSYNCEABLE) " +
                                        "VALUES(@ISDEFAULT,@MINBITRATE,@MAXBITRATE,@BITRATEBUFFER," +
                                        "@VIDEOWIDTH,@VIDEOHEIGHT,@ARMODULAS,@RESIZEENABLE," +
                                        "@ARROUNDENABLE,@ARSCALINGENABLED,@VSYNCEABLE)";
                                    connectionString.ExecuteScalar(SQL, [("@ISDEFAULT", true),
                                        ("@MINBITRATE", MinBitRate), ("@MAXBITRATE", MaxBitRate),
                                        ("@BITRATEBUFFER", BitRateBuffer), ("@VIDEOWIDTH", VideoWidth),
                                        ("@VIDEOHEIGHT", VideoHeight), ("@ARMODULAS", ArModulas),
                                        ("@RESIZEENABLE", ResizeEnable), ("@ARROUNDENABLE", ArRoundEnable),
                                        ("@ARSCALINGENABLED", ArScalingEnabled), ("@VSYNCEABLE", VSyncEnable)]);
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
                            string SQL = "INSERT INTO CONVERTERSETTINGS (" +
                                       "ISDEFAULT,MINBITRATE,MAXBITRATE,BITRATEBUFFER," +
                                       "VIDEOWIDTH,VIDEOHEIGHT,ARMODULAS,RESIZEENABLE," +
                                       "ARROUNDENABLE,ARSCALINGENABLE,VSYNCEABLE)" +
                                       "VALUES(@ISDEFAULT,@MINBITRATE,@MAXBITRATE,@BITRATEBUFFER," +
                                       "@VIDEOWIDTH,@VIDEOHEIGHT,@ARMODULAS,@RESIZEENABLE," +
                                       "@ARROUNDENABLE,@ARSCALINGENABLE,@VSYNCEABLE)";
                            connectionString.ExecuteScalar(SQL, [("@ISDEFAULT", true),
                                        ("@MINBITRATE", MinBitRate), ("@MAXBITRATE", MaxBitRate),
                                        ("@BITRATEBUFFER", BitRateBuffer), ("@VIDEOWIDTH", VideoWidth),
                                        ("@VIDEOHEIGHT", VideoHeight), ("@ARMODULAS", ArModulas),
                                        ("@RESIZEENABLE", ResizeEnable), ("@ARROUNDENABLE", ArRoundEnable),
                                        ("@ARSCALINGENABLE", ArScalingEnabled), ("@VSYNCEABLE", VSyncEnable)]);
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

                this.SetChecked("GPUEncode", key.GetValueBool("GPUEncode", true));
                this.SetChecked("Fisheye", key.GetValueBool("FishEyeRemoval", false));

                this.SetChecked("ChkResize1440p", key.GetValueBool("resize1440p"));

                this.SetChecked("X265Output", key.GetValueBool("X265", true));
                this.SetChecked("ChkAudioConversion", key.GetValueBool("AudioConversionAC3", true));

                this.SetSelectedIndex("cmbH64Target", key.GetValueInt("h264Target", -1));
                this.SetChecked("ChkResize1080shorts", key.GetValueBool("Do1440pShorts", false));

                LineNum = 24;
                SystemSetup = false;
                key.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadSettings LineNum {LineNum} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public bool LoadConverterSettingsFromDB()
        {
            try
            {
                bool result = false;
                CancellationTokenSource cts = new();
                string sql = "SELECT * FROM CONVERTERSETTINGS WHERE ISDEFAULT = @ISDEFAULT";
                connectionString = GetConectionString();
                connectionString.ExecuteReader(sql, [("@ISDEFAULT", true)], cts,
                    (FbDataReader r) =>
                    {
                        result = true;
                        MinBitRate = r["MINBITRATE"] is string minbr ? minbr : "675K";
                        MaxBitRate = r["MAXBITRATE"] is string maxbr ? maxbr : "1150K";
                        BitRateBuffer = r["BITRATEBUFFER"] is string bb ? bb : "1200K";
                        VideoWidth = r["VIDEOWIDTH"] is string vw ? vw : "720";
                        VideoHeight = r["VIDEOHEIGHT"] is string vh ? vh : "";
                        ArModulas = r["ARMODULAS"] is string ar ? ar : "16";
                        ResizeEnable = r["RESIZEENABLE"] is Int16 RS ? RS == 1 : false; ;
                        ArRoundEnable = r["ARROUNDENABLE"] is Int16 AE ? AE == 1 : false;
                        ArScalingEnabled = r["ARSCALINGENABLED"] is Int16 SE ? SE == 1 : false;
                        VSyncEnable = r["VSYNCEABLE"] is Int16 VE ? VE == 1 : false;
                        cts.Cancel();
                    });

                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LoadConverterSettingsFromDB {MethodBase.GetCurrentMethod().Name} {ex.Message} {ex.StackTrace}");
                return false;
            }
        }
        private bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "1.1.1.1";
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
                string SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p", string.Empty);
                string SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);

                key?.Close();
                SourceList.Clear();
                if (SourceDirectory720p != string.Empty)
                {
                    string DownloadsDir = GetDownloadsFolder();

                    List<string> SourceDirs = new List<string>() { SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe };
                    bool IsPrometheus = Environment.MachineName.ToLower().Contains("prometheus");
                    if (!IsPrometheus)
                    {
                        SourceDirs.Insert(0, DownloadsDir);
                    }

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

                            if (ProcessingJobs.Count(job => job.Title == myfilename) == 0)
                            {
                                bool Is1440p = false;
                                bool Is4K = SourceDir.SourceIs4K();

                                bool IsAdobe = SourceDir.SourceIs4KAdobe();
                                string dDir = (Is4K) ? DestDirectory4K : (Is1440p) ? DestDirectory1440p : DestDirectory720p;
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
                                            string doneDir = (Is4K) ? DoneDirectory4K : (Is1440p) ? DoneDirectory1440p : DoneDirectory720p;
                                            if (IsAdobe) doneDir = DoneDirectoryAdobe4K;
                                            string moveto = Path.Combine(doneDir, Path.GetFileName(filename));
                                            MoveIfExists(filename, moveto);
                                        }
                                    }


                                }
                                else AddFileOK = true;
                                if (AddFileOK) AddIfVaid(filename, SourceDir);
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
                if (ProcessingJobs.Count <= totaltasks + total1440ptasks) FileQueChecker.Interval = (int)new TimeSpan(0, 1, 0).TotalMilliseconds;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileQueChecker.Interval = (int)new TimeSpan(0, 1, 0).TotalMilliseconds;
                    FileQueChecker.Start();

                });
            }
        }


        private string GetCpuInfo()
        {
            try
            {
                string result = "";
                // Method 1: Using WMI (Windows Management Instrumentation)
                using (ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {

                        result = $"|{obj["Name"]}|{obj["Manufacturer"]}\n";
                        /*
                            $"Architecture: {obj["Architecture"]}\n" +
                            $"Number of Cores: {obj["NumberOfCores"]}\n" +
                            $"Clock Speed: {obj["MaxClockSpeed"]} MHz";*/
                    }
                }

                /*// Method 2: Using Environment (simpler but less detailed)
                string processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                string processorIdentifier = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

                CpuInfoText.Text += $"\n\nEnvironment Variables:\n" +
                    $"Processor Architecture: {processorArchitecture}\n" +
                    $"Processor Identifier: {processorIdentifier}";*/
                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
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

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                int _cmbaudiomodeSelectedIndex = key.GetValueInt("Audiomode", 0);
                int H264Target = key.GetValueInt("h264Target", -1);
                bool ChkAutoAAC_IsChecked = key.GetValueBool("ChkAutoAAC", true);
                bool _GPUEncode = this.IsChecked("GPUEncode"), _X265Output = this.IsChecked("X265Output");
                key?.Close();
                string myStrQuote = "\"";
                (chkresized, overrider, job.InProcess) = (ResizeEnable, job.X264Override, true);
                Video = CheckForGraphicsCard();
                lbAccelHW.AutoSizeLabel(Video);
                LineNum = 1;
                ffmpeg.VideoCodec Encoder = ffmpeg.VideoCodec.h264;
                string HWInfo = GetCpuInfo();
                HardwareAccelerator HardwareAcceleration = HardwareAccelerator.software;// "qsv";
                if (HWInfo.ToLower().Contains("intel")) HardwareAcceleration = HardwareAccelerator.qsv;
                if (HWInfo.ToLower().Contains("amd")) HardwareAcceleration = HardwareAccelerator.amf;
                if (Video.Contains("AMD") || Video.Contains("Radeon"))
                {
                    HardwareAcceleration = HardwareAccelerator.amf;
                }
                if (Video.Contains("NVIDIA") && !HWInfo.ToLower().Contains("intel"))
                {
                    HardwareAcceleration = HardwareAccelerator.cuda;
                }
                if (_GPUEncode)
                {
                    if ((!overrider) && (_X265Output))
                    {
                        if (Video.Contains("NVIDIA")) Encoder = ffmpeg.VideoCodec.hevc_nvenc;
                        else if (Video.Contains("AMD") || (Video.Contains("Radeon"))) Encoder = ffmpeg.VideoCodec.hevc_amf;
                    }
                    else
                    {
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
                    }
                }
                else
                {
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
                }
                string AppPath = GetExePath();
                double totalseconds = 0;
                //SourceFile = SourceFile.Contains(" ") ? myStrQuote + SourceFile + myStrQuote : SourceFile;
                try
                {
                    if (job.IsMulti && !job.IsMuxed)
                    {
                        List<string> Files = new List<string>();
                        string filename = "";
                        foreach (string sp in job.GetCutList())
                        {
                            filename = sp.Replace("file '", "").Replace("'", "");
                            Files.Add(filename);
                        }
                        ffmpegbridge FileIndexer = new ffmpegbridge();
                        FileIndexer.ReadDuration(Files);

                        while (!FileIndexer.Finished)
                        {
                            Thread.Sleep(100);
                        }
                        List<(string, double)> FileInfos = new List<(string, double)>();
                        FileInfos.AddRange(FileIndexer.FileInfoList);
                        totalseconds = FileIndexer.GetDuration().TotalSeconds;
                        job.TotalSeconds = totalseconds;
                        // Add Get List of files and durations 
                        FileIndexer = null;

                        job.TotalSeconds = totalseconds;
                        bool fnx = false;
                        string srcdir = job.MultiSourceDir.Split('\\').ToList().LastOrDefault();

                        foreach (var _ in SourceFileInfos.Where(imt => imt.SourceDirectory == srcdir).Select(imt => new { }))
                        {
                            fnx = true;
                            break;
                        }
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
                        ffmpegbridge FileIndexer = new ffmpegbridge();
                        FileIndexer.ReadDuration(SourceFile);
                        while (!FileIndexer.Finished)
                        {
                            Thread.Sleep(100);
                        }
                        totalseconds = FileIndexer.GetDuration().TotalSeconds;
                        FileIndexer = null;
                    }
                }
                catch (Exception ex1)
                {
                    if (job.IsMulti && !job.IsMuxed)
                    {
                        List<string> Cuts = job.GetCutList();
                        List<string> Files = Cuts.Select(sp => sp.Replace("file ", "").Replace("'", "").Trim()).ToList();
                        string dstfn = "", srcfile, destfn = Path.GetFileNameWithoutExtension(DestFile);
                        foreach (string pr in Files)
                        {
                            dstfn = ErrorDirectory + "\\" + destfn + "_" + Path.GetFileName(pr);
                            MoveIfExists(pr, dstfn);
                        }
                        dstfn = ErrorDirectory + "\\" + Path.GetFileName(job.MultiFile);
                        MoveIfExists(job.MultiFile, dstfn);
                    }
                    else
                    {
                        SourceFile = SourceFile.Contains(" ") ? myStrQuote + SourceFile + myStrQuote : SourceFile;
                        MoveIfExists(SourceFile, ErrorDirectory + "\\" + Path.GetFileName(SourceFile));
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
                List<TextStream> textStreams = new List<TextStream>();
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
                    if ((job.PosMode is string mode) && (mode.ToLower() == "time")) // CET MOD   
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
                    (videoStream, audioStream, textStreams, Dur) = bridge.ReadMediaFile(Files[0]);
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
                    (videoStream, audioStream, textStreams, Dur) = bridge.ReadMediaFile(SourceFile);
                    if (audioStream.Channels == 0)
                    {
                        (videoStream, audioStream, textStreams, Dur) = bridge.ReadMediaFile(SourceFile);
                    }
                    bridge = null;

                    if (videoStream.Codec is null)
                    {
                        job.IsSkipped = true;
                        job.Fileinfo = "Invalid File Format";
                        return;
                    }
                    LineNum = 51;
                    TotalSecs = Dur.TotalSeconds;
                    LineNum = 52;
                }
                LineNum = 53;
                bool HasText = textStreams.Count > 0;
                int TextStreamId = -1;
                if (HasText)
                {
                    int firststream = -1;
                    for (int i = 0; i < textStreams.Count; i++)
                    {
                        TextStream ts = textStreams[i];
                        if (i == 0 && ts.Id.HasValue)
                        {
                            firststream = ts.Id.Value;
                        }
                        if (firststream != -1 && ts.Language == "en" && ts.Id.HasValue &&
                            ts.Forced == 1 && ts.Default == 1)
                        {
                            TextStreamId = ts.Id.Value - firststream;
                            break;
                        }
                        if (firststream != -1 && ts.Id.HasValue && ts.Default == 1)
                        {
                            TextStreamId = ts.Id.Value - firststream;
                            break;
                        }
                    }
                }

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
                    if (ChkAutoAAC_IsChecked)
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

                    if (job.IsTwitchOut)
                    {
                        audioStream.CopyStream();
                        Encoder = ffmpeg.VideoCodec.h264_amf;
                        if (videoStream.Framerate > 25)
                        {
                            videoStream = videoStream.SetFPS(25.0f);
                        }
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
                            RegistryKey keys = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            int CropHeight = keys.GetValueInt("CropHeight", 1080);
                            int CropWidth = keys.GetValueInt("CropWidth", 608);
                            int CropLeft = keys.GetValueInt("CropLeft", 0);
                            int CropTop = keys.GetValueInt("CropTop", 0);
                            keys?.Close();
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
                                RegistryKey keyx = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                int CropHeight = keyx.GetValueInt("CropHeight", 1080);
                                int CropWidth = keyx.GetValueInt("CropWidth", 608);
                                int CropLeft = keyx.GetValueInt("CropLeft", 0);
                                int CropTop = keyx.GetValueInt("CropTop", 0);
                                keyx?.Close();
                                LineNum = 70;
                                if (videowidth > 1080)
                                {
                                    videoStream = videoStream.SetSize(1920, 1080, -1, -1, Scaling.lanczos, CropWidth, CropHeight, CropLeft, CropTop, "9/16");
                                }
                                LineNum = 71;
                            }

                            else if (job.Is5K)
                            {
                                videoStream = videoStream.SetSize(3840, 2160, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                            }
                        }
                        else if (job.Is4KAdobe)
                        {
                            videoStream = videoStream.SetSize(3840, 2160, aspectratio, ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                        }
                    }
                    else
                    {
                        LineNum = 74;
                        if (!job.Is1440p)
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
                            if ((job.Is1440p) && (videoStream.Width > 2560))
                            {
                                videoStream = videoStream.SetSize(2560, -1, aspectratio,
                                    ArModulas.ToInt(), Scaling.lanczos, 0, 0, 0, 0);
                            }
                            if ((job.Is1440p) && (videoStream.Framerate > 25))
                            {
                                videoStream = videoStream.SetFPS(25.0f);
                            }

                        }
                    }
                    LineNum = 76;
                    string currentpath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    bool mpeg2 = videoStream.Codec.ToString() != "mpeg2video";
                    if (!_GPUEncode)
                    {
                        LineNum = 77;
                        RegistryKey keyr = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        if (Encoder == ffmpeg.VideoCodec.libxvid)
                        {
                            LineNum = 78;
                            int minQ = keyr.GetValueInt("minq", 3), maxQ = key.GetValueInt("maxq", 13);
                            videoStream.Mpeg4ASP(minQ, maxQ);// ASP Set the video stream params here. -qmin 3 - qmax 5 - vtag = XVID -aspect 4:3
                        }
                        LineNum = 79;
                        if (Encoder == ffmpeg.VideoCodec.mpeg4)
                        {
                            LineNum = 80;
                            int qscale = keyr.GetValueInt("qscale", 15);
                            string vTag = keyr.GetValueStr("vTag", "XVID");
                            videoStream.Mpeg4AVC(qscale, vTag); // AVC / X264 -crf 18 , -preset slow -pix_fmt yuv420p
                        }
                        keyr?.Close();
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
                    if (job.Is1440p && job.IsInterlaced) samplesize = 16;
                    if ((job.Is1440p && !job.IsInterlaced) || (job.Is4K && !job.IsInterlaced))
                    {
                        samplesize = (!job.Is4K) ? 8.3M : 23.5M;// was 6.5M : 30M
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
                    if (job.IsTwitchOut)
                    {
                        samplesize = 5.2M;
                    }

                    string MMFile = (job.IsMulti) ? Path.GetFileName(job.DestMFile) : "";
                    string _MinBitRate = (MinRate > 0) ? Math.Round((decimal)MinRate * samplesize).ToString() + "K" : MinBitRate;
                    string _MaxBitRate = (MaxRate > 0) ? Math.Round((decimal)MaxRate * samplesize).ToString() + "K" : MaxBitRate;
                    string _BitRateBuffer = (RateBuffer > 0) ? Math.Round((decimal)RateBuffer * samplesize).ToString() + "K" : BitRateBuffer;
                    LineNum = 85;


                    LockedDeviceID = 0;
                    conversion.AddStream(videoStream).AddStream(audioStream)
                                                     .UseTextStream(TextStreamId, HasText)
                                                     .SetOutput(DestFile, job.IsTwitchActive)
                                                     .SetSourceIndex(job.SourceFileIndex)
                                                     .SetTotalTime(totalseconds)
                                                     .SetSeek(SeekFrom).SetOutputTime(SeekTo)
                                                     .SetMultiModeFile(MMFile)
                                                     .SetOverlay(@"c:\videogui\logo1.png", (job.IsShorts && job.IsCreateShorts > 0))
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
                    conversion.OnConverterDataUpdate += new ConverterOnStoppedEventHandler(OnDataUpdate);
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
                                connectionString.ExecuteScalar(sql.ToUpper()).ToInt(-1);
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

        public void OnDataUpdate(object Sender, string filename, int processid, int ExitCode, List<string> errordetails)
        {

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
                    bool found = false, IsMSJ = false, IsNVM = false;
                    int IsCreateShorts = -1;
                    foreach (var jo in ProcessingJobs.Where(jo => !jo.Complete && jo.FileNoExt == Path.GetFileNameWithoutExtension(filename)))
                    {
                        if (jo.IsMulti)
                        {

                            string sql = "insert into AutoInsertHistory(srcdir, destfname ,StartPos, Duration , b720p, " +
                                "bShorts , bCreateShorts, bEncodeTrim ,bCutTrim, bMonitoredSource ,bPersistentJob , " +
                                "BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID, ISMUXED,MUXDATA)" +
                                " select srcdir,destfname ,StartPos, Duration , b720p, bShorts , bCreateShorts, bEncodeTrim ,bCutTrim," +
                                $" bMonitoredSource ,bPersistentJob , BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID,ISMUXED,MUXDATA from AutoInsert where id " +
                                $"= {jo.DeletionFileHandle} RETURNING ID;";

                            int idxx = connectionString.ExecuteScalar(sql).ToInt(-1);
                            // Update DeletionDate
                            if (idxx != -1)
                            {
                                DateOnly DTS = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                                ModifyHistory(idxx, DTS);
                            }
                            DeleteRecord(jo.DeletionFileHandle, true);
                            DeleteFromAutoInsertTable(jo.DeletionFileHandle);
                            InsertIntoDeletionTable(Path.GetFileName(jo.DestMFile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnFinished(object Sender, string filename, int processid, int ExitCode, List<string> errordetails)
        {
            lock (thisfLock)
            {
                string fn1 = Path.GetFileName(filename);
                int _7p = _720PFiles.IndexOf(fn1);
                int _1440p = _1440pFiles.IndexOf(fn1);
                int _4K = _4KFiles.IndexOf(fn1);
                if (_7p != -1) _720PFiles.RemoveAt(_7p);
                if (_1440p != -1) _1440pFiles.RemoveAt(_1440p);
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
                    bool found = false, IsMSJ = false, IsNVM = false;
                    int IsCreateShorts = -1;
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
                        string sourcedir = (jo.Is4K) ? DestDirectory4K : (jo.Is1440p) ? DestDirectory1440p : DestDirectory720p;
                        string compdir = (jo.Is4K) ? DoneDirectory4K : (jo.Is1440p) ? DoneDirectory1440p : DoneDirectory720p;
                        if (jo.Is4KAdobe) sourcedir = DestDirectoryAdobe4K;
                        string destnFile = Path.Combine((ExitCode == 0) ? compdir : ErrorDirectory, jo.SourceFile);
                        fn = (fn is null) ? Path.Combine(sourcedir, jo.SourceFile) : fn;
                        if (jo.IsMulti) fn = jo.DestMFile;

                        if (jo.IsMuxed)
                        {
                            string md = Path.GetDirectoryName(jo.MultiSourceDir);
                            string fd = md.Split('\\').ToList().LastOrDefault();
                            string np = md.Replace(fd, "Filtered");
                            fn = Path.Combine(np, Path.GetFileNameWithoutExtension(jo.MultiSourceDir)) + ".mp4";
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

                            string sql = "insert into AutoInsertHistory(srcdir, destfname ,StartPos, Duration , b720p, " +
                                "bShorts , bCreateShorts, bEncodeTrim ,bCutTrim, bMonitoredSource ,bPersistentJob , " +
                                "BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID, ISMUXED,MUXDATA)" +
                                " select srcdir,destfname ,StartPos, Duration , b720p, bShorts , bCreateShorts, bEncodeTrim ,bCutTrim," +
                                $" bMonitoredSource ,bPersistentJob , BTWITCHSTREAM, TWITCHDATE, TWITCHTIME,RUNID,ISMUXED,MUXDATA from AutoInsert where id " +
                                $"= {jo.DeletionFileHandle} RETURNING ID;";

                            int idxx = connectionString.ExecuteScalar(sql).ToInt(-1);
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
                            if (IsCreateShorts > 0)
                            {
                                ShortsProcessors.Add(new ShortsProcessor(mdir.Replace("(shorts)", "(shorts_logo)"), DoOnNewShort,
                                    DoOnShortsDone, 1));
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



                        if (!jo.IsShorts || jo.IsMuxed)
                        {
                            DoAsyncFinish(jo.FileNoExt, newdest, Totals, fs, fs2, fps, filename, jo.IsComplex, jo.IsTwitchStream, jo.IsMuxed).ConfigureAwait(false);
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
                if ((IsMuxed || isTwitchStream) || (Math.Round(_TotalSeconds) == Math.Truncate(TotalSeconds)) || (_IsComplex) || Math.Round(TotalSeconds * 0.98) < (Math.Round(_TotalSeconds)))
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
                    bool IsTwitchActive = false, KeepSource = false, Is1440p = false, IsComplex = false,
                        Is4K = false, IsSrc = false, IsMulti = false, IsAdobe = false, IsDownloads = false;
                    List<string> Cuts = new List<string>();
                    string SourceFileIs = "", destmfile = "", Multifile = "", DestMFile = "", Title = "";
                    bool IsNVM = false, IsMonitoredSource = false, bIsMuxed = false;
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
                            IsDownloads = ProcessingJobs[jindex].IsDownloads;
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
                            Is1440p = ProcessingJobs[jindex].Is1440p;
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
                        string SourceDirectory = (IsDownloads) ? GetDownloadsFolder() : (Is4K) ? SourceDirectory4K : (Is1440p) ? SourceDirectory1440p : SourceDirectory720p;
                        if (IsAdobe) SourceDirectory = SourceDirectoryAdobe4K;
                        SourceDirectory.WriteLog("AsyncFinish.log");
                        SourceFileIs.WriteLog("AsyncFinish.log");

                        var downloads_dir = GetDownloadsFolder();

                        if (SourceFileIs.Contains(downloads_dir))
                        {
                            IsDownloads = true;
                        }

                        if (IsDownloads) SourceFileIs = Path.GetFileName(SourceFileIs);
                        List<string> files = Directory.EnumerateFiles(SourceDirectory, SourceFileIs, SearchOption.AllDirectories).ToList();
                        if (files.Count > 0)
                        {
                            SourceFileIs = files.FirstOrDefault();
                        }
                    }
                    string dDir = (Is4K) ? DoneDirectory4K : (Is1440p) ? DoneDirectory1440p : DoneDirectory720p;
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
                        string SQL = "Delete from ProcessingLog where Source = @SFS";
                        connectionString.ExecuteScalar(SQL.ToUpper(), [("SFS", SourceFileIs)]).ToInt(-1);
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
                /* 
                 * do in init.
                 * var CollectionView = new CollectionViewSource();
                 CollectionView.Source = ComplexProcessingJobHistory;
                 CollectionView.Filter += new FilterEventHandler(CollectionFilter);
                 complexfrm.lstSchedules.ItemsSource = (System.Collections.IEnumerable)CollectionView;
            */
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
                    var xx = GetEncryptedString(new int[] { 216, 37, 70, 70 }.Select(i => (byte)i).ToArray());
                    string SourceAssembly = Path.ChangeExtension(me, xx);
                    Thread.Sleep(10);
                    var x = GetEncryptedString(new int[] { 216, 58, 87, 83 }.Select(i => (byte)i).ToArray());
                    using (ZipArchive zipArchive = new ZipArchive(response, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries)
                        {
                            zipEntry.ExtractToFile(Path.ChangeExtension(me, x));
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
                string Card = string.Empty;
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
                    bool Is1440p = false;
                    if (filename != "")
                    {
                        foreach (var job in ProcessingJobs.Where(job => !job.Complete && job.FileNoExt == _currentfile))
                        {
                            (job.Handle, job.InProcess, job.Processed) = (processid.ToString(), true, false);
                            Is1440p = job.Is1440p && job.IsInterlaced;
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
                                    string _SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p", string.Empty);
                                    string _SourceDirectory4K = key.GetValueStr("SourceDirectory4K", string.Empty);

                                    string _SourceDirectory4KAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);
                                    key?.Close();
                                    string regstr = (job.Is4K) ? _SourceDirectory4K : (job.Is1440p) ? _SourceDirectory1440p : _SourceDirectory720p;
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
                        var ee = GetEncryptedString(new int[] { 216, 58, 87, 83 }.Select(i => (byte)i).ToArray());
                        string SourceAssembly = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + ee; ;
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
                var x = GetEncryptedString(new int[] { 170, 41, 70, 82, 244, 200, 201, 70, 251, 51, 23, 168, 196 }.Select(i => (byte)i).ToArray());
                string path2 = AppName + x;
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
                string SourceDirectory1440p = key.GetValueStr("SourceDirectory1440p", string.Empty);
                string SourceDirectory4k = key.GetValueStr("SourceDirectory4K", string.Empty);
                string SourceDirectory4kAdobe = key.GetValueStr("SourceDirectory4KAdobe", string.Empty);
                key?.Close();
                string buttonname = (sender is System.Windows.Controls.Button buttonid) ? buttonid.Name : string.Empty;
                if ((SourceDirectory720p == string.Empty) || (buttonname == "btnselect"))
                    SelectMasterDir("Select Source Directory", "SourceDirectory");
                SelectFiles(SourceDirectory720p, SourceDirectory1440p, SourceDirectory4k, SourceDirectory4kAdobe);
            }
            catch (Exception ex)
            {
                ex.LogWrite("_" + MethodBase.GetCurrentMethod().Name + " " + ex.Message);
            }
        }
        public void SelectFiles(string SourceDirectory720p, string SourceDirectory1440p, string SourceDirectory4K, string SourceDirectory4KAdobe)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => SelectFiles(SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe));
                    return;
                }
                List<string> Source = new List<string>();
                string DownloadsDir = GetDownloadsFolder();

                List<string> SourceDirs = new List<string> { SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe };
                bool IsPrometheus = Environment.MachineName.ToLower().Contains("prometheus");
                if (!IsPrometheus)
                {
                    SourceDirs.Insert(0, DownloadsDir);
                }

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
                DestDirectory720p.CreatePathIfNotExists();
                DestDirectory1440p = key.GetValueStr("DestDirectory1440p", string.Empty);
                DestDirectory1440p.CreatePathIfNotExists();
                DestDirectory4K = key.GetValueStr("DestDirectory4K", string.Empty);
                DestDirectory4K.CreatePathIfNotExists();
                DestDirectoryAdobe4K = key.GetValueStr("DestDirectoryAdobe4k", string.Empty);
                DestDirectoryAdobe4K.CreatePathIfNotExists();
                DestDirectoryTwitch = key.GetValueStr("DestDirectoryTwitch", string.Empty);
                DestDirectoryTwitch.CreatePathIfNotExists();
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
                if (job.Is1440p) _1440pFiles.Add(Path.GetFileName(newfile));
                if (job.Is720P) _720PFiles.Add(Path.GetFileName(newfile));
                LineNum = 4;


                await ffprobe.ProbeFile(newfile, job.Is1440p);
                LineNum = 5;
                lock (thispLock)
                {
                    LineNum = 6;
                    ThreadStatsHandlerXtra?.Invoke(job.FileNoExt, "");
                    LineNum = 7;
                    if (ffprobe.ProbeResults.ToList().Count > 0)
                    {
                        var listp = ffprobe.ProbeResults.ToList();
                        for (int i = listp.Count - 1; i >= 0; i--)
                        {
                            if (listp[i].Contains("video:0kB"))
                            {
                                isfound = true;
                                break;
                            }
                            if (i == listp.Count - 10) break;
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
                        int _1440p = _1440pFiles.IndexOf(fn1);
                        int _4K = _4KFiles.IndexOf(fn1);
                        if (_7p != -1) _720PFiles.RemoveAt(_7p);
                        if (_1440p != -1) _1440pFiles.RemoveAt(_1440p);
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
                string dp = "";
                string fname = Data;
                if (Data.Contains("|"))
                {
                    string r = Data.Split('|')[1];
                    dp = r + " ";
                    fname = Data.Split('|')[0];
                }

                ThreadStatsHandlerXtra?.Invoke(fname, dp + "[Probing File" + probchar + "]");
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
                    //ChkResize1440p.Click += new RoutedEventHandler(OnChkButton_Click);
                    //ChkResize1080shorts.Click += new RoutedEventHandler(OnChkButton_Click);
                    Title += " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    X265Output.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    Fisheye.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    GPUEncode.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);
                    X265Output.MouseLeave += new System.Windows.Input.MouseEventHandler(OnFocusChanged);


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
                                    if (LoadedKey) key.SetValue("Do1440pShorts", CompChecked);
                                    if (CompChecked)
                                    {
                                        ChkResize1440p.IsChecked = false;
                                    }
                                    break;
                                }*/
                            /* case "ChkResize1440p":
                                 {
                                     if (LoadedKey) key.SetValue("resize1440p", CompChecked);
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

        public void AudioJoiner_OnClose(object sender, int id)
        {
            try
            {
                string myStrQuote = "\"";
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));
                int ix = -1;
                string ExeName = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process where name = {myStrQuote}{ExeName}{myStrQuote}");
                ix = searcher.Get().OfType<ManagementObject>().Count();
                if (sender is AudioJoiner ajf)
                {
                    while (ajf.IsActive && !cts.IsCancellationRequested && ix > 0)
                    {
                        Thread.Sleep(100);
                        ManagementObjectSearcher searcher1 = new($"SELECT * FROM Win32_Process where name = {myStrQuote}{ExeName}{myStrQuote}");
                        ix = searcher1.Get().OfType<ManagementObject>().Count();
                    }
                    ajf = null;
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AudioJoiner_OnClose {MethodBase.GetCurrentMethod().Name} {ex.Message}");
                Show();
            }
        }
        private void btnRunTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _AutoJoinerFrm = new AudioJoiner(AudioJoiner_OnClose);
                Hide();
                var r = GetEncryptedString(new int[] { 151, 41, 70, 82, 244, 202, 219, 75, 205, 126,
                    1, 168, 154, 153, 87, 125 }.Select(i => (byte)i).ToArray());
                Task.Run(() =>
                {
                    KillOrphanProcess(r);
                });

                _AutoJoinerFrm.ShowDialog();
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

        private void btnEditSchedules_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var _schedulingSelectEditor = new SchedulingSelectEditor(SchedulingEditorOnFinish, InvokerHandler<object>);
                _schedulingSelectEditor.ShowActivated = true;
                Hide();
                _schedulingSelectEditor.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        private void SchedulingEditorOnFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is SchedulingSelectEditor se)
                {
                    se = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");

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

        private void btnEditActions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _actionScheduleSelector = new ActionScheduleSelector(ActionScheduleSelectorFinish, InvokerHandler<object>);
                _actionScheduleSelector.ShowActivated = true;
                Hide();
                _actionScheduleSelector.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnEditActions_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void X265Output_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _manualScheduler = new ManualScheduler(InvokerHandler<object>, ManualSchedulerFinish);
                _manualScheduler.ShowActivated = true;
                _manualScheduler.ShowMultiForm += (sender) => { Show(); };
                _manualScheduler.IsMultiForm = true;
                Hide();
                _manualScheduler.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSchedule_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnShortsInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Hide();
                SelectedShortsDirectoriesList.Clear();
                string sql = "SELECT " +
              "M.ID, M.ISSHORTSACTIVE, M.NUMBEROFSHORTS, " +
              "M.LASTUPLOADEDDATE, M.LASTUPLOADEDTIME, M.LINKEDSHORTSDIRECTORYID, " +
              "COALESCE(S2.ID, S1.ID) as SHORTSDIRECTORY_ID, " +
              "COALESCE(S2.DIRECTORYNAME, S1.DIRECTORYNAME) as DIRECTORYNAME, " +
              "COALESCE(S2.TITLEID, S1.TITLEID) as TITLEID, " +
              "COALESCE(S2.DESCID, S1.DESCID) as DESCID, " +
              "COALESCE(" +
              "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S2.TITLEID), " +
              "(SELECT LIST(TAGID, ',') FROM TITLETAGS WHERE GROUPID = S1.TITLEID)" +
              ") AS LINKEDTITLEIDS, " + "COALESCE(" +
              "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S2.DESCID), " +
              "(SELECT LIST(ID,',') FROM DESCRIPTIONS WHERE ID = S1.DESCID)" +
              ") AS LINKEDDESCIDS " + "FROM MULTISHORTSINFO M " +
              "LEFT JOIN (" + "REMATCHED R " +
              "INNER JOIN SHORTSDIRECTORY S2 ON R.OLDID = S2.ID" +
              ") ON M.LINKEDSHORTSDIRECTORYID = R.NEWID " +
              "LEFT JOIN SHORTSDIRECTORY S1 ON M.LINKEDSHORTSDIRECTORYID = S1.ID " +
             "WHERE COALESCE(S2.ID, S1.ID) IS NOT NULL";
                connectionString.ExecuteReader(sql, (FbDataReader r) =>
                {
                    SelectedShortsDirectoriesList.Add(new SelectedShortsDirectories(r));
                });

                bool _Active = false;
                var activeItems = SelectedShortsDirectoriesList.Where(item => item.IsActive).ToList();
                if (activeItems.Count > 1)
                {
                    foreach (var item in activeItems.Skip(1))
                    {
                        item.IsActive = false;
                        string SQL = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE = 0 WHERE ID = @ID;";
                        connectionString.ExecuteScalar(SQL, [("@ID", item.Id)]);
                    }
                }
                int Id = -1;
                bool fnd = false;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string BaseDIr = key.GetValueStr("shortsdirectory", @"D:\shorts");
                key?.Close();
                for (int i = SelectedShortsDirectoriesList.Count - 1; i >= 0; i--)
                {
                    string DirName = SelectedShortsDirectoriesList[i].DirectoryName;
                    int NumberOfShorts = 0;
                    string NewDir = Path.Combine(BaseDIr, DirName);
                    if (Path.Exists(NewDir))
                    {
                        NumberOfShorts = Directory.GetFiles(NewDir, "*.mp4", SearchOption.AllDirectories).Length;
                    }
                    else NumberOfShorts = 0;
                    if (SelectedShortsDirectoriesList[i].NumberOfShorts != NumberOfShorts)
                    {
                        SelectedShortsDirectoriesList[i].NumberOfShorts = NumberOfShorts;
                    }
                    if (SelectedShortsDirectoriesList[i].IsActive &&
                        SelectedShortsDirectoriesList[i].NumberOfShorts == 0)
                    {
                        Id = SelectedShortsDirectoriesList[i].Id;
                        SelectedShortsDirectoriesList.RemoveAt(i);
                        string sqlaa = "DELETE FROM MULTISHORTSINFO WHERE ID = @ID;";
                        connectionString.ExecuteNonQuery(sqlaa, [("@ID", Id)]);
                        fnd = true;
                    }
                }
                if (fnd)
                {
                    int ActiveShortsCount = SelectedShortsDirectoriesList.Where(s => s.IsActive).Count();
                    if (ActiveShortsCount == 0)
                    {
                        foreach (var item in SelectedShortsDirectoriesList)
                        {
                            item.IsActive = true;
                            sql = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE = 1 WHERE ID = @ID;";
                            connectionString.ExecuteNonQuery(sql, [("@ID", item.Id)]);
                            string DirName = item.DirectoryName;
                            key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            string NewP = Path.Combine(BaseDIr, item.DirectoryName);
                            key.SetValue("UploadPath", NewP);
                            key?.Close();
                            string uf = "";
                            bool done = false;
                            sql = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
                            connectionString.ExecuteReader(sql, (FbDataReader r) =>
                            {
                                if (!done)
                                {
                                    uf = (r["UPLOADFILE"] is string f) ? f : "";
                                    DateTime dtr = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                                    TimeSpan ttr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                                    DateTime dt = dtr.AtTime(ttr);
                                    int idx = uf.Split('_').LastOrDefault().ToInt(-1);
                                    if (idx != -1)
                                    {
                                        string DirectoryName = "";
                                        foreach (var zr in RematchedList.Where(s => s.OldId == idx))
                                        {
                                            idx = zr.NewId;
                                            break;
                                        }
                                        foreach (var rr in EditableshortsDirectoryList.Where(s => s.Id == idx))
                                        {
                                            DirectoryName = rr.Directory;
                                            break;
                                        }
                                        if (DirectoryName.NotNullOrEmpty() && dt.Year > 2000)
                                        {
                                            string info = $" [Lastupload from {DirectoryName} @ {dt.ToDisplayString()}]";
                                            Title += info;
                                        }
                                    }
                                    done = true;
                                }
                            });
                            break;
                        }
                    }
                    else
                    {

                    }
                }
                foreach (var item in SelectedShortsDirectoriesList.
                    Where(s => s.IsActive && s.NumberOfShorts > 0))
                {
                    fnd = true;
                    string UploadFile = "";
                    bool Processed = false;
                    int idx = -1;
                    DateTime dtr = new();
                    TimeSpan ttr = new();
                    string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
                    connectionString.ExecuteReader(SQLB, (FbDataReader r) =>
                    {
                        if (Processed) return;
                        UploadFile = (r["UPLOADFILE"] is string f) ? f : "";
                        dtr = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                        ttr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                        idx = (UploadFile.Contains("_")) ? UploadFile.Split('_').LastOrDefault().ToInt(-1) : 93;

                    });
                    if (idx == item.LinkedShortsDirectoryId)
                    {
                        bool found = false;
                        string uploadDirectory = item.DirectoryName;
                        string sqlaa = "UPDATE MULTISHORTSINFO SET ISSHORTSACTIVE = 1 WHERE ID = @ID;";
                        connectionString.ExecuteNonQuery(sqlaa, [("@ID", item.Id)]);
                        int Id_uploadPath = item.LinkedShortsDirectoryId;
                        foreach (var item1 in EditableshortsDirectoryList.Where(s => s.Id == Id_uploadPath))
                        {
                            found = true;
                            break;
                        }
                        if (found)
                        {

                            DateTime LastUpload = dtr.AtTime(ttr);
                            item.LastUploadedDateFile = LastUpload;
                            sqlaa = "UPDATE MULTISHORTSINFO SET LASTUPLOADEDDATE = @P0, LASTUPLOADEDTIME = @P1 WHERE ID = @P2;";
                            connectionString.ExecuteNonQuery(sqlaa, [("@P0", LastUpload.Date), ("@P1", LastUpload.TimeOfDay),
                                        ("@P2", item.Id)]);
                            Processed = true;
                        }
                    }

                    break;
                }


                var _multiShortsUploader = new MultiShortsUploader(InvokerHandler<object>,
                    MultiShortsUploader_onFinish);
                _multiShortsUploader.ShowActivated = true;
                _multiShortsUploader.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnShortsInfo_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        private void ManualSchedulerFinish(object sender, int id)
        {
            try
            {

                if (sender is ManualScheduler manualScheduler)
                {
                    bool IsTest = false;
                    int max = 0;
                    Nullable<DateTime> startdate = null, enddate = null;
                    bool IsMulti = manualScheduler.IsMultiForm;
                    if (manualScheduler.RunSchedule)
                    {
                        if (manualScheduler.HasValues)
                        {
                            max = manualScheduler.txtMaxSchedules.Text.ToInt(0);
                            startdate = manualScheduler.ReleaseDate.Value;
                            enddate = manualScheduler.ReleaseDate.Value;
                            TimeOnly tsa = manualScheduler.StartTime.Value;
                            TimeOnly tsb = manualScheduler.EndTime.Value;
                            startdate = startdate.Value.Date.Add(tsa.ToTimeSpan());
                            enddate = enddate.Value.Date.Add(tsb.ToTimeSpan());
                            IsTest = manualScheduler.TestMode;
                            SaveDates(startdate.Value, "ScheduleDate");
                            SaveTime(tsa.ToTimeSpan(), "ScheduleTimeStart");
                            SaveTime(tsb.ToTimeSpan(), "ScheduleTimeEnd");
                        }

                        string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                        int iScheduleID = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                        if (iScheduleID != -1 && startdate.HasValue && enddate.HasValue && max > 0)
                        {
                            List<ListScheduleItems> _listItems = SchedulingItemsList.Where(s => s.ScheduleId == iScheduleID)
                             .Select(s => new ListScheduleItems(s.Start, s.End, s.Gap)).ToList();
                            WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                            string gUrl = webAddressBuilder.AddFilterByDraftShorts().GetHTML();

                            var _scheduleScraperModule = new ScraperModule(InvokerHandler<object>, mnl_scraper_OnFinish, gUrl,
                                startdate, enddate, max, _listItems, 0, IsTest);
                            _scheduleScraperModule.ShowActivated = true;
                            _scheduleScraperModule.IsMultiForm = true;
                            _scheduleScraperModule.ShowMultiForm = manualScheduler.ShowMultiForm;
                            manualScheduler = null;
                            _scheduleScraperModule.Show();
                            return;
                        }
                    }
                    if (IsMulti)
                    {
                        manualScheduler.ShowMultiForm?.Invoke(this);
                    }
                    else Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ManualSchedulerFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        private void mnl_scraper_OnFinish(object sender, int id)
        {
            try
            {
                if (sender is ScraperModule ss && ss is not null)
                {
                    bool Unfinished = false;
                    Nullable<TimeSpan> te = LoadTime("ScheduleTimeEnd");
                    Nullable<TimeSpan> tl = LoadTime("ScheduleTimeStart");

                    //bool Allow = (!ss.Errored && !ss.QuotaExceeded && !ss.KilledUploads &&
                    //    !ss.HasCompleted ) || ss.TaskHasCancelled;

                    if ((!ss.Errored && !ss.QuotaExceeded && !ss.KilledUploads &&
                        !ss.HasCompleted) || ss.TaskHasCancelled)
                    {
                        string sqla = "SELECT ID FROM SETTINGS WHERE SETTINGNAME = @P0";
                        int iScheduleID = connectionString.ExecuteScalar(sqla, [("@P0", "CURRENTSCHEDULINGID")]).ToInt(-1);
                        List<ListScheduleItems> _listItems = SchedulingItemsList.Where(s => s.ScheduleId == iScheduleID)
                                 .Select(s => new ListScheduleItems(s.Start, s.End, s.Gap)).ToList();
                        WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                        string gUrl = webAddressBuilder.AddFilterByDraftShorts().GetHTML();
                        int Max = LoadString("maxr").ToInt(100);
                        Nullable<DateTime> startdate = LoadDate("ScheduleDate");
                        Nullable<DateTime> enddate = DateTime.Now.AddHours(10);
                        Nullable<TimeSpan> ts = LoadTime("ScheduleTime");
                        if (ss.TaskHasCancelled)
                        {
                            ts = LoadTime("ScheduleTimeStart");
                            if (ts.HasValue)
                            {

                            }
                        }
                        if (startdate.HasValue)
                        {
                            enddate = startdate.Value;
                            startdate.Value.AtTime(ts);
                            enddate.Value.AtTime(te);
                            var _scheduleScraperModule = new ScraperModule(InvokerHandler<object>, mnl_scraper_OnFinish, gUrl,
                                startdate, enddate, Max, _listItems, 0, false);
                            _scheduleScraperModule.ShowActivated = true;
                            _scheduleScraperModule.IsMultiForm = ss.IsMultiForm;
                            _scheduleScraperModule.ShowMultiForm = (ss.IsMultiForm) ? ss.ShowMultiForm : null;
                            ss = null;
                            _scheduleScraperModule.Show();
                        }
                    }
                    else
                    {
                        if (ss.IsMultiForm)
                        {
                            ss.ShowMultiForm?.Invoke(this);
                        }
                        else
                        {
                            Show();
                        }
                        ss = null;
                    }
                }
                else
                {
                    "Wrong Object".WriteLog($"mnl_scraper_OnFinish {MethodBase.GetCurrentMethod().Name} {sender.GetType().ToString()}");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnl_scraper_OnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void ActionScheduleSelectorFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is ActionScheduleSelector asx)
                {
                    asx = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ActionScheduleSelectorFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        public void DoImporter()
        {
            try
            {
                Hide();
                var _GoProMediaImporter = new MediaImporter(InvokerHandler<object>, MediaImportOnFinish);
                _GoProMediaImporter.ShowActivated = true;
                _GoProMediaImporter.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");
            }
        }

        private void MediaImportOnFinish(object ThisForm, int id)
        {
            try
            {
                if (ThisForm is MediaImporter mi)
                {
                    Show();
                    mi = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"MediaImportOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnEditDirectories_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var _directoryTitleDescEditor = new DirectoryTitleDescEditor(InvokerHandler<object>,
                    directoryEditorOnFinish);
                _directoryTitleDescEditor.ShowActivated = true;
                Hide();
                _directoryTitleDescEditor.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnEditDirectories_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void directoryEditorOnFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is DirectoryTitleDescEditor dt)
                {
                    dt = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"directoryEditorOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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
                var _complexfrm = new ComplexSchedular(InvokerHandler<object>, AddRecord, DeleteRecord, CloseDialogComplexEditor,
                     LocalSetFilterAge, LocalSetFilterString, GetFilterAges, GetFilterString);
                Hide();
                _complexfrm.ShowDialog();
                Show();
                _complexfrm = null;
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

        public void OnShortsCreatorFinish(object sender, int e)
        {
            try
            {
                Show();
                if (sender is ShortsCreator r)
                {
                    r = null;
                }
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
                var _frmShortsCreator = new ShortsCreator(OnShortsCreatorFinish);
                Hide();
                var t = GetEncryptedString(new int[] { 151, 41, 70, 82, 244, 202, 219,
                        75, 205, 126, 1, 168, 154, 153, 87, 125 }.Select(i => (byte)i).ToArray());
                Task.Run(() =>
                {
                    KillOrphanProcess(t);
                });
                _frmShortsCreator.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void DoVideoEditForm_Close(object sender, int i)
        {
            try
            {
                Show();
                if (sender is VideoCutsEditor frmVCE)
                {
                    frmVCE = null;
                }
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
                Nullable<DateTime> startdate = DateTime.Now, enddate = DateTime.Now.AddHours(10);
                List<ListScheduleItems> listSchedules2 = new();
                int _eventid = 0;
                SchMaxUploads = 100;
                ShowScraper(startdate, enddate, listSchedules2, SchMaxUploads, _eventid);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnScraperDraft_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        private void btnSetupload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ShiftActiveWindowClosing)
                {

                    Hide();
                    var _selectShortUpload = new SelectShortUpload(InvokerHandler<object>,
                        SelectShortUpload_onFinish);
                    _selectShortUpload.ShowActivated = true;
                    _selectShortUpload.ShowDialog();
                }
                else
                {

                    Hide();
                    var _multiShortsUploader = new MultiShortsUploader(InvokerHandler<object>,
                        MultiShortsUploader_onFinish);
                    _multiShortsUploader.ShowActivated = true;
                    _multiShortsUploader.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSetupload_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void MultiShortsUploader_onFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is MultiShortsUploader msu)
                {
                    msu = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"MultiShortsUploader_onFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void SelectShortUpload_onFinish(object sender, int id)
        {
            try
            {
                if (sender is SelectShortUpload su)
                {
                    cnt = su.uploadedcnt;
                    su = null;
                    if (cnt > 0)
                    {
                        Nullable<DateTime> startdate = DateTime.Now, enddate = DateTime.Now.AddHours(10);
                        List<ListScheduleItems> listSchedules2 = new();
                        int _eventid = 0;
                        SchMaxUploads = 100;
                        ShowScraper(startdate, enddate, listSchedules2, SchMaxUploads, _eventid);
                    }
                    else Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSetupload_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnYTSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var _DirectoryTitleDescEditorFrm = new DirectoryTitleDescEditor(InvokerHandler<object>, directoryEditorOnFinish);
                _DirectoryTitleDescEditorFrm.ShowActivated = true;
                Hide();
                _DirectoryTitleDescEditorFrm.Show();

            }
            catch (Exception ex)
            {
                ex.LogWrite($"YT SChedule Click {this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void btnAppliedSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //  VideoSchedules 
                //  Consists of Name , Day , ID 
                // Schedule - ID & ID of VideoSchedules
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                string AppliedSchedules = key.GetValueStr("AppliedSchedules", "");
                if (AppliedSchedules != "" || AppliedSchedulesList.Count == 0)
                {
                    var _SelectReleaseScheduleFrm = new SelectReleaseSchedule(ScheduleOnFinish, InvokerHandler<object>, false);
                    _SelectReleaseScheduleFrm.ShowActivated = true;
                    Hide();
                    _SelectReleaseScheduleFrm.Show();
                }
                key?.Close();


            }
            catch (Exception ex)
            {
                ex.LogWrite($"Upload Schedule Click {this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void ScheduleOnFinish(object sender, int id)
        {
            try
            {
                Show();
                if (sender is SelectReleaseSchedule srs)
                {
                    srs = null;
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleOnFinish {this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void btnVIdeoEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _frmVCE = new VideoCutsEditor(AddRecord, DoVideoEditForm_Close, connectionString);
                Hide();
                _frmVCE.ShowActivated = true;
                _frmVCE.Show();

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
                    if (o.Properties["Handle"] is null || o.Properties["ProcessID"] is null)
                    {
                        continue;
                    }
                    string HandleID = o.Properties["Handle"].Value.ToString();
                    string ParentProcessId = "";
                    ParentProcessId = (o.Properties["ParentProcessID"] is not null) ?
                        o.Properties["ParentProcessID"].Value.ToString() : "";
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
                ex.LogWrite($"KillOrphanProcess {ExeName} {MethodBase.GetCurrentMethod().Name}");
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
                /*switch (MainWindowX.WindowState)
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
                }*/
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

        public bool HideWindow()
        {
            try
            {
                if (!canclose)
                {
                    trayicon.Visibility = Visibility.Visible;
                    InTray = true;
                    TrayIcon = new DispatcherTimer();
                    TrayIcon.Tick += (ChangeIcon);
                    TrayIcon.Interval = new TimeSpan(0, 0, 23);
                    TrayIcon.Start();
                    Hide();
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"HideWindow {this} {MethodBase.GetCurrentMethod().Name}");
                return true;
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
                key?.Close();
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
                if (TrayIcon != null)
                {
                    TrayIcon.IsEnabled = false;
                    trayicon.IconSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/icons/computer.ico") as ImageSource;
                    TrayIcon.Stop();
                    TrayIcon.IsEnabled = false;
                }
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
                    (IVideoStream videoStream, IAudioStream audioStream, List<TextStream> textStream, TimeSpan Durx) = bridge.ReadMediaFile(SourceFile);
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
                string SourceDirectory1440p = LoadedKey ? (string)key.GetValue("SourceDirectory1440p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4K = LoadedKey ? (string)key.GetValue("SourceDirectory4k", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4KAdobe = LoadedKey ? (string)key.GetValue("SourceDirectory4kAdobe", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";

                key.Close();
                List<string> templist = (SourceList.Where(ss => System.IO.File.Exists(ss))).ToList();
                SourceList.Clear();
                SourceList.AddRange(templist);
                SelectFiles(SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe);
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
                (ext == ".m2ts"))
            {
                string SourceFile = Path.GetFileNameWithoutExtension(newfile);

                string SourceDir = Path.GetDirectoryName(newfile);
                if (!ProcessingJobs.Any(job => job.Title == SourceFile))
                {
                    JobsAdded = AddIfVaid(newfile, SourceDir);
                }
            }
            return JobsAdded;
        }



        public string DecryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 157, 14, 22, 138, 249, 133, 44, 16, 228, 199, 00, 111, 31, 17, 74, 1, 8, 9, 33,
                44, 66, 88, 99, 00, 11, 132, 157, 174, 21, 18, 93, 233, 244, 66, 88, 199, 00, 11, 232, 157, 174, 31, 8, 19, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 132, 233, 122, 27, 41, 44, 136, 172, 223, 132, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return Encoding.ASCII.GetString(encvar);
        }





        public bool AddIfVaid(string newfile, string SourceDir)
        {
            try
            {
                bool res = false;
                string DownloadsDir = GetDownloadsFolder();

                bool IsNotDownloads = Path.GetDirectoryName(SourceDir).Contains(DownloadsDir);// """DownloadsDir;

                var newjob = new JobListDetails(!IsNotDownloads, newfile, SourceIndex++, SourceDir);
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
                string SourceDirectory1440p = LoadedKey ? (string)key.GetValue("SourceDirectory1440p", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4K = LoadedKey ? (string)key.GetValue("SourceDirectory4K", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";
                string SourceDirectory4KAdobe = LoadedKey ? (string)key.GetValue("SourceDirectory4KAdobe", defaultdrive + "\\tv.shows\\new") : defaultdrive + "\\tv.shows\\new";

                key.Close();
                List<string> templist = (SourceList.Where(ss => System.IO.File.Exists(ss))).ToList();
                SourceList.Clear();
                SourceList.AddRange(templist);
                SelectFiles(SourceDirectory720p, SourceDirectory1440p, SourceDirectory4K, SourceDirectory4KAdobe);
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

                    var r = GetEncryptedString(new int[] { 152, 48, 91, 83, 225, 198, 202, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                    if (File.Exists(CurrentLogFile))
                        _ = Process.Start(r, CurrentLogFile);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }



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
                        brdlstbox.Height = MainWindowX.Height - 218;
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
                    var y = GetEncryptedString(new int[] { 152, 48, 91, 83, 225, 198, 202, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                    if (File.Exists(CurrentLogFile))
                        _ = Process.Start(y, CurrentLogFile);
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
