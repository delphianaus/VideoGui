using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoGui.ffmpeg.Streams.Video
{
    public class VideoStream : IVideoStream, IFilterable
    {
        public List<(string, bool)> _parameters = new List<(string, bool)>();
        private readonly Dictionary<string, string> _videoFilters = new Dictionary<string, string>();
        private string _watermarkSource;
        private bool PreInput = true;
        private bool PostInput = false;

        internal VideoStream()
        {

        }

        /// <inheritdoc />
        public IEnumerable<IFilterConfiguration> GetFilters()
        {
            if (_videoFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = "-filter_complex",
                    StreamNumber = Index,
                    Filters = _videoFilters
                };
            }
        }

        public bool IsCopy { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public string? timecode { get; internal set; }
        public double Framerate { get; set; }
        public string Ratio { get; set; }
        public TimeSpan Duration { get; set; }
        public string Codec { get; internal set; }
        public int Index { get; internal set; }
        public string Path { get; set; }
        public int? Default { get; internal set; }
        public int? Forced { get; internal set; }
        public string PixelFormat { get; set; }
        public int? Rotation { get; internal set; }

        private int _qmin;
        private int _qmax;
        private int _qscale;
        private string _vtag;
        private int _pass;
        private bool _skipAudio;
        private string _size;


        public string BuildParameters(bool forPosition)
        {
            try
            {
                string res = "";
                foreach (var param in _parameters.Where(param => param.Item2 == forPosition))
                {
                    res += param.Item1 + " ";
                }
                res = res.Trim();
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public IVideoStream Initialize(string _path, string _Codec, string _Ratio, TimeSpan _Duration, string _PixelFormat, int _Width, int _Height, int _Bitrate, float _fps, int _Default, int _Forced, int _id)
        {
            try
            {
                PixelFormat = _PixelFormat;
                Path = _path;

                Duration = _Duration;
                Ratio = _Ratio;
                Codec = _Codec;
                Width = _Width;
                Height = _Height;
                Framerate = _fps;
                Bitrate = _Bitrate;
                Forced = _Forced;
                Default = _Default;
                Index = _id - 1;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetPixelFormat(string format)
        {
            try
            {
                PixelFormat = format;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream ChangeSpeed(double multiplication)
        {
            try
            {
                _videoFilters["setpts"] = GetVideoSpeedFilter(multiplication);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        private string GetVideoSpeedFilter(double multiplication)
        {
            try
            {
                if (multiplication < 0.5 || multiplication > 2.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
                }

                double videoMultiplicator;
                if (multiplication >= 1)
                {
                    videoMultiplicator = 1 - (multiplication - 1) / 2;
                }
                else
                {
                    videoMultiplicator = 1 + (multiplication - 1) * -2;
                }
                return $"{videoMultiplicator.ToFFmpegFormat()}*PTS ";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public IVideoStream Rotate(RotateDegrees rotateDegrees)
        {
            try
            {
                var rotate = rotateDegrees == RotateDegrees.Invert ? " \"transpose=2,transpose=2\" " : $" \"transpose={(int)rotateDegrees}\" ";
                _parameters.Add((rotate, PostInput));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public StreamType StreamType => StreamType.Video;
        public long Bitrate { get; internal set; }

        bool IStream.PreInput => PreInput;

        bool IStream.PostInput => PostInput;

        public IVideoStream CopyStream()
        {
            try
            {
                IsCopy = true;
                return this;// SetCodec(VideoCodec.copy);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream Mpeg4ASP(int minq, int maxq, int pass = 0, bool skipAudio = false)
        {
            try
            {
                (_qmin, _qmax, _vtag, _pass, _skipAudio) = (minq, maxq, "XVID", pass, skipAudio);
                return SetCodec(VideoCodec.libxvid);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream Mpeg4AVC(int qscale = 0, string vTag = "XVID", int pass = 0, bool skipAudio = false)
        {
            try
            {
                (_qscale, _vtag, _pass, _skipAudio) = (qscale, "XVID", pass, skipAudio);
                return SetCodec(VideoCodec.mpeg4);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetLoop(int count, int delay)
        {
            try
            {
                _parameters.AddIfNotExists($"-loop {count}", PostInput);
                if (delay > 0)
                {
                    _parameters.Add(($"-final_delay {delay / 100}", PostInput));
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IVideoStream SetFishEyeRemoval(bool fishEyeRemoval, double LRF, double RRF, double LLC, double RLC)
        {
            try
            {
                if (fishEyeRemoval)
                {
                    const string QS = "\"";
                    _parameters.AddIfNotExists($"lenscorrection={LRF.ToString()}:{RRF.ToString()}:{LLC.ToString()}:{RLC.ToString()}", PostInput);
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IVideoStream Reverse()
        {
            try
            {
                _parameters.AddIfNotExists($"reverse", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetBitrate(long bitrate)
        {
            try
            {
                _parameters.AddIfNotExists($"-b:v {bitrate}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize)
        {
            try
            {
                _parameters.AddIfNotExists($"-b:v {minBitrate}", PostInput);
                _parameters.AddIfNotExists($"-maxrate {maxBitrate}", PostInput);
                _parameters.AddIfNotExists($"-bufsize {bufferSize}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetFlags(params Flag[] flags)
        {
            try
            {
                return SetFlags(flags.Select(x => x.ToString()).ToArray());
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetFlags(params string[] flags)
        {
            try
            {
                var input = string.Join("+", flags);
                if (input[0] != '+')
                {
                    input = "+" + input;
                }
                _parameters.Add(($"-flags {input}", PostInput));
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }


        /// <inheritdoc />
        public IVideoStream SetFramerate(double framerate)
        {
            try
            {
                _parameters.AddIfNotExists($"-r {framerate.ToFFmpegFormat(3)}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetSize(VideoSize size)
        {
            try
            {
                _parameters.AddIfNotExists($"scale={size.ToFFmpegFormat()}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IVideoStream SetFPS(float fps)
        {
            try
            {
                _parameters.AddIfNotExists($"fps={fps}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IVideoStream Crop(int x, int y, int left, int top, string dar = "")
        {
            try
            {
                string crop = x > 0 || y > 0 ? $"crop ={x}:{y}:{left}:{top}" : "";
                if (crop != "") _parameters.AddIfNotExists(crop, PostInput);
                string setdar = (dar != "") ? "setdar={dar}" : "";
                if (setdar != "") _parameters.AddIfNotExists(setdar, PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetSize(int width, int height, double aspectratio, int Modulas, Scaling Scaler, int x, int y, int left, int top, string dar = "")
        {
            try
            {
                if (width > 0)
                {
                    double AdjustedHeight = aspectratio != -1 ? Math.Round(width / aspectratio) : height;
                    if (Modulas != -1)
                    {
                        int rd = Modulas.ToString().ToInt();
                        AdjustedHeight = Math.Round(AdjustedHeight / rd) * rd;
                    }
                    string crop = x > 0 || y > 0 ? $"crop={x}:{y}:{left}:{top}" : "";
                    string setdar = (dar != "") ? $"setdar={dar}" : "";
                    _size = $"scale={width}:{AdjustedHeight}:flags=" + Scaler.ToString();//
                    //crop + setdar;
                    _parameters.AddIfNotExists(_size, PostInput);
                    if ((x > 0) && (y > 0)) _parameters.AddIfNotExists(crop, PostInput);
                    if (setdar != "") _parameters.AddIfNotExists(setdar, PostInput);
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
        public IVideoStream SetSize(int width, int height)
        {
            try
            {
                _parameters.AddIfNotExists($"scale={width}x{height}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(VideoCodec codec)
        {
            try
            {
                string input = codec.ToString();
                if (codec == VideoCodec._8bps)
                {
                    input = "8bps";
                }
                else if (codec == VideoCodec._4xm)
                {
                    input = "4xm";
                }
                else if (codec == VideoCodec._012v)
                {
                    input = "012v";
                }
                return SetCodec($"{input}");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(string codec)
        {
            try
            {
                Codec = codec;
                if (codec != "copy")
                {
                    _parameters.AddIfNotExists($"-c:v {codec}", PostInput);
                }
                else
                {
                    IsCopy = true;
                    //_parameters.AddIfNotExists($"-c copy", PostInput);
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
        public IVideoStream SetBitstreamFilter(BitstreamFilter filter)
        {
            try
            {
                return SetBitstreamFilter($"{filter}");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetBitstreamFilter(string filter)
        {
            try
            {
                _parameters.AddIfNotExists($"-bsf:v {filter}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetSeek(TimeSpan seek)
        {
            try
            {
                if (seek != null)
                {
                    if (seek > Duration)
                    {
                        throw new ArgumentException("Seek can not be greater than video duration. Seek: " + seek.TotalSeconds + " Duration: " + Duration.TotalSeconds);
                    }
                    _parameters.AddIfNotExists($"-ss {seek.ToFFmpeg()}", PreInput);
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
        public IVideoStream SetOutputFramesCount(int number)
        {
            try
            {
                _parameters.AddIfNotExists($"-frames:v {number}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetWatermark(string imagePath, Position position)
        {
            try
            {
                _watermarkSource = imagePath;
                string argument = string.Empty;
                switch (position)
                {
                    case Position.Bottom:
                        argument += "(main_w-overlay_w)/2:main_h-overlay_h";
                        break;
                    case Position.Center:
                        argument += "x=(main_w-overlay_w)/2:y=(main_h-overlay_h)/2";
                        break;
                    case Position.BottomLeft:
                        argument += "5:main_h-overlay_h";
                        break;
                    case Position.UpperLeft:
                        argument += "5:5";
                        break;
                    case Position.BottomRight:
                        argument += "(main_w-overlay_w):main_h-overlay_h";
                        break;
                    case Position.UpperRight:
                        argument += "(main_w-overlay_w):5";
                        break;
                    case Position.Left:
                        argument += "5:(main_h-overlay_h)/2";
                        break;
                    case Position.Right:
                        argument += "(main_w-overlay_w-5):(main_h-overlay_h)/2";
                        break;
                    case Position.Up:
                        argument += "(main_w-overlay_w)/2:5";
                        break;
                }
                _videoFilters["overlay"] = argument;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IVideoStream Split(TimeSpan startTime, TimeSpan duration)
        {
            try
            {
                _parameters.AddIfNotExists($"-ss {startTime.ToFFmpeg()}", PostInput);
                _parameters.AddIfNotExists($"-t {duration.ToFFmpeg()}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_watermarkSource))
                    return new[] { Path, _watermarkSource };
                return new[] { Path };
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <inheritdoc />
        public IVideoStream SetInputFormat(Format inputFormat)
        {
            try
            {
                var format = inputFormat.ToString();
                switch (inputFormat)
                {
                    case Format._3dostr:
                        format = "3dostr";
                        break;
                    case Format._3g2:
                        format = "3g2";
                        break;
                    case Format._3gp:
                        format = "3gp";
                        break;
                    case Format._4xm:
                        format = "4xm";
                        break;
                }
                return SetInputFormat(format);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetInterlaced()
        {
            try
            {
                _parameters.AddIfNotExists($"nnedi=weights='nnedi3_weights.bin':pscrn=new:qual=fast:nns=n16:nsize=s32x4:field=af", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        /// <inheritdoc />
        public IVideoStream SetInputFormat(string format)
        {
            try
            {
                if (format != null)
                    _parameters.AddIfNotExists($"-f {format}", PreInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public IVideoStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            try
            {
                _parameters.AddIfNotExists("-re", PreInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IVideoStream SetStreamLoop(int loopCount)
        {
            try
            {
                _parameters.AddIfNotExists($"-stream_loop {loopCount}", PreInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
    }
}
