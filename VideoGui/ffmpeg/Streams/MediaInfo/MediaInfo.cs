using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Video;
using CliWrap;
using Newtonsoft.Json;
using VideoGui.ffmpeg.Probe;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Net;
using System.Runtime.CompilerServices;

namespace VideoGui.ffmpeg.Streams.MediaInfo
{
    public class MediaInfo : IMediaInfo
    {
        private MediaInfo(string path, string exePath)
        {
            Path = path;
            ExePath = exePath;
        }
        public IEnumerable<IStream> Streams => VideoStreams.Concat<IStream>(AudioStreams);
        public TimeSpan Duration { get; internal set; }
        /// <inheritdoc />
        public long Size { get; internal set; }

        public string timecode { get; internal set; }

        public List<IVideoStream> VideoStreams { get; set; }

        public List<IAudioStream> AudioStreams { get; set; }

        public string Path { get; }

        public string ExePath { get; }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        internal static async Task<IMediaInfo> Get(string filePath, string exePath)
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(130)))
            {
                var cancellationToken = source.Token;
                return await Get(filePath, exePath, cancellationToken);
            }
        }

        internal static async Task<List<IMediaInfo>> Get(List<string> files, string exePath)
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(130)))
            {
                var cancellationToken = source.Token;
                List<IMediaInfo> result = new List<IMediaInfo>();
                foreach (string filePath in files)
                {
                    result.Add(await Get(filePath, exePath, cancellationToken));
                }
                return result;
            }
        }

        internal void ErrorHandler(string arg)
        {

        }

        private static TimeSpan GetVideoDuration(ProbeModel.Stream video, FormatModel.Format format)
        {
            double duration = video.duration > 0.01 ? video.duration : format.duration;
            TimeSpan videoDuration = TimeSpan.FromSeconds(duration);
            return videoDuration;
        }
        private static long GetFrameCount(ProbeModel.Stream vid)
        {
            long frameCount = 0;
            return long.TryParse(vid.nb_frames, out frameCount) ? frameCount : 0;
        }
        private static double GetVideoFramerate(ProbeModel.Stream vid)
        {
            long frameCount = GetFrameCount(vid);
            double duration = vid.duration;
            string[] fr = vid.r_frame_rate.Split('/');

            if (frameCount > 0)
                return Math.Round(frameCount / duration, 3);
            else
                return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
        }

        private static int GetGcd(int width, int height)
        {
            while (width != 0 &&
                  height != 0)
            {
                if (width > height)
                {
                    width -= height;
                }
                else
                {
                    height -= width;
                }
            }
            return width == 0 ? height : width;
        }

        private static string GetVideoAspectRatio(int width, int height)
        {
            int cd = GetGcd(width, height);
            if (cd <= 0)
            {
                return "0:0";
            }
            return width / cd + ":" + height / cd;
        }


        private static TimeSpan GetAudioDuration(ProbeModel.Stream audio)
        {
            double duration = audio.duration;
            TimeSpan audioDuration = TimeSpan.FromSeconds(duration);
            return audioDuration;
        }
        private static TimeSpan CalculateDuration(IEnumerable<IVideoStream> videoStreams, IEnumerable<IAudioStream> audioStreams)
        {
            double audioMax = audioStreams.Any() ? audioStreams.Max(x => x.Duration.TotalSeconds) : 0;
            double videoMax = videoStreams.Any() ? videoStreams.Max(x => x.Duration.TotalSeconds) : 0;
            return TimeSpan.FromSeconds(Math.Max(audioMax, videoMax));
        }

        public static string DecryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 157, 14, 22, 138, 249, 133, 44, 16, 228, 199, 00, 111, 31, 17, 74, 1, 8, 9, 33,
                44, 66, 88, 99, 00, 11, 132, 157, 174, 21, 18, 93, 233, 244, 66, 88, 199, 00, 11, 232, 157, 174, 31, 8, 19, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 132, 233, 122, 27, 41, 44, 136, 172, 223, 132, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return Encoding.ASCII.GetString(encvar);
        }
        public static string GetEncryptedString(byte[] encriptedString)
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
        internal static async Task<IMediaInfo> Get(string filePath, string exePath, CancellationToken cancellationToken)
        {
            try
            {
                var mediaInfo = new MediaInfo(filePath, exePath);
                var path = mediaInfo.Path;
                var stdErrors = new StringBuilder();
                var stdOut = new StringBuilder();
                //exePath.WriteLog();
                var ffp = GetEncryptedString(new int[] { 170, 57, 73, 70, 227, 200, 204, 86, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                await Cli.Wrap(exePath+ffp).
                     WithWorkingDirectory(exePath+"\\").
                     WithArguments(args => args
                     .Add("-v").Add("panic").Add("-print_format").Add("json=c=1")
                     .Add("-show_streams").Add($"{filePath.Replace("\"","")}")).WithValidation(CommandResultValidation.None).
                     WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrors)).
                     WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut)).
                     ExecuteAsync();

                if (stdOut.ToString() != "")
                {
                    ProbeModel probe = JsonConvert.DeserializeObject<ProbeModel>(stdOut.ToString());
                   
                    if (probe.streams == null)
                    {
                        probe.streams = new ProbeModel.Stream[0];
                    }
                    ProbeModel.Stream[] streams = probe.streams;
                    if (!streams.Any())
                    {
                        throw new ArgumentException($"Invalid file. Cannot load file {filePath}");
                    }


                    stdErrors.Clear();
                    stdOut.Clear();
                    var fxfp = GetEncryptedString(new int[] { 170, 57, 73, 70, 227, 200, 204, 86, 188, 120, 21, 164 }.Select(i => (byte)i).ToArray());
                    await Cli.Wrap(exePath+ fxfp).
                        WithWorkingDirectory(exePath).WithArguments(args => args
                       .Add("-v").Add("panic").Add("-print_format").Add("json=c=1")
                       .Add("-show_entries").Add("format=size,duration,bit_rate").Add($"{filePath.Replace("\"", "")}")).
                       WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrors)).
                       WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut)).
                       WithValidation(CommandResultValidation.None).ExecuteAsync();
   
                    if (stdOut.ToString() != "")
                    {
                        string output = stdOut.ToString().Trim();
                        if (output != "{\r\n\r\n}")
                        {
                            var root = JsonConvert.DeserializeObject<FormatModel.Root>(output);
                            if (root != null)
                            {
                                if (mediaInfo.VideoStreams == null)
                                    mediaInfo.VideoStreams = new();
                                if (mediaInfo.AudioStreams == null)
                                    mediaInfo.AudioStreams = new();
                                FormatModel.Format format = root.format;
                                if (format.size != null)
                                {
                                    mediaInfo.Size = long.Parse(format.size);
                                }
                                string TimeCode = "";
                                int count = streams.Where(x => x.codec_type == "video").Count();
                                foreach (var model in streams.Where(x => x.codec_type == "video"))
                                {
                                    VideoStream video = new();
                                    video.Codec = model.codec_name;
                                    video.Width = model.width;
                                    video.Height = model.height;
                                    if (model.tags is not null)
                                    {
                                        video.timecode = model.tags.timecode;
                                        TimeCode = video.timecode;
                                    }
                                    video.Index = model.index;
                                    video.Path = filePath;
                                    video.Bitrate = Math.Abs(model.bit_rate) > 0.01 ? model.bit_rate : format.bit_Rate;
                                    video.PixelFormat = model.pix_fmt;
                                    video.Duration = GetVideoDuration(model, format);
                                    video.Default = model.disposition?._default;
                                    video.Forced = model.disposition?.forced;
                                    video.Rotation = model.tags?.rotate;
                                    video.Framerate = GetVideoFramerate(model);
                                    video.Ratio = GetVideoAspectRatio(model.width, model.height);
                                    mediaInfo.VideoStreams.Add(video);
                                }
                                foreach (var model in streams.Where(x => x.codec_type == "audio"))
                                {
                                    AudioStream audio = new();
                                    audio.Codec = model.codec_name;
                                    audio.Index = model.index;
                                    audio.Duration = GetAudioDuration(model);
                                    audio.Bitrate = Math.Abs(model.bit_rate);
                                    audio.Path = filePath;
                                    if (model.tags is not null)
                                    {
                                        audio.timecode = model.tags.timecode;
                                    }
                                    audio.Channels = model.channels;
                                    audio.SampleRate = model.sample_rate;
                                    audio.Language = model.tags?.language;
                                    audio.Default = model.disposition?._default;
                                    audio.Forced = model.disposition?.forced;
                                    mediaInfo.AudioStreams.Add(audio);
                                }
                                mediaInfo.Duration = CalculateDuration(mediaInfo.VideoStreams, mediaInfo.AudioStreams);
                                mediaInfo.timecode = TimeCode;
                                return mediaInfo;
                                return mediaInfo;
                            }
                        }
                        else
                        {
                            Thread.Sleep(300);
                            var x = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 84, 247, 105, 77, 167, 213, 149, 67, 125, 61, 190 }.Select(i => (byte)i).ToArray());
                            string err = x + filePath;
                            err.WriteLog(MethodBase.GetCurrentMethod().Name) ;
                            return await Get(filePath, exePath, cancellationToken);
                        }
                    }
                }
                return mediaInfo;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid file. Cannot load file"))
                {
                    if (File.Exists(filePath))
                    {
                        Thread.Sleep(500);
                        var r = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 84, 247, 105, 77, 167, 213, 149, 67, 125, 61, 190 }.Select(i => (byte)i).ToArray());
                        string err = r + filePath;
                        err.WriteLog(MethodBase.GetCurrentMethod().Name);
                        return await Get(filePath, exePath, cancellationToken);
                    }
                }
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        internal static async Task<IMediaInfo> Get(FileInfo fileInfo, string exePath)
        {
            if (!File.Exists(fileInfo.FullName))
            {
                //throw new InvalidInputException($"Input file {fileInfo.FullName} doesn't exists.");
            }
            return await Get(fileInfo.FullName, exePath);
        }
    }
}
