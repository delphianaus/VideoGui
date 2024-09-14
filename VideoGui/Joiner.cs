using CliWrap;
using CliWrap.EventStream;
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
    public class Joiner : IDisposable
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        List<string> Data = new List<string>();
        List<string> EData = new List<string>();
        Process process = new Process();
        public OnStart DoOnStart;
        bool ProcessStarted = false;
        public OnProgress DoOnProgress;
        public OnStop DoOnStop;
        public OnAviDemuxStart DoOnAviDemuxStart;
        public OnAviDemuxEnd DoOnAviDemuxEnd;
        public string SourceFile = "" ,DestinationFile = "";
        CancellationTokenSource cancellation = new CancellationTokenSource();
        public Joiner(string Source, string dest, OnStart _DoOnStart, OnProgress _doOnProgress, OnStop _doOnStop, OnAviDemuxStart _dpOnAviDemuxStart, OnAviDemuxEnd _doOnAviDemuxEnd)
        {
            DoOnStart = _DoOnStart;
            DoOnProgress = _doOnProgress;
            DoOnStop = _doOnStop;
            DoOnAviDemuxStart = _dpOnAviDemuxStart;
            DoOnAviDemuxEnd = _doOnAviDemuxEnd;
            SourceFile = Source;
            DoJoin(Source, dest);
        }

        public void DoJoin(string source, string destination)
        {
            Join(source, destination).ConfigureAwait(false);
        }

        public Task RunProces(string processname, string py)
        {
            try
            {
                var qs = "\"";
                string args = $" --run {qs}{py}{qs}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.FileName = processname;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.StartInfo.Arguments = args;
                process.Start();
                DoOnStart?.Invoke(SourceFile);
                ProcessStarted = true;
                process.BeginOutputReadLine();
                process.WaitForExit();
                process.CancelOutputRead();
                cancellationToken.Cancel();
                DoOnAviDemuxEnd(SourceFile, DestinationFile, process.ExitCode);
               
                Task.Run(() => { OnDone(); });
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
               // EData.Add(e.Data);
                if (e.Data.ContainsAll(new string[] { "done", "frames", "elapsed" }))
                {
                    DoOnProgress?.Invoke(e.Data, SourceFile);
                }
                if (e.Data.ContainsAll(new string[] { "[MP4]", "Closing" }))
                {
                    DoOnProgress?.Invoke(e.Data, SourceFile);
                }
            }
        }

        public void OnDone()
        {
            try
            {
                DoOnAviDemuxEnd?.Invoke(SourceFile, DestinationFile, 0);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                   // Data.Add(e.Data);
                    
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public async Task Join(string source, string destination)
        {
            try
            {
                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                DestinationFile = destination;
                var qs = "\"";
                var fname = System.IO.Path.GetFileNameWithoutExtension(source);
                var dir = System.IO.Path.GetDirectoryName(source);
                var fnmp3 = System.IO.Path.Combine(dir, fname) + ".mp3";
                fnmp3 = $"{qs}{fnmp3.Replace("\\", "/")}{qs}";
                var py = System.IO.Path.Combine(dir, fname) + ".py";
                List<string> convert = new List<string>() {
                "#PY  <- Needed to identify #",
                "adm = Avidemux()",
                $"adm.loadVideo({qs}{source.Replace("\\", "/")}{qs})",
                "adm.setHDRConfig(1, 1, 1, 1, 0)",
                "adm.videoCodec(\"Copy\")",
                "adm.audioClearTracks()",
                "adm.setSourceTrackLanguage(0,\"eng\")",
                $"adm.audioAddExternal({fnmp3})",
                "adm.setSourceTrackLanguage(1,\"und\")",
                "if adm.audioTotalTracksCount() <= 1:",
                "    raise(\"Cannot add audio track 1, total tracks: \" + str(adm.audioTotalTracksCount()))",
                "adm.audioAddTrack(1)",
                "adm.audioCodec(0, \"LavAAC\", \"bitrate=128\")",
                "adm.audioSetDrc2(0, 0, 1, 0.001, 0.2, 1, 2, -12)",
                "adm.audioSetEq(0, 0, 0, 0, 0, 880, 5000)",
                "adm.audioSetChannelGains(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)",
                "adm.audioSetChannelDelays(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)",
                "adm.audioSetChannelRemap(0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8)",
                "adm.audioSetShift(0, 0, 0)",
                "adm.setContainer(\"MP4\", \"forceAspectRatio=False\", \"displayWidth=1280\", \"displayAspectRatio=2\", \"addColourInfo=False\", \"colMatrixCoeff=2\", \"colRange=0\", \"colTransfer=2\", \"colPrimaries=2\")",
                $"adm.save({qs}{destination.Replace("\\", "/")}{qs})"};
                if (File.Exists($"{py}"))
                {
                    File.Delete($"{py}");
                }
                using (StreamWriter txtWriter = File.AppendText(py))
                {
                    foreach (var c in convert)
                    {
                        txtWriter.Write($"{c}\r\n","");
                    }
                }
                string ProcessName = $"{qs}C:\\Program Files\\Avidemux 2.8 VC++ 64bits\\avidemux_cli.exe{qs}";
                Task.Run(() => { RunProces(ProcessName, py); });
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
