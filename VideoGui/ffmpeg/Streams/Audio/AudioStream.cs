using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace VideoGui.ffmpeg.Streams.Audio
{

    public class AudioStream : IAudioStream, IFilterable
    {
        private readonly List<(string, bool)> _parameters = new List<(string, bool)>();
        private readonly Dictionary<string, string> _audioFilters = new Dictionary<string, string>();
        private bool PreInput = true;
        private bool PostInput = false;
        private bool IsCopy = false;
        internal AudioStream()
        {

        }

        bool IStream.PreInput => PreInput;
        bool IStream.PostInput => PostInput;
        bool IStream.IsCopy => IsCopy;

        public IAudioStream SetLanguage(string language)
        {
            try
            {
                Language = language;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IAudioStream Reverse()
        {
            try
            {
                _parameters.AddIfNotExists("-af areverse", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream Split(TimeSpan startTime, TimeSpan duration)
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
        public IAudioStream CopyStream()
        {
            try
            {
                IsCopy = true;
                return this;
                //return this.SetCodec(AudioCodec.copy);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public StreamType StreamType => StreamType.Audio;
        public IAudioStream SetChannels(int channels)
        {
            try
            {
                _parameters.AddIfNotExists($"-ac:{Index} {channels}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetBitstreamFilter(BitstreamFilter filter)
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
        public IAudioStream SetBitstreamFilter(string filter)
        {
            try
            {
                _parameters.AddIfNotExists($"-bsf:a {filter}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetMp3(int bitrate)
        {
            try
            {
                SetCodec(AudioCodec.mp3);
                SetBitrate(bitrate);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream AAC_VbrMode(int mode, bool resample = false)
        {
            try
            {
                SetCodec(AudioCodec.aac);
                if (resample)
                {
                    _parameters.AddIfNotExists($" -q:a {mode} -ar 44100", PostInput);
                }
                else _parameters.AddIfNotExists($" -q:a {mode} ", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetBitrate(long bitRate)
        {
            try
            {
                _parameters.AddIfNotExists($"-b:a:{Index} {bitRate}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize)
        {
            try
            {
                _parameters.AddIfNotExists($"-b:a:{Index} {minBitrate}", PostInput);
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
        public IAudioStream SetSampleRate(int sampleRate)
        {
            try
            {
                _parameters.AddIfNotExists($"-ar:{Index} {sampleRate}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream ChangeSpeed(double multiplication)
        {
            try
            {
                _audioFilters["atempo"] = $"{GetAudioSpeed(multiplication)}";
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        private string GetAudioSpeed(double multiplication)
        {
            try
            {
                if (multiplication < 0.5 || multiplication > 2.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
                }
                return $"{multiplication.ToFFmpegFormat()} ";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public IAudioStream SetCodec(AudioCodec codec)
        {
            try
            {
                string input = codec.ToString();// _parameters.Add(new ConversionParameter($"-b:a:{Index} {bitRate}"));
                if (codec == AudioCodec.copy)
                {
                    bool found = false;
                    foreach (var param in _parameters)
                    {
                        if (param.Item1 == $"-c:a {codec.ToString()}")
                        {
                            found = true;
                        }
                    }
                    if (!found) _parameters.Add(($"-c:a {codec.ToString()}", PostInput));
                }
                else if (codec == AudioCodec._4gv)
                {
                    input = "4gv";
                }
                else if (codec == AudioCodec._8svx_exp)
                {
                    input = "8svx_exp";
                }
                else if (codec == AudioCodec._8svx_fib)
                {
                    input = "8svx_fib";
                }
                return SetCodec($"{input}");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public IAudioStream Initialize(string _path, string _Codec, int _Bitrate, int _Channels, int _SampleRate, TimeSpan _Duration, string _Language, int _Default, int _Forced, int _id)
        {
            try
            {
                Bitrate = _Bitrate;
                Path = _path;
                Default = _Default;
                Forced = _Forced;
                Index = _id - 1;
                Duration = _Duration;
                Channels = _Channels;
                Codec = _Codec;
                Language = _Language;
                SampleRate = _SampleRate;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetCodec(string codec)
        {
            try
            {
                Codec = codec;
                _parameters.AddIfNotExists($"-c:a {codec.ToString()}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public int Index { get; internal set; }
        public TimeSpan Duration { get; set; }
        public string Codec { get; internal set; }
        public long Bitrate { get; internal set; }
        public int Channels { get; internal set; }
        public int SampleRate { get; internal set; }

        public string? timecode { get; internal set; }
        public string Language { get; internal set; }
        public int? Default { get; internal set; }
        public int? Forced { get; internal set; }
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }
        public string Path { get; set; }
        public IAudioStream SetSeek(TimeSpan? seek)
        {
            try
            {
                _parameters.AddIfNotExists($"-ss {seek.Value.ToFFmpeg()}", PostInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IEnumerable<IFilterConfiguration> GetFilters()
        {
            if (_audioFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = "-filter:a",
                    StreamNumber = Index,
                    Filters = _audioFilters
                };
            }
        }
        public IAudioStream SetInputFormat(string inputFormat)
        {
            try
            {
                _parameters.AddIfNotExists($"-f {inputFormat}", PreInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetInputFormat(Format inputFormat)
        {
            return this.SetInputFormat(inputFormat.ToString());
        }
        public IAudioStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            try
            {
                _parameters.AddIfNotExists($"-re", PreInput);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }
        public IAudioStream SetStreamLoop(int loopCount)
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
        string IStream.BuildParameters(bool forPosition)
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
    }
}
