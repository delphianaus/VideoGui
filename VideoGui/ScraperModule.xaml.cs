using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;
using Window = System.Windows.Window;
using System.Threading;
using System.Linq;
using VideoGui.Models;
using VideoGui.Models.delegates;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input.Manipulations;
using System.Windows.Media.Animation;
using System.Windows.Automation;
using Path = System.IO.Path;
using System.Linq.Expressions;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using CliWrap;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.DirectoryServices;
using System.Diagnostics;
using System.Xml.Linq;
using CliWrap.EventStream;
using Microsoft.Extensions.Hosting;
using System.DirectoryServices.AccountManagement;
using System.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Task = System.Threading.Tasks.Task;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static System.Windows.Forms.AxHost;
using FirebirdSql.Data.FirebirdClient;

namespace VideoGui
{
    public class uploads
    {
        public string file = "";
        public bool uploaded = false, finished = false;
        public uploads(string _file, bool _uploaded, bool _finished)
        {
            file = _file;
            uploaded = _uploaded;
            finished = _finished;
        }

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScraperModule : Window
    {
        object lockobj = new object();
        int MaxUploads = 0, recs = 0, gmaxrecs = 0, files = 0, max = 0, SlotsPerUpload = 0;
        public int TotalScheduled = 0;
        bool Valid = false, IsUnlisted = false, IsDashboardMode = false, CanSpool = false, FirstRun = true, done = false;
        bool finished = false, Uploading = false, NextRecord = false, Processing = false, clickupload = true;
        public bool IsClosing = false, IsClosed = false, Exceeded = false;
        List<uploads> uploaded = new();
        List<string> nextaddress = new(), Ids = new(), Idx = new(), ufiles = new(), Files = new();// DoneFiles = new();
        WebAddressBuilder webAddressBuilder = null;
        databasehook<object> dbInitializer = null;
        OnFinish DoOnFinish = null;
        int swap = 1;
        bool SwapEnabled = false, IsTitleEditor = false;
        string Title = "", Desc = "";
        string SendKeysString = "", UploadPath = "", LastNode = "", DefaultUrl = "", LastValidFileName = "";
        Dictionary<int, WebView2> wv2Dictionary = new Dictionary<int, WebView2>();
        Dictionary<int, WebView2> ActiveWebView = new Dictionary<int, WebView2>();
        DispatcherTimer InternalTimer = new DispatcherTimer();
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public ScraperModule(databasehook<object> _dbInit, OnFinish _OnFinish, string _Default_url, bool DashboardMode = false,
            string _Title = "", string _Desc = "")
        {
            try
            {
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                IsDashboardMode = DashboardMode;
                Title = _Title;
                Desc = _Desc;
                IsTitleEditor = true;
                dbInitializer = _dbInit;
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke();
                };
                webAddressBuilder = new WebAddressBuilder(null, null, "UCdMH7lMpKJRGbbszk5AUc7w");

            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");

            }
        }

