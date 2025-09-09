using CliWrap;
using CliWrap.EventStream;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using Google.Apis.YouTube.v3.Data;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Core.Raw;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using VideoGui.Models;
using VideoGui.Models.delegates;
using Windows.UI.WebUI;
using Windows.Web.UI.Interop;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using File = System.IO.File;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using String = System.String;
using Task = System.Threading.Tasks.Task;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;
using Window = System.Windows.Window;

namespace VideoGui
{
    public class DirectoriesProbe
    {
        public string Directory { get; set; } = "";
        public string url { get; set; } = "";
        public bool Probed { get; set; } = false;
        public bool MatchNotFound { get; set; } = false;
        public bool Processed { get; set; } = false;
        public DirectoriesProbe(string dir)
        {
            Directory = dir;
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScraperModule : Window
    {
        object lockobj = new object();
        CancellationTokenSource canceltoken = new CancellationTokenSource();
        public int EventId, TotalScheduled = 0;
        int WheelBody = 0, LastId = -1, Tid = -1, Did = -1, WheelMoveNode = 0;
        List<string> lookups = new List<string>();
        string LTitleStr = "", LDescStr = "", DirectoryPath = "";
        int swap = 1, ct = 0, MaxNodes = -1, MaxUploads = 0, recs = 0, gmaxrecs = 0, files = 0, dbfiles = 0, max = 0, SlotsPerUpload = 0,
            ScheduleMax = 0, ts = 0, LastKey = -1, Days = 1, CurrentDay = 1, inserted = 0, WheelMove = 0;
        bool EditDone = false, btndone = false, ExitDialog = false, Waiting = false, IsVideoLookup = false, WaitingFileName = false;
        bool Valid = false, IsVideoLookupShort = false, IsValid = false, IsUnlisted = false, IsDashboardMode = false, CanSpool = false, FirstRun = true, done = false, HasExited = false;
        bool DoNextNode = true, finished = false, TimedOut = false, Uploading = false, NextRecord = false, Processing = false, clickupload = true;
        public bool IsClosing = false, IsClosed = false, Exceeded = false, KilledUploads = false, SwapEnabled = false, IsTitleEditor = false;
        public bool TaskHasCancelled = false, NewSession = false, QuotaExceeded = false, Errored = false;

        string SendKeysString = "", UploadPath = "", LastNode = "", DefaultUrl = "", LastValidFileName = "", TableDestination = "";
        List<ScraperUploads> Scraper_uploaded = new();
        List<string> IdNodes = new(), titles = new List<string>(), nextaddress = new(), Ids = new(), Idx = new(), ufiles = new(), Files = new();// DoneFiles = new();
        public List<string> ScheduledOk = new List<string>(), VideoFiles = new List<string>(), nodeslist = new List<string>();
        public List<ShortsDirectory> ShortsDirectoriesList = new(); // <shortname>
        public List<ListScheduleItems> listSchedules = new List<ListScheduleItems>();
        public List<VideoIdFileName> ScheduledFiles = new List<VideoIdFileName>();
        DispatcherTimer CloseTimer = new DispatcherTimer();
        DirectshortsScheduler directshortsScheduler = null;
        DispatcherTimer CloseScrape = new DispatcherTimer();
        DispatcherTimer TimerSimulate = new DispatcherTimer();
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer LocationTimer = new DispatcherTimer();
        public AddressUpdate DoVideoLookUp = null;
        string TitleStr = "", DescStr = "", TargetUrl = "";
        StatusTypes VStatusType = StatusTypes.PRIVATE;
        WebAddressBuilder webAddressBuilder = null;
        databasehook<object> Invoker = null;
        List<Rematched> RematchedList = new(); // <shortname>
        OnFinishIdObj DoOnFinish = null;
        public bool ScheduledFinished = true;
        System.Threading.Timer UploadsTimer = null;
        TimeOnly CurrentTime = new TimeOnly();
        DateOnly CurrentDate = DateOnly.FromDateTime(DateTime.Now);
        public DateTime StartDate = DateTime.Now, EndDate = DateTime.Now, LastValidDate = DateTime.Now;
        bool IsTest = false, AutoClose = false, AutoClosed = false, IsLocation = false,
            IsMoving = false, HasMoved = false;
        public bool TimedOutClose = false, IsMultiForm = false;
        public Action<object> ShowMultiForm = null;
        public Action<object, string> SendTraceInfo = null;
        List<DirectoriesProbe> Directories = new(); //Directories
        Dictionary<int, WebView2CompositionControl> wv2Dictionary = new Dictionary<int, WebView2CompositionControl>();
        Dictionary<int, WebView2CompositionControl> ActiveWebView = new Dictionary<int, WebView2CompositionControl>();
        DispatcherTimer InternalTimer = new DispatcherTimer();
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        public Nullable<DateTime> ReleaseDate = null, ReleaseEndDate = null;
        public int uploadedcnt = 0;
        public EventTypes ScraperType = EventTypes.VideoUpload;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WHEEL_DELTA = 120; // Standard wheel delta value
        private const int INPUT_MOUSE = 0;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private CoreWebView2CompositionController wv2CompositionController;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetDlgCtrlID(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private const int BM_CLICK = 0x00F5;


        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);
        public void DoLocationTimer()
        {
            try
            {
                LocationTimer.Interval = TimeSpan.FromSeconds(3);
                LocationTimer.Tick += (s, e) =>
                {
                    LocationTimer.Stop();
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("Webleft", Left);
                    key.SetValue("Webtop", Top);
                    key?.Close();
                };
                LocationTimer.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LocationTimer {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish,
            List<string> directories, bool IsShort)
        {
            try
            {
                ScraperType = EventTypes.VideoLookup;
                DoOnFinish = _OnFinish;
                foreach (var dir in directories)
                {
                    Directories.Add(new DirectoriesProbe(dir));
                }
                IsVideoLookupShort = IsShort;
                IsDashboardMode = true;
                AutoClose = true;
                Invoker = _Invoker;
                InitializeComponent();
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                Closing += (s, e) =>
                {
                    IsClosing = true;
                    CloseScrape.Stop();
                    timer?.Stop();
                    TimerSimulate?.Stop();
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, wv2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor Scraper.VideoLookup {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void WebViewFileName_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            try
            {
                if (sender is WebView2 wv)
                {
                    //wv2.IsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"WebViewFileName_CoreWebView2InitializationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish, string _Default_url, string _TargetUrl, int _EventId)
        {
            try
            {
                ScraperType = EventTypes.ScapeSchedule;
                DoOnFinish = _OnFinish;
                TargetUrl = _TargetUrl;
                AutoClose = true;
                DefaultUrl = _Default_url;
                EventId = _EventId;
                IsDashboardMode = true;
                Invoker = _Invoker;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    TimerSimulate?.Stop();
                    timer?.Stop();
                    IsClosing = true;
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, ReportNewAddress, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, wv2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor Scraper.Schedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        Action<string, int, string, string> TargetUpdate = null;
        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish,
            string _Default_url, string _TargetUrl)
        {
            try
            {
                ScraperType = EventTypes.ScrapeDraftSchedules;
                DoOnFinish = _OnFinish;
                TargetUrl = _TargetUrl;
                AutoClose = true;
                DefaultUrl = _Default_url;
                EventId = 0;
                IsDashboardMode = true;
                Invoker = _Invoker;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    TimerSimulate?.Stop();
                    timer?.Stop();
                    IsClosing = true;
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, ReportNewAddress, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, wv2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor Scraper.Schedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void ReportNewAddress(string address, string id)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher?.Invoke(() => ReportNewAddress(address, id));
                    return;
                }
                bool found = false;
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMilliseconds(150));
                while (!cts.IsCancellationRequested)
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(15);
                }
                foreach (var item in wv2Dictionary.Values.Where(item => item.AllowDrop && item.Tag.ToInt(-1) == 1))
                {
                    item.ExecuteScriptAsync("window.gc()");
                    item.SetURL(address);//wv2A1.Source = new Uri(address);
                    break;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ReportNewAddress {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish,
            string _Default_url,
            Nullable<DateTime> Start, Nullable<DateTime> End, int MaxUoploads,
            List<ListScheduleItems> _listSchedules, int _eventid, bool _IsTest)
        {
            try
            {
                ScraperType = EventTypes.ShortsSchedule;
                EventId = _eventid;
                listSchedules = _listSchedules;
                ReleaseDate = Start;
                ReleaseEndDate = End;
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                ScheduleMax = MaxUoploads;
                IsDashboardMode = true;
                IsTest = _IsTest;
                Invoker = _Invoker;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    TimerSimulate?.Stop();
                    IsClosing = true;
                    timer?.Stop();
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();

                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, wv2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor Shorts.Schedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish,
            string _Default_url, int maxuploads = 100, int slotsperupload = 5,
            int _EventId = -1, bool _NewSession = false)
        {
            try
            {
                SwapEnabled = false;
                NewSession = _NewSession;
                EventId = _EventId;
                ScraperType = EventTypes.VideoUpload;
                UploadsTimer = new System.Threading.Timer(Uploads_TimerEvent_Handler, null, -1, Timeout.Infinite);
                MaxUploads = maxuploads;
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                IsDashboardMode = true;
                Invoker = _Invoker;
                IsUnlisted = false;
                SlotsPerUpload = slotsperupload;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    TimerSimulate?.Stop();
                    IsClosing = true;
                    timer?.Stop();
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50;
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, wv2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor VideoUploader {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Uploads_TimerEvent_Handler(object? state)
        {
            try
            {
                int r = DoUploadsCnt();
                if (!NewSession || r < 100)
                {
                    var UploadTask = UploadV2Files(false);
                }
                else
                {
                    UploadsTimer.Change(TimeSpan.FromMinutes(5).TotalMilliseconds.ToInt(0), Timeout.Infinite);
                }
            }
            catch (Exception ex)

            {
                ex.LogWrite($"Uploads_TimerEvent_Handler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public ScraperModule(databasehook<object> _Invoker, OnFinishIdObj _OnFinish,
            string _Default_url, WebView2CompositionControl wb2)
        {
            try
            {
                SwapEnabled = false;
                EventId = -1;
                ScraperType = EventTypes.UploadTest;
                MaxUploads = 2;
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                IsDashboardMode = true;
                Invoker = _Invoker;
                IsUnlisted = false;
                SlotsPerUpload = 2;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    TimerSimulate?.Stop();
                    timer?.Stop();
                    IsClosing = true;
                    canceltoken.Cancel();
                    cancelds();
                    uploadedcnt = TotalScheduled;
                    LocationTimer.Stop();
                    TimerSimulate.Stop();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(this, EventId);
                };
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                wv2Dictionary.Add(1, wv2A1);//20
                wv2Dictionary.Add(2, wv2A2);//30
                wv2Dictionary.Add(3, wv2A3);//40
                wv2Dictionary.Add(4, wv2A4);//50
                wv2Dictionary.Add(5, wv2A5);//60
                wv2Dictionary.Add(6, wv2A6);//70
                wv2Dictionary.Add(7, wv2A7);//80
                wv2Dictionary.Add(8, wv2A8);//90
                wv2Dictionary.Add(9, wv2A9);//100
                wv2Dictionary.Add(10, wv2A10);
                ActiveWebView.Add(1, (wb2 is null) ? wv2 : wb2);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor VideoUploader {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void lblLastNode_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double sumWidths = 0;

            foreach (var child in StatusBar.Items)
            {
                sumWidths += (child as FrameworkElement)?.ActualWidth ?? 0;
            }
            btnClose.Margin = new Thickness(StatusBar.Width - sumWidths - 40, 0, 0, 0);
        }
        private void lblInsert_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblLastNode_SizeChanged(sender, e);
        }
        private void lblUp_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblLastNode_SizeChanged(sender, e);
        }
        private void lblInsertId4_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblLastNode_SizeChanged(sender, e);
        }
        private void lblInsertId5_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblLastNode_SizeChanged(sender, e);
        }
        private void DoNewVideoUpdate(string address)
        {
            try
            {
                while (true)
                {
                    bool sent = false;
                    for (int i = 0; i < wv2Dictionary.Count; i++)
                    {
                        if (wv2Dictionary[i].AllowDrop)
                        {
                            Task.Run(() =>
                            {
                                wv2Dictionary[i].SetURL(webAddressBuilder.ScopeVideo(address).ScopeAddress);
                            });
                            sent = true;
                            break;
                        }
                    }
                    if (!sent)
                    {
                        nextaddress.Add(address);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoNewVideoUpdate {MethodBase.GetCurrentMethod()?.Name} {ex.Message} ");
            }
        }
        async void Wv2s_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (e.IsSuccess && sender is not null)
                {
                    int Id = (sender as WebView2CompositionControl).Name.Replace("wv2A", "").ToInt(-1);
                    string source = (sender as WebView2CompositionControl).Source.AbsoluteUri.ToString(), IntId = "";
                    int p1 = source.IndexOf("video/"), p2 = source.IndexOf("/edit");
                    if (p1 != -1 && p2 != -1)
                    {
                        IntId = source.Substring(p1 + 6, p2 - p1 - 6);
                    }
                    var task = (sender as WebView2CompositionControl).ExecuteScriptAsync("document.body.innerHTML");
                    task.ContinueWith(x => { ProcessHTML(x.Result, Id, IntId, sender); },
                        TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Wv2s_NavigationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async Task<bool> DailyLimitReached()
        {
            if (!Dispatcher.CheckAccess())
            {
                return (bool)(Dispatcher?.Invoke(() => DailyLimitReached().Result));
            }

            try
            {
                var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                return (html.Contains("Daily upload limit reached"));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DailyLimitReached {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
                return false;
            }
        }
        async void InitAsync()
        {
            try
            {
                LocationChanged += (s, e) =>
                {
                    if (canceltoken.Token.IsCancellationRequested) return;
                    if (IsLoaded)
                    {
                        DoLocationTimer();
                    }
                };
                TimerSimulate.Interval = TimeSpan.FromSeconds(1);
                TimerSimulate.Tick += (s, e) =>
                {
                    TimerSimulate.Stop();
                    if (canceltoken.Token.IsCancellationRequested) return;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SimulateWheelUpDown(wv2);
                    }));
                    WheelMove++;
                    if (WheelMove < 10)
                    {
                        TimerSimulate.Start();
                    }
                };
                var env = await CoreWebView2Environment.CreateAsync(null, @"c:\stuff\scraper");
                bool done = false;
                wv2.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    done = true;
                };
                await wv2.EnsureCoreWebView2Async(env);
                var hwndSource = (HwndSource)PresentationSource.FromVisual(wv2);
                if (hwndSource != null)
                {
                    var handle = hwndSource.Handle;
                    wv2CompositionController = await env.CreateCoreWebView2CompositionControllerAsync(handle);
                }
                wv2A1.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A2.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A3.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A4.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A5.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A6.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A7.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A8.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A9.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A10.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                wv2A6.CoreWebView2InitializationCompleted += (s, e) =>
                {
                    (s as WebView2CompositionControl).Tag = 1;
                };
                await wv2.EnsureCoreWebView2Async(env);
                await wv2A1.EnsureCoreWebView2Async(env);
                await wv2A2.EnsureCoreWebView2Async(env);
                await wv2A3.EnsureCoreWebView2Async(env);
                await wv2A4.EnsureCoreWebView2Async(env);
                await wv2A5.EnsureCoreWebView2Async(env);
                await wv2A6.EnsureCoreWebView2Async(env);
                await wv2A7.EnsureCoreWebView2Async(env);
                await wv2A8.EnsureCoreWebView2Async(env);
                await wv2A9.EnsureCoreWebView2Async(env);
                await wv2A10.EnsureCoreWebView2Async(env);
                var connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                connectionString.ExecuteReader(GetUploadReleaseBuilderSql(), (FbDataReader r) =>
                {
                    ShortsDirectoriesList.Add(new ShortsDirectory(r));
                });
                await SetupSubstDrive();
                StatusBar.Items.OfType<FrameworkElement>().Where(child => !(child is Button)).ToList().ForEach(frameworkElement =>
                {
                    frameworkElement.SizeChanged += (object sender, SizeChangedEventArgs e) => { StatusBar.ApplyMargin(); };
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InitAsync {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public string GetUploadReleaseBuilderSql(int index = -1)
        {
            try
            {
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
        private void mainwindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F4 && Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    canceltoken.Cancel();
                    cancelds();
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mainwindow_KeyDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void ExecuteAsAdmin(string arguments)
        {
            try
            {
                string args = arguments;
                if (arguments.Contains(" "))
                {
                    args = "\"" + arguments + "\"";
                }
                var p = GetEncryptedString(new int[] { 149, 50, 75, 24, 244, 223, 203 }.Select(i => (byte)i).ToArray());
                var p1 = GetEncryptedString(new int[] { 217, 60, 15, 69, 228, 197, 221, 71, 188, 120, 21, 164, 148, 166, 21, 56, 118, 218 }.Select(i => (byte)i).ToArray());
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = p,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Verb = "runas",
                    Arguments = p1
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                var x = GetEncryptedString(new int[] { 217, 60, 15, 69, 228, 197, 221, 71, 188, 120, 21, 164, 148, 166, 21, 56 }.Select(i => (byte)i).ToArray());
                ProcessStartInfo startInfo2 = new ProcessStartInfo()
                {
                    FileName = p,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Verb = "runas",
                    Arguments = x + args
                };
                Process process2 = new Process();
                process2.StartInfo = startInfo2;
                process2.Start();
                process2.WaitForExit();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteAsAdmin {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
        public string DecryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 157, 14, 22, 138, 249, 133, 44, 16, 228, 199, 00, 111, 31, 17, 74, 1, 8, 9, 33,
                44, 66, 88, 99, 00, 11, 132, 157, 174, 21, 18, 93, 233, 244, 66, 88, 199, 00, 11, 232, 157, 174, 31, 8, 19, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 132, 233, 122, 27, 41, 44, 136, 172, 223, 132, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return Encoding.ASCII.GetString(encvar);
        }
        public void ExecuteAsNonAdmin(string arguments)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                string args = arguments;
                if (arguments.Contains(" "))
                {
                    args = "\"" + arguments + "\"";
                }
                StringBuilder sb = new StringBuilder();
                var r1 = GetEncryptedString(new int[] { 133, 42, 77, 69, 229, 135, 212, 9, 178 }.Select(i => (byte)i).ToArray());
                var xx = GetEncryptedString(new int[] { 133, 42, 77, 69, 229, 135, 212, 9, 178, 50, 9 }.Select(i => (byte)i).ToArray());
                sb.Append(xx + Environment.NewLine);
                sb.Append(r1 + args);
                string cdir = Environment.CurrentDirectory;
                File.WriteAllText(cdir + "\\map.bat", sb.ToString());
                var r = GetEncryptedString(new int[] { 147, 39, 95, 90, 254, 213, 203, 65, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = r,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Arguments = cdir + "\\map.bat"
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteAsAdmin {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public bool CycleThroughChildWindows()
        {
            try
            {
                ExitDialog = true;
                if (canceltoken.IsCancellationRequested) return false;
                IntPtr mainWindowHandle = FindWindow(null, "Open"); // Provide the title of the active window
                if (mainWindowHandle != IntPtr.Zero)
                {
                    EnumChildWindows(mainWindowHandle, EnumChildWindowCallback, IntPtr.Zero);
                }

                return Valid;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CycleThroughChildWindows {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }
        private bool EnumChildWindowCallback(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                int controlId = GetDlgCtrlID(hWnd);
                if (className.ToString() == "Edit" && controlId == 0x47C && !EditDone) // Check for Edit control with ID 000000000000047C
                {
                    while (true && !canceltoken.IsCancellationRequested)
                    {
                        SendMessage(hWnd, 0x000C, IntPtr.Zero, SendKeysString);
                        List<string> keys = SendKeysString.Split(' ').ToList().Where(s => s != "").ToList();
                        for (int i = 0; i < keys.Count; i++)
                        {
                            keys[i] = keys[i].Replace("\"", "").Replace(".mp4", "");
                        }
                        Thread.Sleep(50);
                        StringBuilder text = new StringBuilder(256);
                        GetWindowText(hWnd, text, 256);
                        if (text.ToString().ContainsAll(keys.ToArray()))
                        {
                            EditDone = true;
                            break;
                        }
                    }
                    return true;
                }
                else if (className.ToString() == "Button" && controlId == 1 && !btndone) // Check for Button control with ID 1
                {
                    SendMessage(hWnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    btndone = true;
                    Valid = true;
                    Thread.Sleep(250); // Wait for the button click action to take effect
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CycleThroughChildWindows {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }
        public async Task SetupSubstDrive()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        SetupSubstDrive();
                    });
                    return;
                }
                string _dest = UploadPath;
                string DefaultPath = Environment.SystemDirectory;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                _dest = FindUploadPath();
                key.Close();
                ExecuteAsNonAdmin($"{_dest}");
                ExecuteAsAdmin($"{_dest}");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetupSubstDrive {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public string FindUploadPath()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shortsdir = key?.GetValueStr("shortsdirectory", @"D:\shorts\");
                string uploaddir = key?.GetValueStr("UploadPath", "");
                key?.Close();
                if (!Path.Exists(uploaddir))
                {
                    string NewPath = uploaddir.Split('\\').LastOrDefault();
                    string uploadsNewPath = Path.Combine(shortsdir, NewPath);
                    if (Path.Exists(uploadsNewPath))
                    {
                        uploaddir = uploadsNewPath;
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
        public List<HtmlNode> GetNodes(string html, string Span_Name)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return new List<HtmlNode>();
                if (html is not null && html.Contains(Span_Name))
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    return doc.DocumentNode.SelectNodes($"//li[@class='{Span_Name}']").ToList();
                }
                else return new List<HtmlNode>();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetupSubstDrive {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return new List<HtmlNode>();
            }
        }
        private void lblTotal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // SetMargin(StatusBar,92);
        }
        private void lblUploaded_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblTotal_SizeChanged(sender, e);
        }
        public async Task UploadV2Files(bool rentry = false)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => UploadV2Files(rentry));
                    return;
                }

                HasExited = false;
                InternalTimer.Interval = TimeSpan.FromMilliseconds(1200);
                InternalTimer.Tick += new EventHandler(SendKeys_Tick);
                string Span_Name = "row style-scope ytcp-multi-progress-monitor";
                string script = @"
        var createButton = document.getElementById('upload-icon');
        if (createButton) {
            createButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            createButton.click();
        }
    ";
                string Script_Close = @"
                var buttons = document.querySelectorAll('#close-button');
                if (buttons.length > 1) {
                    buttons[1].click();
                }
            ";

                int Uploading = 0;
                if (!rentry)
                {
                    string _dest = UploadPath, defaultpath = Environment.SystemDirectory;
                    lblLastNode.Content = "Uploading...";
                    List<string> ufc = new List<string>();
                    if (ShortsDirectoriesList.Count == 0) ShortsDirectoriesList.Add(new ShortsDirectory(-1, UploadPath));
                    Files.Clear();
                    Files.AddRange(Directory.EnumerateFiles("z:\\", "*.mp4", SearchOption.AllDirectories).ToList());
                    for (int i = Files.Count - 1; i >= 0; i--)
                    {
                        if (!Files[i].Contains("_"))
                        {
                            Files.RemoveAt(i);
                        }
                    }
                    int max = 0;
                    string connectStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                    SendKeysString = "";
                    int res = -1;// GetUploadsRecCnt(connectStr, false);
                    if (Invoker?.Invoke(this, new CustomParams_GetUploadsRecCnt(false)) is int cnt) res = cnt;
                    TotalScheduled = res;
                    lblTotal.Content = TotalScheduled.ToString();
                    SetMargin(StatusBar);
                    max = TotalScheduled;
                    ts = max;
                    if (ts < MaxUploads)
                    {
                        foreach (string f in Files.Where(f => File.Exists(f)).Take(SlotsPerUpload))
                        {
                            max++;
                            if (max <= MaxUploads)
                            {
                                lblTotal.Content = $"{TotalScheduled}";
                                SetMargin(StatusBar);
                                ScheduledFiles.Add(new VideoIdFileName(Path.GetFileName(f)));
                                string news = "\"" + @"Z:\" + new DirectoryInfo(Path.GetDirectoryName(f)).Name + "\\" + Path.GetFileName(f) + "\" ";
                                if (SendKeysString.Length + news.Length < 255)
                                {
                                    SendKeysString += news;
                                }
                                else break;
                            }
                        }
                    }
                    if (SendKeysString.Trim() != "")
                    {
                        string r = "";
                        List<string> keys = SendKeysString.Split(' ').ToList().Where(s => s != "").ToList();
                        foreach (string k in keys)
                        {
                            var nm = k.Split(@"\").ToList().LastOrDefault();
                            string newfile = nm.Replace("\"", "");
                            if (newfile.Contains("."))
                            {
                                newfile = newfile.Substring(0, newfile.IndexOf("."));
                            }
                            VideoFiles.Add(newfile);
                            r = r + newfile + " ";
                        }
                        lstMain.Items.Insert(0, $"Inserting {max} Files {r}");
                        await ActiveWebView[1].CoreWebView2.ExecuteScriptAsync("document.getElementById('upload-icon').click();");
                        await ActiveWebView[1].CoreWebView2.ExecuteScriptAsync("document.getElementById('stroke').click();");
                        HasExited = true;
                        Dispatcher.Invoke(() => InternalTimer.Start());
                    }
                }
                else
                {
                    HasExited = false;
                    ExitDialog = false;
                    string connectStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                    bool Exit = false, finished = false;
                    while (true || !finished)
                    {
                        if (ExitDialog) return;
                        if (Exit)
                        {
                            await ActiveWebView[1].ExecuteScriptAsync(Script_Close);
                            break;
                        }
                        Thread.Sleep(50);
                        SetMargin(StatusBar);
                        bool flowControl = await ProcessUploadsBody(Span_Name, Script_Close, connectStr);
                        if (!flowControl)
                        {
                            finished = true;
                            var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            if (html.ToLower().Contains("uploads complete"))
                            {
                                InsertIntoUploadFiles(VideoFiles, connectStr);
                                Close();
                                break;
                            }
                            break;
                        }
                        if (ExitDialog) return;
                        TimerSimulate.Start();
                        NodeUpdate(Span_Name, ScheduledGet);
                        var html1 = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                        List<HtmlNode> Nodes1 = GetNodes(html1, Span_Name);
                        int finishedz = 0;
                        foreach (HtmlNode Node in Nodes1)
                        {
                            finishedz++;
                            int start = Node.InnerText.IndexOf("\n") + 1;
                            string filename1 = Node.InnerText.Substring(start).Split('\n').FirstOrDefault().Trim();
                            if (Node.InnerText.ToLower().Contains("limit") || Node.InnerText.ToLower().Contains("100% uploaded"))
                            {
                                var e = Node.InnerText.ToLower().Contains("limit");
                                Exceeded = Exceeded || e;
                                UploadedHandler(Span_Name, connectStr, filename1);
                                if (!e)
                                {
                                    InsertIntoUploadFiles(new List<string> { filename1 }, connectStr);
                                }
                            }
                        }
                        if (Exceeded)
                        {
                            html1 = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            Nodes1 = GetNodes(html1, Span_Name);
                            foreach (var Node in Nodes1.Where(Node => Node.InnerText.ToLower().Contains("100% uploaded")))
                            {
                                int start = Node.InnerText.IndexOf("\n") + 1;
                                string filename1 = Node.InnerText.Substring(start).Split('\n').FirstOrDefault().Trim();
                                UploadedHandler(Span_Name, connectStr, filename1);
                                InsertIntoUploadFiles(new List<string> { filename1 }, connectStr);
                            }
                        }
                        if (finishedz == Nodes1.Count && Nodes1.Count > 0)
                        {
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromSeconds(4));
                            while (!cts.IsCancellationRequested)
                            {
                                var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                if (html.ToLower().Contains("uploads complete"))
                                {
                                    NodeUpdate(Span_Name, ScheduledGet);
                                    InsertIntoUploadFiles(VideoFiles, connectStr);
                                    break;
                                }
                                Thread.Sleep(100);
                            }
                            Close();
                        }
                        else TimerSimulate.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadV2Files {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
            finally
            {
                HasExited = true;
            }
        }
        public void InsertIntoUploadFiles(List<string> videofiles, string connectStr)
        {
            try
            {
                Invoker?.Invoke(this, new CustomParams_UpdateUploadsRecords(videofiles, DirectoryPath));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertIntoUploadFiles {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void UploadedHandler(string Span_Name, string connectStr, string filename1)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if (filename1 != "" && ScheduledOk.IndexOf(filename1) == -1)
                {
                    string[] files = Directory.GetFiles("Z:\\", filename1, SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                        NodeUpdate(Span_Name, ScheduledGet);
                        lstMain.Items.Insert(0, $"{file} Deleted");
                        InsertIntoUploadFiles(new List<string> { file }, connectStr);
                        Invoker?.Invoke(this, new CustomParams_UpdateStats(file));
                        if (ScheduledOk.IndexOf(filename1) == -1)
                        {
                            ScheduledOk.Add(filename1);
                            string gUrl = webAddressBuilder.ScopeVideo(filename1, false).ScopeAddress;
                            nextaddress.Add(gUrl);
                            VideoFiles.Remove(filename1);
                            TotalScheduled++;
                            uploadedcnt = TotalScheduled;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadedHandler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private int ScheduledGet()
        {
            return TotalScheduled;
        }
        private async Task<bool> NodeUpdate(string Span_Name, GetTotalScheduled ScheduledGet)
        {
            try
            {
                string html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                var Nodes = GetNodes(html, Span_Name);
                int waiting = 0;
                int Uploaded = 0;
                int Uploading = 0;
                foreach (var node in Nodes)
                {
                    if (node.InnerText.ToLower().Contains("waiting"))
                    {
                        waiting++;
                    }
                    else if (node.InnerText.ToLower().Contains("100%") || node.InnerText.ToLower().Contains("100 %"))
                    {
                        Uploaded++;
                    }
                    else if (node.InnerText.ToLower().Contains("uploaded"))
                    {
                        Uploading++;
                    }
                }
                if (ScheduledGet is not null)
                {
                    int Scheduled = ScheduledGet.Invoke();
                    Dispatcher.Invoke(() =>
                    {
                        lblWaiting.Content = waiting.ToString();
                        lblTotal.Content = Scheduled.ToString();
                        lblUploading.Content = Uploading.ToString();
                        lblUploaded.Content = Uploaded.ToString();
                    });
                    SetMargin(StatusBar);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"NodeUpdate {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }
        private void SendKeys_Tick(object? sender, EventArgs e)
        {
            try
            {
                InternalTimer.Stop();
                if (SendKeysString != "")
                {
                    HasExited = true;
                    if (CycleThroughChildWindows())
                    {
                        ExitDialog = false;
                        UploadV2Files(true);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SendKeys_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("WebWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("WebHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("Webleft", Left).ToDouble();
                var _top = key.GetValue("Webtop", Top).ToDouble();
                string rootfolder = FindUploadPath();
                if (Directory.Exists(rootfolder))
                {
                    DirectoryPath = rootfolder.Split(@"\").ToList().LastOrDefault();
                }
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                lstMain.Width = Width - 5;
                var thick = new Thickness(0, 0, 0, 0);
                thick.Left = Width - 190;
                if (ScraperType == EventTypes.ScapeSchedule)
                {
                    lblInsert.Content = "Files in DB";
                    lblUp.Content = "Max Scraped";
                    lblInsertId4.Content = "Current Page:";
                    SetMargin(StatusBar);
                }
                key?.Close();
                if (Parent is MainWindow mainWindow)
                {
                    var r = new CustomParams_SetTimeSpans(null, null);
                    Invoker?.Invoke(this, r);
                }
                wv2.CoreWebView2InitializationCompleted += (sender, args) =>
                {
                    ActiveWebView[1].Source = new Uri(webAddressBuilder.GetChannelURL().Address);
                };
                string connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                int TitleId = -1, DescId = -1, idr = -1;
                connectionString.ExecuteReader("SELECT * FROM REMATCHED", (FbDataReader r) =>
                {
                    RematchedList.Add(new Rematched(r));
                });
                connectionString.ExecuteReader("SELECT * FROM SHORTSDIRECTORY", (FbDataReader r) =>
                {
                    ShortsDirectoriesList.Add(new ShortsDirectory(r));
                });
                List<(int, int)> ErrorList = new List<(int, int)>() { (47, 62), (48, 64), (49, 63) };
                foreach (var (found, title, oldid, newid) in from item in ErrorList
                                                             let found = false
                                                             let title = ""
                                                             let oldid = item.Item1
                                                             let newid = item.Item2
                                                             select (found, title, oldid, newid))
                {
                    foreach (var itemi in ShortsDirectoriesList.Where(s => s.Id == newid))
                    {
                        if (!RematchedList.Any(s => s.OldId == oldid))
                        {
                            string sql = "INSERT INTO REMATCHED (OLDID, NEWID, DIRECTORY) VALUES (@OID, @NID, @DIRECTORY) RETURNING ID;";
                            int idex = connectionString.ExecuteScalar(sql, [("@OID", oldid), ("@NID", newid),
                            ("@DIRECTORY", itemi.Directory)]).ToInt(-1);
                            if (idex != -1)
                            {
                                sql = "SELECT * FROM REMATCHED WHERE ID = @ID;";
                                connectionString.ExecuteReader(sql, [("ID", idex)], (FbDataReader r) =>
                                {
                                    RematchedList.Add(new Rematched(r));
                                });
                            }
                        }
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    InitAsync();
                });
                Width--;
                Width--;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async void ProcessWV2Completed_ShortsScheduler(string html, object sender)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if (html is not null)
                {
                    if (TimerSimulate is not null)
                    {
                        TimerSimulate.Start();
                    }
                    var ehtml = Regex.Unescape(html);
                    if (ehtml is not null)
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(Regex.Unescape(html)); // Replace yourHtmlContent with the actual HTML content
                        var dpd = Regex.Unescape(html);
                        string Span_Name = "page-description style-scope ytcp-table-footer";
                        HtmlNode targetSpan = doc.DocumentNode.SelectSingleNode($"//span[@class='{Span_Name}']");
                        if (targetSpan is not null && targetSpan.InnerText is not null && targetSpan.InnerText != "")
                        {
                            ProcessNode(doc, targetSpan, sender);
                        }
                        else
                        {
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                System.Windows.Forms.Application.DoEvents();
                                Thread.Sleep(100);
                            }
                            html = Regex.Unescape(await (sender as WebView2CompositionControl).ExecuteScriptAsync("document.body.innerHTML"));
                            ProcessWV2Completed_ShortsScheduler(html, sender);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWV2Completed_ShortsScheduler {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private bool DoNodeScrapeUpdate(string Id, string Title, string Desc, string FileName, string status, DateTime? dateTime)
        {
            try
            {
                string t = $"{Title.Replace("\n", "").Replace("\r", "").Trim()} {Id}";
                lstMain.Items.Insert(0, t);
                string newtitle = Title.Replace("\n", "").Replace("\r", "").Trim();
                var spliter = newtitle.Split(' ').ToList();
                newtitle = string.Join(" ", spliter.Where(item => !item.StartsWith("#"))).Trim();
                titles.Add(newtitle);
                Ids.Insert(0, Id);
                Invoker?.Invoke(this, new CustomParams_AddVideoInfo(null, VStatusType, Id, Title,
                FileName, -1, false));
                Id = (Id.Contains("webp/")) ? Id.Replace("webp/", "") : Id;
                webAddressBuilder.ScopeVideo(Id, true);
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoNodeScrapeUpdate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return true;
            }
        }


        private void ProcessNode(HtmlDocument doc, HtmlNode targetSpan, object sender = null)
        {
            try
            {
                IdNodes.Clear();
                if (targetSpan is not null && targetSpan.InnerText != null && targetSpan.InnerText != "")
                {
                    bool fnd2 = false;
                    LastNode = targetSpan.InnerText;
                    if (MaxNodes == -1)
                    {
                        List<string> nodesinfo = LastNode.Split(' ').ToList();
                        if (nodesinfo.Count > 1)
                        {
                            MaxNodes = (nodesinfo.LastOrDefault() is string sp) ? sp.ToInt() : -1;
                        }
                    }
                    Dispatcher.Invoke(() =>
                    {
                        lblInsertId5.Content = LastNode;
                        if (ScraperType == EventTypes.ScapeSchedule)
                        {
                            lblUploading.Content = MaxNodes.ToString();
                        }
                    });
                    bool timeractive = false;
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    // timer event trigger
                    timer.Tick += (s, e) =>
                    {
                        timeractive = true;
                        timer.Stop();
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SimulateWheelUpDown(wv2);
                        }));
                        timeractive = false;
                    };
                    string divclassname = "right-section style-scope ytcp-video-list-cell-video";
                    string idclass = "style-scope ytcp-img-with-fallback";
                    string idNode = "style-scope ytcp-img-with-fallback";
                    List<HtmlNode> idNodes = doc.DocumentNode.SelectNodes($"//img[@class='{idNode}']").ToList();
                    List<HtmlNode> targetSpanList = doc.DocumentNode.SelectNodes($"//div[@class='{divclassname}']").ToList();
                    string StatusNodeStr = "label-span style-scope ytcp-video-row";
                    List<HtmlNode> StatusNode = doc.DocumentNode.SelectNodes($"//span[@class='{StatusNodeStr}']").ToList();
                    string DateNodeStr = " cell-body tablecell-date sortable column-sorted  style-scope ytcp-video-row";
                    //ist<HtmlNode> DateNode = doc.DocumentNode.SelectNodes($"//div[@class='{DateNodeStr}']").ToList();
                    string aclassname = " remove-default-style  style-scope ytcp-video-list-cell-video";

                    timer.Start();
                    for (int i = 0; i < targetSpanList.Count; i++)
                    {
                        if (TimedOut || canceltoken.IsCancellationRequested) break;
                        TimerSimulate.Start();
                        var item = targetSpanList[i];
                        string fid = "";
                        foreach (var attr in item.ChildNodes.Where(child => child.Name == "h3").
                            SelectMany(child => child.ChildNodes.Where(child2 => child2.Name == "a").
                              SelectMany(child2 => child2.Attributes.Where(attr => attr.Name == "href"))))
                        {
                            fid = attr.Value;
                            break;
                        }
                        var node = idNodes[i].OuterHtml;
                        HtmlDocument docxx = new HtmlDocument();
                        docxx.LoadHtml(node);
                        string idNode2 = "style-scope ytcp-img-with-fallback";
                        string Id = fid.Replace("/video/", "").Replace("/edit", "");
                        if (Id == "")
                        {
                            var fullid = docxx.DocumentNode.SelectSingleNode($"//img[@class='{idNode2}']").GetAttributeValue("src", "");
                            int idx1 = fullid.IndexOf(@"/mqdefault.");
                            int idx2 = fullid.IndexOf(@"com/vi");
                            if (idx1 != -1 && idx2 != -1)
                            {
                                Id = fullid.Substring(idx2 + 7, idx1 - idx2 - 7);
                            }
                            else
                            {
                                idx1 = fullid.IndexOf(@"/mq2.jpg");
                                idx2 = fullid.IndexOf(@"com/vi");
                                if (idx1 != -1 && idx2 != -1)
                                {
                                    Id = fullid.Substring(idx2 + 7, idx1 - idx2 - 7);
                                }
                            }
                        }
                        string divlass = " remove-default-style  style-scope ytcp-video-list-cell-video";

                        string html2 = item.OuterHtml; ;
                        HtmlDocument doc2 = new HtmlDocument();
                        doc2.LoadHtml(html2);


                        if (Id != "")
                        {
                            IdNodes.Add(Id);
                            string StatusStr = StatusNode[i].InnerText.Trim();
                            Nullable<DateTime> dateTime = null;
                            if (ScraperType == EventTypes.ShortsSchedule)
                            {
                                TitleStr = "";
                                DescStr = "";

                                // grab video filename. await till its got it
                                string gUrl2 = $"https://studio.youtube.com/video/{Id}/edit";
                                var vidoeid = Invoker.InvokeWithReturn<string>(this, new CustomParams_SelectById(Id));
                                if (vidoeid is null)
                                {
                                    vidoeid = "";
                                }
                                WaitingFileName = vidoeid == "";
                                if (!WaitingFileName)
                                {
                                    bool fnd = false;
                                    string newid = "";
                                    DateTime q = DateTime.Now;
                                    if (vidoeid.Contains('_'))
                                    {
                                        newid = vidoeid.Split('_').LastOrDefault().Trim();
                                    }
                                    else newid = "93";
                                    int newidint = newid.ToInt(-1);
                                    int oldid = newidint;
                                    foreach (var itx in RematchedList.Where(
                                        s => s.OldId == oldid))
                                    {
                                        newidint = itx.NewId;
                                    }
                                    double totalms = (DateTime.Now - q).TotalMilliseconds;
                                    lblWaiting.Content = $"{totalms.ToString("0.00")} ms";
                                    GetTitlesAndDesc(newidint);
                                    TimerSimulate.Start();
                                }
                                else
                                {
                                    wv2A10.NavigationCompleted += wv2_NavigationCompleted_GetFileName;
                                    lookups.Add(gUrl2);
                                    wv2A10.Source = new Uri(gUrl2);
                                    var cts = new CancellationTokenSource();
                                    cts.CancelAfter(TimeSpan.FromSeconds(20000));
                                    TimedOut = false;
                                    string oldtitle = TitleStr;
                                    DateTime q = DateTime.Now;
                                    while (WaitingFileName && !cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                                    {
                                        Thread.Sleep(200);
                                        System.Windows.Forms.Application.DoEvents();
                                        var cys = new CancellationTokenSource();
                                        cys.CancelAfter(TimeSpan.FromMilliseconds(200));
                                        while (!cys.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                                        {
                                            Thread.Sleep(20);
                                            System.Windows.Forms.Application.DoEvents();
                                        }
                                    }
                                    double totalms = (DateTime.Now - q).TotalMilliseconds;
                                    lblInsertId5.Content = $"{totalms.ToString("0.00")} ms";
                                }

                                if (TimedOut || canceltoken.IsCancellationRequested)
                                {
                                    lstMain.Items.Insert(0, $"Timeout on getting filename");
                                    break;
                                }

                                bool Ok = false;
                                if (directshortsScheduler is not null)
                                {
                                    var HandlerOk = directshortsScheduler.ScheduleVideo(Id, TitleStr, DescStr, false);
                                    DoNextNode = HandlerOk == FinishType.Scheduled;
                                    if (HandlerOk == FinishType.Error)
                                    {
                                        bool IsTaskCanceled = false;
                                        for (int ix = 0; ix < Math.Min(11, lstMain.Items.Count); ix++)
                                        {
                                            if (lstMain.Items[ix] is string rx && (rx.ToLower().ContainsAny(new List<string> { "task", "canceled", "ssl", "connection" })))
                                            {
                                                IsTaskCanceled = true;
                                                break;
                                            }
                                        }
                                        if (!IsTaskCanceled)
                                        {
                                            lstMain.Items.Insert(0, $"Error on Scheduling Detected. {Id}");
                                            string webAddress = $"https://studio.youtube.com/video/{Id}/edit";
                                            System.Windows.Clipboard.SetText(webAddress);
                                            canceltoken.Cancel();
                                            break;
                                        }
                                        else
                                        {
                                            DoScheduleTaskCancel();
                                        }
                                    }
                                    else if (HandlerOk == FinishType.LookUpError)
                                    {
                                        lstMain.Items.Insert(0, $"Lookup Error on Scheduling Detected.");
                                        string webAddress = $"https://studio.youtube.com/video/{Id}/edit";
                                        System.Windows.Clipboard.SetText(webAddress);
                                        canceltoken.Cancel();
                                        break;
                                    }
                                    else if (HandlerOk == FinishType.GapTimeZero)
                                    {
                                        lstMain.Items.Insert(0, $"Gap Is Zero Error on Scheduling Detected.");
                                        canceltoken.Cancel();
                                        break;
                                    }
                                    else if (HandlerOk == FinishType.Finished)
                                    {
                                        lstMain.Items.Insert(0, $"Scheduling Complete.");
                                        canceltoken.Cancel();
                                        break;
                                    }
                                }
                                else if (directshortsScheduler is not null)
                                {
                                    TimedOut = true;
                                    lstMain.Items.Insert(0, $"Timeout on Scheduling Detected.");
                                    DoNextNode = false;
                                }
                            }
                            else
                            {
                                if (DoNextNode && (ScraperType == EventTypes.ShortsSchedule ||
                                    ScraperType == EventTypes.ScrapeDraftSchedules))
                                {
                                    DoNextNode = DoNodeScrapeUpdate(Id, TitleStr, DescStr, "", StatusStr, dateTime);
                                    if (ScraperType == EventTypes.ScrapeDraftSchedules)
                                    {
                                        var vid = Invoker.InvokeWithReturn<string>(this, new CustomParams_SelectById(Id));
                                        int LinkedId = -1;
                                        if (vid is string v && v.Contains("_"))
                                        {
                                            LinkedId = v.Split('_')[1].ToInt(-1);
                                        }

                                        Invoker?.Invoke(this, new CustomParams_ProcessTargets(Id, LinkedId, TitleStr, DescStr));
                                    }
                                }
                                else if (DoNextNode && Id != "" && ScraperType == EventTypes.ScapeSchedule)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        if (!lstMain.Items.Contains($"{Id} "))
                                        {
                                            lstMain.Items.Insert(0, $"{Id} ");
                                        }
                                    });
                                    var vid = Invoker.InvokeWithReturn<string>(this, new CustomParams_SelectById(Id));
                                    if (vid is string v && v != "")
                                    {
                                        files++;
                                        dbfiles++;
                                        int lstCnt = lstMain.Items.Count;
                                        foreach (var itemss in lstMain.Items)
                                        {
                                            if (itemss.ToString().Contains("_"))
                                            {
                                                lstCnt--;
                                                break;
                                            }
                                        }

                                        if (lstCnt == 0 && lstMain.Items.Count == MaxNodes)
                                        {
                                            var _DoAutoCancel = new AutoCancel(DoAutoCancelCloseScraper, $"Scraped {files} Files", 5, "Scraper Finished");
                                            _DoAutoCancel.ShowActivated = true;
                                            _DoAutoCancel.Show();
                                        }
                                        else if (lstMain.Items.Count == MaxNodes)
                                        {
                                            CloseScrape.Interval = TimeSpan.FromSeconds(10);
                                            CloseScrape.Tick += (s, e) =>
                                            {
                                                CloseScrape.Stop();
                                                canceltoken.Cancel();
                                                cancelds();
                                                TimedOutClose = true;
                                                Close();
                                            };
                                            CloseScrape.Start();
                                        }
                                        if (files > 0)
                                        {
                                            lblLastNode.Content = $"{files} Processed";

                                        }
                                        if (dbfiles > 0)
                                        {
                                            lblTotal.Content = $"{dbfiles}";
                                            SetMargin(StatusBar);
                                        }

                                        SetMargin(StatusBar);
                                    }
                                    else webAddressBuilder.ScopeVideo(Id, true);

                                }
                            }

                            if (true)
                            {

                            }

                        }
                    }
                    if (LastNode != "")
                    {
                        bool processnextnode = false;
                        List<string> nodes = LastNode.Split(' ').ToList();
                        for (int xi = nodes.Count - 1; xi >= 0; xi--)
                        {
                            if ((nodes[xi].Contains("of")) || (nodes[xi].Contains("about")))
                            {
                                nodes.RemoveAt(xi);
                            }
                        }
                        string Range = nodes.FirstOrDefault();

                        string MaxCnt = nodes.LastOrDefault().Replace(",", "").Trim();
                        int iCnt = MaxCnt.ToInt(-1);
                        string CntNow = Range.Split('–').LastOrDefault();
                        int iCntNow = CntNow.ToInt(-1);
                        if (iCntNow > 0 && iCnt > 0)
                        {
                            processnextnode = (iCntNow < iCnt) ? true : processnextnode;
                        }
                        if (processnextnode && DoNextNode)
                        {
                            int nodesc = 0;
                            nodeslist.Clear();
                            if (directshortsScheduler is null ||
                                !(directshortsScheduler.ScheduleNumber + 1 >= directshortsScheduler.MaxNumberSchedules ||
                                TimedOut))
                            {
                                btnNext_Task(sender);
                                string ehtml = "";
                                while (true && !canceltoken.IsCancellationRequested)
                                {
                                    var cts = new CancellationTokenSource();
                                    cts.CancelAfter(TimeSpan.FromMilliseconds(400));
                                    while (!cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                                    {
                                        Thread.Sleep(200);
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                    var task = wv2.ExecuteScriptAsync("document.body.innerHTML");
                                    task.ContinueWith(x => { ehtml = Regex.Unescape(x.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
                                    if (!ehtml.Contains(LastNode) && ehtml != "") break;
                                }
                                NextRecord = false;
                                var task1 = (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");

                                task1.ContinueWith(x =>
                                {
                                    ProcessWV2Completed_ShortsScheduler(x.Result, sender);
                                }, TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        }
                        else
                        {
                            if (lstMain.Items.Count > 0)
                            {
                                int lstCnt2 = lstMain.Items.Count;
                                foreach (var item in lstMain.Items.OfType<string>().Where(item => item.Contains("_")))
                                {
                                    lstCnt2--;
                                }

                                if (lstCnt2 == 0 && lstMain.Items.Count == MaxNodes)
                                {
                                    var _DoAutoCancel = new AutoCancel(DoAutoCancelCloseScraper, $"Scraped {files} Files", 5, "Scraper Finished");
                                    _DoAutoCancel.ShowActivated = true;
                                    _DoAutoCancel.Show();
                                }
                                else if (lstMain.Items.Count == MaxNodes)
                                {
                                    CloseScrape.Interval = TimeSpan.FromSeconds(10);
                                    CloseScrape.Tick += (s, e) =>
                                    {
                                        CloseScrape.Stop();
                                        canceltoken.Cancel();
                                        cancelds();
                                        TimedOutClose = true;
                                        Close();
                                    };
                                    CloseScrape.Start();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessNode {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void DoAutoCancelCloseScraper(object sender, int i)
        {
            try
            {
                if (sender is AutoCancel frm)
                {
                    if (frm.IsCloseAction)
                    {
                        canceltoken.Cancel();
                        cancelds();
                        Close();
                        return;
                    }

                    frm = null;
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoAutoCancelCloseScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        string LastIdNode = "";
        int MaxCnts = -1;
        private async Task btnNext_Task(object sender)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnNext_Task(sender);
                    });
                    return;
                }
                string script = @"
        var nextButton = document.getElementById('navigate-after');
        if (nextButton) {
            nextButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            nextButton.click();
        }
    ";

                var webView2 = sender as WebView2CompositionControl;
                //webView2.NavigationCompleted += wv2v_NavigationCompleted;
                await webView2.ExecuteScriptAsync(script);

            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnNext_Task {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private async void ProcessWV2Completed(string html, object sender)
        {
            try
            {
                if (html is not null)
                {
                    var ehtml = Regex.Unescape(html);
                    if (ehtml is not null)
                    {

                        SendTraceInfo?.Invoke(this, $"ProcessWV2Completed step1");
                        string Span_Name = "row style-scope ytcp-multi-progress-monitor";
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(ehtml);
                        if (!finished && ehtml is not null && ehtml.Contains("close"))
                        {
                            SendTraceInfo?.Invoke(this, $"ProcessWV2Completed step2");
                            var cts1 = new CancellationTokenSource();
                            cts1.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts1.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            SendTraceInfo?.Invoke(this, $"ProcessWV2Completed step3");
                            Click_Finish();
                            cts1.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts1.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            //                     Click_Upload();
                            SendTraceInfo?.Invoke(this, $"ProcessWV2Completed step4 Exiting");
                            return;
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            ProcessWebViewComplete(sender);
                            return;
                        }
                        return;
                        List<HtmlNode> Nodes = doc.DocumentNode.SelectNodes($"//li[@class='{Span_Name}']").ToList();
                        var uploadedDictionary = Scraper_uploaded.ToDictionary(v => v.file);
                        var processedFiles = new HashSet<string>();

                        Parallel.ForEach(Nodes, item =>
                        {
                            var p = item.InnerText;
                            var pl = p.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).Select(p => p.Trim()).ToList();

                            if (pl.Count > 0 && processedFiles.Add(pl[0]))
                            {
                                if (pl.Contains("Daily upload limit reached"))
                                {
                                    if (uploadedDictionary.TryGetValue(pl[0], out var uploadedFile))
                                    {
                                        uploadedFile.uploaded = false;
                                        uploadedFile.finished = false;
                                    }
                                }
                                else if (pl.Contains("100% uploaded"))
                                {
                                    if (uploadedDictionary.TryGetValue(pl[0], out var uploadedFile))
                                    {
                                        uploadedFile.finished = true;
                                        uploadedFile.uploaded = true;
                                    }
                                }
                            }
                        });
                        /*
                         search = 'id="progress-title-' && '</span' && id="progress-status-" same id && '</span>'

                         */
                    }
                    if (ehtml is not null && ehtml.Contains(">Uploads complete</span>"))
                    {
                        lblLastNode.Content = "Uploads Completed";
                        foreach (var file in Scraper_uploaded.Where(file => !file.uploaded && file.finished))
                        {
                            File.Delete(file.file);
                        }
                    }
                    else
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromMilliseconds(1500));
                        while (!cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                        {
                            Thread.Sleep(100);
                        }
                        ProcessWebViewComplete(sender);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWV2 {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        bool InsertAtZero = false;
        private async void ProcessWV2(string html, object sender)
        {
            try
            {
                if (html is not null)
                {
                    var ehtml = Regex.Unescape(html);
                    if (html is not null && ehtml.Contains("Customize channel"))
                    {
                        Processing = !YouTubeLoaded();  // Load Default URL.(DefaultUrl)
                    }
                    else if (ehtml is not null && ehtml.Contains("Channel dashboard"))
                    {
                        if (ScraperType == EventTypes.ShortsSchedule ||
                            ScraperType == EventTypes.VideoLookup ||
                            ScraperType == EventTypes.ScapeSchedule)
                        {
                            if (ehtml.Contains("@JustinsTrainJourneys"))
                            {
                                InsertAtZero = true;
                                YouTubeLoaded();
                            }
                        }
                        else if (clickupload && ScraperType == EventTypes.VideoUpload)
                        {
                            wv2.AllowDrop = true;
                            clickupload = false;
                            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            string _dest = FindUploadPath();
                            key.Close();
                            UploadPath = _dest;
                            int r = DoUploadsCnt();
                            if (r < 200)
                            {
                                var UploadTask = UploadV2Files(false);
                            }
                            else
                            {
                                UploadsTimer.Change(TimeSpan.FromMinutes(5).TotalMilliseconds.ToInt(0), Timeout.Infinite);
                            }
                        }
                        else if (IsTitleEditor && ScraperType != EventTypes.ScapeSchedule)
                        {
                            ContentClick();
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromSeconds(10));
                            while (!cts.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                //class="style-scope ytcp-settings-dialog">Settings<
                                Thread.Sleep(100);
                                var htmlx = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                if (htmlx.Contains("class=\"style-scope ytcp-settings-dialog\">Settings<"))
                                {
                                    break;
                                }
                            }
                            CloseTimer.Interval = TimeSpan.FromSeconds(3);
                            CloseTimer.Tick += CloseTimer_Tick;
                            CloseTimer.Start();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWV2 {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void CloseTimer_Tick(object? sender, EventArgs e)
        {
            CloseTimer.Stop();
            CloseTimer_TickAsync(sender, e).ConfigureAwait(false);
        }


        public int DoUploadsCnt()
        {
            try
            {
                List<DateTime> Dateslist = new List<DateTime>();
                DateTime Opt = new DateTime(1900, 1, 1);
                string sqla = "SELECT * FROM UPLOADSRECORD u WHERE u.UPLOAD_DATE >= current_date -1 AND UPLOADTYPE = 0 " +
                    "ORDER BY UPLOAD_DATE DESC FETCH FIRST 150 ROWS ONLY";
                string connectStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                connectStr?.ExecuteReader(sqla.ToUpper(), (FbDataReader r) =>
                {
                    var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : Opt;// new DateTime(1900, 1, 1);
                    var tt = (r["UPLOAD_TIME"] is TimeSpan t) ? t : TimeSpan.Zero;// new DateTime(0, 0, 0);
                    var DateA = dt.AtTime(tt);
                    var DateB = DateTime.Now;
                    var timeSpan = (DateA > DateB) ? DateA - DateB : DateB - DateA;
                    if (timeSpan.Hours < 24 && timeSpan.Days == 0)
                    {
                        Dateslist.Add(DateA);
                    }
                });
                return Dateslist.Count;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoUploadsCnt {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return -1;
            }
        }
        private async Task CloseTimer_TickAsync(object? sender, EventArgs e)
        {
            SendKeys.SendWait("{TAB}");
            await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('li.menu-item.remove-default-style.style-scope.ytcp-navigation').click()");
        }
        private async void ProcessWebViewComplete(object sender)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    ProcessWebViewComplete(sender);
                    return;
                }
                if (sender is WebView2 webView2Instance)
                {
                    var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                    if (ScraperType != EventTypes.ShortsSchedule && ScraperType != EventTypes.ScapeSchedule)
                    {
                        task.ContinueWith(x => { ProcessWV2Completed(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        task.ContinueWith(x => { ProcessWV2Completed_ShortsScheduler(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWebView {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private async void ProcessWebView(object sender)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    ProcessWebView(sender);
                    return;
                }
                if (sender is WebView2 webView2Instance)
                {
                    var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                    task.ContinueWith(x => { ProcessWV2(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWebView {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private async void ProcessWebView_Filename(object sender)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    ProcessWebView_Filename(sender);
                    return;
                }
                int Id = (sender as WebView2CompositionControl).Name.Replace("wv2A", "").ToInt(-1);
                string source = (sender as WebView2CompositionControl).Source.AbsoluteUri.ToString(), IntId = "";
                int p1 = source.IndexOf("video/"), p2 = source.IndexOf("/edit");
                IntId = (p1 != -1 && p2 != -1) ? source.Substring(p1 + 6, p2 - p1 - 6) : IntId;
                var task = (sender as WebView2CompositionControl).ExecuteScriptAsync("document.body.innerHTML");
                task.ContinueWith(x => { ProcessHTML_Filename(x.Result, Id, IntId, sender); },
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWebView {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void ProcessHTML_Filename(string html, int id, string IntId, object sender)
        {
            try
            {
                string filename = "";
                if (html is not null)
                {
                    string ehtml = Regex.Unescape(html);
                    if (ehtml is not null && ehtml != "")
                    {
                        if (ehtml.Contains(IntId))
                        {
                            string Search = "div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">";
                            if (ehtml.Contains("div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">"))
                            {
                                int index = ehtml.IndexOf("div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">");
                                html = ehtml.Substring(index + Search.Length, 1500);
                                if (html.Contains('<'))
                                {
                                    index = html.IndexOf("<");
                                    html = html[..(index - 1)];
                                    filename = html.Replace("\n", "").Replace("\r", "").Trim();
                                    if (filename.Contains("_") && IsVideoLookup && ScraperType == EventTypes.ShortsSchedule)
                                    {
                                        string idp = filename.Split("_").ToArray<string>().ToList().LastOrDefault().Trim();
                                        if (idp is not null && idp != "")
                                        {
                                            bool fnd = false;
                                            string newid = idp.Split('_').LastOrDefault().Trim();
                                            newid = newid.Split('.').First().Trim();
                                            foreach (var itx in RematchedList.Where(s => s.OldId == 47))
                                            {
                                                fnd = true;
                                                break;
                                            }
                                            if (!fnd) RematchedList.Add(new Rematched { OldId = 47, NewId = 62 });
                                            fnd = false;
                                            foreach (var itx in RematchedList.Where(s => s.OldId == 49))
                                            {
                                                fnd = true;
                                                break;
                                            }
                                            if (!fnd) RematchedList.Add(new Rematched { OldId = 49, NewId = 63 });
                                            int newidint = newid.ToInt(-1);
                                            int oldid = newidint;
                                            foreach (var itx in RematchedList.Where(
                                                s => s.OldId == oldid))
                                            {
                                                newidint = itx.NewId;
                                            }
                                            GetTitlesAndDesc(newidint);
                                            WaitingFileName = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(500);
                            if (canceltoken.IsCancellationRequested) return;
                            if (wv2Dictionary.ContainsKey(id))
                            {
                                var webView2Instance = wv2Dictionary[id];
                                var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                                task.ContinueWith(x => { ProcessHTML_Filename(x.Result, id, IntId, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                                return;
                            }
                        }
                    }
                    if (filename == "")
                    {
                        Thread.Sleep(500);
                        if (canceltoken.IsCancellationRequested) return;
                        if (wv2Dictionary.ContainsKey(id))
                        {
                            var webView2Instance = wv2Dictionary[id];
                            var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                            task.ContinueWith(x => { ProcessHTML_Filename(x.Result, id, IntId, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessHTML_Filename {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void GetTitlesAndDesc(int id)
        {
            try
            {
                string connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                int TitleId = -1, DescId = -1, idr = -1;
                if (LastId == -1 || LastId != id)
                {
                    var tds = Invoker.InvokeWithReturn<TurlpeDualString>(this, new CustomParams_GetShortsDirectoryById(id));// is TurlpeDualString tds)
                    if (tds is not null)
                    {
                        TitleStr = tds.turlpe1;
                        if (tds.turlpe2 is not null && tds.turlpe2 != "")
                        {
                            DescStr = CleanUpDesc(tds.turlpe2);
                        }
                        idr = tds.Id;
                    }
                    else
                    {
                        if (true)
                        {

                        }
                    }
                    if (idr != -1)
                    {
                        if (TitleStr.NotNullOrEmpty() && DescStr.NotNullOrEmpty())
                        {
                            TitleStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetTitle(id, TitleId));
                            var r = TitleStr.Split(" ").ToList<string>().Where(s => !s.StartsWith("#") &&
                            s != "" && s.ToLower() != "vline").ToList<string>();
                            foreach (var item in r)
                            {
                                TitleStr = TitleStr.Replace(item.Trim(), item.Trim().ToLower().ToPascalCase());
                                if (DescStr.Contains(item.Trim()))
                                {
                                    DescStr = DescStr.Replace(item.Trim(), item.Trim().ToLower().ToPascalCase());
                                }
                            }
                            LTitleStr = TitleStr;
                            LDescStr = DescStr;
                            LastId = idr;
                        }
                    }
                }
                else
                {
                    TitleStr = LTitleStr;
                    DescStr = LDescStr;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetTitlesData {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private string CleanUpDesc(string DescStr)
        {
            try
            {
                string a1 = "Follow me @ https://twitch.tv/justinstrainclips";
                string a2 = "Support Me On Patreon @ https://www.patreon.com/join/JustinsTrainJourneys";
                DescStr = (DescStr.Contains(a1.ToUpper())) ? DescStr.Replace(a1.ToUpper(), a1) : DescStr;
                DescStr = (DescStr.Contains(a2.ToUpper())) ? DescStr.Replace(a2.ToUpper(), a2) : DescStr;
                string dateStr = DescStr.Replace("\n", " ").Replace("\r", " ")
                     .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(item => item.All(char.IsDigit) && item.Length == 6);
                int idxp = DescStr.IndexOf(dateStr);
                if (idxp != -1)
                {
                    string rDescStr = DescStr.Substring(0, idxp).Trim();
                    string pDesc = rDescStr.ToPascalCase();
                    DescStr = (pDesc != rDescStr) ? DescStr.Replace(rDescStr, pDesc, StringComparison.Ordinal) : DescStr;
                }
                return DescStr;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CleanUpDesc {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return DescStr;
            }
        }
        private async void wv2_NavigationCompleted_GetFileName(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if ((e is not null && e.IsSuccess) || e is null)
                {
                    NextRecord = false;
                    ProcessWebView_Filename(sender);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"wv2_NavigationCompleted_GetFileName {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async void wv2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if ((e is not null && e.IsSuccess) || e is null)
                {
                    NextRecord = false;
                    ProcessWebView(sender);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"wv2_NavigationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async void wv2v_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if ((e is not null && e.IsSuccess) || e is null)
                {
                    NextRecord = false;

                    if (sender is WebView2 webView2Instance)
                    {
                        string Urlx = webView2Instance.Source.ToString();
                        if (Urlx is not null)
                        {

                        }
                    }
                    var task = (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");

                    task.ContinueWith(x => { ProcessWV2Completed_ShortsScheduler(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"wv2_NavigationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void SetMargin(object statusbar, int offset = 78)
        {
            try
            {
                if (statusbar is StatusBar statusBar)
                {
                    double sumWidths = statusBar.Items.OfType<FrameworkElement>().Sum(fe => fe.ActualWidth);
                    var closeButton = statusBar.Items.OfType<Button>().FirstOrDefault(btn => btn.Name.Equals("btnclose", StringComparison.OrdinalIgnoreCase));
                    if (closeButton != null)
                    {
                        closeButton.Margin = new Thickness(statusBar.Width - sumWidths - offset, 0, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetMargin {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        async void NextTask()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        NextTask();
                    });
                    return;
                }
                string script = @"
        var nextButton = document.getElementById('navigate-after');
        if (nextButton) {
            nextButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            nextButton.click();
        }
    ";
                for (int i = VideoFiles.Count - 1; i >= 0; i--)
                {
                    string filename = VideoFiles[i];
                    nextaddress.Add(filename);
                    VideoFiles.RemoveAt(i);
                }

                await ActiveWebView[1].ExecuteScriptAsync(script);
                wv2_NavigationCompleted(wv2, null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"NextTask {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        async void UploadsClick()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        UploadsClick();
                    });
                    return;
                }
                string script = @"
        var nextButton = document.getElementById('Uploads');
        if (nextButton) {
            nextButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            nextButton.click();
        }
    ";
                await ActiveWebView[1].ExecuteScriptAsync(script);

            }
            catch (Exception ex)
            {
                ex.LogWrite($"NextTask {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        async void ContentClick()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        ContentClick();
                    });
                    return;
                }
                string script = @"
        var nextButton = document.getElementById('settings-item');
        if (nextButton) {
            nextButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            nextButton.click();
        }
    ";
                await ActiveWebView[1].ExecuteScriptAsync(script);

            }
            catch (Exception ex)
            {
                ex.LogWrite($"NextTask {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancelds();
                canceltoken.Cancel();

                if (SwapEnabled)
                {
                    WebView2CompositionControl temp = brdmain.Child as WebView2CompositionControl;
                    temp.NavigationCompleted += Wv2s_NavigationCompleted;
                    wv2Dictionary[swap].NavigationCompleted += wv2_NavigationCompleted;
                    brdmain.Child = wv2Dictionary[swap] as WebView2CompositionControl;
                    wv2Dictionary[swap] = temp;
                    if (swap < wv2Dictionary.Count)
                    {
                        ActiveWebView.Clear();
                        swap++;
                        ActiveWebView.Add(1, wv2Dictionary[swap]);
                        ActiveWebView[1].Source = new Uri(webAddressBuilder.GetChannelURL().Address);
                    }
                }
                else
                {
                    KilledUploads = true;
                    await BuildFiles();

                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async Task BuildFiles()
        {
            try
            {
                if (ScraperType == EventTypes.UploadTest)
                {
                    foreach (var item in ScheduledFiles)
                    {
                        if (canceltoken.IsCancellationRequested) break;
                        if (item.VideoId == "")
                        {
                            await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('button[aria-label=\"{item.FileName}\"]').click()");
                            var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            //https://youtube.com/shorts/xYtfpf3CPoc
                            if (html.Contains("https://youtube.com/shorts/"))
                            {
                                var link = Regex.Match(html, "https://youtube.com/shorts/([a-zA-Z0-9\\-_]+)").Groups[1].Value;
                                if (link is not null && link != "")
                                {
                                    IsVideoLookup = true;
                                    DoNewVideoUpdate(webAddressBuilder.ScopeVideo(link).ScopeAddress);
                                }
                            }
                            else if (html.Contains("https://youtu.be/"))
                            {
                                var index = html.IndexOf("https://youtu.be/");
                                int len = "https://youtu.be/".Length;
                                string vid = html.Substring(index + len, 11);
                                DoNewVideoUpdate(webAddressBuilder.ScopeVideo(vid).ScopeAddress);
                            }
                        }
                        else
                        {
                            IsVideoLookup = true;
                            string gUrl = webAddressBuilder.ScopeVideo(item.VideoId).ScopeAddress;
                            DoNewVideoUpdate(gUrl);

                        }
                    }
                    while (Waiting && IsVideoLookup)
                    {
                        Thread.Sleep(100);
                        System.Windows.Forms.Application.DoEvents();
                        if (canceltoken.IsCancellationRequested) break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BuildFiles {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void wv2_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(this);
                lblInsert.Content = pos.ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"");
            }
        }
        public async void Select_Upload()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        Select_Upload();
                        return;
                    });
                }
                string script = @"
        var createButton = document.getElementById('sublabel style-scope ytcp-uploads-file-picker');
        if (createButton) {
            createButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            createButton.click();
        }
    ";
                await ActiveWebView[1].ExecuteScriptAsync(script);
                wv2_NavigationCompleted(wv2, null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Select_Upload {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async void Click_Finish()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        Click_Finish();
                    });
                    return;
                }
                string script = @"
        var createButton = document.getElementById('close-button');
        if (createButton) {
            createButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            createButton.click();
        }
    ";
                await ActiveWebView[1].ExecuteScriptAsync(script);
                wv2_NavigationCompleted(wv2, null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Click_Finish {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async void Click_Upload()
        {
            try
            {

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        Click_Upload();
                        return;
                    });
                }
                string script = @"
        var createButton = document.getElementById('upload-icon');
        if (createButton) {
            createButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            createButton.click();
        }
    ";
                await ActiveWebView[1].ExecuteScriptAsync(script);
                wv2_NavigationCompleted(ActiveWebView[1], null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Click_Upload {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async void Close_Upload(WebView2CompositionControl wv2)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        Close_Upload(wv2);
                        return;
                    });
                }
                // Execute script to click the second button with the ID "close-button"
                await wv2.CoreWebView2.ExecuteScriptAsync(@"
                var buttons = document.querySelectorAll('#close-button');
                if (buttons.length > 1) {
                    buttons[1].click();
                }
            ");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Click_Upload {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public bool YouTubeLoaded()
        {
            try
            {
                if (webAddressBuilder is not null)
                {
                    if (ScraperType != EventTypes.VideoLookup)
                    {
                        if (ScraperType == EventTypes.ShortsSchedule && directshortsScheduler is null && ReleaseDate.HasValue && ReleaseEndDate.HasValue)
                        {
                            string connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                            directshortsScheduler = new DirectshortsScheduler(() => { Show(); },
                                DoOnScheduleComplete, listSchedules,
                                ReleaseDate.Value, ReleaseEndDate.Value,
                                DoReportSchedule,
                                DoScheduleTaskCancel,
                                DoQuotaExceeded,
                                ScheduleMax, canceltoken, IsTest);
                            directshortsScheduler.connectionString = connectionStr;
                        }
                        IsVideoLookup = true;
                        if (ScraperType == EventTypes.ScapeSchedule)
                        {
                            bool ready = true;
                            while (!ready)
                            {
                                foreach (var item in wv2Dictionary.Values)
                                {
                                    ready = ready && item.Tag.ToInt(-1) == 1;
                                }
                                Thread.Sleep(100);
                            }
                            DefaultUrl = TargetUrl;
                        }
                        if (DefaultUrl is not null && DefaultUrl != "")
                        {
                            ActiveWebView[1].ZoomFactor = 0.6;
                            if (ScraperType != EventTypes.VideoUpload)
                            {
                                ActiveWebView[1].NavigationCompleted += wv2v_NavigationCompleted;
                            }
                            if (ScraperType == EventTypes.VideoUpload) clickupload = true;
                            ActiveWebView[1].Source = new Uri(DefaultUrl);
                            return true;
                        }
                    }
                    else
                    {
                        bool found = false;
                        for (int i = 0; i < Directories.Count; i++)
                        {
                            if (!Directories[i].Probed)
                            {

                                webAddressBuilder.AddFilterByShorts();
                                webAddressBuilder.AddFilterByTitle(Directories[i].Directory);
                                DefaultUrl = webAddressBuilder.GetHTML();
                                Directories[i].Probed = true;
                                Directories[i].url = DefaultUrl;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            ActiveWebView[1].ZoomFactor = 0.46;
                            ActiveWebView[1].NavigationCompleted += wv2Lookup_NavigationCompleted;
                            ActiveWebView[1].Source = new Uri(DefaultUrl);
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"YouTubeLoaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }

        private void DoQuotaExceeded(string message)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        DoQuotaExceeded(message);
                    });
                    return;
                }
                ;
                if (message == "")
                {
                    QuotaExceeded = true;
                }
                else
                {
                    Errored = true;
                    message.WriteLog("QuotaExceeded");
                    lstMain.Items.Insert(0, $"Schedule Error: {message} @ {DateTime.Now}");
                }
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoQuotaExceeded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }

        private void DoScheduleTaskCancel()
        {
            try
            {
                ScheduledFinished = false;
                TaskHasCancelled = true;
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoScheduleTaskCancel {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void wv2Lookup_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (canceltoken.IsCancellationRequested) return;
            if ((e is not null && e.IsSuccess) || e is null)
            {
                NextRecord = false;
                var task = (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
                task.ContinueWith(x => { ProcessWV2Completed_VideoLookup(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        private void ProcessWV2Completed_VideoLookup(string html, object sender)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                if (html is not null)
                {
                    bool NoMatch = false;
                    var ehtml = Regex.Unescape(html);
                    if (ehtml is not null && ehtml == "")
                    {
                        ActiveWebView[1].NavigationCompleted += wv2Lookup_NavigationCompleted;
                        ActiveWebView[1].Source = new System.Uri(DefaultUrl);
                    }
                    else if (ehtml is not null)
                    {
                        if (ehtml.Contains("No matching videos") || ehtml.Contains("No matching shorts"))
                        {
                            NoMatch = true;
                        }
                        for (int i = 0; i < Directories.Count; i++)
                        {
                            if (Directories[i].url == DefaultUrl)
                            {
                                Directories[i].MatchNotFound = NoMatch;
                                Directories[i].Processed = true;
                                break;
                            }
                        }
                        string url = "";
                        for (int i = 0; i < Directories.Count; i++)
                        {
                            if (!Directories[i].Probed)
                            {
                                url = webAddressBuilder.AddFilterByTitle(Directories[i].Directory).GetHTML();
                                Directories[i].Probed = true;
                                Directories[i].url = url;
                                break;
                            }
                        }

                        if (url != "")
                        {
                            ActiveWebView[1].NavigationCompleted += wv2Lookup_NavigationCompleted;
                            ActiveWebView[1].Source = new Uri(url);
                        }
                        else
                        {
                            DoOnLookupComplete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWV2Completed_VideoLookup {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void DoOnLookupComplete()
        {
            try
            {
                cancelds();
                canceltoken.Cancel();
                var _DoAutoCancel = new AutoCancel(DoAutoCancelClose, "", 5, "Scheduling Finished");
                _DoAutoCancel.ShowActivated = true;
                _DoAutoCancel.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoOnLookupComplete {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private bool DoOnScheduleComplete()
        {
            try
            {
                if (!TimedOut && !canceltoken.IsCancellationRequested)
                {
                    bool res = false;
                    int r = directshortsScheduler.ScheduleNumber;
                    if (r >= ScheduleMax - 1) r = ScheduleMax - 1;
                    lstMain.Items.Insert(0, $"{r + 1} Schedules Complete");

                    if (Days > 1)
                    {
                        if (CurrentDay > Days - 1) return false;
                        if (directshortsScheduler is not null)
                        {
                            directshortsScheduler.ListScheduleIndex = 0;
                            DateTime StartDate = ReleaseDate.HasValue ? ReleaseDate.Value : DateTime.Now;
                            DateOnly SDate = DateOnly.FromDateTime(StartDate.Date).AddDays(CurrentDay);
                            TimeOnly STime = TimeOnly.FromDateTime(StartDate);
                            DateTime sDateTime = new DateTime(SDate, STime);
                            DateTime EndDate = ReleaseEndDate.HasValue ? ReleaseEndDate.Value : DateTime.Now;
                            DateOnly eDate = DateOnly.FromDateTime(EndDate.Date).AddDays(CurrentDay);
                            TimeOnly eTime = TimeOnly.FromDateTime(EndDate);
                            DateTime eDateTime = new DateTime(eDate, eTime);
                            directshortsScheduler?.ScheduleNewDay(sDateTime, eDateTime);
                            CurrentDay++;
                            return true;
                        }
                        return res;
                    }
                    else
                    {
                        cancelds();
                        canceltoken.Cancel();
                        HasCompleted = true;
                        Nullable<TimeSpan> st = LoadTime("ScheduleTimeStart");
                        Nullable<TimeSpan> et = LoadTime("ScheduleTimeEnd");
                        if (st.HasValue && et.HasValue)
                        {
                            st = LoadTime("ScheduleTimeStart_Default");
                            if (st is null)
                            {
                                st = new TimeSpan(11, 0, 0);
                                SaveTime(st.Value, "ScheduleTimeStart_Default");
                            }
                            SaveTime(st.Value, "ScheduleTimeStart");
                            st = LoadTime("ScheduleTimeEnd_Default");
                            if (st is null)
                            {
                                st = new TimeSpan(20, 0, 0);
                                SaveTime(st.Value, "ScheduleTimeEnd_Default");
                            }
                            SaveTime(st.Value, "ScheduleTimeEnd");
                            Nullable<DateTime> dt = LoadDate("ScheduleDate");
                            if (dt.HasValue && dt.Value.Year < 1900)
                            {
                                var yt = DateTime.Now.Year;
                                var dif = dt.Value.Year - yt;
                                dt.Value.AddYears(dif);
                                SaveDates(dt.Value.AddDays(1), "ScheduleDate");
                            }
                        }
                        var _DoAutoCancel = new AutoCancel(DoAutoCancelClose, "", 5, "Scheduling Finished");
                        _DoAutoCancel.ShowActivated = true;
                        _DoAutoCancel.Show();
                    }
                    return res;
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoOnScheduleComplete {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }
        public void SaveDates(DateTime startdate, string name, bool IsDateTime = false)
        {
            try
            {
                var connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
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
        public Nullable<DateTime> LoadDate(string name)
        {
            try
            {
                var connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
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
        private void DoAutoCancelClose(object sender, int i)
        {
            try
            {
                if (sender is AutoCancel frm)
                {
                    if (frm.IsCloseAction)
                    {
                        canceltoken.Cancel();
                        cancelds();
                        frm.Close();
                        Close();
                        return;
                    }
                    frm = null;
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoAutoCancelClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public bool HasCompleted = false;
        private void DoReportSchedule(DateTime dateTime, string id, string title)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    int r = directshortsScheduler.ScheduleNumber;
                    string t = $"{id} {title.Replace("\n", "").Replace("\r", "").Trim()}  {r + 1} {dateTime}";
                    lstMain.Items.Insert(0, t);
                    lblLastNode.Content = $"{r} Scheduled";
                    SetMargin(StatusBar);
                    System.Windows.Forms.Application.DoEvents();
                    var nextSchedule = directshortsScheduler.LastScheduledTime.
                    AddMinutes(directshortsScheduler.LastGap);
                    Nullable<TimeSpan> st = LoadTime("ScheduleTimeStart");
                    Nullable<TimeSpan> et = LoadTime("ScheduleTimeEnd");
                    if (nextSchedule.TimeOfDay > st.Value && nextSchedule.TimeOfDay < et.Value)
                    {
                        SaveTime(nextSchedule.TimeOfDay, "ScheduleTimeStart");
                        TimeOnly nexttime = TimeOnly.FromTimeSpan(nextSchedule.TimeOfDay);
                        int mins = directshortsScheduler.GetNextGap(nexttime);
                        TimeSpan nextgap = new TimeSpan(0, 0, mins);
                        SaveTime(nextgap, "NextValidGap");
                        HasCompleted = false;
                    }
                    else
                    {
                        HasCompleted = true;
                        DateTime nextsch = nextSchedule.Date.AddDays(1);

                        var stdef = LoadTime("ScheduleTimeStart_Default");
                        if (stdef.HasValue)
                        {
                            SaveDates(nextsch, "ScheduleDate");
                            var starttime = nextsch.AtTime(stdef.Value);
                            SaveTime(starttime.TimeOfDay, "ScheduleTimeStart");
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoReportSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void MenuClicker(WebView2CompositionControl webView)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => { MenuClicker(webView); });
                return;
            }
            try
            {
                webView.Dispatcher.Invoke(() =>
                {
                    webView.Focus();
                    SimulateMouseWheel(webView, true, 10);
                    System.Threading.Thread.Sleep(100);
                    SimulateMouseWheel(webView, false, 10);
                    webView.Focus();
                });

                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"MenuClicker {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        public Nullable<TimeSpan> LoadTime(string name)
        {
            try
            {
                var connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
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
                var connectionString = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
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
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    brdmain.Width = Width - 20;
                    brdmain.Height = Height - (258 - 70);
                    ActiveWebView[1].Width = brdmain.Width - 10;
                    ActiveWebView[1].Height = brdmain.Height - 13;
                    var p = new Thickness(0, 0, 0, 0);
                    p.Left = Width - 692;
                    lstMain.Width = Width - 25;
                    StatusBar.Width = Width - 5;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("WebWidth", ActualWidth);
                    key.SetValue("WebHeight", ActualHeight);
                    key.SetValue("Webleft", Left);
                    key.SetValue("Webtop", Top);
                    key?.Close();
                    SetMargin(StatusBar);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void cancelds()
        {
            try
            {
                if (directshortsScheduler is not null)
                {
                    directshortsScheduler.canceltoken.Cancel();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"cancelds {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async Task<ButtonReturnType> IsButtonEnabled(WebView2CompositionControl webView2)
        {
            try
            {
                var res = ButtonReturnType.NotPresent;
                var html = await webView2.ExecuteScriptAsync("document.body.innerHTML");
                var ehtml = Regex.Unescape(html);
                if (ehtml.Contains("<button class=\"ytcp-button-shape-impl ytcp-button-shape-impl--filled ytcp-button-shape-impl--disabled ytcp-button-shape-impl--size-m\" aria-label=\"Save\" title=\"\" disabled=\"\" style=\"\">"))
                {
                    return ButtonReturnType.Disabled;
                }
                if (ehtml.Contains("<button class=\"ytcp-button-shape-impl ytcp-button-shape-impl--filled ytcp-button-shape-impl--mono ytcp-button-shape-impl--size-m\" aria-label=\"Save\" title=\"\" style=\"\">"))
                {
                    return ButtonReturnType.Enabled;
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsButtonEnabled {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return ButtonReturnType.NotPresent;
            }
        }
        private static IntPtr MakeLParam(int x, int y)
        {
            return (IntPtr)((y << 16) | (x & 0xFFFF));
        }
        private static IntPtr MakeWParam(int keys, int delta)
        {
            return (IntPtr)((delta << 16) | keys);
        }
        public void SimulateMouseWheel(WebView2CompositionControl webView, bool isUp, int repeatCount = 10)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => { SimulateMouseWheel(webView, isUp, repeatCount); });
                    return;
                }

                if (webView == null || canceltoken.IsCancellationRequested || repeatCount == 0) return;

                // Get the current cursor position
                POINT currentPos;
                GetCursorPos(out currentPos);

                // Use composition controller to send wheel event
                if (wv2CompositionController != null)
                {
                    var deltaY = (isUp ? WHEEL_DELTA : -WHEEL_DELTA) * repeatCount;
                    var clientPoint = webView.PointFromScreen(new Point(currentPos.X, currentPos.Y));
                    System.Drawing.Point clientPoint2 = new System.Drawing.Point((int)clientPoint.X, (int)clientPoint.Y);
                    wv2CompositionController.SendMouseInput(CoreWebView2MouseEventKind.Wheel, 
                        CoreWebView2MouseEventVirtualKeys.None,(uint)deltaY, clientPoint2);
                    Thread.Sleep(50); // Small delay after wheel event
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SimulateMouseWheel {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async Task SimulateWheelUpDownAsync(WebView2CompositionControl webView, Point? elementPoint = null)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => { SimulateWheelUpDownAsync(webView, elementPoint); });
                    return;
                }


                if (canceltoken.IsCancellationRequested || webView is null) return;
                var html = Regex.Unescape(await webView.ExecuteScriptAsync("document.body.innerHTML"));
                if (html.Contains("Daily upload limit reached")) return;
                if (!html.Contains("<span class=\"count style-scope ytcp-multi-progress-monitor\">")) return;
                await webView.Dispatcher.InvokeAsync(async () =>
                {
                    webView.Focus();
                    webView.BringIntoView();
                    System.Threading.Thread.Sleep(100);
                    // Get upload counter span position
                    var script = @"var span = document.querySelector('span.count.style-scope.ytcp-multi-progress-monitor');
                             if(span) {
                                 var rect = span.getBoundingClientRect();
                                 return JSON.stringify({x: rect.left + rect.width/2, y: rect.top + rect.height/2});
                             }
                             return null;";
                    var result = await webView.ExecuteScriptAsync(script);
                    Point targetPoint;
                    if (result != "null")
                    {
                        var pos = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, double>>(result.Trim('"'));
                        targetPoint = new Point(pos["x"], pos["y"]);
                        targetPoint.X += 50;
                        targetPoint.Y += 50;
                    }
                    else
                    {
                        // Fallback to bottom-right if span not found
                        targetPoint = new Point(webView.ActualWidth - 50, webView.ActualHeight - 100);
                    }
                    Point position = webView.PointToScreen(targetPoint);
                    SetCursorPos((int)position.X, (int)position.Y);
                    Thread.Sleep(50); // Wait for cursor to settle
                    SimulateMouseWheel(webView, true, 10);
                    System.Threading.Thread.Sleep(100);
                    SimulateMouseWheel(webView, false, 10);
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SimulateWheelUpDownAsync {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        public void SimulateWheelUpDown(WebView2CompositionControl webView, Point? elementPoint = null)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => SimulateWheelUpDown(webView, elementPoint));
                    return;
                }
                Task.Run(() =>
                {
                    SimulateWheelUpDownAsync(webView, elementPoint);
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SimulateWheelUpDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
        private async Task<bool> ProcessUploadsBody(string Span_Name, string Script_Close, string connectStr)
        {
            int ExitCode = -1;
            try
            {
                List<Uploads> clicks = new List<Uploads>();

                timer.Interval = TimeSpan.FromSeconds(1);
                bool timeractive = false;
                int scrollcnt = 0;
                timer.Tick += (s, e) =>
                {
                    timeractive = true;
                    timer.Stop();
                    scrollcnt++;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SimulateWheelUpDown(wv2);
                    }));
                    WheelBody++;
                    if (WheelBody < 10)
                    {
                        timer.Start();
                    }
                    timeractive = false;
                };
                while (true && !canceltoken.IsCancellationRequested)
                {
                    if (ExitDialog) return false;
                    var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                    if (html is not null && html.Contains("Daily upload limit reached"))
                    {
                        Exceeded = true;
                        finished = true;
                        ExitDialog = true;
                        break;
                    }
                    bool found = false;
                    List<HtmlNode> Nodes = GetNodes(html, Span_Name);
                    if (html.ToLower().ContainsAll(new string[] { "daily", "upload", "limit" }))
                    {
                        Exceeded = true;
                        finished = true;
                        ExitDialog = true;
                        if (DoUploadsCnt() < 100)
                        {
                            await BuildFiles();
                            Close();
                        }
                        break;
                    }
                    if (VideoFiles.Count == 1 && Regex.IsMatch(html.ToLower(), @"processing will begin shortly|your video template has been saved as draft|saving|save and close|title-row style-scope ytcp-uploads-dialog"))
                    {
                        string[] files = Directory.GetFiles("Z:\\", VideoFiles[0] + ".*", SearchOption.AllDirectories);
                        if (files.Count() > 0)
                        {
                            File.Delete(files[0]);
                        }
                        InsertIntoUploadFiles(VideoFiles, connectStr);
                        ExitDialog = true;
                        break;
                    }
                    if (html.ToLower().Contains("uploads complete"))
                    {
                        NodeUpdate(Span_Name, ScheduledGet);
                        InsertIntoUploadFiles(VideoFiles, connectStr);
                        DeleteFiles(VideoFiles, "Z:\\");
                        Close();
                        break;
                    }
                    else NodeUpdate(Span_Name, ScheduledGet);
                    if (Nodes.Count == 0)
                    {
                        Thread.Sleep(150);
                        continue;
                    }

                    bool waitingcnt = Nodes.Count > 0 && Nodes.Where(nodex => nodex.InnerText.Contains("Waiting")).Count() > 0;
                    if (waitingcnt)
                    {
                        html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                        List<HtmlNode> Nodes2 = GetNodes(html, Span_Name);
                        if (Nodes2.Count != Nodes.Count)
                        {
                            Nodes = Nodes2;
                        }
                        else
                        {
                            html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            Nodes = GetNodes(html, Span_Name);
                        }
                    }
                    int CompleteCnt = 0;
                    if (Nodes.Count > 0)
                    {
                        html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                        if (html.ToLower().Contains("uploads complete"))
                        {
                            NodeUpdate(Span_Name, ScheduledGet);
                            InsertIntoUploadFiles(VideoFiles, connectStr);
                            DeleteFiles(VideoFiles, "Z:\\");
                            break;
                        }
                        NodeUpdate(Span_Name, ScheduledGet);

                        for (int i = Nodes.Count - 1; i >= 0; i--)
                        {
                            int start = Nodes[i].InnerText.IndexOf("\n") + 1;
                            string filename1 = Nodes[i].InnerText.Substring(start).Split('\n').FirstOrDefault().Trim();
                            if (ExitDialog)
                            {
                                return false;
                            }
                            scrollcnt = 0;
                            if (Regex.IsMatch(Nodes[i].InnerText, @"complete|100% uploaded"))
                            {
                                CompleteCnt++;
                                UploadedHandler(Span_Name, connectStr, filename1);
                                DeleteFiles(new List<string> { filename1 }, "Z:\\");
                            }
                            if (Regex.IsMatch(Nodes[i].InnerText, @"Waiting"))
                            {
                                var cyss = new CancellationTokenSource();
                                cyss.CancelAfter(TimeSpan.FromSeconds(2));
                                while (!cyss.IsCancellationRequested)
                                {
                                    System.Windows.Forms.Application.DoEvents();
                                    Thread.Sleep(15);
                                }
                                if (!clicks.Any(clicks => clicks.FileName == filename1))
                                {
                                    clicks.Add(new Uploads(filename1, "Waiting"));

                                    string newfile = filename1;//.Replace("\"", "");
                                    if (newfile.Contains("."))
                                    {
                                        newfile = newfile.Substring(0, newfile.IndexOf("."));
                                    }
                                    var buttonLabel = $"Edit video {filename1}";
                                    SendTraceInfo?.Invoke(this, $"Getting Edit Window For {newfile}");
                                    lstMain.Items.Insert(0, $"Getting Edit Window For {newfile}");
                                    await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('button[aria-label=\"{buttonLabel}\"]').click()");
                                    SendTraceInfo?.Invoke(this, $"Got Edit Window For {newfile}");
                                    var ctsx = new CancellationTokenSource();
                                    ctsx.CancelAfter(TimeSpan.FromSeconds(5));
                                    bool fnd = false;
                                    while (!ctsx.IsCancellationRequested)
                                    {

                                        SendTraceInfo?.Invoke(this, $"Waiting For Edit Window For {newfile}");
                                        var htmlx = await wv2.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
                                        var ehtmlx = Regex.Unescape(htmlx);
                                        if (!Regex.IsMatch(ehtmlx.ToLower(), @"youtube.be") || Regex.IsMatch(ehtmlx.ToLower(), @"youtube.com/shorts"))
                                        {
                                            fnd = true;
                                            SendTraceInfo?.Invoke(this, $"Found Edit Window For {newfile}");
                                            ctsx.Cancel();

                                            break;
                                        }
                                        SendTraceInfo?.Invoke(this, $"Finished Waiting For Edit Window For {newfile}");
                                        Thread.Sleep(100);
                                    }
                                    var htmlx1 = await wv2.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
                                    var ehtmlx1 = Regex.Unescape(htmlx1);
                                    TimerSimulate.Start();
                                    if (ehtmlx1.ToLower().Contains("uploads complete"))
                                    {
                                        canceltoken.Cancel();
                                        DeleteFiles(VideoFiles, "Z:\\");
                                        break;
                                    }
                                    var cts = new CancellationTokenSource();
                                    while (!cts.IsCancellationRequested)
                                    {
                                        SendTraceInfo?.Invoke(this, $"Starting Probe");
                                        html = await wv2.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
                                        var ehtml = Regex.Unescape(html);
                                        if (ehtml is not null && ehtml.Contains("Daily upload limit reached"))
                                        {
                                            SendTraceInfo?.Invoke(this, $"Daily Upload Limit Reached");
                                            Exceeded = true;
                                            finished = true;
                                            ExitDialog = true;
                                            cts.Cancel();
                                            Close();
                                        }
                                        if (ExitDialog || Exceeded)
                                        {
                                            ExitCode = 0;
                                            SendTraceInfo?.Invoke(this, $"ExitDialog Or Exceeded");
                                            InsertIntoUploadFiles(VideoFiles, connectStr);
                                            cts.Cancel();
                                            return false;
                                        }
                                        SendTraceInfo?.Invoke(this, $"Detecting Uploads Complete {filename1}");
                                        var htmlcheck = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                        if (Regex.IsMatch(htmlcheck, @"Uploads complete"))
                                        {
                                            SendTraceInfo?.Invoke(this, $"Uploads complete Detected");
                                            cts.Cancel();
                                            InsertIntoUploadFiles(VideoFiles, connectStr);
                                            DeleteFiles(VideoFiles, "Z:\\");
                                            continue;
                                        }
                                        string vid = "";
                                        var ctxs = new CancellationTokenSource();
                                        ctxs.CancelAfter(TimeSpan.FromSeconds(5));
                                        while (true && !ctxs.IsCancellationRequested)
                                        {
                                            htmlcheck = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                            SendTraceInfo?.Invoke(this, $"Getting videoid {filename1}");
                                            if (htmlcheck.Contains("Channel analytics")) break;
                                            var index = htmlcheck.IndexOf("youtube.com/shorts");
                                            int len = "youtube.com/shorts".Length;
                                            if (index != -1)
                                            {
                                                vid = htmlcheck.Substring(index + len, 11);
                                                break;
                                            }
                                            else
                                            {
                                                var index1 = htmlcheck.IndexOf("youtube.be");
                                                int len1 = "youtube.be".Length;
                                                if (index1 != -1)
                                                {
                                                    vid = htmlcheck.Substring(index1 + len1, 11);
                                                    ctsx.Cancel();
                                                    break;
                                                }
                                                Thread.Sleep(500);
                                            }
                                        }
                                        foreach (var item in ScheduledFiles.Where(item => item.FileName == filename1))
                                        {
                                            item.VideoId = vid;
                                            SendTraceInfo?.Invoke(this, $"Got videoid {vid} for {filename1}");
                                            break;
                                        }
                                        if (Regex.IsMatch(htmlcheck.ToLower(), @"daily limit|upload complete ... processing will begin shortly|checks complete. no issues found.|processing up to"))
                                        {
                                            finished = true;
                                            InsertIntoUploadFiles(VideoFiles, connectStr);
                                            ExitCode = htmlcheck.ToLower() switch
                                            {
                                                var x when x.Contains("daily limit") => 1,
                                                var x when x.Contains("upload complete ... processing will begin shortly") => 2,
                                                var x when x.Contains("checks complete. no issues found.") => 2,
                                                var x when x.Contains("daily limit") => 3,
                                                var x when x.Contains("processing up to") => 4,
                                                _ => 5,
                                            };
                                            SendTraceInfo?.Invoke(this, $"Sending Close Click {ExitCode}");
                                            Close_Upload(wv2);
                                            TimerSimulate.Start();
                                            SendTraceInfo?.Invoke(this, $"Sent Close Click");
                                            var cts1 = new CancellationTokenSource();
                                            cts1.CancelAfter(TimeSpan.FromSeconds(5));
                                            while (!cts1.IsCancellationRequested)
                                            {
                                                System.Windows.Forms.Application.DoEvents();
                                                Thread.Sleep(15);
                                            }
                                            cts.Cancel();
                                            SendTraceInfo?.Invoke(this, $"Exiting loop");
                                            break;
                                        }
                                        if (!Regex.IsMatch(htmlcheck, @"title-row style-scope ytcp-uploads-dialog"))
                                        {
                                            foreach (var click in clicks.Where(clicks => clicks.FileName == filename1))
                                            {
                                                click.Status = "Uploading";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // processing i of count of files.
                        if (!timeractive && scrollcnt < 3)
                        {
                            timer.Start();
                        }
                        Thread.Sleep(100);

                        html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                        Nodes = GetNodes(html, Span_Name);
                    }
                    if (CompleteCnt == Nodes.Count && Nodes.Count > 0)
                    {
                        break;
                    }
                    NodeUpdate(Span_Name, ScheduledGet);
                }
                var htmlcheck1 = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                if (Regex.IsMatch(htmlcheck1, @"Uploads complete|processing will begin shortly|your video template has been saved as draft|saving|save and close|title-row style-scope ytcp-uploads-dialog|daily limit"))
                {

                    DeleteFiles(VideoFiles, "Z:\\");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduledGet {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return false;
            }
        }

        public void DeleteFiles(List<string> files, string basedirectory)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => { DeleteFiles(files, basedirectory); });
                    return;
                }
                if (TotalScheduled == 0) TotalScheduled += files.Count;
                foreach (string file in files)
                {
                    var ft = Directory.EnumerateFiles(basedirectory, file + ".*",
                        SearchOption.AllDirectories).FirstOrDefault();
                    if (ft is not null)
                    {
                        File.Delete(ft);
                        lstMain.Items.Insert(0, $"{file} Deleted");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DeleteFiles {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void ProcessHTML(string html, int id, string IntId, object sender)
        {
            try
            {
                if (canceltoken.IsCancellationRequested) return;
                string filename = "";
                if (html is not null)
                {
                    string ehtml = Regex.Unescape(html);
                    if (ehtml is not null && ehtml != "")
                    {
                        if (ehtml.Contains(IntId))
                        {
                            string Search = "div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">";
                            if (ehtml.Contains("div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">"))
                            {
                                int index = ehtml.IndexOf("div id=\"original-filename\" class=\"value style-scope ytcp-video-info\">");
                                html = ehtml.Substring(index + Search.Length, 1500);
                                if (html.Contains('<'))
                                {
                                    index = html.IndexOf("<");
                                    html = html[..(index - 1)];
                                    filename = html.Replace("\n", "").Replace("\r", "").Trim();
                                    if (filename.Contains("_") && IsVideoLookup && ScraperType == EventTypes.UploadTest)
                                    {
                                        string idp = filename.Split("_").ToArray<string>().ToList().LastOrDefault().Trim();
                                        if (idp is not null && idp != "")
                                        {
                                            if (Invoker?.Invoke(this, new CustomParams_AddDirectory(idp)) is TurlpeDualString tds)
                                            {
                                                TitleStr = tds.turlpe1;
                                                DescStr = tds.turlpe2;
                                            }
                                            if (TitleStr != "")
                                            {
                                                if ((sender as WebView2CompositionControl).CoreWebView2 != null)
                                                {
                                                    string script = "var divElements = document.querySelectorAll('[aria-label=\"Tell viewers about your video (type @ to mention a channel)\"]');" +
                                                       "divElements.forEach(function(divElement) {" +
                                                      $"   divElement.textContent = '{TitleStr}';" +
                                                       "});";

                                                    (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync(script);
                                                }
                                            }
                                            if (DescStr != "")
                                            {
                                                string script2 = "var divElements = document.querySelectorAll('[aria-label=\"Add a title that describes your video (type @ to mention a channel)\"]');" +
                                                       "divElements.forEach(function(divElement) {" +
                                                      $"   divElement.textContent = '{DescStr}';" +
                                                       "});";
                                                (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync(script2);
                                            }
                                            if (TitleStr != "" || DescStr != "")
                                            {
                                                if ((sender as WebView2CompositionControl).CoreWebView2 != null)
                                                {
                                                    while (true)
                                                    {
                                                        var p = IsButtonEnabled((sender as WebView2CompositionControl)).GetAwaiter().GetResult();
                                                        if (p == ButtonReturnType.Enabled)
                                                        {
                                                            break;
                                                        }
                                                        Thread.Sleep(100);
                                                    }
                                                    string script1 = "var saveButton = document.querySelector('.ytcp-button-shape-impl__button-text-content');" +
                                                                   "if (saveButton) {" +
                                                                   "   saveButton.click();" +
                                                                   "}";
                                                    (sender as WebView2CompositionControl).CoreWebView2.ExecuteScriptAsync(script1);
                                                    while (true)
                                                    {
                                                        var p = IsButtonEnabled((sender as WebView2CompositionControl)).GetAwaiter().GetResult();
                                                        if (p == ButtonReturnType.Disabled)
                                                        {
                                                            break;
                                                        }
                                                        Thread.Sleep(100);
                                                    }
                                                }
                                                for (int i = ScheduledFiles.Count - 1; i >= 0; i--)
                                                {
                                                    if (ScheduledFiles[i].FileName == filename)
                                                    {
                                                        ScheduledFiles.RemoveAt(i);
                                                    }
                                                }
                                                if (ScheduledFiles.Count == 0)
                                                {
                                                    Waiting = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (filename == "")
                        {
                            Thread.Sleep(500);
                            if (wv2Dictionary.ContainsKey(id))
                            {
                                var webView2Instance = wv2Dictionary[id];
                                var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                                task.ContinueWith(x => { ProcessHTML(x.Result, id, IntId, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                                return;
                            }
                        }
                    }
                }
                filename = System.IO.Path.GetFileNameWithoutExtension(filename.Replace("/n", "").Trim());
                int index1 = Ids.IndexOf(IntId);
                if (index1 == -1)
                {
                    for (int i = 0; i < lstMain.Items.Count; i++)
                    {
                        if (lstMain.Items[i].ToString().Contains($"{IntId}"))
                        {
                            index1 = i;
                            break;
                        }
                    }
                    if (index1 != -1)
                    {
                        if (!(lstMain.Items[index1] as string).Contains(filename))
                        {
                            lstMain.Items[index1] += $" {filename}";
                            files++;

                            SetMargin(StatusBar);
                            int lstCnt = lstMain.Items.Count;
                            foreach (var item in lstMain.Items)
                            {
                                if (item.ToString().Contains(" "))
                                {
                                    lstCnt--;
                                }
                            }
                            if (files == lstCnt && lstMain.Items.Count == MaxCnts)
                            {
                                var _DoAutoCancel = new AutoCancel(DoAutoCancelCloseScraper, $"Scraped {files} Files", 5, "Scraper Finished");
                                _DoAutoCancel.ShowActivated = true;
                                _DoAutoCancel.Show();
                            }
                        }
                    }
                    SetMargin(StatusBar);
                }
                else
                {
                    if (!(lstMain.Items[index1] as string).Contains(filename))
                    {
                        lstMain.Items[index1] += $" {filename}";
                    }
                }
                if (ScraperType == EventTypes.ScapeSchedule)
                {
                    Invoker?.Invoke(this, new CustomParams_InsertIntoTable(IntId, filename));
                    inserted++;
                }
                if (IntId == LastIdNode && inserted >= MaxCnts)
                {
                    Hide();
                    var _DoAutoCancel = new AutoCancel(DoAutoCancelClose, "", 5, "Scaping Finished");
                    _DoAutoCancel.ShowActivated = true;
                    _DoAutoCancel.Show();
                    return;
                }
                files++;
                Dispatcher.Invoke(() =>
                {
                    if (files > 0)
                    {
                        lblLastNode.Content = $"{files} Processed";
                        SetMargin(StatusBar);
                    }
                });
                if (wv2Dictionary.ContainsKey(id))
                {
                    var webView2Instance = wv2Dictionary[id];
                    webView2Instance.AllowDrop = true;

                }
                CanSpool = true;
                if (nextaddress.Count > 0)
                {
                    int Avaialble = 0;
                    if (wv2A1.AllowDrop) Avaialble++;
                    if (wv2A2.AllowDrop) Avaialble++;
                    if (wv2A3.AllowDrop) Avaialble++;
                    if (wv2A4.AllowDrop) Avaialble++;
                    if (wv2A5.AllowDrop) Avaialble++;
                    if (wv2A6.AllowDrop) Avaialble++;
                    /*if (wv2A7.AllowDrop) Avaialble++;
                    if (wv2A8.AllowDrop) Avaialble++;
                    if (wv2A9.AllowDrop) Avaialble++;
                    if (wv2A10.AllowDrop) Avaialble++;*/
                    for (int ix = 0; ix < Avaialble; ix++)
                    {
                        if (nextaddress.Count > 0)
                        {
                            string url = nextaddress[0];
                            nextaddress.RemoveAt(0);
                            for (int iy = 1; iy < 7; iy++)
                            {
                                if (wv2Dictionary.ContainsKey(iy))
                                {
                                    if (wv2Dictionary[iy].AllowDrop)
                                    {
                                        wv2Dictionary[iy].SetURL(webAddressBuilder.ScopeVideo(url).ScopeAddress);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessHTML {MethodBase.GetCurrentMethod()?.Name} {ex.Message} ");
            }
        }
    }
}