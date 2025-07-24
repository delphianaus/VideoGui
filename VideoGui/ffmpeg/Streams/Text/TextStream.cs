using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace VideoGui.ffmpeg.Streams.Text
{

    public class TextStream : ITextStream
    {
        private bool IsCopy = true;
        private string codecIDInfo;

        public TextStream(string codec, TimeSpan duration, string codecID, string codecIDInfo, string title, string language, int compressionMode, int @default, int forced, int iD)
        {
            Codec = codec;
            Duration = duration;
            CodecId = codecID;
            this.codecIDInfo = codecIDInfo;
            Title = title;
            Language = language;
            CompressionMode = compressionMode;
            Default = @default;
            Forced = forced;
            Id = iD;
        }

        public TextStream()
        {
        }

        public StreamType StreamType => StreamType.Text;
        public ITextStream Initialize(string _Codec, TimeSpan _Duration,
            string _CodecInfo, string _Title, string _CodecId, string _Language,
            int _CompressionMode,int _Default, int _Forced, int _id)
        {
            try
            {
                Default = _Default;
                Forced = _Forced;
                Index = _id - 1;
                Duration = _Duration;
                Codec = _Codec;
                Language = _Language;
                CompressionMode = _CompressionMode;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return this;
            }
        }

        public string BuildParameters(bool forPosition)
        {
            return string.Empty;
        }

        public IEnumerable<string> GetSource()
        {
            return new List<string>();
        }

        public int Index { get; internal set; }
        public TimeSpan Duration { get; set; }
        public string Codec { get; internal set; }
        public string Language { get; internal set; }
        public int? Default { get; internal set; }
        public int? Forced { get; internal set; }
        public int? CompressionMode { get; internal set; }
        public string Format { get; internal set; }

        public string CodecId { get; internal set; }

        public string CodecInfo { get; internal set; }

        public string Title { get; internal set; }

        public int? Id { get; internal set; }

        public bool PreInput => false;

        public bool PostInput => false;

        bool IStream.IsCopy => true;
        public string Path { get ; set; }
    }
}
