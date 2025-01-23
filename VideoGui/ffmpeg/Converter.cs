using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using CliWrap;
using CliWrap.EventStream;
using Microsoft.Extensions.Logging;
using Nancy.ViewEngines;
using VideoGui.ffmpeg.Events;
using VideoGui.ffmpeg.Streams;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.MediaInfo;
using VideoGui.ffmpeg.Streams.Video;
using VideoGui.Models.delegates;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.WebRequestMethods;


namespace VideoGui.ffmpeg
{
    public partial class Converter : IConverter, IDisposable
    {
        string myStrQuote = "\"";
        List<(string, bool)> _parameters = new List<(string, bool)>();
        List<string> _twitchparameters = new List<string>();

        List<(string, bool)> _userparameters = new List<(string, bool)>();
        private readonly List<IStream> _streams = new List<IStream>();
        private readonly Dictionary<string, int> _inputFileMap = new Dictionary<string, int>();
        private string OutputFilePath = "";
        private string _output = "";
        private VsyncParams _vsyncmode;
        private bool _IsProbe = false, _IsComplex = false, _IsConcat = false, IsMuxed = false;
        private string ComplexBitRate = string.Empty, MuxData = "";
        public bool _Is1080p;
        public int mtscnt = 0;
        private TimeSpan startime = TimeSpan.Zero;
        private Command cmd = null;
        CancellationTokenSource cancellation = new CancellationTokenSource();
        bool _hasInputBuilder = false;
        private string defaultpath = "";
        private bool _ScanInterlaced = false;
        private int _SourceIndex = 0;
        private string _parameterAsstring = "";
        private string _source, _src = "", _dest = "";
        private bool SendProbEvents = false;
        private int ProcessID = -1;
        double totalseconds = -1;
        private List<string> DestinationFiles = new List<string>();
        string MutliModeFileName = "";

        string ComplexEncoder = string.Empty;
        public int ProbeID = -1;
        public string LogoSource = "";
        public bool IsLogo = false;

        private Func<string, string> _buildInputFileName = null;
        private Func<string, string> _buildOutputFileName = null;
        //public List<string> IConverter.ProbeResults = new List<string>();

        public List<string> ProbeResults = new List<string>();
        public List<string> cutlist = new List<string>();
        IEnumerable<string> IConverter.ProbeResults { get => ProbeResults; }

        //ConverterProbDataEventHandler IConverter.OnProbeData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public event _AddConverterProgress OnProgressEventHandler;
        public event ConverterProbDataEventHandler OnProbeData;
        public event ConverterOnStartedEventHandler OnConverterStarted;
        public event ConverterOnProgressEventHandler OnConverterProgress;
        public event ConverterOnSeekEventHandler OnConverterOnSeek;
        public event ConverterOnStoppedEventHandler OnConverterStopped;
        public event ConverterOnStoppedEventHandler OnConverterDataUpdate;
        public event ConverterOnDataEventHandler OnConverteringData;


        public Converter(_AddConverterProgress data)
        {
            OnProgressEventHandler = data;
        }

        public Converter()
        {
        }

