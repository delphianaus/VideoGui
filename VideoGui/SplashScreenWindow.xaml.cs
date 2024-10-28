using CliWrap;
using Microsoft.Win32;
using Nancy;
using Nancy.Owin;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static VideoGui.Extensions;
using static VideoGui.MainWindow;
using System.Management;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : System.Windows.Window
    {
        System.Windows.Forms.Timer DbLayerInitiateTimer, UpdateProgess;
        MainWindow MainAppWindow;
        string filename_pegpeg, link;
        long bytesdone = 0, oldbytesdone = -1;
        bool done = false;
        bool ffmpegready = false;
        string Root, ffmpeg_ver = "", ffmpeg_gitver = "";
        Int64 contentLength = 0;
        int statusupdate = 0;
        float percent = 0;
        long prcdone = 0;
        string onfinish = "";
        public SplashScreenWindow()
        {
            try
            {
                InitializeComponent();
                KillFFMPEG().ConfigureAwait(true);
                DbLayerInitiateTimer = new System.Windows.Forms.Timer();
                DbLayerInitiateTimer.Tick += new EventHandler(DbLayerInitiateTimer_Tick);
                DbLayerInitiateTimer.Interval = (int)new TimeSpan(0, 0, 1).TotalMilliseconds;
                DbLayerInitiateTimer.Start();

                UpdateProgess = new System.Windows.Forms.Timer();
                UpdateProgess.Tick += new EventHandler(UpdateProgess_Tick);
                UpdateProgess.Interval = (int)new TimeSpan(0, 0, 1).TotalMilliseconds;
                UpdateProgess.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public bool IsAMDGPUVERSIONOK()
        {
            try
            {
                bool res = true;
                String Card = string.Empty;
                string DriverVer = "";
                ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyData description = mo.Properties["Description"];
                    PropertyData Driver = mo.Properties["DriverVersion"];

                    if ((description.Value != null) && (Driver.Value != null))
                    {
                        Card = description.Value.ToString();
                        DriverVer = Driver.Value.ToString();
                        if (Card.ToLower().Contains("amd radeon rx") && DriverVer != "31.0.14001.45012")
                        {
                            res = false;
                        }
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
        private async Task<bool> KillFFMPEG()
        {
            try
            {
                string myStrQuote = "\"";
                ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process where name = {myStrQuote}ffmpeg.exe{myStrQuote}");
                foreach (ManagementObject o in searcher.Get())
                {
                    string HandleID = o.Properties["Handle"].Value.ToString();
                    string ParentProcessId = o.Properties["ParentProcessID"].Value.ToString();
                    string ID = o.Properties["ProcessID"].Value.ToString();
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
                   
                }
                var pt = Process.GetProcessesByName("ffmpeg.exe");
                
                if (pt != null)
                {
                    if (pt.Count() > 0) KillFFMPEG().ConfigureAwait(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        private void Progress_HttpReceiveProgressFFMPEG(object? sender, HttpProgressEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Progress_HttpReceiveProgressFFMPEG(sender, e));
                    return;
                }
                lblStatus.Content = "Status : [Downloading FFMPEG " + e.ProgressPercentage.ToString() + " %]";
                if (e.BytesTransferred < e.TotalBytes)
                {
                    lblStatus.Content = "Status : [Downloading FFMPEG " + e.ProgressPercentage.ToString() + " %]";
                }
                else
                {
                    lblStatus.Content = "Status : [Finsihed Downloading FFMPEG]";

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 14a2 " + ex.Message);
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

        private void UpdateProgess_Tick(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DbLayerInitiateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DbLayerInitiateTimer.Stop();
                RunMainApp();


            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
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


        public void UpdateStatus(string status)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => UpdateStatus(status));
                    return;
                }
                lblStatus.Content = status;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " WebClientDownload_FFMPEG_Completed " + ex.Message);
            }
        }

        public void UpdateDownloadProgress(long _percent, bool _done)
        {
            try
            {
                
                if (!done)
                {
                    string str = $"Status : [Downloading FFMPEG {_percent} %]";
                    Dispatcher.Invoke(() =>
                    {
                        lblStatus.Content = (lblStatus.Content == str) ? lblStatus.Content : str;
                    });
                   
                }
                else
                {
                    done = _done;
                    prcdone = -1;
                    onfinish = "Completed";
                   
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " WebClientDownload_FFMPEG_Completed " + ex.Message);
            }
        }


        public void PercentageUpdate(long _percent)
        {
            try
            {
               
                if (percent != 1000)
                {
                    prcdone = _percent;
                   
                }
                else
                {
                    prcdone += -1;
                    onfinish = "Errored";
                    done = true;
                    statusupdate = 5;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " WebClientDownload_FFMPEG_Completed " + ex.Message);
            }
        }

        public void OnDownloadDone()
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => OnDownloadDone());
                    return;
                }
                done = true;
                string str = $"Status : [Downloading FFMPEG Completed Ok]";
                lblStatus.Content = (lblStatus.Content == str) ? lblStatus.Content : str;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " WebClientDownload_FFMPEG_Completed " + ex.Message);
            }
        }
        public async Task RunFFMPEGDownload(string URL)
        {
            int lineno = 0;
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => RunFFMPEGDownload(URL));
                    return;
                }

                /*
                  public delegate void OnPercentUpdate(long _percent);
                  public delegate void OnFinish();
                 */
                OnPercentUpdate _OnPercentUpdate = PercentageUpdate;
                OnFinish _OnFinished = OnDownloadDone;
                string TitleA = "";
                int zipsize = 0;
                //progress.HttpReceiveProgress += Progress_HttpReceiveProgressFFMPEG;
                CancellationTokenSource ProcessingCancellationTokenSource = new CancellationTokenSource();
                var client = HttpClientFactory.Create();
                lblStatus.Content = "Status : [Downloading FFMPEG]";
                statusupdate = 0;
                lineno = 1;

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.OpenRead(URL);
                contentLength = Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
                wc.Dispose();
                done = false;
                percent = 0;
                lineno = 2;
                using (var file = await client.GetStreamAsync(URL).ConfigureAwait(false)) 
                {
                    lineno = 3;
                    using (var Ms = new MemoryStream())
                    {
                        lineno = 4;
                        while (true)
                        {
                            done = false;
                            await Task.Run(() => file.CopyToAsync(Ms, _OnPercentUpdate, _OnFinished, contentLength, ProcessingCancellationTokenSource.Token).ConfigureAwait(false));
                            while (!done)
                            {
                                Thread.Sleep(100);
                            }
                            if (Ms.Length == contentLength) break;
                        }


                        //var cts = new CancellationTokenSource();
                        //cts.CancelAfter(TimeSpan.FromMinutes(5));
                        //while ((!cts.IsCancellationRequested) && (!done))
                        // {
                        //     Thread.Sleep(250);
                        // }

                        statusupdate = 1;
                        List<Stream> ZipStreams = new List<Stream>();
                        List<string> ZipFileNames = new List<string>();
                        lineno = 9;
                        string AppName = GetExePath();// System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        string AppPath = "C:\\Program Files\\7-Zip\\7z.dll";
                        SevenZipExtractor.SetLibraryPath(AppPath);
                        using (var arch = new SevenZipExtractor(Ms))
                        {
                            lineno = 10;
                            foreach (string filename in arch.ArchiveFileNames)
                            {
                                string[] names = { "\\bin\\ffmpeg", "\\bin\\ffprobe" };
                                foreach (string name in names)
                                {
                                    if (filename.Contains(name))
                                    {
                                        Stream ms = new MemoryStream();
                                        ZipStreams.Add(ms);
                                        arch.ExtractFile(filename, ms);
                                        int i = filename.IndexOf(name);
                                        string Name2 = filename.Substring(i + "\\bin\\".Length);
                                        string destinationPath = AppName + "\\";
                                        destinationPath += System.IO.Path.GetFileName(Name2);
                                        ZipFileNames.Add(destinationPath);
                                    }
                                }
                            }
                        }
                        lineno = 11;

                        int streamindex = 0;
                        List<Task> ListOfTasks = new List<Task>();
                        statusupdate = 2;
                        foreach (Stream mss in ZipStreams)
                        {
                            string filename = ZipFileNames[streamindex++];
                            ListOfTasks.Add(Task.Run(() => WriteStream(mss, filename)));
                        }
                        Task.WaitAll(ListOfTasks.ToArray());
                        statusupdate = 3;
                        lineno = 12;
                        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                        Registry.CurrentUser.CreateSubKey(@"SOFTWARE\VideoProcessor");
                        key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                        key?.SetValue("ffmpeg_date", ffmpeg_ver);
                        key?.SetValue("ffmpeg_gitver", ffmpeg_gitver);
                        key?.Close();
                        statusupdate = 4;
                        ffmpegready = true;

                    }
                }
            }
            catch (Exception ex)
            {
                statusupdate = 5;
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + $" @ {lineno}  " + ex.Message);
            }


        }



        public string GetExePath()
        {
            try
            {
                string res = "";
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string defaultprogramlocation = key?.GetValueStr("defaultprogramlocation", "c:\\videogui");
                key?.Close();
                res = (Debugger.IsAttached) ? defaultprogramlocation : System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        private void Progress_HttpReceiveProgress(object? sender, HttpProgressEventArgs e)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Progress_HttpReceiveProgress(sender, e));
                    return;
                }
                if ((e.BytesTransferred < e.TotalBytes) && (!ffmpegready))
                {
                    lblStatus.Content = "Status : [unzipping 7zip]";
                    if (e.ProgressPercentage == 100)
                    {
                        ffmpegready = true;
                    }
                }
                else
                {
                    lblStatus.Content = "Status : [downloading 7zip]" + e.ProgressPercentage.ToString() + " %]";
                    ffmpegready = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 14a2 " + ex.Message);
            }

        }
        public async Task GetUpdateAsync(string URL, IUpdateParserHTML parser, bool test = false)
        {
            bool finished = false;
            try
            {
                var client = HttpClientFactory.Create();
                var response = client.GetStringAsync(URL).ConfigureAwait(true);
                string result = response.GetAwaiter().GetResult();
                (link, filename_pegpeg, ffmpeg_ver, ffmpeg_gitver) = parser.ParseHTML(result);
                if (link != "") 
                {
                    try
                    {
                        await RunFFMPEGDownload(link).ConfigureAwait(false);
                        finished = true;
                    }
                    catch (Exception ex1)
                    {
                        ex1.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex1.Message);

                    }
                }
                else
                {
                    ffmpegready = true;
                }
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
            finally
            {
                if (!finished)
                {
                    ffmpegready = true;

                }
            }

        }

        public async Task RunDownload7Zip(string URL)
        {

            try
            {
                var progress = new ProgressMessageHandler();
                progress.HttpReceiveProgress += Progress_HttpReceiveProgress;
                var client = HttpClientFactory.Create();
                using (var response = await client.GetStreamAsync(URL))
                {
                    using (ZipArchive zipArchive = new ZipArchive(response, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries)
                        {
                            string filename = zipEntry.FullName;
                            string dest = "c:\\program files\\7-zip\\" + System.IO.Path.GetFileName(filename);
                            if (System.IO.File.Exists(dest))
                            {
                                dest = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                            }
                            string fname = System.IO.Path.GetFileName(dest);
                            string dirid = System.IO.Path.GetDirectoryName(dest);
                            if (!Directory.Exists(dirid)) Directory.CreateDirectory(dirid);
                            if (fname != "")
                            {
                                DeleteIfExists(dest);
                                zipEntry.ExtractToFile(dest, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 14a2 " + ex.Message);
            }
        }
        public void Download7zip()
        {
            try
            {

                string URL = "https://drive.google.com/uc?export=download&id=1IKlzLTJScQZOFcxCO1Yd-h7EZTVdAmVF";
                RunDownload7Zip(URL).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 14a2 " + ex.Message);
            }
        }

        public void Terminate()
        {
            try
            {
                var ctse = new CancellationTokenSource();
                ctse.CancelAfter(TimeSpan.FromSeconds(3));
                while (!ctse.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
                string AppNamesxx = Process.GetCurrentProcess().ProcessName;
                int id = Process.GetCurrentProcess().Id;
                var ps1 = Process.GetProcessesByName(AppNamesxx);
                foreach (var p in ps1.Where(s => s.Id == id))
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 14a2 " + ex.Message);
            }
        }
        public void RunMainApp()
        {
            try
            {
                bool isAdmin = false;
                if (!isAdmin.IsAdministrator())
                {
                    lblStatus.Content = "Status : Shutting Down Requires Admin Rights";
                    Terminate();
                }
                string AppNames = Process.GetCurrentProcess().ProcessName;
                int _Id = Process.GetCurrentProcess().Id;
                var ps = Process.GetProcessesByName(AppNames).Where(i=>i.Id!=_Id).ToList();
                ffmpegready = true;
                if (ps !=null)
                {
                    bool found = false;
                    int cnt = 0;
                    foreach (Process v in ps)
                    {
                        found = false;
                        if (!v.HasExited)
                        {
                            cnt++;
                            break;
                        }
                    }
                    if (cnt >= 1)
                    {
                        lblStatus.Content = "Status : Already Running";
                        Thread.Sleep(4000);
                        Terminate();
                    }

                    if (!IsAMDGPUVERSIONOK())
                    {
                        lblStatus.Content = "AMD Radeon Driver Required 31.0.14001.45012, Please Install Adrenalin 23.2.1";
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromSeconds(2));
                        while (!cts.IsCancellationRequested)
                        {
                            Thread.Sleep(250);
                        }
                        Terminate();
                    }

                    lblYouTubeHelper.Content += " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    var cts1 = new CancellationTokenSource();
                    cts1.CancelAfter(TimeSpan.FromSeconds(3));
                    while (!cts1.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(() => RunMainApp());
                        return;
                    }
                    lblStatus.Content = "Status : Checking For 7ZIP update";
                    string AppName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    AppName += "\\7z.dll";
                    if (!((System.IO.File.Exists("C:\\Program Files\\7-Zip\\7z.dll") || (System.IO.File.Exists(AppName)))))
                    {
                        Task.Run(() => Download7zip());
                    }
                    string sbpath = "";
                    string AppPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    if (AppPath != "")
                    {
                        lblStatus.Content = "Status : Checking For FFMPEG update";
                        statusupdate = 0;
                        ffmpegready = false;
                        bytesdone = 0;
                        int oldid = -1;
                        done = true;
                        //CheckForFFPEGUpdate();

                        lblStatus.Content = "Status : Downloading FFMPEG";
                        while (!ffmpegready && !done)
                        {
                            switch (statusupdate)
                            {
                                case 0:
                                    {
                                        if (prcdone != -1)
                                        {
                                            string str = $"Status : [Downloading FFMPEG {prcdone} %]";
                                            lblStatus.Content = str;
                                        }
                                        else
                                        {
                                            string str = $"Status : [Downloading FFMPEG {onfinish}]";
                                            lblStatus.Content =  str;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        lblStatus.Content = "Status : [Unzippping FFMPEG]";
                                        break;
                                    }
                                case 2:
                                    {
                                        lblStatus.Content = "Status : [Unzippped FFMPEG]";
                                        break;
                                    }
                                case 3:
                                    {
                                        lblStatus.Content = "Status : [Updating Registry]";
                                        break;
                                    }
                                case 5:
                                    {
                                        lblStatus.Content = "Error : [FFMPEG Update]";
                                        ffmpegready = true;
                                        break;
                                    }


                            }

                        }
                        lblStatus.Content = "Status : Launching Main App";
                        MainAppWindow = new MainWindow(DoOnFinish);
                        Hide();
                        MainAppWindow.ShowActivated = true;
                        MainAppWindow.Show();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                Close();
            }
        }

        private void DoOnFinish()
        {
            if (MainAppWindow.HideWindow())
            {
                lblStatus.Content = "Status : Shutting Down App";
                Terminate();
            }
        }

       
    }
}
