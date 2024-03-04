using FolderBrowserEx;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
using CliWrap.EventStream;
using CliWrap;
using VideoGui.Models.delegates;
using System.Threading;
using System.Management;
using System.Diagnostics;
using MS.WindowsAPICodePack.Internal;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ShortsCreator.xaml
    /// </summary>
    public partial class ShortsCreator : Window
    {
        string shortsfile = "", mondir = "";
        int max = 0;
        List<string> Data = new List<string>();
        List<string> EData = new List<string>();
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        OnFinish DoOnFinish;
        public OnStart DoOnStart;
        public OnProgress DoOnProgress;
        public OnStop DoOnStop;
        public OnAviDemuxStart DoOnAviDemuxStart;
        public OnAviDemuxEnd DoOnAviDemuxEnd;
        int NumberShorts = 0;
        public ShortsCreator(OnFinish _DoOnFinish)
        {
            InitializeComponent();
            DoOnFinish = _DoOnFinish;
        }


        public void DoOnStartEvent(string SourceFileName)
        {
            try
            {
                NumberShorts++;
                lblShortNo.AutoSizeLabel(NumberShorts.ToString());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public void OnShortsCreated()
        {
            try
            {
                float p = (float)(NumberShorts / 25.0);
                decimal r = Math.Truncate((decimal)p ) ;
                int maxp = r.ToInt();

                string dir = Path.GetDirectoryName(shortsfile);
                string np = Path.GetFileName(shortsfile);
                np = np.Replace("(shorts)", "").Replace(".mp4", "").Trim();
                string subp = Path.Combine(dir, np);    
                List<int> list = new List<int>();
                for(int i2=0; i2<NumberShorts-1; i2++)
                { 
                    list.Add(i2+1);
                }
                for(int px=0; px<r; px++)
                {
                    Directory.CreateDirectory(Path.Combine(subp,(px+1).ToString()));
                }

                int cp = 0;
                Random rnd = new Random();
                while (list.Count > 0)
                {
                    if (cp < r)
                    {
                        cp++;
                    }
                    else cp = 1;
                    int iir = rnd.Next(list.Count);
                    string destDir = Path.Combine(subp, "" + (cp), $"{list[iir]}.mkv");
                    string oldfile = Path.Combine(subp, $"{list[iir]}.mkv");
                    if (oldfile!=destDir)
                    {
                        File.Move(oldfile, destDir);
                    }
                    list.RemoveAt(iir);
                }
                string LastFile = Directory.EnumerateFiles(subp, "*.mkv",SearchOption.TopDirectoryOnly).ToList().FirstOrDefault();
                if (File.Exists(LastFile))
                {
                    List<string> dirs = Directory.EnumerateDirectories(subp).ToList();
                    int mx, MaxNumber = 0;
                    string dirp = dirs.FirstOrDefault()?.ToString();
                    if (Directory.Exists(dirp))
                    {
                        MaxNumber = Directory.EnumerateFiles(dirp, "*.mkv").ToList().Count();
                    }
                    string DirectoryToUse = dirp;
                    foreach(var directory in dirs)
                    {
                        mx = Directory.EnumerateFiles(directory, "*.mkv").ToList().Count();
                        if (mx < MaxNumber)
                        {
                            MaxNumber = mx;
                            DirectoryToUse = directory;
                        }
                    }
                    string gp = Path.GetFileName(LastFile);
                    string destDir = Path.Combine(subp, DirectoryToUse, $"{gp}");
                    File.Move(LastFile, destDir);
                }
                MessageBox.Show("Shorts Created");
            }
            catch(Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public async Task CreateShorts(string source, OnFinish DoOnFinish)
        {
            try
            {
               EData.Clear();
               Data.Clear();
                var qs = "\"";
                var fname = System.IO.Path.GetFileName(source);
                var fnamex = System.IO.Path.GetFileNameWithoutExtension(source);
                var dir = System.IO.Path.GetDirectoryName(source);
                var fnmp3 = System.IO.Path.Combine(dir, fname);
                var sx = $"{qs}{source.Replace("\\", "/")}{qs}";
                fnmp3 = $"{qs}{fnmp3.Replace("\\", "/")}{qs}";
                var py = System.IO.Path.Combine(dir, fnamex) + ".py";
                List<string> convert = new List<string>() {
                    "#--automatically built--",
                    "filecounter = 1","" +
                    "",
                    "segid = 1",
                    "segsizeid = 1",
                    "segmentsize = 31000000",
                    "segsize1 = 20000000",
                    "segsize2 = 23000000",
                    "segsize3 = 32000000",
                    "segsize4 = 21000000",
                    "segsize5 = 37000000",
                    "seggap = 3000000",
                    "seggap2 = 3000000",
                    "seggap1 = 1000000",
                    "seggap4 = 3000000",
                    "seggap3 = 100000",
                    "startseg = 0",
                    "adm = Avidemux()",
                    "ed = Editor()",
                   // "gui = Gui()",
                    "filename = "+sx,
                    "fname = basename(filename.replace(\" (shorts)\",\"\").replace(\".mkv\",\"\").replace(\".mp4\",\"\"))",
                    "dir = dirname(filename)",
                //    "",
                    "adm.loadVideo(filename)",
                //    "    raise(filename)",
                    "endseg = adm.markerB",
                    "adm.clearSegments()",
                    "while (startseg + segmentsize) <= endseg:",
                    "    adm.clearSegments()",
                    "    adm.addSegment(0, 0, endseg)",
                    "    adm.markerA = startseg",
                    "    adm.markerB = startseg + segmentsize",
                    "    adm.setHDRConfig(1, 1, 1, 1, 0)",
                    "    adm.videoCodec(\"Copy\")",
                    "    adm.audioClearTracks()",
                    "    adm.setSourceTrackLanguage(0,\"eng\")",
                    "    if adm.audioTotalTracksCount() <= 0:",
                    "      raise(\"Cannot add audio track 0, total tracks: \" + str(adm.audioTotalTracksCount()))",
                    "    adm.audioAddTrack(0)",
                    "    adm.audioCodec(0, \"copy\")",
                    "    adm.audioSetDrc2(0, 0, 1, 0.001, 0.2, 1, 2, -12)",
                    "    adm.audioSetEq(0, 0, 0, 0, 0, 880, 5000)",
                    "    adm.audioSetChannelGains(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)",
                    "    adm.audioSetChannelDelays(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)",
                    "    adm.audioSetChannelRemap(0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8)",
                    "    adm.audioSetShift(0, 0, 0)",
                    "    adm.setContainer(\"MKV\", \"forceAspectRatio=False\", \"displayWidth=1280\", \"displayAspectRatio=2\", \"addColourInfo=False\", \"colMatrixCoeff=2\", \"colRange=0\", \"colTransfer=2\", \"colPrimaries=2\")",
                    "    adm.save(dir + fname + \"/\" +str(filecounter) + \".mkv\")",
                    "    filecounter+=1",
                    "    segid += 1",
                    "    segsizeid += 1",
                    "    if (segid > 4):",
                    "       segid = 1",
                    "    if (segsizeid > 5):",
                    "       segsizeid = 1",
                    "    if (segsizeid == 1):",
                    "       segmentsize = segsize1",
                    "    if (segsizeid == 2):",
                    "       segmentsize = segsize2",
                    "    if (segsizeid == 3):",
                    "       segmentsize = segsize3",
                    "    if (segsizeid == 4):",
                    "       segmentsize = segsize4",
                    "    if (segsizeid == 5):",
                    "       segmentsize = segsize5",
                    "    if (segid==1):",
                    "      seggap = seggap1",
                    "    if (segid==2):",
                    "      seggap = seggap2",
                    "    if (segid==3):",
                    "      seggap = seggap3",
                    "    if (segid==4):",
                    "      seggap = seggap4",
                    "    startseg += seggap + segmentsize",
                    "    if (startseg + segmentsize > endseg):",
                    "         segentsize = (segmentsize+startseg) - endseg",
                    "##########" };

                if (File.Exists($"{py}"))
                {
                    File.Delete($"{py}");
                }
                using (StreamWriter txtWriter = File.AppendText(py))
                {
                    foreach (var c in convert)
                    {
                        txtWriter.Write($"{c}\r\n", "");
                    }
                }
                dir = "C:\\VideoGui";
                string FileToWrite = $"C:\\Program Files\\Avidemux 2.8 VC++ 64bits\\avidemux_cli.exe";
             
                Task.Run(() => { RunProces(FileToWrite, py); });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }
       
       

        
        List<string> convert = new List<string>();
        public Task RunProces(string processname,string py)
        {
            try
            {
                var qs = "\"";
                Process process = new Process();
                string args = $" --run {qs}{py}{qs}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = processname;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.StartInfo.Arguments = args;
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();
                process.CancelErrorRead();
                cancellationToken.Cancel();
                if (File.Exists(py))
                {
                    File.Delete(py);
                }
                Task.Run(() => { OnShortsCreated(); });
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //convert.Add("*"+e.Data);
            if (e.Data is not null)
            {
                if (e.Data.ContainsAll(new string[] { "Encoding", "Phase", "Saving" }))
                {
                    DoOnStartEvent("");
                }
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                //convert.Add(e.Data);
                if (e.Data is not null)
                {
                   
                    if (e.Data.ContainsAll(new string[] { "Encoding", "Phase", "Saving" }))
                    {
                        DoOnStartEvent("");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public async Task DeletePath(string path)
        {
            try
            {
                Directory.Delete(path);
                max--;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public async Task DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                max--;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        private void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("ShortsFile", @"c:\");
                key?.Close();

                var fld = new OpenFileDialog();
                fld.Filter = "mp4|*.mp4";
                fld.DefaultExt = "*.mp4";
                fld.DefaultDirectory = Root;
                fld.Multiselect = false;
                var fd = fld.ShowDialog();
                if ((fd != null) && (fd.Value == true))
                {
                    shortsfile = fld.FileName;
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("ShortsFile", Path.GetDirectoryName(shortsfile));
                    key2?.Close();
                    var bridge = new ffmpegbridge();
                    bridge.ReadFile(shortsfile, true);
                    while (!bridge.Finished)
                    {
                        Thread.Sleep(100);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    TimeSpan dur = bridge.GetDuration();
                    lblDuration.Content = dur.ToCustomTimeString();
                    txtsrcdir.Text = Path.GetFileName(shortsfile);
                    string Dir = Path.GetDirectoryName(shortsfile);
                    string NewDir = txtsrcdir.Text.Replace("(shorts)", "").Trim();
                    NewDir = NewDir.Replace(".mp4", "").Trim();
                    string newpath = Path.Combine(Dir, NewDir).Trim();
                    if (Directory.Exists(newpath))
                    {
                        List<string> files = Directory.EnumerateFiles(newpath,"*.*",SearchOption.AllDirectories).ToList();
                        foreach (string _file in files)
                        {
                            while (max > 16)
                            {
                                Thread.Sleep(50);
                                System.Windows.Forms.Application.DoEvents();
                            }
                            max++;
                            DeleteFile(_file).ConfigureAwait(false);
                        }
                        max = 0;
                        List<string> dirs = Directory.EnumerateDirectories(newpath).ToList();
                        foreach(string _dir in dirs)
                        {
                            while (max > 16)
                            {
                                Thread.Sleep(50);
                                System.Windows.Forms.Application.DoEvents();
                            }
                            max++;
                            DeletePath(_dir).ConfigureAwait(false);
                        }
                    }
                    else Directory.CreateDirectory(newpath);
                    DoOnStart = DoOnStartEvent;
                    mondir = newpath;
                    CreateShorts(shortsfile, OnShortsCreated).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DoOnFinish?.Invoke();
        }

        private void btnCreatShorts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frmShortsCreator.Height += 20;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
