using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VideoGui.ffmpeg.Streams.Text
{
    public interface ITextStream : IStream
    {
        TimeSpan Duration { get; set; }
        string Language { get; }
        string Format { get; }
        string CodecId { get; }
        string CodecInfo { get; }
        string Codec { get; }
        string Title { get; }
        int? Default { get; }
        int? Forced { get; }
        int? Id { get;  }
        int? CompressionMode { get; }
       
        ITextStream Initialize(string _Codec, TimeSpan _Duration, 
            string _CodecInfo, string _Title, string _CodecId, string _Language, 
            int _CompressionMode ,int _Default, int _Forced, int _id);


    }
}
