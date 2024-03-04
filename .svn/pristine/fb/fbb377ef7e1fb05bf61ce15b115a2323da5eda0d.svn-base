using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoGui.ffmpeg.Streams.MediaInfo;
using VideoGui.Models.delegates;
using static System.Resources.ResXFileRef;


namespace VideoGui.ffmpeg
{

    public abstract partial class FFmpegCli
    {
        public static Converters Converters = new Converters();
        private _AddConverterProgress AddProgressDelegate;
        public FFmpegCli()
        {

        }
        public FFmpegCli(_AddConverterProgress _ADCP)
        {
            AddProgressDelegate = _ADCP;
        }
    }


    public class Converters
    {
        public IConverter New(_AddConverterProgress data)
        {
            return Converter.New(data);
        }

        public IConverter New()
        {
            return Converter.New();
        }

        internal Converters()
        {

        }
    }
}
