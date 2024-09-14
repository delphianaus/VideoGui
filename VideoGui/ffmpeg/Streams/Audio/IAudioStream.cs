using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VideoGui.ffmpeg.Streams.Audio
{
    public interface IAudioStream : IStream
    {
        TimeSpan Duration { get; set; }
        long Bitrate { get; }
        int SampleRate { get; }
        int Channels { get; }
        string Language { get; }
        int? Default { get; }
        int? Forced { get; }
        IAudioStream SetMp3(int bitrate);
        IAudioStream SetLanguage(string language);
        IAudioStream AAC_VbrMode(int Mode, bool resample = false);
        IAudioStream CopyStream();
        IAudioStream SetChannels(int channels);
        IAudioStream SetCodec(AudioCodec codec);
        IAudioStream SetCodec(string codec);
        IAudioStream SetBitstreamFilter(BitstreamFilter filter);
        IAudioStream SetBitrate(long bitRate);
        IAudioStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize);
        IAudioStream SetSampleRate(int sampleRate);
        IAudioStream ChangeSpeed(double multiplicator);
        IAudioStream Split(TimeSpan startTime, TimeSpan duration);
        IAudioStream SetSeek(TimeSpan? seek);
        IAudioStream SetBitstreamFilter(string filter);
        IAudioStream SetInputFormat(string inputFormat);
        IAudioStream SetInputFormat(Format inputFormat);
        IAudioStream UseNativeInputRead(bool readInputAtNativeFrameRate);
        IAudioStream SetStreamLoop(int loopCount);

        IAudioStream Initialize(string _path, string _Codec, int _Bitrate, int _Channels, int _SampleRate, TimeSpan _Duration, string _Language, int _Default, int _Forced, int _id);


    }
}
