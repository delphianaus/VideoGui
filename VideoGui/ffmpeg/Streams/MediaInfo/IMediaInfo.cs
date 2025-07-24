using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Text;
using VideoGui.ffmpeg.Streams.Video;

namespace VideoGui.ffmpeg.Streams.MediaInfo
{
    public interface IMediaInfo
    {
        IEnumerable<IStream> Streams { get; }
        string Path { get; }
        string ExePath { get; }
        TimeSpan Duration { get; }
        long Size { get; }
        List<IVideoStream> VideoStreams { get; }
        List<IAudioStream> AudioStreams { get; }
        List<ITextStream> TextStreams { get; }
    }
}
