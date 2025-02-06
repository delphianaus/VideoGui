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
using Nancy.TinyIoc;

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


                return;// below code encrpts string and returns it as c# code
                string str = GetEncryptedString("ffmpeg");

                if (str != "")
                {
                    System.Windows.Clipboard.SetText(str);
                }

                Terminate();
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
                var r = GetEncryptedString(new int[] { 165, 26, 99, 115, 210, 243, 142, 25, 178, 91, 
                    63, 142, 249, 220, 120, 113, 55, 173, 206, 201, 134, 206, 205, 105, 23, 91, 223, 
                    250, 59, 243, 113, 171, 63, 252, 103 }.Select(i => (byte)i).ToArray());
                var d = GetEncryptedString(new int[] { 178, 58, 92, 85, 227, 206, 222, 71, 251, 114, 3 }.Select(i => (byte)i).ToArray());
                var dv = GetEncryptedString(new int[] { 178, 45, 70, 64, 244, 213, 248, 86, 224, 110, 4, 174, 218 }.Select(i => (byte)i).ToArray());
                ManagementObjectSearcher searcher = new(r);
                var amd = GetEncryptedString(new int[] { 151, 50, 75, 22, 227, 198, 202, 86, 253, 115, 77, 179, 204 }.Select(i => (byte)i).ToArray()); ;
                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyData description = mo.Properties[d];
                    PropertyData Driver = mo.Properties[dv];

                    if ((description.Value != null) && (Driver.Value != null))
                    {
                        Card = description.Value.ToString();
                        DriverVer = Driver.Value.ToString();
                        if (Card.ToLower().Contains(amd) && DriverVer != "31.0.14001.45012")
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
                var x = GetEncryptedString(new int[] { 166, 45, 64, 85, 244, 212, 221, 122, 214 }.Select(i => (byte)i).ToArray());
                var pp = GetEncryptedString(new int[] { 166, 62, 93, 83, 255, 211, 254, 65, 253, 126, 8, 178, 199, 181, 107 }.Select(i => (byte)i).ToArray());
                var d = GetEncryptedString(new int[] { 165, 26, 99, 115, 210, 243, 142, 25, 178, 91, 63, 142, 249, 220, 120, 113, 55, 
                    173, 206, 201, 128, 213, 198, 111, 29, 107, 195, 180, 56, 
                    233, 123, 181, 54, 185, 123, 229, 249, 81 }.Select(i => (byte)i).ToArray());
                string s= GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 86, 234, 120 }.Select(i => (byte)i).ToArray());
                var h = GetEncryptedString(new int[] { 190, 62, 65, 82, 253, 194 }.Select(i => (byte)i).ToArray());
                ManagementObjectSearcher searcher = new(d+$" = {myStrQuote}{s}{myStrQuote}");
                foreach (ManagementObject o in searcher.Get())
                {
                    string HandleID = o.Properties[h].Value.ToString();
                    string ParentProcessId = o.Properties[pp].Value.ToString();
                    string ID = o.Properties[x].Value.ToString();
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

               
                
                var pt = Process.GetProcessesByName(GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192 }.Select(i => (byte)i).ToArray()));
                
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


                var r = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 70, 41, 174, 195, 146, 67, 119, 56, 250, 149, 248, 183, 135, 239, 74, 53, 72, 245, 211, 111 }.Select(i => (byte)i).ToArray()); ;

                lblStatus.Content = r + e.ProgressPercentage.ToString() + " %]";
                if (e.BytesTransferred < e.TotalBytes)
                {
                    lblStatus.Content = r + e.ProgressPercentage.ToString() + " %]";
                }
                else
                {
                    var s = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 70, 43, 168, 218, 143, 70, 112, 60, 250, 220, 210, 191, 208, 199, 96, 23, 121, 
                        212, 253, 33, 230, 62, 129, 21, 212, 69, 193, 211, 105 }.Select(i => (byte)i).ToArray());
                    lblStatus.Content = s;

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

        public bool isLoggedOn()
        {
            try
            {
                var x = GetEncryptedString(new int[] { 129, 54, 65, 90, 254, 192, 193, 93 }.Select(i => (byte)i).ToArray());
                Process[] pname = Process.GetProcessesByName(x);
                return (pname.Length == 0) ? false : true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }
        private void DbLayerInitiateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DbLayerInitiateTimer.Stop();
                if (isLoggedOn())
                {
                    RunMainApp();
                }
                else
                {
                    DbLayerInitiateTimer.Interval = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
                    DbLayerInitiateTimer.Start();
                }
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
                var p = GetEncryptedString(new int[] { 214, 8, 74, 84, 210, 203, 199, 86, 252, 105, 41, 174, 195, 146, 67,
                    119, 56, 250, 163, 208, 150, 234, 249, 73, 63, 71, 243, 251, 34, 241, 114, 162, 
                    39, 252, 113, 164 }.Select(i => (byte)i).ToArray());
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + p + ex.Message);
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
                var p = GetEncryptedString(new int[] { 214, 8, 74, 84, 210, 203, 199, 86, 252, 105, 41, 174, 195, 146, 67,
                    119, 56, 250, 163, 208, 150, 234, 249, 73, 63, 71, 243, 251, 34, 241, 114, 162,
                    39, 252, 113, 164 }.Select(i => (byte)i).ToArray());
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + p  + ex.Message);
            }
        }

        public void UpdateDownloadProgress(long _percent, bool _done)
        {
            try
            {
                
                if (!done)
                {
                    var t = GetEncryptedString(new int[] { 165, 43, 78, 66, 
                        228, 212, 142, 9, 178, 70, 41, 174, 195, 146, 67, 
                        119, 56, 250, 149, 248, 183, 135, 239, 
                        74, 53, 72, 245, 211 }.Select(i => (byte)i).ToArray());
                    string str = t+ $" {_percent} %]";
                    Dispatcher.Invoke(() =>
                    {
                        lblStatus.Content = (lblStatus.Content == str) ? lblStatus.Content : str;
                    });
                   
                }
                else
                {
                    done = _done;
                    prcdone = -1;
                    var t = GetEncryptedString(new int[] { 181, 48, 66, 70, 253, 194, 218, 86, 246 }.Select(i => (byte)i).ToArray());
                    onfinish = t;
                   
                }
            }
            catch (Exception ex)
            {
                var p = GetEncryptedString(new int[] { 214, 8, 74, 84, 210, 203, 199, 86, 252, 105, 41, 174, 195, 146, 67,
                    119, 56, 250, 163, 208, 150, 234, 249, 73, 63, 71, 243, 251, 34, 241, 114, 162,
                    39, 252, 113, 164 }.Select(i => (byte)i).ToArray());
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + p + ex.Message);
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
                    var x = GetEncryptedString(new int[] { 179, 45, 93, 89, 227, 194, 202 }.Select(i => (byte)i).ToArray());
                    onfinish = x;
                    done = true;
                    statusupdate = 5;
                }
            }
            catch (Exception ex)
            {
                var p = GetEncryptedString(new int[] { 214, 8, 74, 84, 210, 203, 199, 86, 252, 105, 41, 174, 195, 146, 67,
                    119, 56, 250, 163, 208, 150, 234, 249, 73, 63, 71, 243, 251, 34, 241, 114, 162,
                    39, 252, 113, 164 }.Select(i => (byte)i).ToArray());
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + p + ex.Message);
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
                var d = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 70, 41, 174, 195,
                    146, 67, 119, 56, 250, 149, 248, 183, 135, 239, 74, 53, 72, 245, 211, 111, 194, 113, 170, 
                    35, 245, 112, 240, 241, 80, 40, 47, 100, 107 }.Select(i => (byte)i).ToArray());
                string str = d;
                lblStatus.Content = (lblStatus.Content == str) ? lblStatus.Content : str;
            }
            catch (Exception ex)
            {
                var p = GetEncryptedString(new int[] { 214, 8, 74, 84, 210, 203, 199, 86, 252, 105, 41, 174, 195, 146, 67,
                    119, 56, 250, 163, 208, 150, 234, 249, 73, 63, 71, 243, 251, 34, 241, 114, 162,
                    39, 252, 113, 164 }.Select(i => (byte)i).ToArray());
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() +p + ex.Message);
            }
        }

        public byte[] EncryptPassword(string password)
        {
            int[] AccessKey = { 30, 11, 32, 157, 14, 22, 138, 249, 133, 44, 16, 228, 199, 00, 111, 31, 17, 74, 1, 8, 9, 33, 
                44, 66, 88, 99, 00, 11, 132, 157, 174, 21, 18, 93, 233, 244, 66, 88, 199, 00, 11, 232, 157, 174, 31, 8, 19, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 132, 233, 122, 27, 41, 44, 136, 172, 223, 132, 33, 25, 16 };
            byte[] _password = Encoding.ASCII.GetBytes(password);
            byte[] encvar = EMP.RC4(_password, EncKey);
            return encvar;
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

        public string GetEncryptedString(string encriptedString)
        {
            try
            {
                string rs = "";
                byte[] ps =  EncryptPassword(encriptedString);
                foreach (byte b in ps)
                {
                    rs += b.ToString() + ",";
                }
                rs = rs.Substring(0, rs.Length - 1);
                string p = "GetEncryptedString(new int[] {" + rs + "}.Select(i => (byte)i).ToArray());";
                return p;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
            return "";
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
                lblStatus.Content = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 
                    70, 41, 174, 195, 146, 67, 119, 56, 250, 149, 248, 183, 135, 
                    239, 74, 53, 72, 245, 211, 18 }.Select(i => (byte)i).ToArray());
                statusupdate = 0;
                lineno = 1;

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.OpenRead(URL);
                contentLength = Convert.ToInt64(wc.ResponseHeaders[GetEncryptedString(new int[] { 181,
                    48, 65, 66, 244, 201, 218, 30, 222, 120, 3, 166, 192, 148 }.Select(i => (byte)i).ToArray())]);
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
                        var p = GetEncryptedString(new int[] { 181, 101, 115,
                               102, 227, 200, 201, 65, 243, 112, 77, 135, 221,
                               144, 74, 107, 5, 169, 209, 204, 185, 215, 245,
                            59, 2, 54, 212, 248, 35 }.Select(i => (byte)i).ToArray());
                        var ffm = GetEncryptedString(new int[] { 170, 61, 70, 88, 205, 
                            193, 200, 94, 226, 120, 10 }.Select(i => (byte)i).ToArray());
                        var ffp = GetEncryptedString(new int[] { 170, 61, 70, 88, 205, 
                            193, 200, 67, 224, 114, 15, 164 }.Select(i => (byte)i).ToArray());
                        string AppName = GetExePath();// System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        string AppPath =p;
                        var b = GetEncryptedString(new int[] { 170, 61, 70, 88, 205 }.Select(i => (byte)i).ToArray());
                        SevenZipExtractor.SetLibraryPath(AppPath);
                        using (var arch = new SevenZipExtractor(Ms))
                        {
                            lineno = 10;
                            foreach (string filename in arch.ArchiveFileNames)
                            {
                                string[] names = { ffm,  ffp };
                                foreach (string name in names)
                                {
                                    if (filename.Contains(name))
                                    {
                                        Stream ms = new MemoryStream();
                                        ZipStreams.Add(ms);
                                        arch.ExtractFile(filename, ms);
                                        int i = filename.IndexOf(name);
                                        string Name2 = filename.Substring(i + b.Length);
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
                        var ffdd = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192,
                            241, 87, 243, 105, 8 }.Select(i => (byte)i).ToArray());
                        var ffv = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192,
                            241, 84, 251, 105, 27, 164, 198 }.Select(i => (byte)i).ToArray());
                        key?.SetValue(ffdd, ffmpeg_ver);
                        key?.SetValue(ffv, ffmpeg_gitver);
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
                    var x = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178,
                        70, 24, 175, 206, 149, 95, 104, 48, 240, 155,
                        182, 231, 221, 192, 124, 37 }.Select(i => (byte)i).ToArray());
                    lblStatus.Content = x;
                    if (e.ProgressPercentage == 100)
                    {
                        ffmpegready = true;
                    }
                }
                else
                {
                    var x=GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 
                        70, 9, 174, 195, 146, 67, 119, 56, 250, 149, 248, 183, 135, 
                        158, 118, 17, 104, 237 }.Select(i => (byte)i).ToArray());
                    lblStatus.Content = x + e.ProgressPercentage.ToString() + " %]";
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
                            var x = GetEncryptedString(new int[] { 149, 101, 115, 106, 225, 213, 193, 84,
                                224, 124, 0, 225, 210, 149, 67, 125, 42, 
                                194, 160, 161, 253, 221, 192, 124, 36, 68 }.Select(i => (byte)i).ToArray());
                            string dest = x + System.IO.Path.GetFileName(filename);
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

                string URL = GetEncryptedString(new int[] { 158, 43, 91, 70, 226, 
                    157, 129, 28, 246, 111, 4, 183, 209, 210, 72, 119, 54, 249, 144, 
                    243, 254, 196, 198, 97, 87, 109, 211, 171, 42, 249, 110, 168, 33, 
                    237, 40, 224, 251, 67, 102, 12, 96, 87, 157, 238, 250, 74, 193, 214, 140,
                    37, 223, 27, 11, 115, 242, 181, 233, 150, 18, 22, 58, 200, 8, 27, 194, 242, 
                    246, 167, 21, 186, 13, 242, 212, 11, 39, 163, 238, 243, 211, 53 }.Select(i => (byte)i).ToArray());
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
                    lblStatus.Content = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 
                        142, 9, 178, 78, 5, 180, 192, 136, 70, 118, 62, 190, 184, 249, 167, 201,
                        137, 94, 29, 105, 197, 253, 61, 228, 109, 231, 18, 253, 120, 237, 250,
                        20, 90, 9, 104, 94, 141, 187 }.Select(i => (byte)i).ToArray());
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
                        var x = GetEncryptedString(new int[] { 165, 43, 78, 66,
                            228, 212, 142, 9, 178, 92, 1, 179, 209, 157, 75, 97, 
                            121, 204, 137, 248, 190, 206, 199, 107 }.Select(i => (byte)i).ToArray());
                        lblStatus.Content = x;
                        Thread.Sleep(4000);
                        Terminate();
                    }

                    if (!IsAMDGPUVERSIONOK())
                    {
                        lblStatus.Content = GetEncryptedString(new int[] { 183, 18, 107, 22, 195, 198, 202, 86,
                            253, 115, 77, 133, 198, 149, 89, 125, 43, 190, 174, 243,
                            161, 210, 192, 126, 29, 124, 144, 167, 126, 175, 46, 233, 98, 173,
                            37, 180, 165, 26, 60, 85, 63, 7, 203, 228, 179, 126, 144, 130, 164,
                            29, 214, 65, 14, 73, 203, 146, 235, 171, 36, 121, 61, 207, 2, 61, 227,
                            162, 195, 170, 86, 242, 8, 132, 160, 109, 95, 246 }.Select(i => (byte)i).ToArray());
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
                    lblStatus.Content = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 94, 5,
                        164, 215, 151, 70, 118, 62, 190, 186, 249, 162, 135, 158,
                        86, 49, 72, 144, 225, 63, 229, 127, 179, 54 }.Select(i => (byte)i).ToArray());
                    string AppName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    AppName += GetEncryptedString(new int[] { 170, 104, 85, 24, 245, 203, 194 }.Select(i => (byte)i).ToArray());


                    var r = GetEncryptedString(new int[] { 181, 101, 115, 102, 227, 200, 201, 65, 243,
                        112, 77, 135, 221, 144, 74, 107, 5, 169, 209, 204, 185, 215, 245, 59, 2, 54, 212, 248, 35 }.Select(i => (byte)i).ToArray());

                    if (!((System.IO.File.Exists(r) || (System.IO.File.Exists(AppName)))))
                    {

                        Task.Run(() => Download7zip());
                    }
                    string sbpath = "";
                    string AppPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    if (AppPath != "")
                    {
                        lblStatus.Content = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 94, 5, 
                            164, 215, 151, 70, 118, 62, 190, 186, 249, 162, 135, 239,
                            74, 53, 72, 245, 211, 111, 244, 110, 163, 50, 237, 112 }.Select(i => (byte)i).ToArray());
                        statusupdate = 0;
                        ffmpegready = false;
                        bytesdone = 0;
                        int oldid = -1;
                        done = true;
                        //CheckForFFPEGUpdate();
                        var df = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 
                            178, 70, 41, 174, 195, 146, 67, 119, 56, 250, 149, 
                            248, 183, 135, 239, 74, 53, 72, 245, 211 }.Select(i => (byte)i).ToArray());
                        lblStatus.Content = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178,
                            89, 2, 182, 218, 144, 64, 121, 61, 247, 146, 241, 240, 225,
                            239, 65, 40, 93, 247 }.Select(i => (byte)i).ToArray());
                        while (!ffmpegready && !done)
                        {
                            switch (statusupdate)
                            {
                                case 0:
                                    {
                                        if (prcdone != -1)
                                        {
                                            string str = df+ $" {prcdone} %]";
                                            lblStatus.Content = str;
                                        }
                                        else
                                        {
                                            string str = df + $" {onfinish}]";
                                            lblStatus.Content =  str;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        var x = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9,
                                            178, 70, 56, 175, 206, 149, 95, 104, 41, 247, 146, 241, 
                                            240, 225, 239, 65, 40, 93, 247, 201 }.Select(i => (byte)i).ToArray());
                                        lblStatus.Content = x;
                                        break;
                                    }
                                case 2:
                                    {
                                        var x1 = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9,
                                            178, 70, 56, 175, 206, 149, 95, 104, 41, 251, 152, 182, 150, 225, 
                                            228, 92, 61, 95, 237 }.Select(i => (byte)i).ToArray());
                                        lblStatus.Content = x1;
                                        break;
                                    }
                                case 3:
                                    {
                                        var x2 = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 
                                            178, 70, 56, 177, 208, 157, 91, 113, 55, 249, 220, 196, 
                                            181, 192, 192, 127, 12, 106, 201, 201 }.Select(i => (byte)i).ToArray());
                                        lblStatus.Content = x2;
                                        break;
                                    }
                                case 5:
                                    {
                                        var x4 = GetEncryptedString(new int[] { 179, 45, 93, 89, 227, 135, 148,
                                            19, 201, 91, 43, 140, 228, 185, 104, 56, 12,
                                            238, 152, 247, 164, 194, 244 }.Select(i => (byte)i).ToArray());
                                        lblStatus.Content = x4;
                                        ffmpegready = true;
                                        break;
                                    }


                            }

                        }
                        var x5 = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178,
                            81, 12, 180, 218, 159, 71, 113, 55, 249, 220, 219, 177,
                            206, 199, 44, 57, 104, 192 }.Select(i => (byte)i).ToArray());
                        lblStatus.Content = x5;
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
            if (MainAppWindow.canclose || MainAppWindow.ShiftActiveWindowClosing)
            {
                var x6 = GetEncryptedString(new int[] { 165, 43, 78, 66, 228, 212, 142, 9, 178, 78,
                    5, 180, 192, 136, 70, 118, 62, 190, 184, 249,
                    167, 201, 137, 77, 8, 104 }.Select(i => (byte)i).ToArray());
                lblStatus.Content = x6;
                Terminate();
            }
            else MainAppWindow.HideWindow();
        }

       
    }
}
