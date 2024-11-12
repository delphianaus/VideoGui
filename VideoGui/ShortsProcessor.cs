using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoGui.Models.delegates;

namespace VideoGui
{
    public class ShortsProcessor : IDisposable
    {
        Process process = new Process();
        public string SourceFile = "";
        _StatsHandler DoOnStats = null;
        _StatsHandler DoOnFinish = null;
        string Source = "";
        bool ProcessStarted = false;
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        int NumberShorts = 0;
        bool ShortType = false;
        public ShortsProcessor(string _SourceFile, _StatsHandler _DoOnStats, _StatsHandler _DoOnFinish, int _ShortType)
        {
            SourceFile = _SourceFile;
            DoOnFinish = _DoOnFinish;
            DoOnStats = _DoOnStats;
            ShortType = (_ShortType == 1) ? true : (_ShortType == 2) ? false: false;
            Task.Run(() => { CreateShorts(_SourceFile); });
        }
        public async Task CreateShorts(string source)
        {
            try
            {
                var qs = "\"";
                var fname = System.IO.Path.GetFileName(source);
                var fnamex = System.IO.Path.GetFileNameWithoutExtension(source);
                var dir = System.IO.Path.GetDirectoryName(source);
                var fnmp3 = System.IO.Path.Combine(dir, fname);
                string pr = source.ToLower().Replace("(shorts_logo)","").Replace(".mp4","").Replace(".mkv","").Trim();
                string pr2 = source.Substring(0, pr.Length);
                var sx = $"{qs}{source.Replace("\\", "/")}{qs}";
                fnmp3 = $"{qs}{fnmp3.Replace("\\", "/")}{qs}";
                if (Directory.Exists(pr2))
                {
                    foreach (var filex in Directory.EnumerateFiles(pr2, "*.*", SearchOption.AllDirectories))
                    {
                        File.Delete(filex);
                    }

                    Directory.Delete(pr2, true);
                }

                TimeSpan  SegmentSize = (ShortType) ? TimeSpan.FromMinutes(2.0) :
                                                      TimeSpan.FromSeconds(57);
                TimeSpan SegSize1 = (ShortType) ? TimeSpan.FromMinutes(2.1):
                                                  TimeSpan.FromSeconds(42);
                TimeSpan SegSize2 = (ShortType) ? TimeSpan.FromMinutes(1.3) :
                                             TimeSpan.FromSeconds(32);
                TimeSpan SegSize3 = (ShortType) ? TimeSpan.FromSeconds(60) :
                                             TimeSpan.FromSeconds(22);
                TimeSpan SegSize4 = (ShortType) ? TimeSpan.FromSeconds(45) :
                                             TimeSpan.FromSeconds(52);
                TimeSpan SegSize5 = (ShortType) ? TimeSpan.FromSeconds(25) :
                                             TimeSpan.FromSeconds(38);

                Directory.CreateDirectory(pr2); 
                var py = System.IO.Path.Combine(dir, fnamex) + ".py";
                List<string> convert = new List<string>() {
                    "#--automatically built--",
                    "filecounter = 1","" +
                    "",
                    "segid = 1",
                    "segsizeid = 1",
                    $"segmentsize = {SegmentSize.TotalMicroseconds}" ,
                    $"segsize1 = {SegSize1.TotalMicroseconds}",
                    $"segsize2 = {SegSize2.TotalMicroseconds}",
                    $"segsize3 = {SegSize3.TotalMicroseconds}",
                    $"segsize4 = {SegSize4.TotalMicroseconds}",
                    $"segsize5 = {SegSize5.TotalMicroseconds}",
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
                    "    adm.save(dir + fname + \"/\" +str(filecounter) + \".mp4\")",
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
        public Task RunProces(string processname, string py)
        {
            try
            {
                var qs = "\"";
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
                ProcessStarted=true;
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
            try
            {
                if ((e.Data is not null) && (e.Data.ContainsAll(new string[] { "Encoding", "Phase", "Saving" })))
                {
                    NumberShorts++;
                    DoOnStats?.Invoke(NumberShorts, Source);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnShortsCreated()
        {
            try
            {
                float p = (float)(NumberShorts / 25.0);
                decimal r = Math.Truncate((decimal)p);
                int maxp = r.ToInt();

                string dir = Path.GetDirectoryName(SourceFile);
                string np = Path.GetFileName(SourceFile);
                np = np.Replace("(shorts_logo)", "").Replace(".mp4", "").Trim();
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
                    string destDir = Path.Combine(subp, "" + (cp), $"{list[iir]}.mp4");
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
                    string gp = Path.GetFileName(LastFile);
                    string destDir = Path.Combine(subp, DirectoryToUse, $"{gp}");
                    File.Move(LastFile, destDir);
                }
                DoOnFinish?.Invoke(NumberShorts,Source);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void Dispose()
        {
            try
            {
                if ((ProcessStarted) && (!process.HasExited))
                {
                    Thread.Sleep(250);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