        private List<string> GetInputs()
        {
            try
            {
                var builder = new List<string>();
                var index = 0;
                foreach (var source in _streams.SelectMany(x => x.GetSource()).Distinct())
                {
                    if (source is not null)
                    {
                        if (_inputFileMap.Count > 0)
                        {
                            _inputFileMap[source] = index++;
                        }

                        if (_IsConcat)
                        {
                            string destdir = DestinationFiles.FirstOrDefault().Replace("file '", "").Replace("'", "");
                            string safe = "safe -0 ";
                            string FileToWrite = Path.GetDirectoryName(destdir) + $"\\sourcefiles{_SourceIndex}.txt";
                            if (System.IO.File.Exists(FileToWrite))
                            {
                                List<string> ListOfFIles = System.IO.File.ReadAllLines(FileToWrite).ToList<string>();
                                bool fnd = true;
                                foreach (var file in ListOfFIles)
                                {
                                    if (DestinationFiles.IndexOf(file) == -1)
                                    {
                                        fnd = false;
                                        break;
                                    }
                                }
                                if (!fnd)
                                {
                                    System.IO.File.Delete(FileToWrite);
                                    System.IO.File.AppendAllLines(FileToWrite, DestinationFiles);
                                }
                            }
                            else
                            {
                                System.IO.File.Delete(FileToWrite);
                                System.IO.File.AppendAllLines(FileToWrite, DestinationFiles);
                            }
                            builder.Add($"-f");
                            builder.Add($"concat");
                            if (safe != "")
                            {
                                builder.Add($"-safe");
                                builder.Add($"0");
                            }
                            builder.Add($"-i");
                            builder.Add(FileToWrite);
                        }
                        else
                        {
                            builder.Add($"-i");
                            if (source is not null)
                            {
                                builder.Add(source.Replace("\"", ""));
                            }
                        }
                        _source = source;
                    }
                }
                return builder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Task ErrorHandler(string args)
        {
            try
            {
                OnProbeData?.Invoke(this, ProbeID, _source);
                //ProbeData.Add(args);
                ProbeID++;
                string[] breaks = { "frame=", "fps=", "bitrate=", "time=" };
                if (args.Contains("Application provided invalid, non monotonically increasing dts to muxer in stream"))
                {
                    mtscnt++;
                    if (mtscnt > 10)
                    {
                        cancellation.Cancel();
                    }
                    else if ((mtscnt > 10) && _Is1080p)
                    {
                        cancellation.CancelAfter(TimeSpan.FromSeconds(1));
                        return Task.CompletedTask;
                    }
                }
                if (args.Contains("Output #0, null, to") && _Is1080p)
                {
                    cancellation.CancelAfter(TimeSpan.FromSeconds(1));
                    return Task.CompletedTask;
                }
                if (!args.Contains("Application provided invalid, non monotonically increasing dts to muxer in stream"))
                {
                    ProbeResults.Add(args);
                    if (ProbeID > 4) ProbeID = 0;
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return Task.CompletedTask;
            }
        }

        public Task DataHandler(string args)
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



        private List<string> GetParameters(bool Input = true)
        {
            try
            {
                if ((_IsProbe) || (_IsComplex))
                {
                    return null;
                }
                List<string> res = new List<string>();
                if (!Input)
                {
                    foreach (var _ in _streams.Where(stream => stream.IsCopy && stream is VideoStream).Select(stream => new { }))
                    {
                        return res;
                    }
                }


                foreach (var rr in (_parameters.Where(param => param.Item2 == Input).Select(param => param.Item1)).ToList())
                {
                    res.AddRange(rr.Split(" ").ToList<string>());
                }

                foreach (var r in res)
                {
                    _parameterAsstring += _parameterAsstring == "" ? r : " " + r;
                }
                if (_IsConcat) res.Insert(0, "-y");// auto over right.
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }


        private List<string> GetStreamsPreInputs()
        {
            try
            {
                if (_IsProbe)
                {
                    return null;
                }
                var builder = new List<string>();
                foreach (IStream stream in _streams)
                {
                    builder.AddRange(stream.BuildParameters(stream.PreInput).Split(" ").ToList<string>());
                }
                foreach (var res in builder)
                {
                    _parameterAsstring += _parameterAsstring == "" ? res : " " + res;
                }

                return builder == null ? null : builder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public IConverter SetSourceIndex(int index)
        {
            try
            {
                _SourceIndex = index;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public IConverter SetOverlay(string Source, bool IsShortVideo)
        {
            try
            {
                LogoSource = Source;
                IsLogo = IsShortVideo;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public IConverter SetMuxing(bool _IsMuxed, string _MuxData)
        {
            try
            {
                IsMuxed = _IsMuxed;
                MuxData = _MuxData;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }
        public IConverter SetMultiModeFile(string filename)
        {
            try
            {
                MutliModeFileName = filename;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public IConverter SetTotalTime(double _totalseconds)
        {
            try
            {
                totalseconds = _totalseconds;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public string GetOutputFilename()
        {
            try
            {
                if (_IsProbe)
                {
                    return "";
                }
                string res = _buildOutputFileName("_%03d");
                _parameterAsstring += _parameterAsstring == "" ? res : " " + res;
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        bool ProcessRunning = false;
        int InternalProcessId = -1;

        public async Task<IConversionResult> ProbeFile(string filename, bool is1080p)
        {
            try
            {
                _Is1080p = is1080p;
                if ((defaultpath == "") || (Debugger.IsAttached))
                {
                    defaultpath = (!Debugger.IsAttached) ? Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) : Extensions.GetAppPath();
                }
                _parameterAsstring = "";
                _source = Path.GetFileNameWithoutExtension(filename);
                _src = filename;
                DateTime startTime = DateTime.Now;
                var x = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 86, 234, 120 }.Select(i => (byte)i).ToArray());
                cmd = Cli.Wrap(defaultpath + "\\"+x).
                    WithArguments(args => args
                   .Add("-hide_banner")
                    .Add("-i")
                    .Add($"{filename}")
                    .Add("-c")
                    .Add("copy")
                    .Add("-a")
                    .Add("copy")
                    .Add("-s")
                    .Add("copy")
                    .Add("-f").Add("null").Add("output.mkv"))
                  .WithWorkingDirectory(defaultpath).
                  WithValidation(CommandResultValidation.None).
                  WithStandardErrorPipe(PipeTarget.ToDelegate(ErrorHandler)).
                  WithStandardOutputPipe(PipeTarget.ToDelegate(DataHandler));
                InternalProcessId = cmd.ExecuteAsync().ProcessId;
                ProcessRunning = true;

                return new ConversionResult
                {
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    Arguments = _parameterAsstring
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return new ConversionResult
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    Arguments = _parameterAsstring
                };

            }
        }

        public void SortCutlist()
        {
            try
            {
                List<string> res = new List<string>();
                List<CutListToFrom> SortCutList = new List<CutListToFrom>();
                foreach (var cu in cutlist)
                {
                    List<string> cutlis = cu.Split('-').ToList();
                    string ct1 = cutlis.FirstOrDefault(), ct2 = cutlis.LastOrDefault();
                    var ts1 = DateTime.ParseExact(ct1, "HH.mm.ss", CultureInfo.InvariantCulture).TimeOfDay;
                    var ts2 = DateTime.ParseExact(ct2, "HH.mm.ss", CultureInfo.InvariantCulture).TimeOfDay;
                    ts2 -= ts1;
                    SortCutList.Add(new CutListToFrom(ts1, ts2));
                }
                var events = SortCutList.OrderBy(e => e.From).ToList();
                foreach (var e in events)
                {
                    string sp = $"{e.From.TotalSeconds}|{e.To.TotalSeconds}";
                    res.Add(sp);
                }
                if (res.Count > 0)
                {
                    cutlist.Clear();
                    cutlist.AddRange(res);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public async Task<IConversionResult> Start()
        {
            int errn = 0;
            try
            {
                _Is1080p = false;
                bool bIsEncoding = false;
                bool bSideDataRec = false;
                errn = 1;
                List<string> ProbeData = new List<string>();
                errn = 2;
                bool bMetaData = false;
                int percent = 0;
                Int64 maxframes = -1;
                string maxduration = "";
                TimeSpan Eta = TimeSpan.Zero;
                TimeSpan MaxDuration = TimeSpan.Zero;
                errn = 3;
                if ((_buildOutputFileName == null) || _IsProbe)
                    _buildOutputFileName = (number) => { return _output; };
                DateTime startTime = DateTime.Now;
                if ((defaultpath == "") || (Debugger.IsAttached))
                {
                    defaultpath = (!Debugger.IsAttached) ? Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) : Extensions.GetAppPath();
                }
                _parameterAsstring = "";
                errn = 4;
                bool vcopy = false, acopy = false, onseek = false;
                if (!IsMuxed)
                {
                    List<string> argsx = new List<string>();
                    List<string> args2x = new List<string>();
                    argsx.AddRange(GetParameters());
                    errn = 114;
                    argsx.AddRange(GetInputs());
                    errn = 115;

                    args2x.AddRange(GetStreamsPreInputs());
                    errn = 116;

                    args2x.AddRange(GetStreamsPostInputs());
                    errn = 117;

                    args2x.AddRange(GetFilters());
                    errn = 118;

                    args2x.AddRange(GetMap());
                    errn = 119;

                    args2x.AddRange(GetParameters(false));
                    errn = 120;

                    args2x.AddRange(GetUserDefinedParameters(true));
                    //errn = 121;



                    _dest = GetOutputFilename().Replace("\"", "");
                    errn = 5;
                    for (int i = args2x.Count - 1; i > -1; i--)
                    {
                        if (args2x[i].Trim() == "")
                        {
                            args2x.RemoveAt(i);
                        }
                    }

                    cmd = null;
                    if (_source is not null)
                    {
                        var fn = _source.Replace("\"", "");
                        if (_dest != "")
                        {
                            var rr = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                            cmd = Cli.Wrap(defaultpath + rr).
                                WithArguments(args => args
                                .Add(GetParameters())
                                .Add(GetInputs())
                                .Add(args2x)
                                .Add($"{_dest}"))
                                .WithValidation(CommandResultValidation.None).
                               WithWorkingDirectory(defaultpath);
                        }
                        else
                        {
                            var r1 = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                            cmd = Cli.Wrap(defaultpath + r1).
                                WithArguments(args => args
                                .Add(GetParameters())
                                .Add(GetInputs())
                                .Add(args2x)
                                .Add(_twitchparameters))
                                .WithValidation(CommandResultValidation.None).
                               WithWorkingDirectory(defaultpath);
                        }
                    }

                    errn = 6;
                    ProbeData.Clear();
                    errn = 7;
                    foreach (IStream stream in _streams)
                    {
                        if (stream is VideoStream vss)
                        {
                            vcopy = stream.IsCopy;
                        }
                        if (stream is AudioStream ass)
                        {
                            acopy = stream.IsCopy;
                        }
                    }
                    errn = 8;
                    if (cmd is not null)
                    {
                        if (_twitchparameters.Count > 0)
                        {
                            string cmdp = cmd.ToString();
                            string fn = Path.GetDirectoryName(_source);
                            string filen = fn.Split("\\").ToList().LastOrDefault();
                            System.IO.File.WriteAllText(@"c:\videogui\stream_" + filen + ".bat", cmdp);
                            if (true)
                            {

                            }
                        }
                    }
                }
                else
                {
                    foreach (var source in _streams.SelectMany(x => x.GetSource()).Distinct())
                    {
                        _source = source;
                        break;
                    }
                    //bool vcopy = true, acopy = false
                    onseek = false;
                    string src = _source;
                    string sep1 = "b", sep2 = "p", sep3 = "o";
                    var liststems = MuxData.Split(',').ToList();
                    if (liststems.Count <= 3)
                    {
                        sep1 = liststems[0];
                        sep2 = liststems[1];
                        sep3 = liststems[2];
                    }
                    string BaseDirectory = Path.GetDirectoryName(_source), Ext = Path.GetExtension(src);
                    string FileNameNoExt = Path.GetFileNameWithoutExtension(src);
                    string srcb = Path.Combine(BaseDirectory, FileNameNoExt + sep1 + ".mp3");
                    string srcp = Path.Combine(BaseDirectory, FileNameNoExt + sep2 + ".mp3");
                    string srco = Path.Combine(BaseDirectory, FileNameNoExt + sep3 + ".mp3");
                    string BaseDirectoryLast = BaseDirectory.Split('\\').Last();
                    string DestDirectory = BaseDirectory.Replace(BaseDirectoryLast, "Filtered");
                    if (!Directory.Exists(DestDirectory))
                    {
                        Directory.CreateDirectory(DestDirectory);
                    }
                    string DestFileName = Path.Combine(DestDirectory, FileNameNoExt + ".mp4");

                    var r2 = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                    cmd = Cli.Wrap(defaultpath + r2).
                        WithArguments(args => args
                        .Add("-i").Add(src).Add("-i").Add(srcb).Add("-i").Add(srcp).Add("-i").Add(srco)
                        .Add("-filter_complex").Add("[1:a][2:a][3:a]amerge=inputs=3[a]", true)
                        .Add("-map").Add("0:v").Add("-map").Add("[a]").Add("-c:v").Add("copy")
                        .Add("-c:a").Add("aac").Add("-b:a").Add("160K").Add("-ac").Add("2")
                        .Add(DestFileName))
                        .WithValidation(CommandResultValidation.None).
                       WithWorkingDirectory(defaultpath);
                    _dest = DestFileName;
                }
                await foreach (var commandEvent in cmd.ListenAsync())
                {
                    switch (commandEvent)
                    {
                        case StartedCommandEvent StartedEvent:
                            {
                                errn = 9;
                                ProcessID = StartedEvent.ProcessId;
                                InternalProcessId = ProcessID;
                                ProcessRunning = true;
                                OnConverterStarted?.Invoke(this, _source, ProcessID);
                                break;
                            }
                        case StandardOutputCommandEvent OutputEvent:
                            {
                                errn = 10;
                                ProbeData.Add(OutputEvent.Text);
                                break;
                            }
                        case StandardErrorCommandEvent ErrorEvent:
                            {
                                errn = 11;
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
                                    if (idx != -1)
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
                                    break;
                                }
                                else
                                {
                                    if (!bMetaData)
                                    {
                                        bMetaData = data.Contains("Metadata:") || bMetaData;
                                        if (!vcopy && !acopy)
                                            break;
                                    }
                                }
                                if (bIsEncoding && (bMetaData || (vcopy || acopy)))
                                {
                                    string ComplexFile = "";
                                    if (_IsComplex && data.ContainsAll(new string[] { "Output", "#", "to" }))
                                    {
                                        int idx = data.IndexOf("'");
                                        string tempf = data.Substring(idx);
                                        idx = tempf.IndexOf("'");
                                        string fnm = tempf.Substring(0, idx - 1);
                                        ComplexFile = fnm;
                                    }
                                    double perc = -1;
                                    if (data.Contains("time="))
                                    {
                                        int idx = data.IndexOf("time=");
                                        int idx2 = data.IndexOf("bitrate");
                                        if (idx != 1)
                                        {
                                            string sp = data.Substring(idx + 5, idx2 - (idx + 5)).Trim();
                                            onseek = false;
                                            ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                            ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                            if (onseek)
                                            {
                                                OnConverterOnSeek?.Invoke(this, ComplexFile, sp, ProcessID);
                                                continue;
                                            }

                                            if (sp != "" && IsMuxed)
                                            {
                                                TimeSpan Tse = TimeSpan.Zero;
                                                TimeSpan.TryParse(sp, out Tse);
                                                double totalsecs = Tse.TotalSeconds;
                                                if (totalseconds > 0)
                                                {
                                                    //totalseconds -= startime.TotalSeconds;
                                                    perc = (100 / totalseconds) * totalsecs;
                                                }
                                                if (data.ContainsAll(new string[] { "frame", "fps", "size", "bitrate", "speed" }))
                                                {

                                                }
                                            }

                                        }
                                    }

                                    if (data.ContainsAll(new string[] { "size", "bitrate", "speed" }))
                                    {
                                        ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                        ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                        OnConverteringData?.Invoke(this, data, ComplexFile.Replace("\"", ""), ProcessID);
                                        OnProgressEventHandler?.Invoke(_dest, data);
                                        string frm = "";
                                        if (data.Contains("frame="))
                                        {
                                            int pos1 = data.IndexOf("frame=");
                                            frm = data.Substring(pos1 + 6);
                                        }
                                        long currentframe = -1;
                                        if (data.Contains("fps") && frm != "")
                                        {
                                            string frm2x = (frm.Substring(0, frm.IndexOf("fps")).Trim());
                                            long.TryParse(frm2x, out currentframe);
                                        }
                                        int percentdone = 0;
                                        if ((maxframes != -1) && (!_IsComplex && currentframe != -1))
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
                                        if ((maxframes == -1) && (!_IsComplex))
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
                                            //else percentdone = 0;
                                        }
                                        if (_IsComplex)
                                        {
                                            // findfile in DestFIle; Get Index , Get Cut , Get DUration, from time work out %
                                        }
                                        ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                        ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                        OnConverterProgress?.Invoke(this, ComplexFile, percentdone, Eta, MaxDuration, ProcessID);
                                    }
                                }
                                break;
                            }
                        case ExitedCommandEvent ExitEvent:
                            {
                                if (IsLogo && !IsMuxed)
                                {
                                    OnConverterDataUpdate?.Invoke(this, _dest.Replace("\"", ""), ProcessID, ExitEvent.ExitCode, ProbeData);
                                    AddLogo();
                                }
                                else
                                {
                                    errn = 12;
                                    OnConverterStopped?.Invoke(this, _dest.Replace("\"", ""), ProcessID, ExitEvent.ExitCode, ProbeData);
                                }
                                break;
                            }
                    }
                }
                return new ConversionResult
                {
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    Arguments = _parameterAsstring
                };

            }
            catch (Exception ex)
            {
                ex.LogWrite(errn.ToString() + "|" + MethodBase.GetCurrentMethod().Name);


                return new ConversionResult
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    Arguments = _parameterAsstring
                };

            }
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
        public async Task AddLogo()
        {
            try
            {
                _Is1080p = false;
                bool bIsEncoding = false;
                bool bSideDataRec = false;
                List<string> ProbeData = new List<string>();
                bool bMetaData = false;
                int percent = 0;
                Int64 maxframes = -1;
                string maxduration = "";
                TimeSpan Eta = TimeSpan.Zero;
                TimeSpan MaxDuration = TimeSpan.Zero;
                DateTime startTime = DateTime.Now;
                bool onseek = false, vcopy = false, acopy = true;
                _twitchparameters.AddRange(GetParameters());
                _twitchparameters.Add("-i");
                _twitchparameters.Add(_dest);
                _twitchparameters.Add("-i");
                _twitchparameters.Add(LogoSource);
                _twitchparameters.Add("-filter_complex");
                _twitchparameters.Add("[0:v][1:v]overlay=510:10");
                _twitchparameters.Add("-c:a");
                _twitchparameters.Add("copy");
                _twitchparameters.Add("-b:v");
                _twitchparameters.Add("1385K");
                _twitchparameters.Add("-maxrate");
                _twitchparameters.Add("2330K");
                _twitchparameters.AddRange(GetParameters(false));
                _twitchparameters.Add(_dest.Replace("(shorts)", "(shorts_logo)"));
                for (int i = _twitchparameters.Count - 1; i > -1; i--)
                {
                    if (_twitchparameters[i].Trim() == "")
                    {
                        _twitchparameters.RemoveAt(i);
                    }
                }


                var r3 = GetEncryptedString(new int[] { 170, 57, 73, 91, 225, 194, 201, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                cmd = Cli.Wrap(defaultpath + r3).
                                 WithArguments(args => args
                                 .Add(_twitchparameters))
                                 .WithValidation(CommandResultValidation.None).
                                WithWorkingDirectory(defaultpath);
                await foreach (var commandEvent in cmd.ListenAsync())
                {
                    switch (commandEvent)
                    {
                        case StartedCommandEvent StartedEvent:
                            {
                                ProcessID = StartedEvent.ProcessId;
                                InternalProcessId = ProcessID;
                                ProcessRunning = true;
                                OnConverterStarted?.Invoke(this, _source, ProcessID);
                                break;
                            }
                        case StandardOutputCommandEvent OutputEvent:
                            {
                                ProbeData.Add(OutputEvent.Text);
                                break;
                            }
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
                                    if (idx != -1)
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
                                    break;
                                }
                                else
                                {
                                    if (!bMetaData)
                                    {
                                        bMetaData = data.Contains("Metadata:") || bMetaData;
                                        if (!vcopy && !acopy)
                                            break;
                                    }
                                }
                                if (bIsEncoding && (bMetaData || (vcopy || acopy)))
                                {
                                    string ComplexFile = "";
                                    if (_IsComplex && data.ContainsAll(new string[] { "Output", "#", "to" }))
                                    {
                                        int idx = data.IndexOf("'");
                                        string tempf = data.Substring(idx);
                                        idx = tempf.IndexOf("'");
                                        string fnm = tempf.Substring(0, idx - 1);
                                        ComplexFile = fnm;
                                    }
                                    double perc = -1;
                                    if (data.Contains("time="))
                                    {
                                        int idx = data.IndexOf("time=");
                                        int idx2 = data.IndexOf("bitrate");
                                        if (idx != 1)
                                        {
                                            string sp = data.Substring(idx + 5, idx2 - (idx + 5)).Trim();
                                            if (sp.StartsWith("-"))
                                            {
                                                onseek = false;
                                                ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                                ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                                OnConverterOnSeek?.Invoke(this, ComplexFile, sp, ProcessID);
                                                continue;
                                            }
                                            else if (!onseek)
                                            {
                                                OnConverterOnSeek?.Invoke(this, ComplexFile, "", ProcessID);
                                                onseek = true;
                                            }
                                            if (sp != "")
                                            {
                                                TimeSpan Tse = TimeSpan.Zero;
                                                TimeSpan.TryParse(sp, out Tse);
                                                double totalsecs = Tse.TotalSeconds;
                                                if (totalseconds > 0)
                                                {
                                                    //totalseconds -= startime.TotalSeconds;
                                                    perc = (100 / totalseconds) * totalsecs;
                                                }
                                            }
                                        }
                                    }

                                    if (data.ContainsAll(new string[] { "frame", "fps", "size", "bitrate", "speed" }))
                                    {
                                        ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                        ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                        OnConverteringData?.Invoke(this, data, ComplexFile.Replace("\"", ""), ProcessID);
                                        OnProgressEventHandler?.Invoke(_dest, data);
                                        int pos1 = data.IndexOf("frame=");
                                        string frm = data.Substring(pos1 + 6);
                                        string frm2x = frm.Substring(0, frm.IndexOf("fps")).Trim();
                                        long currentframe = -1;
                                        long.TryParse(frm2x, out currentframe);
                                        int percentdone = 0;
                                        if ((maxframes != -1) && (!_IsComplex))
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
                                        if ((maxframes == -1) && (!_IsComplex))
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
                                            //else percentdone = 0;
                                        }
                                        if (_IsComplex)
                                        {
                                            // findfile in DestFIle; Get Index , Get Cut , Get DUration, from time work out %
                                        }
                                        ComplexFile = (!_IsComplex) ? _source : ComplexFile;
                                        ComplexFile = (MutliModeFileName != "") ? MutliModeFileName : ComplexFile;
                                        OnConverterProgress?.Invoke(this, ComplexFile, percentdone, Eta, MaxDuration, ProcessID);
                                    }
                                }
                                break;
                            }
                        case ExitedCommandEvent ExitEvent:
                            {
                                OnConverterStopped?.Invoke(this, _dest.Replace("\"", ""), ProcessID, ExitEvent.ExitCode, ProbeData);
                                break;
                            }

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} {MethodBase.GetCurrentMethod().Name}");
            }
        }

        private IEnumerable<string> GetUserDefinedParameters(bool Input = true)
        {
            try
            {
                if (_IsComplex)
                {
                    return null;
                }
                var result = new List<string>();
                foreach (var parameter in _userparameters.Where(parameter => parameter.Item2 == Input))
                {
                    result.AddRange(parameter.Item1.Split(" ").ToList<string>());
                    _parameterAsstring += _parameterAsstring == "" ? parameter.Item1 : " " + parameter.Item1;
                }
                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        private List<string> GetMap()
        {
            try
            {
                if ((_IsProbe) || (_IsComplex))
                {
                    return null;
                }
                var builder = new List<string>();
                foreach (IStream stream in _streams)
                {
                    if (_hasInputBuilder) // If we have an input builder we always want to map the first video stream as it will be created by our input builder
                    {
                        builder.Add($"-map");
                        builder.Add("0:0");
                    }
                    foreach (var source in stream.GetSource())
                    {
                        builder.Add($"-map");
                        if (_hasInputBuilder)
                        {
                            // If we have an input builder we need to add one to the input file index to account for the input created by our input builder.
                            builder.Add($"{_inputFileMap[source] + 1}:{stream.Index}");
                        }
                        else
                        {
                            if (_inputFileMap.Count > 0)
                            {
                                builder.Add($"{_inputFileMap[source]}:{stream.Index}");
                            }
                            else
                            {
                                builder.Add($"0:{stream.Index}");
                            }
                        }
                    }
                }

                foreach (var res in builder)
                {
                    _parameterAsstring += _parameterAsstring == "" ? res : " " + res;
                }
                return builder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        private List<string> GetFilters()
        {
            try
            {
                if (_IsProbe)
                {
                    return null;
                }
                var builder = new List<string>();
                var configurations = new List<IFilterConfiguration>();
                foreach (IStream stream in _streams)
                {
                    if (stream is IFilterable filterable)
                    {
                        configurations.AddRange(filterable.GetFilters());
                    }
                }
                IEnumerable<IGrouping<string, IFilterConfiguration>> filterGroups = configurations.GroupBy(configuration => configuration.FilterType);
                foreach (IGrouping<string, IFilterConfiguration> filterGroup in filterGroups)
                {
                    builder.Add($"{filterGroup.Key} \"");
                    foreach (IFilterConfiguration configuration in configurations.Where(x => x.FilterType == filterGroup.Key))
                    {
                        var values = new List<string>();
                        foreach (KeyValuePair<string, string> filter in configuration.Filters)
                        {
                            string map = $"[{configuration.StreamNumber}]";
                            string value = string.IsNullOrEmpty(filter.Value) ? $"{filter.Key} " : $"{filter.Key}={filter.Value}";
                            values.Add($"{map} {value}");
                        }
                        builder.AddRange(values);
                    }
                    //builder.Append("\" ");
                }
                foreach (var res in builder)
                {
                    _parameterAsstring += _parameterAsstring == "" ? res : " " + res;
                }
                return builder == null ? null : builder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }
        internal static IConverter New()
        {
            return new Converter();
        }

        internal static IConverter New(_AddConverterProgress data)
        {
            return new Converter(data);
        }

        private List<string> GetStreamsPostInputs()
        {
            try
            {
                if ((_IsProbe) || (_IsComplex))
                {
                    return null;
                }
                string command = "";
                int cnt = 0;
                bool vcopy = false, acopy = false;
                var builder = new List<string>();
                foreach (IStream stream in _streams)
                {
                    if (stream is VideoStream vss)
                    {
                        vcopy = stream.IsCopy;
                    }
                    if (stream is AudioStream ass)
                    {
                        acopy = stream.IsCopy;
                    }
                }
                foreach (IStream stream in _streams)
                {
                    if (stream is VideoStream vs)
                    {
                        List<string> inputfilter = new List<string>();
                        inputfilter = stream.BuildParameters(stream.PostInput).Split(" ").ToList();

                        foreach (string filter in inputfilter)
                        {
                            if (filter != "")
                            {
                                command += (command != "") ? $",{filter}" : filter;
                                cnt++;
                            }
                        }
                        if ((vcopy) && (acopy))
                        {
                            builder.Add("-c");
                            builder.Add("copy");
                        }
                        else
                        {
                            if ((vcopy) && (!acopy))
                            {
                                builder.Add("-c:v");
                                builder.Add("copy");
                            }

                        }
                        if ((cnt > 0) && (!vcopy))
                        {
                            builder.Add("-vf");
                            const string myStrQuote = "";
                            builder.Add($"{command}");
                        }
                    }
                    else builder.AddRange(stream.BuildParameters(stream.PostInput).Split(" ").ToList());
                }
                foreach (var res in builder)
                {
                    _parameterAsstring += _parameterAsstring == "" ? res : " " + res;
                }
                return builder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public IConverter SetPreset(ConversionPreset preset)
        {
            try
            {
                _parameters.Add(($"-preset {preset.ToString().ToLower()}", true));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IConverter SetVideoSyncMethod(VideoSyncMethod method)
        {
            try
            {
                if (method == VideoSyncMethod.auto)
                {
                    _parameters.Add(($"-vsync -1", true));
                }
                else
                {
                    _parameters.Add(($"-vsync {method}", true));
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }

        }

        public IConverter AddParameter(string parameter, bool parameterPosition = true)
        {
            try
            {
                _userparameters.Add((parameter, parameterPosition));// post input is TRUE
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter AddStream<T>(params T[] streams) where T : IStream
        {
            try
            {
                foreach (T stream in streams)
                {
                    if (stream != null)
                    {
                        _streams.Add(stream);
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IConverter SetVideoBitrate(string Minibitrate, string Maxbitrate, string Buffersize, VideoCodec Outputcodec, bool IsComplex, bool ComplexMode, bool IsCopy = false)
        {
            try
            {
                if (!IsCopy)
                {
                    _IsComplex = IsComplex;
                    AddParameter(string.Format("-b:v {0}", Minibitrate));
                    AddParameter(string.Format("-maxrate {0}", Maxbitrate));
                    AddParameter(string.Format("-bufsize {0}", Buffersize));
                    if (IsComplex)
                    {
                        ComplexBitRate = string.Format("-b:v {0}", Minibitrate);
                        ComplexBitRate += string.Format(" -maxrate {0}", Maxbitrate);
                        ComplexBitRate += string.Format(" -bufsize {0}", Buffersize);
                        if (!ComplexMode)
                        {
                            ComplexBitRate += " -x264opts nal-hrd=cbr:force-cfr=1";
                        }
                    }
                    if (Outputcodec == VideoCodec.h264) AddParameter("-x264opts nal-hrd=cbr:force-cfr=1");
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IConverter SetFrameRate(double frameRate)
        {
            try
            {
                _parameters.Add(($"-framerate {frameRate.ToFFmpegFormat(3)}", true));
                _parameters.Add(($"-r {frameRate.ToFFmpegFormat(3)}", true));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetSeek(TimeSpan? seek)
        {
            try
            {
                if (seek.HasValue)
                {
                    if (seek != TimeSpan.Zero)
                    {
                        _parameters.Add(($"-ss {seek.Value.ToFFmpeg()}", true));
                        startime = seek.Value;
                        totalseconds -= startime.TotalSeconds;
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetInputTime(TimeSpan? time)
        {
            try
            {
                if (time.HasValue)
                {
                    _parameters.Add(($"-ss {time.Value.ToFFmpeg()}", true));
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IConverter SetOutputTime(TimeSpan? time)
        {
            try
            {
                if (time.HasValue)
                {
                    if (time != TimeSpan.Zero)
                    {
                        _parameters.Add(($"-t {time.Value.ToFFmpeg()}", true));
                        totalseconds = time.Value.TotalSeconds;
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter UseMultiThread(bool multiThread)
        {
            try
            {
                var threads = multiThread ? Environment.ProcessorCount : 1;
                _parameters.Add(($"-threads {Math.Min(threads, 16)}", false));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            return UseHardwareAcceleration($"{hardwareAccelerator}", decoder.ToString(), encoder.ToString(), device);
        }
        public IConverter UseHardwareAcceleration(string hardwareAccelerator, string decoder, string encoder, int device = 0)
        {
            try
            {
                _parameters.Add(($"-hwaccel {hardwareAccelerator}", true));
                _parameters.Add(($"-c:v {decoder}", true));
                ComplexEncoder = $"-c:v {encoder?.ToString()}";
                _parameters.Add(($"-c:v {encoder?.ToString()}", false));
                if (device != 0)
                {
                    _parameters.Add(($"-hwaccel_device {device}", true));
                }
                UseMultiThread(false);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetCutList(List<string> cutList)
        {
            try
            {
                cutlist.AddRange(cutList);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }


        public IConverter SetConcat(bool setconcat, List<string> Files)
        {
            try
            {
                _IsConcat = setconcat;
                if (setconcat)
                {
                    DestinationFiles.Clear();
                    DestinationFiles.AddRange(Files);
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetOutput(string outputPath, bool IsTwitchStream = false)
        {
            try
            {
                const string myStrQuote = "\"";
                if (IsTwitchStream)
                {
                    //live_1061414984_Vu5NrETzHYqB1f4bZO12dxaCOfUkxf
                    _twitchparameters.Add("-f");
                    _twitchparameters.Add("flv");
                    _twitchparameters.Add($"rtmp://syd03.contribute.live-video.net/app/{outputPath}");
                    _output = "";
                }
                else
                {
                    OutputFilePath = outputPath.Contains(" ") ? myStrQuote + outputPath + myStrQuote : outputPath;
                    _output = outputPath.Trim().Contains(" ") ? outputPath.Escape() : outputPath;
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetAudioBitrate(long bitrate)
        {
            try
            {
                _parameters.Add(($"-b:a {bitrate}", true));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetVSync(VsyncParams vsyncmode)
        {
            try
            {
                _vsyncmode = vsyncmode;
                //if (vsyncmode != -1) AddParameter($"-vsync {vsyncmode}");
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }



        public IConverter UseShortest(bool useShortest)
        {
            try
            {
                if (useShortest)
                {
                    _parameters.Add(($"-shortest", true));
                }
                else
                {
                    _parameters.Remove(($"-shortest", true));
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IConverter SetDefaultPath(string path)
        {
            try
            {
                defaultpath = path;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }

        }

        public static async Task<IMediaInfoInternal> GetMediaInfo(string fileName, string exePath)
        {
            return await MediaInfoInternal.Get(fileName, exePath);
        }

        public void Dispose()
        {
            try
            {
                if (ProcessRunning)
                {
                    var p = Process.GetProcessById(InternalProcessId);
                    if (p is Process pmt)
                    {
                        if (!pmt.HasExited && pmt.ProcessName.Contains("ffmpeg"))
                        {
                            pmt.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
