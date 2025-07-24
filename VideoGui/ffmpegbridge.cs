using MediaInfo;
using MediaInfo.Model;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Text;
using VideoGui.ffmpeg.Streams.Video;
using static System.Net.WebRequestMethods;

namespace VideoGui
{
    public class ffmpegbridge
    {
        int MaxFile = 0;
        private Object thisLockduration = new Object();
        TimeSpan Duration = TimeSpan.Zero;
        public bool Finished = false;
        public List<(string, double)> FileInfoList = new List<(string, double)>();
        public TimeSpan GetDuration()
        {
            try
            {
                return Duration;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return TimeSpan.Zero;
            }
        }
        public void ReadDuration(string folder, bool UseVideoDuration = false)
        {
            try
            {
                ReadDurations(new List<string> { folder }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public (IVideoStream, IAudioStream, List<TextStream>, TimeSpan) ReadMediaFile(string filePath)
        {
            try
            {
                ffmpeg.Streams.Video.IVideoStream videostream = new ffmpeg.Streams.Video.VideoStream();
                ffmpeg.Streams.Audio.IAudioStream audiostream = new ffmpeg.Streams.Audio.AudioStream();
                List<ffmpeg.Streams.Text.TextStream> textstream = new List<ffmpeg.Streams.Text.TextStream>();
                
                var mw1 = new MediaInfo.MediaInfo();
                mw1.Open(filePath);
                double durgen = mw1.Get(StreamKind.General, 0, "Duration").ToDouble();
                TimeSpan DurationGen = TimeSpan.FromMilliseconds(durgen);

                int vstrcnt = mw1.CountGet(MediaInfo.StreamKind.Video);
                int astrcnt = mw1.CountGet(MediaInfo.StreamKind.Audio);
                int cstrcnt = mw1.CountGet(MediaInfo.StreamKind.Other);
                int tstrcnt = mw1.CountGet(MediaInfo.StreamKind.Text);
                if (tstrcnt > 0)
                {
                    /*        ID                          : 3
                       Format                      : ASS
                       Codec ID                    : S_TEXT/ASS
                       Codec ID/Info               : Advanced Sub Station Alpha
                       Duration                    : 39 min 38 s
                       Compression mode            : Lossless
                       Title                       : English (forced)
                       Language                    : English
                       Default                     : Yes
                       Forced                      : Yes
                     */
                    int ID = mw1.Get(StreamKind.Text, 0, "ID").ToInt();
                    string Codec = mw1.Get(StreamKind.Text, 0, "Format");
                    string CodecID = mw1.Get(StreamKind.Text, 0, "Codec ID");
                    string CodecIDInfo = mw1.Get(StreamKind.Text, 0, "Codec ID/Info");
                    double dur = mw1.Get(StreamKind.Video, 0, "Duration").ToDouble();
                    TimeSpan Duration = TimeSpan.FromMilliseconds(dur);
                    string Title = mw1.Get(StreamKind.Text, 0, "Title");
                    string Language = mw1.Get(StreamKind.Text, 0, "Language");
                    int CompressionMode = mw1.Get(StreamKind.Text, 0, "Compression mode").ToInt();
                    int Default = mw1.Get(StreamKind.Text, 0, "Default").ToIntYesNo();
                    int Forced = mw1.Get(StreamKind.Text, 0, "Forced").ToIntYesNo();
                    textstream.Add(new TextStream(Codec,  Duration,CodecID, CodecIDInfo,
                       Title, Language, CompressionMode, Default, Forced,ID));
                    /*
                     public ITextStream Initialize(string _Codec, 
                    TimeSpan _Duration,
                    string _CodecInfo, 
                    string _Title, 
                    string _CodecId, 
                    string _Language,
                    int _Default, int _Forced, int _id)*/

                }
                if (vstrcnt > 0)
                {
                    string Codec = mw1.Get(MediaInfo.StreamKind.Video, 0, "Format").Replace("-", "").ToLower();
                    int ID = mw1.Get(StreamKind.Video, 0, "ID").ToInt();
                    int Width = mw1.Get(StreamKind.Video, 0, "Width").ToInt();
                    int Height = mw1.Get(StreamKind.Video, 0, "Height").ToInt();
                    double dur = mw1.Get(StreamKind.Video, 0, "Duration").ToDouble();
                    TimeSpan Duration = TimeSpan.FromMilliseconds(dur);
                    int BitDepth = mw1.Get(StreamKind.Video, 0, "BitDepth").ToInt();
                    int Bitrate = mw1.Get(StreamKind.Video, 0, "BitRate").ToInt();
                    int Default = mw1.Get(StreamKind.Video, 0, "Default").ToIntYesNo();
                    int Forced = mw1.Get(StreamKind.Video, 0, "Forced").ToIntYesNo();
                    string AspectRatioMode = mw1.Get(StreamKind.Video, 0, "AspectRatio/String"); //as formatted string
                    double AspectRatio = mw1.Get(StreamKind.Video, 0, "AspectRatio").ToDouble();
                    double FrameRate = mw1.Get(StreamKind.Video, 0, "FrameRate").ToDouble();
                    string pixelfrm = mw1.Get(StreamKind.Video, 0, "ColorSpace") + mw1.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "p";
                    if (BitDepth == 10)
                    {
                        pixelfrm += "10le";
                    }
                    videostream.Initialize(filePath, Codec, AspectRatioMode, Duration, pixelfrm, Width, Height, Bitrate, (float)FrameRate, Default, Forced, ID);
                }
                if (astrcnt > 0)
                {
                    for (int i = 0; i < astrcnt; i++)
                    {
                        int ID2 = mw1.Get(StreamKind.Audio, i, "ID").ToInt();
                        if (ID2 != 0)
                        {
                            string Language = mw1.Get(StreamKind.Audio, i, "Language");
                            if (Language == "en") Language += "g";
                            if (Language == "eng" || (Language == "" && astrcnt == 1) || astrcnt == 1)
                            {
                                string Codec = mw1.Get(MediaInfo.StreamKind.Audio, i, "Format").Replace("-", "").ToLower();
                                TimeSpan Duration = TimeSpan.FromMilliseconds(mw1.Get(StreamKind.Audio, i, "Duration").ToDouble());
                                int Bitrate = mw1.Get(StreamKind.Audio, i, "BitRate").ToInt();
                                string BitrateMode = mw1.Get(StreamKind.Audio, i, "BitRate_Mode");
                                string CompressionMode = mw1.Get(StreamKind.Audio, i, "Compression_Mode");
                                string ChannelPositions = mw1.Get(StreamKind.Audio, i, "ChannelPositions");
                                int SamplingRate = mw1.Get(StreamKind.Audio, i, "SamplingRate").ToInt();
                                int channels = mw1.Get(StreamKind.Audio, i, "Channels").ToInt();
                                int Default2 = mw1.Get(StreamKind.Audio, i, "Default").ToIntYesNo();
                                int Forced2 = mw1.Get(StreamKind.Audio, i, "Forced").ToIntYesNo();
                                if (Language == "en") Language += "g";
                                audiostream.Initialize(filePath, Codec, Bitrate, channels, SamplingRate, Duration, Language, Default2, Forced2, ID2);
                            }
                        }
                    }
                }
                mw1.Close();
                return (videostream, audiostream, textstream,DurationGen);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return (null, null, null,TimeSpan.Zero);
            }
        }
        public ffmpegbridge()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void ReadDuration(List<string> files, bool UseVideoDuration = false)
        {
            try
            {
                FileInfoList.Clear();
                ReadDurations(files).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public async Task ReadDurations(List<string> files, bool UseVideoDuration = false)
        {
            try
            {
                MaxFile = 0;
                Duration = TimeSpan.Zero;
                FileInfoList.Clear();
                foreach (string file in files)
                {
                    while (MaxFile > 8)
                    {
                        Thread.Sleep(25);
                    }
                    ReadFile(file).ConfigureAwait(false);
                }
                while (MaxFile > 0)
                {
                    Thread.Sleep(100);
                }
                Finished = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public async Task ReadFile(string filePath, bool UseVideoDuration = false)
        {
            try
            {
                MaxFile++;
                Double TBL = 0;
                var mw1 = new MediaInfo.MediaInfo();
                mw1.Open(filePath);
                if (UseVideoDuration)
                {
                    var videoduration = mw1.Get(MediaInfo.StreamKind.General, 0, "Duration");
                    if (videoduration != "")
                    {
                        float flt = 0;
                        float.TryParse(videoduration, out flt);
                        if (flt > 0)
                        {
                            flt = flt / 1000;
                            lock (thisLockduration)
                            {
                                Duration += TimeSpan.FromSeconds(flt);
                                TBL = TimeSpan.FromSeconds(flt).TotalSeconds;
                            }
                        }
                    }
                }
                else
                {
                    var s = mw1.Get(MediaInfo.StreamKind.Other, 0, "TimeCode_FirstFrame");
                    var e = mw1.Get(MediaInfo.StreamKind.Other, 0, "TimeCode_LastFrame");
                    if ((s == "") || (e == ""))
                    {
                        var videoduration = mw1.Get(MediaInfo.StreamKind.General, 0, "Duration");
                        if (videoduration != "")
                        {
                            float flt = 0;
                            float.TryParse(videoduration, out flt);
                            if (flt > 0)
                            {
                                flt = flt / 1000;
                                lock (thisLockduration)
                                {
                                    Duration += TimeSpan.FromSeconds(flt);
                                    TBL = TimeSpan.FromSeconds(flt).TotalSeconds;
                                }
                            }
                        }
                    }
                    else
                    {
                        TimeSpan timeframe = s.FromFFmpegTime();
                        TimeSpan Timeframeend = e.FromFFmpegTime();
                        Timeframeend -= timeframe;
                        lock (thisLockduration)
                        {
                            Duration += Timeframeend;
                            TBL = Timeframeend.TotalSeconds;
                        }
                    }
                }
                mw1.Close();
                FileInfoList.Add((filePath, TBL));
                Finished = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (MaxFile > 0)
                {
                    MaxFile--;
                }
            }
        }

    }
}
