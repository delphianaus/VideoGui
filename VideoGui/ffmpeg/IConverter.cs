using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VideoGui.ffmpeg.Events;
using VideoGui.ffmpeg.Streams;
using VideoGui.Models.delegates;

namespace VideoGui.ffmpeg
{
    public interface IConverter
    {
        IEnumerable<string> ProbeResults { get; }
        event _AddConverterProgress OnProgressEventHandler;
        event ConverterProbDataEventHandler OnProbeData;
        event ConverterOnStartedEventHandler OnConverterStarted;
        event ConverterOnProgressEventHandler OnConverterProgress;
        event ConverterOnStoppedEventHandler OnConverterStopped;
        event ConverterOnStoppedEventHandler OnConverterDataUpdate;
        event ConverterOnDataEventHandler OnConverteringData;
        event ConverterOnSeekEventHandler OnConverterOnSeek;
        

        public IConverter New(_AddConverterProgress data)
        {
            return Converter.New(data);
        }


        IConverter AddStream<T>(params T[] streams) where T : IStream;
        IConverter SetSeek(TimeSpan? seek);
        IConverter UseTextStream(int textStream = 0, bool UseTextStream = false);
        IConverter SetSourceIndex(int index);
        IConverter SetPreset(ConversionPreset preset);
        IConverter SetTotalTime(double _totalseconds);
        IConverter SetVideoBitrate(string Minibitrate, string Maxbitrate, string Buffersize, VideoCodec Outputcodec, bool IsComplex, bool ComplexMode, bool IsCopy = false);
        IConverter SetVideoSyncMethod(VideoSyncMethod method);
        IConverter SetFrameRate(double frameRate);
        IConverter UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0);
        IConverter UseHardwareAcceleration(string hardwareAccelerator, string decoder, string encoder, int device = 0);
        IConverter UseMultiThread(bool multiThread);
        IConverter SetOutput(string outputPath, bool IsTwitchStream = false);

        

        IConverter SetConcat(bool concat, List<string> Files);
        IConverter SetCutList(List<string> cutList);
        IConverter SetAudioBitrate(long bitrate);
        IConverter SetVSync(VsyncParams vsyncmode);
        IConverter SetInputTime(TimeSpan? time);
        IConverter SetOutputTime(TimeSpan? time);
        IConverter UseShortest(bool useShortest);
        IConverter SetDefaultPath(string path);

        IConverter SetMuxing(bool IsMuxed, string MuxData);

        IConverter SetMultiModeFile(string filename);

        IConverter SetOverlay(string Source, bool IsShortVideo);

        Task<IConversionResult> Start();
        Task<bool> ProbeFile(string filename, bool is1440p);
        /// <returns>IConversion object</returns>
        //public Snippets FromSnippet = new Snippets()
        //IConverter event ConverterProbDataEventHandler OnProbeData;
    }


}
