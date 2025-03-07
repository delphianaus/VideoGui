using CliWrap;
using CliWrap.EventStream;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using Google.Apis.YouTube.v3.Data;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using System;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using VideoGui.Models;
using VideoGui.Models.delegates;
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
        int swap = 1, ct = 0, MaxNodes = -1, MaxUploads = 0, recs = 0, gmaxrecs = 0, files = 0, max = 0, SlotsPerUpload = 0,
            ScheduleMax = 0, ts = 0, LastKey = -1, Days = 1, CurrentDay = 1;
        bool EditDone = false, btndone = false, ExitDialog = false, Waiting = false, IsVideoLookup = false, WaitingFileName = false;
        bool Valid = false, IsVideoLookupShort = false, IsValid = false, IsUnlisted = false, IsDashboardMode = false, CanSpool = false, FirstRun = true, done = false, HasExited = false;
        bool DoNextNode = true, finished = false, TimedOut = false, Uploading = false, NextRecord = false, Processing = false, clickupload = true;
        public bool IsClosing = false, IsClosed = false, Exceeded = false, KilledUploads = false, SwapEnabled = false, IsTitleEditor = false;
        string SendKeysString = "", UploadPath = "", LastNode = "", DefaultUrl = "", LastValidFileName = "", TableDestination = "";
        List<ScraperUploads> Scraper_uploaded = new();
        List<string> IdNodes = new(), titles = new List<string>(), nextaddress = new(), Ids = new(), Idx = new(), ufiles = new(), Files = new();// DoneFiles = new();
        public List<string> ScheduledOk = new List<string>(), VideoFiles = new List<string>(), nodeslist = new List<string>();
        public List<ShortsDirectory> ShortsDirectories = new(); // <shortname>
        public List<ListScheduleItems> listSchedules = new List<ListScheduleItems>();
        public List<VideoIdFileName> ScheduledFiles = new List<VideoIdFileName>();
        DispatcherTimer CloseTimer = new DispatcherTimer();
        DirectshortsScheduler directshortsScheduler = null;
        public AddressUpdate DoVideoLookUp = null;
        string TitleStr = "", DescStr = "";
        StatusTypes VStatusType = StatusTypes.PRIVATE;
        WebAddressBuilder webAddressBuilder = null;
        databasehook<object> dbInitializer = null;
        OnFinishId DoOnFinish = null;
        TimeOnly CurrentTime = new TimeOnly();
        DateOnly CurrentDate = DateOnly.FromDateTime(DateTime.Now);
        public DateTime StartDate = DateTime.Now, EndDate = DateTime.Now, LastValidDate = DateTime.Now;
        bool IsTest = false, AutoClose = false;
        AutoCancel DoAutoCancel = null;
        List<DirectoriesProbe> Directories = new(); //Directories
        //string Title = "", Desc = "";
        Dictionary<int, WebView2> wv2Dictionary = new Dictionary<int, WebView2>();
        Dictionary<int, WebView2> ActiveWebView = new Dictionary<int, WebView2>();
        DispatcherTimer InternalTimer = new DispatcherTimer();
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        public Nullable<DateTime> ReleaseDate = null, ReleaseEndDate = null;
        public EventTypes ScraperType = EventTypes.VideoUpload;
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
        public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        private const int BM_CLICK = 0x00F5;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);
        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        public ScraperModule(databasehook<object> _dbInit, OnFinishId _OnFinish, List<string> directories, bool IsShort)
        {
            try
            {
                ScraperType = EventTypes.VideoLookup;
                _dbInit = _dbInit;
                _OnFinish = _OnFinish;
                foreach (var dir in directories)
                {
                    Directories.Add(new DirectoriesProbe(dir));
                }
                IsVideoLookupShort = IsShort;
                IsDashboardMode = true;
                dbInitializer = _dbInit;
                InitializeComponent();
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");
                Closing += (s, e) =>
                {
                    IsClosing = true;
                    canceltoken.Cancel();
                    cancelds();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(EventId);
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

        public ScraperModule(databasehook<object> _dbInit, OnFinishId _OnFinish, string _Default_url,
               string TableDest, int _EventId)
        {
            try
            {
                ScraperType = EventTypes.ScapeSchedule;
                _dbInit = _dbInit;
                _OnFinish = _OnFinish;
                _Default_url = _Default_url;
                TableDestination = TableDest;
                EventId = _EventId;
                IsDashboardMode = true;
                dbInitializer = _dbInit;
                InitializeComponent();
                Closing += (s, e) =>
                {
                    var html = wv2.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML").ConfigureAwait(true).GetAwaiter().GetResult();
                    var ehtml = Regex.Unescape(html);
                    if (ehtml is not null && ehtml.Contains("Daily upload limit reached"))
                    {
                        Exceeded = true;
                    }
                    IsClosing = true;
                    canceltoken.Cancel();
                    cancelds();
                };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(EventId);
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
                ex.LogWrite($"Constructor Scraper.Schedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }


        public ScraperModule(databasehook<object> _dbInit, OnFinishId _OnFinish, string _Default_url,
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
                dbInitializer = _dbInit;
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; canceltoken.Cancel(); cancelds(); };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(EventId);
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

        public ScraperModule(databasehook<object> _dbInit, OnFinishId _OnFinish, string _Default_url,
            int maxuploads = 100, int slotsperupload = 5, int _EventId = -1)
        {
            try
            {
                SwapEnabled = false;
                EventId = _EventId;
                ScraperType = EventTypes.VideoUpload;
                MaxUploads = maxuploads;
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                IsDashboardMode = true;
                dbInitializer = _dbInit;
                IsUnlisted = false;
                SlotsPerUpload = slotsperupload;
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; canceltoken.Cancel(); cancelds(); };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(EventId);
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

        public ScraperModule(databasehook<object> _dbInit, OnFinishId _OnFinish, string _Default_url, WebView2 wb2)
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
                dbInitializer = _dbInit;
                IsUnlisted = false;
                SlotsPerUpload = 2;
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; canceltoken.Cancel(); cancelds(); };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke(EventId);
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
                if (canceltoken.IsCancellationRequested) return;
                if (e.IsSuccess && sender is not null)
                {
                    int Id = (sender as WebView2).Name.Replace("wv2A", "").ToInt(-1);
                    string source = (sender as WebView2).Source.AbsoluteUri.ToString(), IntId = "";
                    int p1 = source.IndexOf("video/"), p2 = source.IndexOf("/edit");
                    if (p1 != -1 && p2 != -1)
                    {
                        IntId = source.Substring(p1 + 6, p2 - p1 - 6);
                    }
                    var task = (sender as WebView2).ExecuteScriptAsync("document.body.innerHTML");
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
                var env = await CoreWebView2Environment.CreateAsync(null, @"c:\stuff\scraper");
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
                _dest = key.GetValueStr("UploadPath", "");
                key.Close();
                ExecuteAsNonAdmin($"{_dest}");
                ExecuteAsAdmin($"{_dest}");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetupSubstDrive {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                    if (ShortsDirectories.Count == 0) ShortsDirectories.Add(new ShortsDirectory(-1, UploadPath));
                    Files.Clear();
                    foreach (var d in ShortsDirectories)
                    {
                        Files.AddRange(Directory.EnumerateFiles(d.Directory, "*.mp4", SearchOption.AllDirectories).ToList());
                    }
                    int max = 0;

                    SendKeysString = "";
                    string connectionStr = (dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn) ? conn : "";
                    string sql = "select count(Id) from UPLOADSRECORD WHERE UPLOAD_DATE = @P0 AND UPLOADTYPE = 0";
                    object res = connectionStr.ExecuteScalar(sql, [("@p0", DateTime.Now.Date)]);
                    if (res is long resxx)
                    {
                        TotalScheduled = resxx.ToInt();
                        lblTotal.Content = TotalScheduled.ToString();
                    }

                    max = TotalScheduled;
                    ts = TotalScheduled;
                    if (ts < MaxUploads)
                    {
                        foreach (string f in Files.Where(f => File.Exists(f)).Take(SlotsPerUpload))
                        {
                            max++;
                            if (max <= MaxUploads)
                            {
                                lblTotal.Content = $"{TotalScheduled}";
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
                    string connectStr = dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn ? conn : "";
                    List<Uploads> clicks = new List<Uploads>();
                    List<bool> filesDone = Enumerable.Repeat(false, Files.Count).ToList();
                    bool Exit = false, finished = false;
                    while (true || !finished)
                    {
                        if (ExitDialog) return;
                        if (Exit)
                        {
                            await ActiveWebView[1].ExecuteScriptAsync(Script_Close);
                            break;
                        }
                        Thread.Sleep(50);// GET HTML




                        while (true && !canceltoken.IsCancellationRequested)
                        {
                            if (ExitDialog) return;
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
                            if (html.ToLower().Contains("daily limit"))
                            {
                                int uploaded = 0;
                                string sql = "select count(Id) from UPLOADSRECORD WHERE UPLOAD_DATE = @P0 AND UPLOADTYPE = 0";
                                string cconnectStr = dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn1 ? conn1 : "";
                                uploaded = cconnectStr.ExecuteScalar(sql, [("@p0", DateTime.Now.Date)]).ToInt(0);
                                if (uploaded < 100)
                                {
                                    dbInitializer?.Invoke(this, new CustomParams_Wait());
                                    await BuildFiles();
                                    Close();
                                }
                            }
                            if (html.ToLower().Contains("uploads complete"))
                            {
                                NodeUpdate(Span_Name, ScheduledGet);
                                break;
                            }
                            else NodeUpdate(Span_Name, ScheduledGet);
                            if (Nodes.Count == 0)
                            {
                                Thread.Sleep(250);
                                continue;
                            }
                            Uploading = 0;
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
                                    // lstMain.Items.Insert(0, "loop exited");
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
                                    break;
                                }
                                NodeUpdate(Span_Name, ScheduledGet);
                                for (int i = 0; i < Nodes.Count; i++)
                                {
                                    if (ExitDialog)
                                    {
                                        return;
                                    }
                                    if (Regex.IsMatch(Nodes[i].InnerText, @"Complete|100% uploaded"))
                                    {
                                        CompleteCnt++;
                                        int _start = Nodes[i].InnerText.IndexOf("\n") + 1;
                                        int _end = Nodes[i].InnerText.IndexOf("\n", _start);
                                        string filename1 = Nodes[i].InnerText.Substring(_start, _end - _start).Trim();
                                        UploadedHandler(Span_Name, connectStr, filename1);
                                    }
                                    if (Regex.IsMatch(Nodes[i].InnerText, @"Waiting"))
                                    {
                                        var filenameMatch = Regex.Match(Nodes[i].InnerText, @"\n([^ ]+)\n");
                                        if (filenameMatch.Success && !clicks.Any(clicks => clicks.FileName == filenameMatch.Groups[1].Value))
                                        {
                                            clicks.Add(new Uploads(filenameMatch.Groups[1].Value, "Waiting"));
                                            var filename = filenameMatch.Groups[1].Value.Trim();
                                            string newfile = filename.Replace("\"", "");
                                            if (newfile.Contains("."))
                                            {
                                                newfile = newfile.Substring(0, newfile.IndexOf("."));
                                            }
                                            var buttonLabel = $"Edit video {filename}";
                                            lstMain.Items.Insert(0, $"Getting Edit Window For {newfile}");
                                            await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('button[aria-label=\"{buttonLabel}\"]').click()");
                                            var cts = new CancellationTokenSource();
                                            while (!cts.IsCancellationRequested)
                                            {
                                                html = await wv2.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
                                                var ehtml = Regex.Unescape(html);
                                                if (ehtml is not null && ehtml.Contains("Daily upload limit reached"))
                                                {
                                                    Exceeded = true;
                                                }

                                                if (ExitDialog)
                                                {
                                                    cts.Cancel();
                                                    return;
                                                }

                                                var htmlcheck = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                                if (htmlcheck is not null)
                                                {
                                                    if (htmlcheck.Contains("https://youtu.be/"))
                                                    {
                                                        var index = htmlcheck.IndexOf("https://youtu.be/");
                                                        if (index != -1)
                                                        {
                                                            int len = "https://youtu.be/".Length;
                                                            string vid = htmlcheck.Substring(index + len, 11);
                                                            foreach (var item in ScheduledFiles.Where(item => item.FileName == filename))
                                                            {
                                                                item.VideoId = vid;
                                                                break;
                                                            }

                                                            dbInitializer?.Invoke(this, new InsertIntoTable(vid, filename));
                                                        }
                                                    }
                                                    if (Regex.IsMatch(htmlcheck, @"Upload Complete|Daily limit|Processing will begin shortly"))
                                                    {
                                                        break;
                                                    }
                                                    if (!Regex.IsMatch(htmlcheck, @"title-row style-scope ytcp-uploads-dialog"))
                                                    {
                                                        //update clicks  upload record
                                                        foreach (var click in clicks.Where(clicks => clicks.FileName == filename))
                                                        {
                                                            click.Status = "Uploading";
                                                            break;
                                                        }
                                                        cts.CancelAfter(TimeSpan.FromMilliseconds(500));
                                                        while (!cts.IsCancellationRequested)
                                                        {
                                                            Thread.Sleep(50);
                                                        }
                                                        var index1 = htmlcheck.IndexOf("https://youtu.be/");
                                                        if (index1 != -1)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                                Thread.Sleep(100);
                                            }
                                            html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                            Nodes = GetNodes(html, Span_Name);
                                            if (Regex.IsMatch(html.ToLower(), @"saving|save and close|title-row style-scope ytcp-uploads-dialog|daily limit"))
                                            {
                                                Thread.Sleep(300);
                                                await ActiveWebView[1].ExecuteScriptAsync(Script_Close);
                                            }
                                            found = true;
                                        }
                                    }
                                }
                            }
                            if (CompleteCnt == Nodes.Count && Nodes.Count > 0)
                            {
                                break;
                            }
                            NodeUpdate(Span_Name, ScheduledGet);
                            if (found) continue;// gets next html and looks for waiting video
                        }
                        if (ExitDialog) return;
                        NodeUpdate(Span_Name, ScheduledGet);
                        var html1 = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                        List<HtmlNode> Nodes1 = GetNodes(html1, Span_Name);
                        int finishedz = 0;
                        for (int i = 0; i < Nodes1.Count; i++)
                        {
                            finishedz++;
                            if (Nodes1[1].InnerText.ToLower().Contains("limit"))
                            {
                                Exceeded = true;
                            }
                            else
                            {
                                if (Nodes1[i].InnerText.ToLower().Contains("100% uploaded"))
                                {
                                    int start = Nodes1[i].InnerText.IndexOf("\n") + 1;
                                    int end = Nodes1[i].InnerText.IndexOf("\n", start);
                                    string filename1 = Nodes1[i].InnerText.Substring(start, end - start).Trim();
                                    UploadedHandler(Span_Name, connectStr, filename1);
                                }
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
                                    break;
                                }
                                Thread.Sleep(100);
                            }
                            var htmlx = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            File.WriteAllLines($"E:\\gopro9\\ScraperLog_{ct++}.txt", new string[] { htmlx });
                            // build files
                            await BuildFiles();
                            Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadV2Files {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
            finally
            {
                var htmlx = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                File.WriteAllLines($"E:\\gopro9\\ScraperLog_{ct++}.txt", new string[] { htmlx });
                HasExited = true;

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
                        int res = -1;
                        string sql = "select ID from UPLOADSRECORD WHERE UPLOADFILE = @P0 AND UPLOADTYPE = 0";
                        res = connectStr.ExecuteScalar(sql.ToUpper(), [("@P0", filename1)]).ToInt(-1);
                        if (res == -1)
                        {
                            sql = "INSERT INTO UPLOADSRECORD(UPLOADFILE, UPLOAD_DATE, UPLOAD_TIME,UPLOADTYPE) VALUES (@P0,@P1,@P2,@P3) RETURNING ID";
                            res = connectStr.ExecuteScalar(sql.ToUpper(), [("@P0", filename1), ("@P1", DateTime.Now.Date), ("@P2", DateTime.Now.TimeOfDay), ("@P3", 0)]).ToInt(-1);
                        }
                        else
                        {
                            sql = "UPDATE UPLOADSRECORD SET UPLOAD_DATE = @P1, UPLOAD_TIME = @P2 WHERE ID = @P0";
                            connectStr.ExecuteNonQuery(sql.ToUpper(), [("@P0", res), ("@P1", DateTime.Now.Date), ("@P2", DateTime.Now.TimeOfDay)]);
                        }
                        if (ScheduledOk.IndexOf(filename1) == -1)
                        {
                            ScheduledOk.Add(filename1);
                            string gUrl = webAddressBuilder.ScopeVideo(filename1, false).ScopeAddress;
                            nextaddress.Add(gUrl);
                            VideoFiles.Remove(filename1);
                            TotalScheduled++;
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
                RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("WebWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("WebHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("Webleft", Left).ToDouble();
                var _top = key.GetValue("Webtop", Top).ToDouble();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                lstMain.Width = Width - 5;
                var thick = new Thickness(0, 0, 0, 0);
                thick.Left = Width - 190;

                //btnClickUpload.Margin = thick; 
                key?.Close();
                if (Parent is MainWindow mainWindow)
                {
                    var r = new CustomParams_SetTimeSpans(null, null);
                    dbInitializer?.Invoke(this, r);
                }
                wv2.CoreWebView2InitializationCompleted += (sender, args) =>
                {
                    ActiveWebView[1].Source = new Uri(webAddressBuilder.GetChannelURL().Address);
                };
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
                            while (!cts.IsCancellationRequested)
                            {
                                System.Windows.Forms.Application.DoEvents();
                                Thread.Sleep(100);
                            }
                            html = Regex.Unescape(await (sender as WebView2).ExecuteScriptAsync("document.body.innerHTML"));
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
                dbInitializer?.Invoke(this, new CustomParams_AddVideoInfo(null, VStatusType, Id, Title,
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

        List<string> lookups = new List<string>();
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
                        lblLastNode.Content = LastNode;
                    });
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
                    for (int i = 0; i < targetSpanList.Count; i++)
                    {
                        if (TimedOut || canceltoken.IsCancellationRequested) break;
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
                                WaitingFileName = true;
                                wv2A10.NavigationCompleted += wv2_NavigationCompleted_GetFileName;
                                lookups.Add(gUrl2);
                                wv2A10.Source = new Uri(gUrl2);
                                var cts = new CancellationTokenSource();
                                cts.CancelAfter(TimeSpan.FromSeconds(300));
                                TimedOut = false;
                                string oldtitle = TitleStr;
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

                                if (TimedOut || cts.IsCancellationRequested || canceltoken.IsCancellationRequested)
                                {
                                    TitleStr = oldtitle;
                                    lstMain.Items.Insert(0, $"Timeout on getting filename. {TitleStr}");
                                    break;
                                }
                                bool Ok = false;
                                if (directshortsScheduler is not null && !cts.IsCancellationRequested)
                                {
                                    Ok = directshortsScheduler.ScheduleVideo(Id, TitleStr, DescStr, false);
                                    DoNextNode = Ok;
                                    if (!Ok)
                                    {
                                        lstMain.Items.Insert(0, $"Error on Scheduling Detected.");
                                        canceltoken.Cancel();
                                        break;
                                    }
                                }
                                else if (directshortsScheduler is not null && cts.IsCancellationRequested)
                                {
                                    TimedOut = true;
                                    lstMain.Items.Insert(0, $"Timeout on Scheduling Detected.");
                                    DoNextNode = false;
                                }
                            }
                            else
                            {
                                if (DoNextNode && ScraperType == EventTypes.ShortsSchedule)
                                {
                                    DoNextNode = DoNodeScrapeUpdate(Id, TitleStr, DescStr, "", StatusStr, dateTime);
                                }
                                else if (DoNextNode && Id != "" && ScraperType == EventTypes.ScapeSchedule)
                                {
                                    webAddressBuilder.ScopeVideo(Id, true);
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
                        string MaxCnt = nodes.LastOrDefault();
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
                                var task1 = (sender as WebView2).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");

                                task1.ContinueWith(x =>
                                {
                                    ProcessWV2Completed_ShortsScheduler(x.Result, sender);
                                }, TaskScheduler.FromCurrentSynchronizationContext());


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

        private async Task btnNext_Task(object sender)
        {
            try
            {

                string script = @"
        var nextButton = document.getElementById('navigate-after');
        if (nextButton) {
            nextButton.addEventListener('click', function() {
                window.chrome.webview.postMessage(JSON.stringify({ type: 'buttonClick' }));
            });
            nextButton.click();
        }
    ";

                var webView2 = sender as WebView2;
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
                        string Span_Name = "row style-scope ytcp-multi-progress-monitor";
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(ehtml);
                        if (!finished && ehtml is not null && ehtml.Contains("close"))
                        {
                            var cts1 = new CancellationTokenSource();
                            cts1.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts1.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            Click_Finish();
                            cts1.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts1.IsCancellationRequested && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            //                     Click_Upload();

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
                        if (ScraperType == EventTypes.ShortsSchedule || ScraperType == EventTypes.VideoLookup)
                        {
                            if (ehtml.Contains("@JustinsTrainJourneys"))
                            {
                                InsertAtZero = true;
                                YouTubeLoaded();
                            }
                        }
                        else if (clickupload && ScraperType != EventTypes.ShortsSchedule)
                        {
                            wv2.AllowDrop = true;
                            clickupload = false;
                            //UploadNewVideos();
                            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            string _dest = key.GetValueStr("UploadPath", "");
                            key.Close();
                            UploadPath = _dest;
                            var UploadTask = UploadV2Files(false);
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

        private async Task CloseTimer_TickAsync(object? sender, EventArgs e)
        {
            SendKeys.SendWait("{TAB}");
            await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('li.menu-item.remove-default-style.style-scope.ytcp-navigation').click()");

            //await wv2.CoreWebView2.ExecuteScriptAsync("document.activeElement.dispatchEvent(new KeyboardEvent('keydown', { key: 'Tab', keyCode: 9 }));");
        }
        private void ProcessVideoInfo(object sender, string ehtml)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(ehtml);
                string Span_Name = "page-description style-scope ytcp-table-footer";
                HtmlNode targetSpan = doc.DocumentNode.SelectSingleNode($"//span[@class='{Span_Name}']");
                if (LastNode != "")
                {
                    if (targetSpan is not null && targetSpan.InnerText == LastNode)
                    {
                        var ctsx = new CancellationTokenSource();
                        ctsx.CancelAfter(TimeSpan.FromMilliseconds(1500));
                        while (!ctsx.IsCancellationRequested)
                        {
                            Thread.Sleep(100);
                        }
                        LastNode = (targetSpan is not null) ? targetSpan.InnerText : LastNode;
                    }
                    LastNode = (targetSpan is not null) ? targetSpan.InnerText : LastNode;
                }
                if (targetSpan is not null && targetSpan.InnerText is not null && targetSpan.InnerText != "")
                {
                    string MaxNode = targetSpan.InnerHtml;
                    if (MaxNode is not null && MaxNode != "")
                    {
                        int MaxRecords = MaxNode.Split(" ").ToList().LastOrDefault().ToInt();
                        string FirstPart = MaxNode.Replace(MaxRecords.ToString(), "").Replace("of", "").Trim();
                        var p = FirstPart.Split('–').ToList();
                        int CurrentRange = p.LastOrDefault().ToInt();
                        NextRecord = (CurrentRange < MaxRecords) ? true : NextRecord;
                        gmaxrecs = (gmaxrecs == 0) ? MaxRecords : gmaxrecs;
                    }
                    string SpanName = "page-description style-scope ytcp-table-footer";
                    targetSpan = doc.DocumentNode.SelectSingleNode($"//span[@class='{SpanName}']");
                    LastNode = (targetSpan is not null) ? targetSpan.InnerText : LastNode;
                    Dispatcher.Invoke(() =>
                    {
                        lblLastNode.Content = LastNode;
                        SetMargin(StatusBar);
                    });
                    ProcessChannelContent(doc, targetSpan);
                }
                else
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(2));
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                    ProcessWebView(sender);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"");
            }
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
                    if (ScraperType != EventTypes.ShortsSchedule)
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


                    // task.ContinueWith(x => { ProcessWV2(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
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
                int Id = (sender as WebView2).Name.Replace("wv2A", "").ToInt(-1);
                string source = (sender as WebView2).Source.AbsoluteUri.ToString(), IntId = "";
                int p1 = source.IndexOf("video/"), p2 = source.IndexOf("/edit");
                IntId = (p1 != -1 && p2 != -1) ? source.Substring(p1 + 6, p2 - p1 - 6) : IntId;
                var task = (sender as WebView2).ExecuteScriptAsync("document.body.innerHTML");
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
                                            int TitleId = -1, DescId = -1, Id = -1;
                                            string connectionStr = dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn ? conn : "";
                                            int idd = idp.Replace(".mp4", "").ToInt(-1);
                                            string sql = $"SELECT * FROM SHORTSDIRECTORY WHERE ID = @UID";
                                            connectionStr.ExecuteReader(sql, [("UID", idd)], (FbDataReader r) =>
                                            {
                                                Id = (r["ID"] is Int32 i) ? i : -1;
                                                TitleId = (r["TITLEID"] is int tid) ? tid : -1;
                                                DescId = (r["DESCID"] is int did) ? did : -1;
                                            });
                                            if (id != -1)
                                            {
                                                if (DescId != -1)
                                                {
                                                    var p = new CustomParams_GetDesc(idd, DescId);
                                                    dbInitializer?.Invoke(this, p);
                                                    DescStr = p.name;
                                                }
                                                if (TitleId != -1)
                                                {
                                                    var p = new CustomParams_GetTitle(idd, TitleId);
                                                    dbInitializer?.Invoke(this, p);
                                                    TitleStr = p.name;
                                                    var r = p.name.Split(" ").ToArray<string>().ToList();
                                                    foreach (var (item, t) in from item in r.Where(s => s.ToLower() != "vline" && !s.StartsWith("#"))
                                                                              let t = item.ToCamelCase()
                                                                              select (item, t))
                                                    {
                                                        TitleStr = TitleStr.Replace(item, t);
                                                        if (DescStr.Contains(item))
                                                        {
                                                            DescStr = DescStr.Replace(item, t);
                                                        }
                                                    }
                                                }


                                            }
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
                    var task = (sender as WebView2).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");

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
        public void ProcessChannelContent(HtmlDocument doc, HtmlAgilityPack.HtmlNode targetSpan)
        {
            try
            {
                string divclassname = "right-section style-scope ytcp-video-list-cell-video";
                string idclass = "style-scope ytcp-img-with-fallback";
                string idNode = "style-scope ytcp-img-with-fallback";
                List<HtmlNode> idNodes = doc.DocumentNode.SelectNodes($"//img[@class='{idNode}']").ToList();
                List<HtmlNode> targetSpanList = doc.DocumentNode.SelectNodes($"//div[@class='{divclassname}']").ToList();
                string StatusNodeStr = "label-span style-scope ytcp-video-row";
                List<HtmlNode> StatusNode = doc.DocumentNode.SelectNodes($"//span[@class='{StatusNodeStr}']").ToList();
                string DateNodeStr = " cell-body tablecell-date sortable column-sorted  style-scope ytcp-video-row";
                //  List<HtmlNode> DateNode = doc.DocumentNode.SelectNodes($"//div[@class='{DateNodeStr}']").ToList();
                string aclassname = " remove-default-style  style-scope ytcp-video-list-cell-video";
                for (int i = 0; i < targetSpanList.Count; i++)
                {
                    if (canceltoken.IsCancellationRequested) break;
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
                        int idx1 = fullid.IndexOf(@"/mqdefault."), idx2 = fullid.IndexOf(@"com/vi");
                        if (idx1 != -1 && idx2 != -1)
                        {
                            Id = fullid.Substring(idx2 + 7, idx1 - 7 - idx2);
                        }
                    }
                    string divlass = " remove-default-style  style-scope ytcp-video-list-cell-video";
                    string html2 = item.OuterHtml; ;
                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.LoadHtml(html2);
                    foreach (var vitem in item.ChildNodes)
                    {
                        if (canceltoken.IsCancellationRequested) break;
                        TitleStr = (vitem.Name.ToLower() == "h3") ? vitem.InnerText : TitleStr;
                        DescStr = (item.Name.ToLower() == "div") ? vitem.InnerText : DescStr;
                        if (DescStr != "" && TitleStr != "") break;
                    }
                    if (TitleStr != "" && Id != "" && DescStr != "")
                    {
                        if (Idx.IndexOf(Id) == -1)
                        {
                            Idx.Add(Id);
                            recs++;
                            string ntitle = TitleStr.Replace("/n", "").Trim();
                            Dispatcher.Invoke(() =>
                            {
                                lblInsert.Content = recs.ToString();
                                lstMain.Items.Insert(0, $"{Id} {ntitle}");
                            });
                            dbInitializer?.Invoke(this, new CustomParmam_NewVideoInfo(ntitle, Id, IsUnlisted));
                            bool DoVideoInsert = true;
                            var p = new CustomParams_GetVideoFileName(Id, IsUnlisted);
                            dbInitializer?.Invoke(this, p);
                            if (p.found)
                            {
                                DoVideoInsert = (!p.found);
                                int index = Ids.IndexOf(Id);
                                if (index != -1)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        lstMain.Items[index] += $" {p.filename}";
                                        files++;
                                        if (files > 0)
                                        {
                                            lblLastNode.Content = $"{files} Inserted";
                                        }
                                        SetMargin(StatusBar);
                                    });
                                }
                                else
                                {
                                    files++;
                                    if (files > 0)
                                    {
                                        lblLastNode.Content = $"{files} Inserted";
                                    }
                                    for (int ix = 0; ix < lstMain.Items.Count; i++)
                                    {
                                        if (lstMain.Items[ix].ToString().Contains($"{Id}"))
                                        {
                                            lstMain.Items[ix] += $"{p.filename}";
                                            break;
                                        }
                                    }
                                    SetMargin(StatusBar);
                                }
                            }
                            if (DoVideoInsert)
                            {
                                DoNewVideoUpdate(Id);
                            }
                        }
                    }
                }
                if (NextRecord && LastNode != "")
                {
                    NextTask();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessChannelContent {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                    WebView2 temp = brdmain.Child as WebView2;
                    temp.NavigationCompleted += Wv2s_NavigationCompleted;
                    wv2Dictionary[swap].NavigationCompleted += wv2_NavigationCompleted;
                    brdmain.Child = wv2Dictionary[swap] as WebView2;
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
                    //var htmlx = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                    // File.WriteAllLines($"E:\\gopro9\\ScraperLog_{DateTime.Now}.txt", new string[] { htmlx });
                    // build files
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

        //select-files-button
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
                ex.LogWrite($"Click_Upload {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                ex.LogWrite($"Click_Upload {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public async void Click_Upload()
        {
            try
            {

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


        public async void Close_Upload()
        {
            try
            {
                // Execute script to click the second button with the ID "close-button"
                await ActiveWebView[1].CoreWebView2.ExecuteScriptAsync(@"
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
        private async void btnClickUpload_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                Click_Upload();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClickUpload_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                            string connectionStr = dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn ? conn : "";
                            directshortsScheduler = new DirectshortsScheduler(() => { Show(); }, DoOnScheduleComplete, listSchedules,
                                ReleaseDate.Value, ReleaseEndDate.Value, DoReportSchedule, ScheduleMax, IsTest);
                            directshortsScheduler.connectionString = connectionStr;
                        }
                        IsVideoLookup = true;

                        //string URL = webAddressBuilder.AddFiltersByDRAFT_UNLISTED(false).Finalize().Address;
                        if (DefaultUrl is not null && DefaultUrl != "")
                        {
                            ActiveWebView[1].ZoomFactor = 0.6;
                            ActiveWebView[1].NavigationCompleted += wv2v_NavigationCompleted;
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

        private void wv2Lookup_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (canceltoken.IsCancellationRequested) return;
            if ((e is not null && e.IsSuccess) || e is null)
            {
                NextRecord = false;
                var task = (sender as WebView2).CoreWebView2.ExecuteScriptAsync("document.body.innerHTML");
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
                if (DoAutoCancel is not null)
                {
                    if (DoAutoCancel.IsClosing) DoAutoCancel.Close();
                    while (!DoAutoCancel.IsClosed && !canceltoken.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    DoAutoCancel.Close();
                    DoAutoCancel = null;
                }
                DoAutoCancel = new AutoCancel(DoAutoCancelClose, "", 5, "Scheduling Finished");
                DoAutoCancel.ShowActivated = true;
                DoAutoCancel.Show();
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
                        if (DoAutoCancel is not null)
                        {
                            if (DoAutoCancel.IsClosing) DoAutoCancel.Close();
                            while (!DoAutoCancel.IsClosed && !canceltoken.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                                System.Windows.Forms.Application.DoEvents();
                            }
                            DoAutoCancel.Close();
                            DoAutoCancel = null;
                        }
                        DoAutoCancel = new AutoCancel(DoAutoCancelClose, "", 5, "Scheduling Finished");
                        DoAutoCancel.ShowActivated = true;
                        DoAutoCancel.Show();
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

        private void DoAutoCancelClose()
        {
            try
            {
                if ((DoAutoCancel is not null && DoAutoCancel.IsCloseAction))
                {
                    canceltoken.Cancel();
                    cancelds();
                    DoAutoCancel.Close();
                    Close();
                    return;
                }
                DoAutoCancel = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoAutoCancelClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void DoReportSchedule(DateTime dateTime, string id, string title)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    int r = directshortsScheduler.ScheduleNumber;
                    string t = $"{title.Replace("\n", "").Replace("\r", "").Trim()} {id} {r + 1} {dateTime}";
                    lstMain.Items.Insert(0, t);
                    System.Windows.Forms.Application.DoEvents();
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoReportSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                    RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
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


        public async Task<ButtonReturnType> IsButtonEnabled(WebView2 webView2)
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
                                            int TitleId = -1;
                                            int DescId = -1;
                                            int Id = -1;
                                            string connectionStr = dbInitializer?.Invoke(this, new CustomParams_GetConnectionString()) is string conn ? conn : "";
                                            string sql = $"SELECT * FROM SHORTSDIRECTORY WHERE ID = {idp}";
                                            connectionStr.ExecuteReader(sql, (FbDataReader r) =>
                                            {
                                                Id = (r["ID"] is Int32 i) ? i : -1;
                                                TitleId = (r["TITLEID"] is Int16 tid) ? tid : -1;
                                                DescId = (r["DESCID"] is Int16 did) ? did : -1;
                                            });

                                            if (id != -1)
                                            {
                                                if (TitleId != -1)
                                                {
                                                    var p = new CustomParams_GetTitle(TitleId, Id);
                                                    dbInitializer?.Invoke(this, p);
                                                    TitleStr = p.name;
                                                }
                                                if (DescId != -1)
                                                {
                                                    var p = new CustomParams_GetDesc(DescId, Id);
                                                    dbInitializer?.Invoke(this, p);
                                                    DescStr = p.name;
                                                }
                                            }

                                            if (TitleStr != "")
                                            {
                                                if ((sender as WebView2).CoreWebView2 != null)
                                                {
                                                    // Define the JavaScript code to replace the text content
                                                    string script = "var divElements = document.querySelectorAll('[aria-label=\"Tell viewers about your video (type @ to mention a channel)\"]');" +
                                                       "divElements.forEach(function(divElement) {" +
                                                      $"   divElement.textContent = '{TitleStr}';" +
                                                       "});";

                                                    // Execute the JavaScript code within the WebView2 control
                                                    (sender as WebView2).CoreWebView2.ExecuteScriptAsync(script);
                                                }
                                            }
                                            if (DescStr != "")
                                            {
                                                string script2 = "var divElements = document.querySelectorAll('[aria-label=\"Add a title that describes your video (type @ to mention a channel)\"]');" +
                                                       "divElements.forEach(function(divElement) {" +
                                                      $"   divElement.textContent = '{DescStr}';" +
                                                       "});";

                                                // Execute th       e JavaScript code within the WebView2 control
                                                (sender as WebView2).CoreWebView2.ExecuteScriptAsync(script2);
                                            }
                                            if (TitleStr != "" || DescStr != "")
                                            {
                                                if ((sender as WebView2).CoreWebView2 != null)
                                                {
                                                    while (true && !canceltoken.IsCancellationRequested)
                                                    {
                                                        var p = IsButtonEnabled((sender as WebView2)).GetAwaiter().GetResult();
                                                        if (p == ButtonReturnType.Enabled)
                                                        {
                                                            break;
                                                        }
                                                        Thread.Sleep(100);
                                                    }
                                                    // Define the JavaScript code to click the "Save" button
                                                    string script1 = "var saveButton = document.querySelector('.ytcp-button-shape-impl__button-text-content');" +
                                                                   "if (saveButton) {" +
                                                                   "   saveButton.click();" +
                                                                   "}";
                                                    // Execute the JavaScript code within the WebView2 control
                                                    (sender as WebView2).CoreWebView2.ExecuteScriptAsync(script1);
                                                    while (true && !canceltoken.IsCancellationRequested)
                                                    {
                                                        var p = IsButtonEnabled((sender as WebView2)).GetAwaiter().GetResult();
                                                        if (p == ButtonReturnType.Disabled)
                                                        {
                                                            break;
                                                        }
                                                        Thread.Sleep(100);
                                                    }
                                                }
                                                for (int i = ScheduledFiles.Count - 1; i >= 0; i--)
                                                {
                                                    if (canceltoken.IsCancellationRequested) break;
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
                        lstMain.Items[index1] += $" {filename}";
                        SetMargin(StatusBar);
                    }
                }
                else lstMain.Items[index1] += $" {filename}";
                if (ScraperType == EventTypes.ScapeSchedule)
                {
                    dbInitializer?.Invoke(this, new CustomParams_InsertWithId(IntId, filename, IsUnlisted));
                }
                files++;
                Dispatcher.Invoke(() =>
                {
                    if (files > 0)
                    {
                        lblLastNode.Content = $"{files} Inserted";
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

    internal class InsertIntoTable
    {
        private string vid;
        private string filename;

        public InsertIntoTable(string vid, string filename)
        {
            this.vid = vid;
            this.filename = filename;
        }
    }
}