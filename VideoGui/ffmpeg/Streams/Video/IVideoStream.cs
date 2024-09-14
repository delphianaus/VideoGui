using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VideoGui.ffmpeg.Streams.Video
{
    public interface IVideoStream : IStream
    {
        public TimeSpan Duration { get; set; }
        int Width { get; }
        int Height { get; }
        public double Framerate { get; set; }


        string Ratio { get; set; }
        long Bitrate { get; }
        int? Default { get; }
        int? Forced { get; }
        string PixelFormat { get; }
        int? Rotation { get; }

        IVideoStream Initialize(string _path, string _Codec, string _Ratio, TimeSpan _Duration, string _PixelFormat, int _Width, int _Height, int _Bitrate, float _fps, int _Default, int _Forced, int _id);
        public string BuildParameters(bool forPosition);
        IVideoStream SetInterlaced();
        IVideoStream SetPixelFormat(string format);
        IVideoStream Rotate(RotateDegrees rotateDegrees);
        IVideoStream ChangeSpeed(double multiplicator);
        IVideoStream SetWatermark(string imagePath, Position position);
        IVideoStream Reverse();
        IVideoStream SetFishEyeRemoval(bool fishEyeRemoval, double LRF, double RRF, double LLC, double RLC);
        IVideoStream SetFlags(params Flag[] flags);
        IVideoStream SetFlags(params string[] flags);
        IVideoStream SetFramerate(double framerate);
        IVideoStream SetFPS(float fps);
        IVideoStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize);
        IVideoStream SetBitrate(long bitrate);
        IVideoStream SetSize(VideoSize size);
        IVideoStream SetSize(int width, int height, double aspectratio, int Modulas, Scaling Scaler, int x, int y, int left, int top, string dar = "");
        IVideoStream SetSize(int width, int height);
        IVideoStream SetCodec(VideoCodec codec);
        IVideoStream SetCodec(string codec);
        IVideoStream Crop(int x, int y, int left, int top, string dar = "");
        IVideoStream CopyStream();
        IVideoStream Mpeg4ASP(int minq, int maxq, int pass = 0, bool skipAudio = false);
        IVideoStream Mpeg4AVC(int qscale = 0, string vTag = "XVID", int pass = 0, bool skipAudio = false);
        IVideoStream SetBitstreamFilter(BitstreamFilter filter);
        IVideoStream SetLoop(int count, int delay = 0);
        IVideoStream SetOutputFramesCount(int number);
        IVideoStream SetSeek(TimeSpan seek);
        IVideoStream Split(TimeSpan startTime, TimeSpan duration);
        IVideoStream SetBitstreamFilter(string filter);
        IVideoStream SetInputFormat(string inputFormat);
        IVideoStream SetInputFormat(Format inputFormat);
        IVideoStream UseNativeInputRead(bool readInputAtNativeFrameRate);
        IVideoStream SetStreamLoop(int loopCount);
    }
}