        public ScraperModule(databasehook<object> _dbInit, OnFinish _OnFinish, string _Default_url,
            bool _isUnlisted = false, bool DashboardMode = false,
            int maxuploads = 100, int slotsperupload = 5, bool enabledswap = false)
        {
            try
            {
                SwapEnabled = enabledswap;
                MaxUploads = maxuploads;
                DoOnFinish = _OnFinish;
                DefaultUrl = _Default_url;
                IsDashboardMode = DashboardMode;
                dbInitializer = _dbInit;
                IsUnlisted = _isUnlisted;
                SlotsPerUpload = slotsperupload;
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    DoOnFinish?.Invoke();
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
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
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
                    int Id = (sender as WebView2).Name.Replace("wv2A", "").ToInt(-1);
                    string source = (sender as WebView2).Source.AbsoluteUri.ToString(), IntId = "";
                    int p1 = source.IndexOf("video/"), p2 = source.IndexOf("/edit");
                    if (p1 != -1 && p2 != -1)
                    {
                        IntId = source.Substring(p1 + 6, p2 - p1 - 6);
                    }
                    var task = (sender as WebView2).ExecuteScriptAsync("document.body.innerHTML");
                    task.ContinueWith(x => { ProcessHTML(x.Result, Id, IntId); },
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
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InitAsync {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Verb = "runas",
                    Arguments = "/c subst.exe Z: /D"
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                ProcessStartInfo startInfo2 = new ProcessStartInfo()
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Verb = "runas",
                    Arguments = "/c subst.exe Z: " + args
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
        public void ExecuteAsNonAdmin(string arguments)
        {
            try
            {
                string args = arguments;
                if (arguments.Contains(" "))
                {
                    args = "\"" + arguments + "\"";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("subst z: /d" + Environment.NewLine);
                sb.Append("subst z: " + args);
                string cdir = Environment.CurrentDirectory;
                File.WriteAllText(cdir + "\\map.bat", sb.ToString());
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = @"explorer.exe",
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
        public bool CycleThroughChildWindows()
        {
            try
            {
                ExitDialog = true;

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

        bool EditDone = false, btndone = false;
        private bool EnumChildWindowCallback(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                int controlId = GetDlgCtrlID(hWnd);
                if (className.ToString() == "Edit" && controlId == 0x47C && !EditDone) // Check for Edit control with ID 000000000000047C
                {
                    while (true)
                    {
                        SendMessage(hWnd, 0x000C, IntPtr.Zero, SendKeysString);
                        List<string> keys = SendKeysString.Split(' ').ToList().Where(s => s != "").ToList();
                        for (int i = 0; i < keys.Count; i++)
                        {
                            keys[i] = keys[i].Replace("\"", "");
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
                if (html is not null && html.Contains(Span_Name))
                {
                    HtmlDocument doc = new HtmlDocument();
                    0doc.LoadHtml(html);
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

        bool ExitDialog = false, HasExited = false;
        public List<string> ScheduledOk = new List<string>();
        int ts = 0;
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

                    Files.Clear();
                    Files = Directory.EnumerateFiles(UploadPath, "*.mp4", SearchOption.AllDirectories).ToList();
                    int max = 0;

                    SendKeysString = "";
                    var p1 = new CustomParams_GetConnectionString();
                    dbInitializer?.Invoke(this, p1);
                    string connectionStr = p1.ConnectionString;
                    using (var connection = new FbConnection(connectionStr))
                    {
                        connection.Open();
                        string sql = "select count(Id) from UPLOADSRECORD WHERE UPLOAD_DATE = @P0";
                        using (var command = new FbCommand(sql.ToUpper(), connection))
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@p0", DateTime.Now.Date);
                            object res = command.ExecuteScalar();
                            if (res is long resxx)
                            {
                                TotalScheduled = resxx.ToInt();
                                lblTotal.Content = TotalScheduled.ToString();
                            }
                        }
                        connection.Close();
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
                                SendKeysString += "\"" + @"Z:\" + new DirectoryInfo(Path.GetDirectoryName(f)).Name + "\\" + Path.GetFileName(f) + "\" ";
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
                            r = r + nm + " ";
                        }

                        lstMain.Items.Insert(0, $"{max} {r}");
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
                    string connectStr = "";
                    var p = new CustomParams_GetConnectionString();
                    dbInitializer?.Invoke(this, p);
                    connectStr = p.ConnectionString;

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
                        while (true)
                        {
                            if (ExitDialog) return;
                            var html = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                            bool found = false;
                            List<HtmlNode> Nodes = GetNodes(html, Span_Name);
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
                                            var buttonLabel = $"Edit video {filename}";
                                            lstMain.Items.Insert(0, filename + " = " + buttonLabel);
                                            await ActiveWebView[1].ExecuteScriptAsync($"document.querySelector('button[aria-label=\"{buttonLabel}\"]').click()");
                                            var cts = new CancellationTokenSource();
                                            while (!cts.IsCancellationRequested)
                                            {
                                                if (ExitDialog)
                                                {
                                                    cts.Cancel();
                                                    return;
                                                }
                                                var htmlcheck = Regex.Unescape(await ActiveWebView[1].ExecuteScriptAsync("document.body.innerHTML"));
                                                if (htmlcheck is not null)
                                                {
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
                                                        break;
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

                            Close();
                        }

                    }
                    /*if (!Exceeded && finished && MaxUploads > 0)
                    {
                        bool ready = false;
                        while (true)
                        {
                            if (ExitDialog)
                            {
                                return;
                            }
                            var html = await wv2.ExecuteScriptAsync("document.body.innerHTML");
                            if (html.Contains("Uploads complete"))
                            {
                                List<HtmlNode> Nodes1x = GetNodes(html, Span_Name);
                                var deleteTasks = Nodes1x.Where(node => node.InnerText.ToLower().Contains("100% uploaded"))
                                    .Select(node =>
                                    {
                                        string filename = node.InnerText.Split("\n").Where(x => x.Trim() != "").Select(p => p.Trim()).FirstOrDefault();
                                        return Task.Run(() =>
                                        {
                                            Directory.EnumerateFiles("Z:\\", filename, SearchOption.AllDirectories).ToList().ForEach(File.Delete);
                                        });
                                    });
                                await Task.WhenAll(deleteTasks);
                                break;
                            }
                            await Task.Delay(200);
                        }
                    }*/
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

        private void UploadedHandler(string Span_Name, string connectStr, string filename1)
        {
            try
            {
                if (filename1 != "" && ScheduledOk.IndexOf(filename1) == -1)
                {
                    string[] files = Directory.GetFiles("Z:\\", filename1, SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                        NodeUpdate(Span_Name, ScheduledGet);
                        lstMain.Items.Insert(0, $"{file} Deleted");
                        int res = -1;
                        using (var connection = new FbConnection(connectStr))
                        {
                            connection.Open();
                            string sql = "select * from UPLOADSRECORD WHERE UPLOADFILE = @P0";
                            using (var command = new FbCommand(sql.ToUpper(), connection))
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@p0", file);
                                object result = command.ExecuteScalar();
                                if (result is Int16 idxx)
                                {
                                    res = idxx;
                                }
                            }
                            if (res == -1)
                            {
                                sql = "INSERT INTO UPLOADSRECORD(UPLOADFILE, UPLOAD_DATE, UPLOAD_TIME) VALUES (@P0,@P1,@P2) RETURNING ID";
                                using (var command = new FbCommand(sql.ToUpper(), connection))
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@p0", file);
                                    command.Parameters.AddWithValue("@p1", DateTime.Now.Date);
                                    command.Parameters.AddWithValue("@p2", DateTime.Now.TimeOfDay);
                                    object result = command.ExecuteScalar();
                                    if (result is Int16 idxx)
                                    {
                                        res = idxx;
                                    }
                                }
                            }
                            else
                            {
                                sql = "UPDATE UPLOADSRECORD SET UPLOAD_DATE = @P1, UPLOAD_TIME = @P2 WHERE ID = @P0";
                                using (var command = new FbCommand(sql.ToUpper(), connection))
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@p0", res);
                                    command.Parameters.AddWithValue("@p1", DateTime.Now.Date);
                                    command.Parameters.AddWithValue("@p2", DateTime.Now.TimeOfDay);
                                    command.ExecuteNonQuery();
                                }
                            }
                            connection.Close();
                        }
                        ScheduledOk.Add(filename1);
                        TotalScheduled++;
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
                wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
                Dispatcher.Invoke(() =>
                {
                    InitAsync();
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Wv2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            try
            {
                wv2.Source = new Uri(webAddressBuilder.GetChannelURL().Address);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Wv2_CoreWebView2InitializationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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
                            while (!cts1.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            Click_Finish();
                            cts1.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts1.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            //                     Click_Upload();

                            return;
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromMilliseconds(500));
                            while (!cts.IsCancellationRequested)
                            {
                                Thread.Sleep(100);
                            }
                            ProcessWebViewComplete(sender);
                            return;
                        }
                        return;
                        List<HtmlNode> Nodes = doc.DocumentNode.SelectNodes($"//li[@class='{Span_Name}']").ToList();
                        var uploadedDictionary = uploaded.ToDictionary(v => v.file);
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
                        foreach (var file in uploaded.Where(file => !file.uploaded && file.finished))
                        {
                            File.Delete(file.file);
                        }
                    }
                    else
                    {
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromMilliseconds(1500));
                        while (!cts.IsCancellationRequested)
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


        private async void ProcessWV2(string html, object sender)
        {
            try
            {
                if (html is not null)
                {
                    var ehtml = Regex.Unescape(html);
                    if (html is not null && ehtml.Contains("Customize channel"))
                    {
                        Processing = !YouTubeLoaded();
                    }
                    else if (ehtml is not null && ehtml.Contains("Channel dashboard"))
                    {
                        if (clickupload)
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
                        else if (IsTitleEditor)
                        {
                            ContentClick();
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromSeconds(10));
                            while (!cts.IsCancellationRequested)
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
        DispatcherTimer CloseTimer = new DispatcherTimer();
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
                    task.ContinueWith(x => { ProcessWV2Completed(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
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
        private async void wv2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
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
                List<HtmlNode> DateNode = doc.DocumentNode.SelectNodes($"//div[@class='{DateNodeStr}']").ToList();
                string aclassname = " remove-default-style  style-scope ytcp-video-list-cell-video";
                for (int i = 0; i < targetSpanList.Count; i++)
                {
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
                    string Title = "", Desc = "", html2 = item.OuterHtml; ;
                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.LoadHtml(html2);
                    foreach (var vitem in item.ChildNodes)
                    {
                        Title = (vitem.Name.ToLower() == "h3") ? vitem.InnerText : Title;
                        Desc = (item.Name.ToLower() == "div") ? vitem.InnerText : Desc;
                        if (Desc != "" && Title != "") break;
                    }
                    if (Title != "" && Id != "" && Desc != "")
                    {
                        if (Idx.IndexOf(Id) == -1)
                        {
                            Idx.Add(Id);
                            recs++;
                            string ntitle = Title.Replace("/n", "").Trim();
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
        public bool KilledUploads = false;
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
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

        private async void btnClickUpload_Click(object sender, RoutedEventArgs e)
        {
            await ActiveWebView[1].CoreWebView2.ExecuteScriptAsync(@"
                var buttons = document.querySelectorAll('#uploads');
                if (buttons.length > 1) {
                    buttons[1].click();
                }
            ");
        }

        public bool YouTubeLoaded()
        {
            try
            {
                if (webAddressBuilder is not null)
                {
                    //string URL = webAddressBuilder.AddFiltersByDRAFT_UNLISTED(false).Finalize().Address;
                    if (DefaultUrl is not null && DefaultUrl != "")
                    {
                        ActiveWebView[1].Source = new Uri(DefaultUrl);
                        return true;
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
                    btnClickUpload.Margin = p;

                    lstMain.Width = Width - 25;
                    StatusBar.Width = Width - 5;
                    RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("WebWidth", ActualWidth);
                    key.SetValue("WebHeight", ActualHeight);
                    key.SetValue("Webleft", Left);
                    key.SetValue("Webtop", Top);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }


        public void ProcessHTML(string html, int id, string IntId)
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
                                task.ContinueWith(x => { ProcessHTML(x.Result, id, IntId); }, TaskScheduler.FromCurrentSynchronizationContext());
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
                    }
                }
                else lstMain.Items[index1] += $" {filename}";
                dbInitializer?.Invoke(this, new CustomParams_InsertWithId(IntId, filename, IsUnlisted));
                files++;
                Dispatcher.Invoke(() =>
                {
                    if (files > 0)
                    {
                        lblLastNode.Content = $"{files} Inserted";
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