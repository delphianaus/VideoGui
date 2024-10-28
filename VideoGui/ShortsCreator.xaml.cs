﻿using FolderBrowserEx;
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
using VideoGui.Models;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

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

        public async Task<string> AddLogo(string _dest, string logosource)
        {
            try
            {
                double totalseconds = -1;
                ffmpegbridge FileIndexer = new ffmpegbridge();
                FileIndexer.ReadDuration(_dest);

                while (!FileIndexer.Finished)
                {
                    Thread.Sleep(100);
                }

                totalseconds = FileIndexer.GetDuration().TotalSeconds;
                bool bIsEncoding = false;
                bool bSideDataRec = false;
                string desth = _dest;
                if (desth.Contains("(shorts)"))
                {
                    desth = _dest.Replace("(shorts)", "");
                }
                string newdest = Path.GetDirectoryName(desth) + "\\" + Path.GetFileNameWithoutExtension(desth) + "(shorts_logo)" + Path.GetExtension(desth);

                List<string> ProbeData = new List<string>();
                bool bMetaData = false;
                List<string> _twitchparameters = new List<string>();
                int percent = 0;
                Int64 maxframes = -1;
                string maxduration = "";
                TimeSpan Eta = TimeSpan.Zero;
                TimeSpan MaxDuration = TimeSpan.Zero;
                DateTime startTime = DateTime.Now;
                bool onseek = false, vcopy = false, acopy = true;
                //-hwaccel vulkan -c:v hevc
                _twitchparameters.Add("-y");
                _twitchparameters.Add("-hwaccel");
                _twitchparameters.Add("vulkan");
                _twitchparameters.Add("-c:v");
                _twitchparameters.Add("hevc");
                _twitchparameters.Add("-i");
                _twitchparameters.Add(_dest);
                _twitchparameters.Add("-i");
                _twitchparameters.Add(logosource);
                _twitchparameters.Add("-filter_complex");
                _twitchparameters.Add("[0:v][1:v]overlay=510:10");
                _twitchparameters.Add("-c:a");
                _twitchparameters.Add("copy");
                _twitchparameters.Add("-b:v");
                _twitchparameters.Add("1385K");
                _twitchparameters.Add("-maxrate");
                _twitchparameters.Add("2330K");
                _twitchparameters.Add("-c:v");
                _twitchparameters.Add("hevc_amf");
                _twitchparameters.Add("-threads");
                _twitchparameters.Add("1");
                _twitchparameters.Add(newdest);
                for (int i = _twitchparameters.Count - 1; i > -1; i--)
                {
                    if (_twitchparameters[i].Trim() == "")
                    {
                        _twitchparameters.RemoveAt(i);
                    }
                }


                string defaultpath = (Debugger.IsAttached) ? @"C:\VideoGui" : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                var cmd = Cli.Wrap(defaultpath + "\\ffmpeg.exe").
                                 WithArguments(args => args
                                 .Add(_twitchparameters))
                                 .WithValidation(CommandResultValidation.None).
                                WithWorkingDirectory(defaultpath);
                await foreach (var commandEvent in cmd.ListenAsync())
                {
                    switch (commandEvent)
                    {


                        case StandardErrorCommandEvent ErrorEvent:
                            {
                                string data = ErrorEvent.Text;
                                ProbeData.Add(data);

                                if (data.ContainsAll(new string[] { "NUMBER_OF_FRAMES" }))
                                {
                                    List<string> frames = data.Trim().Split(":").ToList<string>();
                                    string frmecnt = frames.Last<string>().Trim();
                                    if (!frmecnt.Contains("NUMBER_OF_FRAMES"))
                                    {
                                        Int64.TryParse(frmecnt, out maxframes);
                                    }
                                }
                                if (data.ToUpper().Contains("DURATION:"))
                                {
                                    List<string> frames = data.Trim().Split(" ").ToList<string>();
                                    int idx = frames.IndexOf("Duration:");
                                    if (idx != -1 && MaxDuration == TimeSpan.Zero)
                                    {
                                        string frmecnt = frames[idx + 1].Trim();
                                        frmecnt = frmecnt.Replace(",", "");
                                        int max = (frmecnt.Length > 11) ? 11 : frmecnt.Length;
                                        maxduration = frmecnt.Trim().Substring(0, max);
                                        TimeSpan.TryParse(maxduration, out MaxDuration);
                                        if (MaxDuration == TimeSpan.Zero)
                                        {
                                            MaxDuration = TimeSpan.FromSeconds(totalseconds);
                                        }
                                    }
                                }
                                if (!bIsEncoding)
                                {
                                    bIsEncoding = data.Contains("Press [q] to stop, [?] for help") || bIsEncoding;
                                    continue;
                                }
                                else
                                {
                                    if (!bMetaData)
                                    {
                                        bMetaData = data.Contains("Metadata:") || bMetaData;
                                        if (!vcopy && !acopy)
                                            continue;
                                    }
                                }
                                if (bIsEncoding && (bMetaData || (vcopy || acopy)))
                                {
                                    
                                    double perc = -1;
                                    if (data.Contains("time="))
                                    {
                                        int idx = data.IndexOf("time=");
                                        int idx2 = data.IndexOf("bitrate");
                                        if (idx != 1)
                                        {
                                            string sp = data.Substring(idx + 5, idx2 - (idx + 5)).Trim();

                                            if (sp != "")
                                            {
                                                TimeSpan Tse = TimeSpan.Zero;
                                                TimeSpan.TryParse(sp, out Tse);
                                                double totalsecs = Tse.TotalSeconds;
                                                if (totalseconds > 0)
                                                {
                                                    perc = (100 / totalseconds) * totalsecs;
                                                }
                                            }
                                        }
                                    }

                                    if (data.ContainsAll(new string[] { "frame", "fps", "size", "bitrate", "speed" }))
                                    {
                                        int pos1 = data.IndexOf("frame=");
                                        string frm = data.Substring(pos1 + 6);
                                        string frm2x = frm.Substring(0, frm.IndexOf("fps")).Trim();
                                        long currentframe = -1;
                                        long.TryParse(frm2x, out currentframe);
                                        int percentdone = 0;
                                        if ((maxframes != -1))
                                        {
                                            float mx = maxframes;
                                            float pcd = (100 / mx);
                                            pcd = pcd * currentframe;
                                            percentdone = Convert.ToInt32(pcd);
                                            if (percentdone > 100)
                                            {
                                                percentdone = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (perc != -1)
                                            {
                                                percentdone = Convert.ToInt32(perc);
                                            }
                                        }

                                        int pos2 = data.IndexOf("time=");
                                        string frm2 = data.Substring(pos2 + 5);
                                        frm2 = frm2.Substring(0, frm2.IndexOf("bitrate")).Trim();
                                        TimeSpan.TryParse(frm2.Trim(), out Eta);
                                        if (maxframes == -1)
                                        {
                                            double mx = MaxDuration.TotalSeconds;
                                            if (MaxDuration.TotalSeconds > 0)
                                            {
                                                double pcd = (100 / mx);
                                                pcd = pcd * Eta.TotalSeconds;
                                                percentdone = Convert.ToInt32(pcd);
                                                if (percentdone > 100)
                                                {
                                                    percentdone = 0;
                                                }
                                            }
                                        }
                                        // Dispatcher.Invoke(() =>
                                        //  {
                                        pg1.Value = percentdone;
                                        lblPercent.Content = percentdone.ToString((percentdone < 1) ? "0.0" : "0") + "%";
                                        
                                        //  });
                                    }
                                }
                                break;
                            }
                        case ExitedCommandEvent ExitEvent:
                            {

                                lblProgress.Content = "Progress : Complete";// + percentdone + " " + Eta;

                                break;
                            }

                    }
                }
                return newdest;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
                return "";
            }
        }

        public void OnShortsCreated()
        {
            try
            {

                float p = (float)(NumberShorts / 25.0);
                decimal r = Math.Truncate((decimal)p);
                int maxp = r.ToInt();

                string dir = Path.GetDirectoryName(shortsfile);
                string np = Path.GetFileName(shortsfile);
                np = np.Replace("(shorts_logo)", "").Replace("(shorts)", "").Replace(".mp4", "").Trim();
                string subp = Path.Combine(dir, np);
                List<int> list = new List<int>();
                for (int i2 = 0; i2 < NumberShorts - 1; i2++)
                {
                    list.Add(i2 + 1);
                }
                for (int px = 0; px < r; px++)
                {
                    Directory.CreateDirectory(Path.Combine(subp, (px + 1).ToString()));
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
                    string destDir = Path.Combine(subp, "" + (cp), $"{list[iir].ToInt().ToString("X")}.mp4");
                    string oldfile = Path.Combine(subp, $"{list[iir]}.mp4");
                    if (oldfile != destDir)
                    {
                        File.Move(oldfile, destDir);
                    }
                    list.RemoveAt(iir);
                }
                string LastFile = Directory.EnumerateFiles(subp, "*.mp4", SearchOption.TopDirectoryOnly).ToList().FirstOrDefault();
                if (File.Exists(LastFile))
                {
                    List<string> dirs = Directory.EnumerateDirectories(subp).ToList();
                    int mx, MaxNumber = 0;
                    string dirp = dirs.FirstOrDefault()?.ToString();
                    if (Directory.Exists(dirp))
                    {
                        MaxNumber = Directory.EnumerateFiles(dirp, "*.mp4").ToList().Count();
                    }
                    string DirectoryToUse = dirp;
                    foreach (var directory in dirs)
                    {
                        mx = Directory.EnumerateFiles(directory, "*.mp4").ToList().Count();
                        if (mx < MaxNumber)
                        {
                            MaxNumber = mx;
                            DirectoryToUse = directory;
                        }
                    }
                    string gp = Path.GetFileNameWithoutExtension(LastFile);
                    string ext = Path.GetExtension(LastFile);
                    string destDir = Path.Combine(subp, DirectoryToUse, $"{gp.ToInt().ToString("X")}{ext}");
                    File.Move(LastFile, destDir);
                }
                MessageBox.Show("Shorts Created");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }
        public async Task CreateShorts(string source, OnFinish DoOnFinish)
        {
            try
            {
                // upgraded to new shorts length
                source = await AddLogo(shortsfile, @"C:\videogui\logo.png");

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
                    $"segmentsize = {TimeSpan.FromMinutes(2.4).TotalMicroseconds}" ,
                    $"segsize1 = {TimeSpan.FromMinutes(2.05).TotalMicroseconds}",
                    $"segsize2 = {TimeSpan.FromMinutes(1.25).TotalMicroseconds}",
                    $"segsize3 = {TimeSpan.FromSeconds(45).TotalMicroseconds}",
                    $"segsize4 = {TimeSpan.FromSeconds(45).TotalMicroseconds}",
                    $"segsize5 = {TimeSpan.FromSeconds(35).TotalMicroseconds}",
                    $"segsize6 = {TimeSpan.FromSeconds(25).TotalMicroseconds}",
                    $"segsize7 = {TimeSpan.FromSeconds(25).TotalMicroseconds}",
                    "seggap = 2000000",
                    "seggap2 = 1500000",
                    "seggap1 = 100000",
                    "seggap4 = 1000000",
                    "seggap3 = 100000",
                    "startseg = 0",
                    "adm = Avidemux()",
                    "ed = Editor()",
                   // "gui = Gui()",
                    "filename = "+sx,
                    "fname = basename(filename.replace(\" (shorts_logo)\",\"\").replace(\".mkv\",\"\").replace(\".mp4\",\"\"))",
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
                    "    adm.setContainer(\"MP4\", \"forceAspectRatio=False\", \"displayWidth=1280\", \"displayAspectRatio=2\", \"addColourInfo=False\", \"colMatrixCoeff=2\", \"colRange=0\", \"colTransfer=2\", \"colPrimaries=2\")",
                    "    adm.save(dir + fname + \"/\" +str(filecounter) + \".MP4\")",
                    "    filecounter+=1",
                    "    segid += 1",
                    "    segsizeid += 1",
                    "    if (segid > 4):",
                    "       segid = 1",
                    "    if (segsizeid > 7):",
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
                    "    if (segsizeid == 6):",
                    "       segmentsize = segsize6",
                    "    if (segsizeid == 7):",
                    "       segmentsize = segsize7",
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
        public Task RunProces(string processname, string py)
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
                        List<string> files = Directory.EnumerateFiles(newpath, "*.*", SearchOption.AllDirectories).ToList();
                        foreach (string _file in files)
                        {
                            while (max > 16)
                            {
                                Thread.Sleep(50);
                            }
                            max++;
                            DeleteFile(_file).ConfigureAwait(false);
                        }
                        max = 0;
                        List<string> dirs = Directory.EnumerateDirectories(newpath).ToList();
                        foreach (string _dir in dirs)
                        {
                            while (max > 16)
                            {
                                Thread.Sleep(50);
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frmShortsCreator.Height += 20;
        }

        private void btnCreatShorts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
